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
    private double? _fanPercent;
    private bool _hasReading;
    private bool _hasFanReading;

    public TemperatureMonitorService(
        ISerialEngine serialEngine,
        MonitoringOptions options,
        ILogger<TemperatureMonitorService> logger)
    {
        _serialEngine = serialEngine;
        _options = options;
        _logger = logger;

        _serialEngine.ConnectionStateChanged += (_, args) =>
        {
            if (args.CurrentState != ConnectionState.Connected)
            {
                _hasReading = false;
                _hotend = 0;
                _targetHotend = 0;
                _bed = 0;
                _targetBed = 0;
                _fanPercent = null;
                _hasFanReading = false;
            }
        };
    }

    public bool HasReading => _hasReading;

    public bool HasFanReading => _hasFanReading;

    public double? FanSpeedPercent => _fanPercent;

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
                        _hasReading = true;
                        _hotend = hotend;
                        _targetHotend = targetHotend;
                        _bed = bed;
                        _targetBed = targetBed;
                        if (MarlinTemperatureParser.TryParseFanSpeed(response, out var fan))
                        {
                            _fanPercent = fan;
                            _hasFanReading = true;
                        }

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
