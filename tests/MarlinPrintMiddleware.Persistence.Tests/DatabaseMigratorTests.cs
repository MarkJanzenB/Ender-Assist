using FluentAssertions;
using MarlinPrintMiddleware.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging.Abstractions;

namespace MarlinPrintMiddleware.Persistence.Tests;

public class DatabaseMigratorTests
{
    [Fact]
    public async Task MigrateAsync_AppliesSchemaOnFreshDatabase()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"enderassist_schema_{Guid.NewGuid():N}.db");
        try
        {
            var options = new DatabaseOptions { ConnectionString = $"Data Source={dbPath}" };
            var factory = new SqliteConnectionFactory(options);
            var migrator = new DatabaseMigrator(factory, NullLogger<DatabaseMigrator>.Instance);

            await migrator.MigrateAsync();

            await using var connection = factory.CreateConnection();
            await using var command = connection.CreateCommand();
            command.CommandText =
                "SELECT name FROM sqlite_master WHERE type='table' AND name IN ('print_jobs','printer_profiles','settings','schema_version')";
            var tables = new List<string>();
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tables.Add(reader.GetString(0));
            }

            tables.Should().BeEquivalentTo(["print_jobs", "printer_profiles", "schema_version", "settings"]);
        }
        finally
        {
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }
        }
    }

    [Fact]
    public async Task MigrateAsync_IsIdempotent()
    {
        using var ctx = new PersistenceTestContext();
        var migrator = new DatabaseMigrator(ctx.ConnectionFactory, NullLogger<DatabaseMigrator>.Instance);

        await migrator.MigrateAsync();
        await migrator.Invoking(m => m.MigrateAsync()).Should().NotThrowAsync();
    }
}
