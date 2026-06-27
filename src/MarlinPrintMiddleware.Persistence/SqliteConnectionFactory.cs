using Microsoft.Data.Sqlite;

namespace MarlinPrintMiddleware.Persistence;

public interface ISqliteConnectionFactory
{
    SqliteConnection CreateConnection();
}

public sealed class SqliteConnectionFactory : ISqliteConnectionFactory
{
    private readonly DatabaseOptions _options;

    public SqliteConnectionFactory(DatabaseOptions options)
    {
        _options = options;
    }

    public SqliteConnection CreateConnection()
    {
        var connection = new SqliteConnection(_options.ConnectionString);
        connection.Open();
        return connection;
    }
}
