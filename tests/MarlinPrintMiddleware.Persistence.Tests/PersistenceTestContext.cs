using MarlinPrintMiddleware.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging.Abstractions;

namespace MarlinPrintMiddleware.Persistence.Tests;

public sealed class PersistenceTestContext : IDisposable
{
    public PersistenceTestContext()
    {
        DbPath = Path.Combine(Path.GetTempPath(), $"enderassist_test_{Guid.NewGuid():N}.db");
        Options = new DatabaseOptions { ConnectionString = $"Data Source={DbPath}" };
        ConnectionFactory = new SqliteConnectionFactory(Options);
        var migrator = new DatabaseMigrator(ConnectionFactory, NullLogger<DatabaseMigrator>.Instance);
        migrator.MigrateAsync().GetAwaiter().GetResult();
    }

    public string DbPath { get; }

    public DatabaseOptions Options { get; }

    public ISqliteConnectionFactory ConnectionFactory { get; }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        if (File.Exists(DbPath))
        {
            try { File.Delete(DbPath); } catch (IOException) { }
        }
    }
}
