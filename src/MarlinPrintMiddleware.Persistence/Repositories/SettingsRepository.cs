using System.Text.Json;
using MarlinPrintMiddleware.Core.Interfaces;
using Microsoft.Data.Sqlite;

namespace MarlinPrintMiddleware.Persistence.Repositories;

public sealed class SettingsRepository : ISettingsRepository
{
    private readonly ISqliteConnectionFactory _connectionFactory;

    public SettingsRepository(ISqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT value FROM settings WHERE key = $key";
        command.Parameters.AddWithValue("$key", key);

        var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        if (result is null or DBNull)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>((string)result);
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(value);

        await using var connection = _connectionFactory.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO settings (key, value) VALUES ($key, $value)
            ON CONFLICT(key) DO UPDATE SET value = excluded.value
            """;
        command.Parameters.AddWithValue("$key", key);
        command.Parameters.AddWithValue("$value", json);
        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }
}
