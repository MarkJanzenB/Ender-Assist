using MarlinPrintMiddleware.Monitoring;
using MarlinPrintMiddleware.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MarlinPrintMiddleware.Monitoring;

public static class MonitoringServiceExtensions
{
    public static IServiceCollection AddMonitoring(this IServiceCollection services)
    {
        services.AddSingleton<MonitoringOptions>();
        services.AddSingleton<TemperatureMonitorService>();
        services.AddHostedService(sp => sp.GetRequiredService<TemperatureMonitorService>());
        services.AddSingleton<IPrinterStatusService, PrinterStatusService>();
        services.AddHostedService(sp => (PrinterStatusService)sp.GetRequiredService<IPrinterStatusService>());
        return services;
    }
}
