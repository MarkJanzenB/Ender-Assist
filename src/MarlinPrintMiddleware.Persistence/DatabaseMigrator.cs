using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace MarlinPrintMiddleware.Persistence;

public interface IDatabaseMigrator
{
    Task MigrateAsync(CancellationToken cancellationToken = default);
}

public sealed class DatabaseMigrator : IDatabaseMigrator
{
    private static readonly (int Version, string FileName)[] Migrations =
    [
        (1, "001_initial.sql"),
        (2, "002_job_metadata.sql")
    ];

    private readonly ISqliteConnectionFactory _connectionFactory;
    private readonly ILogger<DatabaseMigrator> _logger;

    public DatabaseMigrator(ISqliteConnectionFactory connectionFactory, ILogger<DatabaseMigrator> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();

        var currentVersion = await GetCurrentVersionAsync(connection, cancellationToken).ConfigureAwait(false);
        foreach (var (version, fileName) in Migrations)
        {
            if (currentVersion >= version)
            {
                continue;
            }

            var sqlPath = Path.Combine(AppContext.BaseDirectory, "Migrations", fileName);
            if (!File.Exists(sqlPath))
            {
                throw new InvalidOperationException($"Migration script not found: {sqlPath}");
            }

            var script = await File.ReadAllTextAsync(sqlPath, cancellationToken).ConfigureAwait(false);
            await using var command = connection.CreateCommand();
            command.CommandText = script;
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Applied database migration to version {Version}", version);
            currentVersion = version;
        }
    }

    private static async Task<int> GetCurrentVersionAsync(SqliteConnection connection, CancellationToken cancellationToken)
    {
        await using var check = connection.CreateCommand();
        check.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='schema_version'";
        var exists = await check.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        if (exists is null)
        {
            return 0;
        }

        await using var versionCmd = connection.CreateCommand();
        versionCmd.CommandText = "SELECT COALESCE(MAX(version), 0) FROM schema_version";
        var result = await versionCmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        return Convert.ToInt32(result);
    }
}
