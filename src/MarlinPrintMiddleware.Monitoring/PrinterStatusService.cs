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

        _serialEngine.ConnectionStateChanged += (_, _) => PublishStatus();
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
            if (MarlinPositionParser.TryParse(response, out var x, out var y, out var z))
            {
                _positionX = x;
                _positionY = y;
                _positionZ = z;
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
        Eta = status.Eta
    };
}
