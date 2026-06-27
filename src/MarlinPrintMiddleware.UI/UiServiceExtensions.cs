using MarlinPrintMiddleware.UI.ViewModels;
using MarlinPrintMiddleware.UI.Views;
using Microsoft.Extensions.DependencyInjection;

namespace MarlinPrintMiddleware.UI;

public static class UiServiceExtensions
{
    public static IServiceCollection AddUi(this IServiceCollection services)
    {
        services.AddSingleton<MainViewModel>();
        services.AddTransient<MainWindow>();
        return services;
    }
}
