using MarlinPrintMiddleware.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MarlinPrintMiddleware.Persistence;

public static class PersistenceServiceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, DatabaseOptions? options = null)
    {
        services.AddSingleton(options ?? DatabaseOptions.ForAppData());
        services.AddSingleton<ISqliteConnectionFactory, SqliteConnectionFactory>();
        services.AddSingleton<IDatabaseMigrator, DatabaseMigrator>();
        services.AddSingleton<IPrintJobRepository, Repositories.PrintJobRepository>();
        services.AddSingleton<IPrinterProfileRepository, Repositories.PrinterProfileRepository>();
        services.AddSingleton<ISettingsRepository, Repositories.SettingsRepository>();
        services.AddSingleton<IMacroRepository, Repositories.MacroRepository>();
        services.AddHostedService<DatabaseInitializer>();
        return services;
    }
}

internal sealed class DatabaseInitializer : IHostedService
{
    private readonly IDatabaseMigrator _migrator;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(IDatabaseMigrator migrator, ILogger<DatabaseInitializer> logger)
    {
        _migrator = migrator;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _migrator.MigrateAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Database initialized");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
