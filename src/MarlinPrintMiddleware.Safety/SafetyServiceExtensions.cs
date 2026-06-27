using MarlinPrintMiddleware.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MarlinPrintMiddleware.Safety;

public static class SafetyServiceExtensions
{
    public static IServiceCollection AddSafety(this IServiceCollection services)
    {
        services.AddSingleton<SafetyOptions>();
        services.AddSingleton<EmergencyStopService>();
        services.AddSingleton<IEmergencyStopService>(sp => sp.GetRequiredService<EmergencyStopService>());
        services.AddSingleton<IPauseResumeService, PauseResumeService>();
        services.AddHostedService<ThermalSafetyService>();
        return services;
    }
}
