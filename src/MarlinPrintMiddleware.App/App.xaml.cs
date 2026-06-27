using System.Windows;
using System.Windows.Threading;
using MarlinPrintMiddleware.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MarlinPrintMiddleware.App;

public partial class App : Application
{
    private IHost? _host;

    public App()
    {
        DispatcherUnhandledException += (_, args) =>
        {
            MessageBox.Show(args.Exception.ToString(), "Ender Assist — UI error", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true;
        };
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ShutdownMode = ShutdownMode.OnMainWindowClose;

        try
        {
            _host = HostBootstrap.BuildHost();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            MainWindow = mainWindow;
            mainWindow.Show();

            await _host.StartAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.ToString(),
                "Ender Assist failed to start",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(1);
        }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync().ConfigureAwait(true);
            _host.Dispose();
        }

        base.OnExit(e);
    }
}
