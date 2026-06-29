using MarlinPrintMiddleware.Core.Enums;
using MarlinPrintMiddleware.Core.Interfaces;
using MarlinPrintMiddleware.Core.Models;
using MarlinPrintMiddleware.Monitoring.Parsers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MarlinPrintMiddleware.Monitoring;

public sealed class PrinterStatusService : BackgroundService, IPrinterStatusService
{
    private readonly ISerialEngine _serialEngine;
    private readonly IPrintQueueService _queueService;
    private readonly TemperatureMonitorService _temperatureMonitor;
    private readonly MonitoringOptions _options;
    private readonly ILogger<PrinterStatusService> _logger;
    private readonly object _sync = new();

    private PrinterStatus _status = new();
    private DateTimeOffset? _printStartedAt;
    private double _positionX;
    private double _positionY;
    private double _positionZ;
    private double _positionE;
    private bool _hasLivePosition;
    private DateTimeOffset _lastPositionPoll = DateTimeOffset.MinValue;

    public PrinterStatusService(
        ISerialEngine serialEngine,
        IPrintQueueService queueService,
        TemperatureMonitorService temperatureMonitor,
        MonitoringOptions options,
        ILogger<PrinterStatusService> logger)
    {
        _serialEngine = serialEngine;
        _queueService = queueService;
        _temperatureMonitor = temperatureMonitor;
        _options = options;
        _logger = logger;

        _serialEngine.ConnectionStateChanged += (_, _) =>
        {
            if (_serialEngine.State != ConnectionState.Connected)
            {
                _hasLivePosition = false;
            }

            PublishStatus();
        };
        _serialEngine.StreamProgress += OnStreamProgress;
        _temperatureMonitor.TemperatureUpdated += (_, _) => PublishStatus();
    }

    public PrinterStatus CurrentStatus
    {
        get
        {
            lock (_sync)
            {
                return CloneStatus(_status);
            }
        }
    }

    public event EventHandler<PrinterStatus>? StatusChanged;

    public async Task RefreshPositionAsync(CancellationToken cancellationToken = default)
    {
        if (_serialEngine.State != ConnectionState.Connected)
        {
            return;
        }

        try
        {
            var response = await _serialEngine.SendCommandAsync("M114", cancellationToken).ConfigureAwait(false);
            if (MarlinPositionParser.TryParse(response, out var x, out var y, out var z, out var e))
            {
                _positionX = x;
                _positionY = y;
                _positionZ = z;
                _positionE = e;
                _hasLivePosition = true;
                _lastPositionPoll = DateTimeOffset.UtcNow;
                PublishStatus();
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogDebug(ex, "Position poll failed");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_serialEngine.State == ConnectionState.Connected
                && DateTimeOffset.UtcNow - _lastPositionPoll >= TimeSpan.FromMilliseconds(_options.PositionPollIntervalMs))
            {
                await RefreshPositionAsync(stoppingToken).ConfigureAwait(false);
            }

            PublishStatus();
            await Task.Delay(_options.StatusRefreshIntervalMs, stoppingToken).ConfigureAwait(false);
        }
    }

    private void OnStreamProgress(object? sender, Core.Events.GCodeStreamProgressEventArgs e)
    {
        lock (_sync)
        {
            _status.Progress = e.Progress;
            _printStartedAt ??= DateTimeOffset.UtcNow;
        }

        PublishStatus();
    }

    private void PublishStatus()
    {
        lock (_sync)
        {
            var snapshot = _queueService.GetSnapshot();
            var currentJob = snapshot.CurrentJobId is long id
                ? snapshot.Jobs.FirstOrDefault(j => j.Id == id)
                : snapshot.Jobs.FirstOrDefault(j => j.Status == JobStatus.Printing);

            _status.ConnectionState = _serialEngine.State;
            _status.PrintState = _queueService.PrintState;
            _status.HotendTemp = _temperatureMonitor.HotendTemp;
            _status.BedTemp = _temperatureMonitor.BedTemp;
            _status.TargetHotend = _temperatureMonitor.TargetHotend;
            _status.TargetBed = _temperatureMonitor.TargetBed;
            _status.HasLiveTemperature = _temperatureMonitor.HasReading
                && _serialEngine.State == ConnectionState.Connected;
            _status.PositionX = _hasLivePosition ? _positionX : null;
            _status.PositionY = _hasLivePosition ? _positionY : null;
            _status.PositionZ = _hasLivePosition ? _positionZ : null;
            _status.PositionE = _hasLivePosition ? _positionE : null;
            _status.HasLivePosition = _hasLivePosition && _serialEngine.State == ConnectionState.Connected;
            _status.FanSpeedPercent = _temperatureMonitor.HasFanReading ? _temperatureMonitor.FanSpeedPercent : null;
            _status.HasLiveFan = _temperatureMonitor.HasFanReading
                && _serialEngine.State == ConnectionState.Connected;
            _status.TotalLayers = currentJob?.TotalLayers;
            _status.CurrentJobName = currentJob?.Name;
            _status.Progress = currentJob?.Progress ?? _status.Progress;

            if (_printStartedAt is not null && _status.Progress > 5)
            {
                var elapsed = DateTimeOffset.UtcNow - _printStartedAt.Value;
                _status.Elapsed = elapsed;
                var remainingFraction = (100 - _status.Progress) / _status.Progress;
                _status.Eta = TimeSpan.FromTicks((long)(elapsed.Ticks * remainingFraction));
            }
            else
            {
                _status.Elapsed = _printStartedAt is null ? TimeSpan.Zero : DateTimeOffset.UtcNow - _printStartedAt.Value;
                _status.Eta = null;
            }

            if (_queueService.PrintState is PrintState.Idle or PrintState.Completed or PrintState.Cancelled or PrintState.Failed)
            {
                _printStartedAt = null;
                _status.Progress = 0;
                _status.CurrentLayer = null;
            }

            StatusChanged?.Invoke(this, CloneStatus(_status));
        }
    }

    private static PrinterStatus CloneStatus(PrinterStatus status) => new()
    {
        ConnectionState = status.ConnectionState,
        PrintState = status.PrintState,
        HotendTemp = status.HotendTemp,
        BedTemp = status.BedTemp,
        TargetHotend = status.TargetHotend,
        TargetBed = status.TargetBed,
        Progress = status.Progress,
        CurrentJobName = status.CurrentJobName,
        Elapsed = status.Elapsed,
        Eta = status.Eta,
        HasLiveTemperature = status.HasLiveTemperature,
        PositionX = status.PositionX,
        PositionY = status.PositionY,
        PositionZ = status.PositionZ,
        PositionE = status.PositionE,
        HasLivePosition = status.HasLivePosition,
        FanSpeedPercent = status.FanSpeedPercent,
        HasLiveFan = status.HasLiveFan,
        CurrentLayer = status.CurrentLayer,
        TotalLayers = status.TotalLayers
    };
}
