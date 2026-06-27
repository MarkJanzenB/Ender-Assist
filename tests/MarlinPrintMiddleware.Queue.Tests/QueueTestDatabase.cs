using MarlinPrintMiddleware.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging.Abstractions;

namespace MarlinPrintMiddleware.Queue.Tests;

internal sealed class QueueTestDatabase : IDisposable
{
    public QueueTestDatabase()
    {
        DbPath = Path.Combine(Path.GetTempPath(), $"enderassist_queue_{Guid.NewGuid():N}.db");
        ConnectionFactory = new SqliteConnectionFactory(new DatabaseOptions
        {
            ConnectionString = $"Data Source={DbPath}"
        });
        new DatabaseMigrator(ConnectionFactory, NullLogger<DatabaseMigrator>.Instance)
            .MigrateAsync().GetAwaiter().GetResult();
    }

    public string DbPath { get; }

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
