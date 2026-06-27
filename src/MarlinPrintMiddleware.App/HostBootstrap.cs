using MarlinPrintMiddleware.Core.Interfaces;
using MarlinPrintMiddleware.Monitoring;
using MarlinPrintMiddleware.Persistence;
using MarlinPrintMiddleware.Queue;
using MarlinPrintMiddleware.Safety;
using MarlinPrintMiddleware.Serial;
using MarlinPrintMiddleware.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MarlinPrintMiddleware.App;

public static class HostBootstrap
{
    public static IHost BuildHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices(ConfigureServices)
            .ConfigureLogging(logging =>
            {
                logging.AddDebug();
                logging.SetMinimumLevel(LogLevel.Information);
            })
            .Build();
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging();
        services.AddSingleton<SerialOptions>();
        services.AddSingleton<ISerialPortDiscovery, SerialPortDiscovery>();
        services.AddSingleton<ISerialEngine, SerialEngine>();
        services.AddHostedService(sp => (SerialEngine)sp.GetRequiredService<ISerialEngine>());

        services.AddPersistence();
        services.AddPrintQueue();
        services.AddMonitoring();
        services.AddSafety();
        services.AddUi();
    }
}
