using MarlinPrintMiddleware.Core.Enums;
using MarlinPrintMiddleware.Core.Events;
using MarlinPrintMiddleware.Core.Interfaces;
using MarlinPrintMiddleware.Monitoring;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MarlinPrintMiddleware.Safety;

public sealed class ThermalSafetyService : BackgroundService, IThermalWarningService
{
    private readonly TemperatureMonitorService _temperatureMonitor;
    private readonly IPrintQueueService _queueService;
    private readonly SafetyOptions _options;
    private readonly ILogger<ThermalSafetyService> _logger;

    private double? _lastHotendDuringPrint;

    public ThermalSafetyService(
        TemperatureMonitorService temperatureMonitor,
        IPrintQueueService queueService,
        SafetyOptions options,
        ILogger<ThermalSafetyService> logger)
    {
        _temperatureMonitor = temperatureMonitor;
        _queueService = queueService;
        _options = options;
        _logger = logger;
    }

    public event EventHandler<ThermalWarningEventArgs>? ThermalWarning;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _temperatureMonitor.TemperatureUpdated += OnTemperatureUpdated;
        return Task.CompletedTask;
    }

    private void OnTemperatureUpdated(object? sender, EventArgs e)
    {
        if (_queueService.PrintState != PrintState.Printing)
        {
            _lastHotendDuringPrint = null;
            return;
        }

        var hotend = _temperatureMonitor.HotendTemp;
        if (_lastHotendDuringPrint is double last && last - hotend > _options.HotendDropWarningCelsius)
        {
            var message = $"Hotend temperature dropped {last - hotend:F1}°C during print.";
            _logger.LogWarning(message);
            ThermalWarning?.Invoke(this, new ThermalWarningEventArgs(message));
        }

        _lastHotendDuringPrint = hotend;
    }
}
