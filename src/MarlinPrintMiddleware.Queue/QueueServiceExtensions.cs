using MarlinPrintMiddleware.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MarlinPrintMiddleware.Queue;

public static class QueueServiceExtensions
{
    public static IServiceCollection AddPrintQueue(this IServiceCollection services)
    {
        services.AddSingleton<GCodeParser>();
        services.AddSingleton<IPrintQueueService, PrintQueueService>();
        return services;
    }
}
