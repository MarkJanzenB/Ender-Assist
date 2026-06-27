using System.Collections.Concurrent;
using MarlinPrintMiddleware.Core.Enums;
using MarlinPrintMiddleware.Core.Events;
using MarlinPrintMiddleware.Core.Exceptions;
using MarlinPrintMiddleware.Core.Interfaces;
using MarlinPrintMiddleware.Core.Models;
using MarlinPrintMiddleware.Serial.Parsers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MarlinPrintMiddleware.Serial;

/// <summary>
/// Background serial engine that owns all SerialPort access and Marlin protocol synchronization.
/// </summary>
public sealed class SerialEngine : BackgroundService, ISerialEngine
{
    private readonly ILogger<SerialEngine> _logger;
    private readonly SerialOptions _options;
    private readonly MarlinHandshakeParser _handshakeParser = new();
    private readonly Func<string, int, ISerialPort> _portFactory;
    private readonly OkSynchronizer _okSynchronizer;
    private readonly SemaphoreSlim _commandGate = new(1, 1);
    private readonly ConcurrentQueue<PendingCommand> _commandQueue = new();
    private readonly AutoResetEvent _commandSignal = new(false);

    private ISerialPort? _port;
    private CancellationTokenSource? _engineCts;
    private Task? _readLoopTask;
    private string? _connectedPortName;
    private int _connectedBaudRate = 115200;
    private int _plannerBufferSize = 4;
    private int _lastAcknowledgedLine;
    private int _streamUnackedLines;
    private TaskCompletionSource? _streamSlotAvailable;
    private volatile bool _isStreaming;

    public SerialEngine(ILogger<SerialEngine> logger, SerialOptions? options = null)
        : this(logger, static (port, baud) => new SystemSerialPortWrapper(port, baud), options)
    {
    }

    internal SerialEngine(
        ILogger<SerialEngine> logger,
        Func<string, int, ISerialPort> portFactory,
        SerialOptions? options = null,
        int plannerBufferSize = 4)
    {
        _logger = logger;
        _options = options ?? new SerialOptions();
        _portFactory = portFactory;
        _plannerBufferSize = plannerBufferSize;
        _okSynchronizer = new OkSynchronizer(TimeSpan.FromMilliseconds(_options.CommandTimeoutMs));
    }

    internal SerialEngine(
        ILogger<SerialEngine> logger,
        Func<string, int, ISerialPort> portFactory,
        int plannerBufferSize,
        TimeSpan? commandTimeout)
        : this(logger, portFactory, new SerialOptions { CommandTimeoutMs = (int)(commandTimeout?.TotalMilliseconds ?? 30000) }, plannerBufferSize)
    {
    }

    public ConnectionState State { get; private set; } = ConnectionState.Disconnected;

    public FirmwareInfo? FirmwareInfo { get; private set; }

    public event EventHandler<SerialLineEventArgs>? LineReceived;

    public event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;

    public event EventHandler<ConnectionLostEventArgs>? ConnectionLost;

    public event EventHandler<GCodeStreamProgressEventArgs>? StreamProgress;

    public async Task ConnectAsync(string portName, int baudRate, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(portName);

        await _commandGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (State is ConnectionState.Connected or ConnectionState.Connecting)
            {
                throw new SerialConnectionException("Serial port is already connected or connecting.");
            }

            SetState(ConnectionState.Connecting);
            _connectedPortName = portName;
            _connectedBaudRate = baudRate;

            _port = _portFactory(portName, baudRate);
            try
            {
                _port.Open();
            }
            catch (Exception ex)
            {
                SetState(ConnectionState.Error);
                throw new SerialConnectionException($"Failed to open serial port '{portName}'.", ex);
            }

            _engineCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var handshakeLines = await PerformHandshakeAsync(_engineCts.Token).ConfigureAwait(false);
            var parsed = _handshakeParser.Parse(handshakeLines)
                ?? throw new MarlinProtocolException("Empty handshake response from printer.");
            FirmwareInfo = parsed;

            if (!FirmwareInfo.IsMarlin)
            {
                SetState(ConnectionState.Error);
                throw new MarlinProtocolException("Connected device did not identify as Marlin firmware.");
            }

            _readLoopTask = Task.Run(() => ReadLoopAsync(_engineCts.Token), CancellationToken.None);
            SetState(ConnectionState.Connected);
            _logger.LogInformation(
                "Connected to {Port} at {BaudRate} baud ({Firmware})",
                portName,
                baudRate,
                FirmwareInfo.FirmwareName);
        }
        catch
        {
            await CleanupPortAsync().ConfigureAwait(false);
            throw;
        }
        finally
        {
            _commandGate.Release();
        }
    }

    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        await _commandGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await CleanupPortAsync().ConfigureAwait(false);
            SetState(ConnectionState.Disconnected);
            FirmwareInfo = null;
            _logger.LogInformation("Serial port disconnected.");
        }
        finally
        {
            _commandGate.Release();
        }
    }

    public async Task<string> SendCommandAsync(string command, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(command);

        if (State != ConnectionState.Connected || _port is null || !_port.IsOpen)
        {
            throw new SerialConnectionException("Serial port is not connected.");
        }

        var trimmed = command.Trim();

        if (IsEmergencyCommand(trimmed))
        {
            _okSynchronizer.BeginWait();
            WriteCommand(trimmed);
            await _okSynchronizer.WaitForOkAsync(cancellationToken).ConfigureAwait(false);
            return trimmed;
        }

        await _commandGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var pending = new PendingCommand(trimmed, new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously));
            _commandQueue.Enqueue(pending);
            _commandSignal.Set();

            using var registration = cancellationToken.Register(() => pending.Completion.TrySetCanceled(cancellationToken));
            return await pending.Completion.Task.ConfigureAwait(false);
        }
        finally
        {
            _commandGate.Release();
        }
    }

    public async Task StreamGCodeAsync(IAsyncEnumerable<GCodeLine> lines, CancellationToken cancellationToken = default)
    {
        if (State != ConnectionState.Connected || _port is null || !_port.IsOpen)
        {
            throw new SerialConnectionException("Serial port is not connected.");
        }

        await _commandGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            _isStreaming = true;
            var sentLines = 0;
            var totalLines = 0;
            var validLines = new List<GCodeLine>();

            await foreach (var line in lines.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                if (ShouldSkipLine(line))
                {
                    continue;
                }

                validLines.Add(line);
            }

            totalLines = validLines.Count;

            foreach (var line in validLines)
            {
                cancellationToken.ThrowIfCancellationRequested();

                while (_streamUnackedLines >= _plannerBufferSize)
                {
                    await WaitForStreamAckAsync(cancellationToken).ConfigureAwait(false);
                }

                WriteCommand(line.Content);
                _streamUnackedLines++;
                sentLines++;
                _lastAcknowledgedLine = line.LineNumber;

                var progress = totalLines == 0 ? 0 : sentLines * 100.0 / totalLines;
                StreamProgress?.Invoke(this, new GCodeStreamProgressEventArgs(line.LineNumber, sentLines, totalLines, progress));
            }

            while (_streamUnackedLines > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await WaitForStreamAckAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            _isStreaming = false;
            _commandGate.Release();
        }
    }

    public async Task ReconnectAsync(string? portName = null, int? baudRate = null, CancellationToken cancellationToken = default)
    {
        var targetPort = portName ?? _connectedPortName;
        var targetBaud = baudRate ?? _connectedBaudRate;

        if (string.IsNullOrWhiteSpace(targetPort))
        {
            throw new SerialConnectionException("No port name available for reconnect.");
        }

        var delays = new[] { TimeSpan.FromMilliseconds(250), TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1) };

        for (var attempt = 0; attempt < delays.Length; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                if (State is ConnectionState.Connected or ConnectionState.Connecting)
                {
                    await DisconnectAsync(cancellationToken).ConfigureAwait(false);
                }

                await ConnectAsync(targetPort, targetBaud, cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Reconnect succeeded on attempt {Attempt} to {Port}.", attempt + 1, targetPort);
                return;
            }
            catch (Exception ex) when (attempt < delays.Length - 1)
            {
                _logger.LogWarning(ex, "Reconnect attempt {Attempt} failed for {Port}. Retrying...", attempt + 1, targetPort);
                await Task.Delay(delays[attempt], cancellationToken).ConfigureAwait(false);
            }
        }

        throw new SerialConnectionException($"Failed to reconnect to '{targetPort}' after {delays.Length} attempts.");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _commandSignal.WaitOne(TimeSpan.FromMilliseconds(100));

            if (_port is null || !_port.IsOpen || State != ConnectionState.Connected)
            {
                DrainQueuedCommands(new SerialConnectionException("Serial port is not connected."));
                continue;
            }

            if (!_commandQueue.TryDequeue(out var pending))
            {
                continue;
            }

            try
            {
                var response = await ExecuteQueuedCommandAsync(pending.Command, stoppingToken).ConfigureAwait(false);
                pending.Completion.TrySetResult(response);
            }
            catch (Exception ex)
            {
                pending.Completion.TrySetException(ex);
            }
        }
    }

    private async Task<string> ExecuteQueuedCommandAsync(string command, CancellationToken cancellationToken)
    {
        if (_port is null || !_port.IsOpen)
        {
            throw new SerialConnectionException("Serial port is not connected.");
        }

        _okSynchronizer.BeginWait();
        WriteCommand(command);

        await _okSynchronizer.WaitForOkAsync(cancellationToken).ConfigureAwait(false);
        return command;
    }

    private async Task<IReadOnlyList<string>> PerformHandshakeAsync(CancellationToken cancellationToken)
    {
        using var handshakeTimeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        handshakeTimeout.CancelAfter(TimeSpan.FromMilliseconds(_options.HandshakeTimeoutMs));

        _okSynchronizer.BeginWait();
        WriteCommand("M115");

        var lines = new List<string>();
        var completed = false;

        while (!completed)
        {
            var line = await ReadLineFromPortAsync(handshakeTimeout.Token).ConfigureAwait(false);
            if (line is null)
            {
                continue;
            }

            var consumed = await _okSynchronizer.ProcessLineAsync(line, handshakeTimeout.Token).ConfigureAwait(false);
            if (!consumed)
            {
                lines.Add(line);
                RaiseLineReceived(line);
            }
            else if (MarlinResponseParser.IsOk(line))
            {
                completed = true;
            }
        }

        return lines;
    }

    private async Task ReadLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested && _port is not null && _port.IsOpen)
            {
                var line = await ReadLineFromPortAsync(cancellationToken).ConfigureAwait(false);
                if (line is null)
                {
                    continue;
                }

                if (_okSynchronizer.HasPending)
                {
                    var consumed = await _okSynchronizer.ProcessLineAsync(line, cancellationToken).ConfigureAwait(false);
                    if (consumed)
                    {
                        continue;
                    }
                }
                else if (_isStreaming && MarlinResponseParser.IsOk(line))
                {
                    ReleaseStreamSlot();
                    continue;
                }
                else if (MarlinResponseParser.IsOk(line))
                {
                    continue;
                }

                RaiseLineReceived(line);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (IOException ex)
        {
            HandleConnectionLost(ex);
        }
        catch (Exception ex)
        {
            HandleConnectionLost(ex);
        }
    }

    private async Task<string?> ReadLineFromPortAsync(CancellationToken cancellationToken)
    {
        if (_port is null)
        {
            return null;
        }

        try
        {
            return await _port.ReadLineAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (IOException ex)
        {
            HandleConnectionLost(ex);
            return null;
        }
    }

    private void WriteCommand(string command)
    {
        if (_port is null || !_port.IsOpen)
        {
            throw new SerialConnectionException("Serial port is not connected.");
        }

        try
        {
            _port.WriteLine(command);
        }
        catch (IOException ex)
        {
            HandleConnectionLost(ex);
            throw new SerialConnectionException("Failed to write command to serial port.", ex);
        }
    }

    private void HandleConnectionLost(Exception cause)
    {
        if (State == ConnectionState.Disconnected)
        {
            return;
        }

        var previous = State;
        SetState(ConnectionState.Error);
        _okSynchronizer.CancelPending(new SerialConnectionException("Serial connection lost.", cause));
        DrainQueuedCommands(new SerialConnectionException("Serial connection lost.", cause));

        _logger.LogError(cause, "Serial connection lost from state {State}.", previous);
        ConnectionLost?.Invoke(this, new ConnectionLostEventArgs(previous, _lastAcknowledgedLine, cause));
    }

    private async Task WaitForStreamAckAsync(CancellationToken cancellationToken)
    {
        if (_streamUnackedLines == 0)
        {
            return;
        }

        var waiter = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _streamSlotAvailable = waiter;

        using var registration = cancellationToken.Register(() => waiter.TrySetCanceled(cancellationToken));

        await waiter.Task.ConfigureAwait(false);
        _streamSlotAvailable = null;
    }

    private void ReleaseStreamSlot()
    {
        if (_streamUnackedLines <= 0)
        {
            return;
        }

        _streamUnackedLines--;
        _streamSlotAvailable?.TrySetResult();
    }

    private static bool ShouldSkipLine(GCodeLine line)
    {
        if (line.IsComment)
        {
            return true;
        }

        var content = line.Content.Trim();
        return content.Length == 0 || content.StartsWith(';');
    }

    private static bool IsEmergencyCommand(string command)
    {
        return command.Trim().StartsWith("M112", StringComparison.OrdinalIgnoreCase);
    }

    private void RaiseLineReceived(string line)
    {
        LineReceived?.Invoke(this, new SerialLineEventArgs(line, DateTimeOffset.UtcNow));
    }

    private void SetState(ConnectionState newState)
    {
        if (State == newState)
        {
            return;
        }

        var previous = State;
        State = newState;
        ConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(previous, newState));
    }

    private void DrainQueuedCommands(Exception exception)
    {
        while (_commandQueue.TryDequeue(out var pending))
        {
            pending.Completion.TrySetException(exception);
        }
    }

    private async Task CleanupPortAsync()
    {
        _engineCts?.Cancel();

        if (_readLoopTask is not null)
        {
            try
            {
                await _readLoopTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        _readLoopTask = null;
        _engineCts?.Dispose();
        _engineCts = null;

        if (_port is not null)
        {
            try
            {
                if (_port.IsOpen)
                {
                    _port.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error while closing serial port.");
            }

            _port.Dispose();
            _port = null;
        }

        _streamUnackedLines = 0;
        _streamSlotAvailable?.TrySetCanceled();
        _streamSlotAvailable = null;
        _okSynchronizer.CancelPending();
        DrainQueuedCommands(new SerialConnectionException("Serial port disconnected."));
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await DisconnectAsync(cancellationToken).ConfigureAwait(false);
        await base.StopAsync(cancellationToken).ConfigureAwait(false);
    }

    private sealed record PendingCommand(string Command, TaskCompletionSource<string> Completion);
}
