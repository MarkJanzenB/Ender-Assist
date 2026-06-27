namespace MarlinPrintMiddleware.Persistence;

public sealed class DatabaseOptions
{
    public string ConnectionString { get; set; } = "Data Source=enderassist.db";

    public static string DefaultAppDataPath()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "EnderAssist");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, "data.db");
    }

    public static DatabaseOptions ForAppData() => new()
    {
        ConnectionString = $"Data Source={DefaultAppDataPath()}"
    };

    public static DatabaseOptions InMemory() => new()
    {
        ConnectionString = "Data Source=:memory:"
    };
}
