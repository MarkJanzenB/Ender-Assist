using MarlinPrintMiddleware.Core.Interfaces;
using MarlinPrintMiddleware.Serial;
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
                logging.SetMinimumLevel(LogLevel.Debug);
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

        services.AddSingleton<IPrintQueueService, StubPrintQueueService>();
        services.AddSingleton<IPrintJobRepository, StubPrintJobRepository>();
        services.AddSingleton<IPrinterProfileRepository, StubPrinterProfileRepository>();
        services.AddSingleton<ISettingsRepository, StubSettingsRepository>();
    }
}
