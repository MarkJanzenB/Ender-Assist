using MarlinPrintMiddleware.Core.Enums;
using MarlinPrintMiddleware.Core.Interfaces;
using MarlinPrintMiddleware.Monitoring.Parsers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MarlinPrintMiddleware.Monitoring;

public sealed class TemperatureMonitorService : BackgroundService
{
    private readonly ISerialEngine _serialEngine;
    private readonly MonitoringOptions _options;
    private readonly ILogger<TemperatureMonitorService> _logger;

    private double _hotend;
    private double _targetHotend;
    private double _bed;
    private double _targetBed;

    public TemperatureMonitorService(
        ISerialEngine serialEngine,
        MonitoringOptions options,
        ILogger<TemperatureMonitorService> logger)
    {
        _serialEngine = serialEngine;
        _options = options;
        _logger = logger;
    }

    public double HotendTemp => _hotend;

    public double TargetHotend => _targetHotend;

    public double BedTemp => _bed;

    public double TargetBed => _targetBed;

    public event EventHandler? TemperatureUpdated;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_serialEngine.State == ConnectionState.Connected)
                {
                    var response = await _serialEngine.SendCommandAsync("M105", stoppingToken).ConfigureAwait(false);
                    if (MarlinTemperatureParser.TryParse(response, out var hotend, out var targetHotend, out var bed, out var targetBed))
                    {
                        _hotend = hotend;
                        _targetHotend = targetHotend;
                        _bed = bed;
                        _targetBed = targetBed;
                        TemperatureUpdated?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogDebug(ex, "Temperature poll failed");
            }

            await Task.Delay(_options.TemperaturePollIntervalMs, stoppingToken).ConfigureAwait(false);
        }
    }
}
