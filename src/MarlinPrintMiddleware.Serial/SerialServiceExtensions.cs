using MarlinPrintMiddleware.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MarlinPrintMiddleware.Serial;

public static class SerialServiceExtensions
{
    public static IServiceCollection AddSerialServices(this IServiceCollection services)
    {
        services.AddSingleton<ISerialConsoleService, SerialConsoleService>();
        services.AddSingleton<IPrinterControlService, PrinterControlService>();
        return services;
    }
}
