using MarlinPrintMiddleware.Core.Interfaces;
using MarlinPrintMiddleware.Core.Models;
using Microsoft.Data.Sqlite;

namespace MarlinPrintMiddleware.Persistence.Repositories;

public sealed class PrinterProfileRepository : IPrinterProfileRepository
{
    private readonly ISqliteConnectionFactory _connectionFactory;

    public PrinterProfileRepository(ISqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PrinterProfile?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM printer_profiles WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        return await reader.ReadAsync(cancellationToken).ConfigureAwait(false)
            ? Map(reader)
            : null;
    }

    public async Task<IReadOnlyList<PrinterProfile>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM printer_profiles ORDER BY name";

        var profiles = new List<PrinterProfile>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            profiles.Add(Map(reader));
        }

        return profiles;
    }

    public async Task<PrinterProfile?> GetDefaultAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM printer_profiles WHERE is_default = 1 LIMIT 1";

        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        return await reader.ReadAsync(cancellationToken).ConfigureAwait(false)
            ? Map(reader)
            : null;
    }

    public async Task<PrinterProfile> CreateAsync(PrinterProfile profile, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        if (profile.IsDefault)
        {
            await ClearDefaultAsync(connection, transaction, cancellationToken).ConfigureAwait(false);
        }

        await using var command = connection.CreateCommand();
        command.Transaction = (SqliteTransaction)transaction;
        command.CommandText = """
            INSERT INTO printer_profiles (name, port, baud_rate, buffer_size, is_default)
            VALUES ($name, $port, $baud_rate, $buffer_size, $is_default);
            SELECT last_insert_rowid();
            """;
        Bind(profile, command);

        profile.Id = Convert.ToInt64(await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false));
        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        return profile;
    }

    public async Task UpdateAsync(PrinterProfile profile, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        if (profile.IsDefault)
        {
            await ClearDefaultAsync(connection, transaction, cancellationToken).ConfigureAwait(false);
        }

        await using var command = connection.CreateCommand();
        command.Transaction = (SqliteTransaction)transaction;
        command.CommandText = """
            UPDATE printer_profiles SET
                name = $name,
                port = $port,
                baud_rate = $baud_rate,
                buffer_size = $buffer_size,
                is_default = $is_default
            WHERE id = $id
            """;
        Bind(profile, command);
        command.Parameters.AddWithValue("$id", profile.Id);
        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM printer_profiles WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static async Task ClearDefaultAsync(
        SqliteConnection connection,
        System.Data.Common.DbTransaction transaction,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.Transaction = (SqliteTransaction)transaction;
        command.CommandText = "UPDATE printer_profiles SET is_default = 0";
        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static PrinterProfile Map(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt64(reader.GetOrdinal("id")),
        Name = reader.GetString(reader.GetOrdinal("name")),
        Port = reader.GetString(reader.GetOrdinal("port")),
        BaudRate = reader.GetInt32(reader.GetOrdinal("baud_rate")),
        BufferSize = reader.GetInt32(reader.GetOrdinal("buffer_size")),
        IsDefault = reader.GetInt32(reader.GetOrdinal("is_default")) == 1
    };

    private static void Bind(PrinterProfile profile, SqliteCommand command)
    {
        command.Parameters.AddWithValue("$name", profile.Name);
        command.Parameters.AddWithValue("$port", profile.Port);
        command.Parameters.AddWithValue("$baud_rate", profile.BaudRate);
        command.Parameters.AddWithValue("$buffer_size", profile.BufferSize);
        command.Parameters.AddWithValue("$is_default", profile.IsDefault ? 1 : 0);
    }
}
