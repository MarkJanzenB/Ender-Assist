using MarlinPrintMiddleware.Core.Enums;
using MarlinPrintMiddleware.Core.Interfaces;
using MarlinPrintMiddleware.Core.Models;
using Microsoft.Data.Sqlite;

namespace MarlinPrintMiddleware.Persistence.Repositories;

public sealed class PrintJobRepository : IPrintJobRepository
{
    private readonly ISqliteConnectionFactory _connectionFactory;

    public PrintJobRepository(ISqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PrintJob?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM print_jobs WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        return await reader.ReadAsync(cancellationToken).ConfigureAwait(false)
            ? Map(reader)
            : null;
    }

    public async Task<IReadOnlyList<PrintJob>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT * FROM print_jobs ORDER BY priority DESC, queue_order ASC, created_at ASC";

        return await ReadAllAsync(command, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<PrintJob>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT * FROM print_jobs WHERE status = $status ORDER BY priority DESC, queue_order ASC, created_at ASC";
        command.Parameters.AddWithValue("$status", JobStatus.Pending.ToString());

        return await ReadAllAsync(command, cancellationToken).ConfigureAwait(false);
    }

    public async Task<PrintJob> CreateAsync(PrintJob job, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO print_jobs (
                file_path, name, status, priority, progress, total_lines, last_line_sent,
                queue_order, created_at, started_at, completed_at, error_message)
            VALUES (
                $file_path, $name, $status, $priority, $progress, $total_lines, $last_line_sent,
                $queue_order, $created_at, $started_at, $completed_at, $error_message);
            SELECT last_insert_rowid();
            """;

        Bind(job, command);
        var id = Convert.ToInt64(await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false));
        job.Id = id;
        return job;
    }

    public async Task UpdateAsync(PrintJob job, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE print_jobs SET
                file_path = $file_path,
                name = $name,
                status = $status,
                priority = $priority,
                progress = $progress,
                total_lines = $total_lines,
                last_line_sent = $last_line_sent,
                queue_order = $queue_order,
                created_at = $created_at,
                started_at = $started_at,
                completed_at = $completed_at,
                error_message = $error_message
            WHERE id = $id
            """;

        Bind(job, command);
        command.Parameters.AddWithValue("$id", job.Id);
        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task UpsertAsync(PrintJob job, CancellationToken cancellationToken = default) =>
        job.Id == 0 ? CreateAsync(job, cancellationToken) : UpdateAsync(job, cancellationToken);

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM print_jobs WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static async Task<IReadOnlyList<PrintJob>> ReadAllAsync(
        SqliteCommand command,
        CancellationToken cancellationToken)
    {
        var jobs = new List<PrintJob>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            jobs.Add(Map(reader));
        }

        return jobs;
    }

    private static PrintJob Map(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt64(reader.GetOrdinal("id")),
        FilePath = reader.GetString(reader.GetOrdinal("file_path")),
        Name = reader.GetString(reader.GetOrdinal("name")),
        Status = Enum.Parse<JobStatus>(reader.GetString(reader.GetOrdinal("status"))),
        Priority = (JobPriority)reader.GetInt32(reader.GetOrdinal("priority")),
        Progress = reader.GetDouble(reader.GetOrdinal("progress")),
        TotalLines = reader.GetInt32(reader.GetOrdinal("total_lines")),
        LastLineSent = reader.GetInt32(reader.GetOrdinal("last_line_sent")),
        QueueOrder = reader.GetInt32(reader.GetOrdinal("queue_order")),
        CreatedAt = DateTimeOffset.Parse(reader.GetString(reader.GetOrdinal("created_at"))),
        StartedAt = ReadNullableDateTimeOffset(reader, "started_at"),
        CompletedAt = ReadNullableDateTimeOffset(reader, "completed_at"),
        ErrorMessage = reader.IsDBNull(reader.GetOrdinal("error_message"))
            ? null
            : reader.GetString(reader.GetOrdinal("error_message"))
    };

    private static DateTimeOffset? ReadNullableDateTimeOffset(SqliteDataReader reader, string column)
    {
        var ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? null : DateTimeOffset.Parse(reader.GetString(ordinal));
    }

    private static void Bind(PrintJob job, SqliteCommand command)
    {
        command.Parameters.AddWithValue("$file_path", job.FilePath);
        command.Parameters.AddWithValue("$name", job.Name);
        command.Parameters.AddWithValue("$status", job.Status.ToString());
        command.Parameters.AddWithValue("$priority", (int)job.Priority);
        command.Parameters.AddWithValue("$progress", job.Progress);
        command.Parameters.AddWithValue("$total_lines", job.TotalLines);
        command.Parameters.AddWithValue("$last_line_sent", job.LastLineSent);
        command.Parameters.AddWithValue("$queue_order", job.QueueOrder);
        command.Parameters.AddWithValue("$created_at", job.CreatedAt.ToString("O"));
        command.Parameters.AddWithValue("$started_at", job.StartedAt?.ToString("O") ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("$completed_at", job.CompletedAt?.ToString("O") ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("$error_message", job.ErrorMessage ?? (object)DBNull.Value);
    }
}
