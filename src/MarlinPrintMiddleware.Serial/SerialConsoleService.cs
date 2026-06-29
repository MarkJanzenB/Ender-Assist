using MarlinPrintMiddleware.Core.Enums;
using MarlinPrintMiddleware.Core.Events;
using MarlinPrintMiddleware.Core.Interfaces;
using MarlinPrintMiddleware.Core.Models;

namespace MarlinPrintMiddleware.Serial;

public sealed class SerialConsoleService : ISerialConsoleService
{
    private const int MaxLines = 500;

    private readonly ISerialEngine _serialEngine;
    private readonly List<ConsoleLine> _lines = [];
    private readonly object _sync = new();

    public SerialConsoleService(ISerialEngine serialEngine)
    {
        _serialEngine = serialEngine;
        _serialEngine.LineReceived += OnLineReceived;
        _serialEngine.ConnectionStateChanged += OnConnectionStateChanged;
    }

    public IReadOnlyList<ConsoleLine> Lines
    {
        get
        {
            lock (_sync)
            {
                return _lines.Where(PassesFilter).ToList();
            }
        }
    }

    public ConsoleLogFilter Filter { get; set; } = ConsoleLogFilter.All;

    public event EventHandler? LinesChanged;

    public async Task SendAsync(string command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            return;
        }

        if (_serialEngine.State != ConnectionState.Connected)
        {
            AddLine(command.Trim(), ConsoleLineDirection.System, "Not connected — command not sent.");
            return;
        }

        var trimmed = command.Trim();
        AddLine(trimmed, ConsoleLineDirection.Tx);

        try
        {
            await _serialEngine.SendCommandAsync(trimmed, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            AddLine(trimmed, ConsoleLineDirection.System, $"Error: {ex.Message}");
        }
    }

    public void Clear()
    {
        lock (_sync)
        {
            _lines.Clear();
        }

        LinesChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnLineReceived(object? sender, SerialLineEventArgs e)
    {
        AddLine(e.Line, ConsoleLineDirection.Rx, timestamp: e.ReceivedAt);
    }

    private void OnConnectionStateChanged(object? sender, ConnectionStateChangedEventArgs e)
    {
        if (e.CurrentState == ConnectionState.Connected)
        {
            AddLine(string.Empty, ConsoleLineDirection.System, "Serial connected.");
        }
        else if (e.PreviousState == ConnectionState.Connected)
        {
            AddLine(string.Empty, ConsoleLineDirection.System, "Serial disconnected.");
        }
    }

    private void AddLine(string text, ConsoleLineDirection direction, string? overrideText = null, DateTimeOffset? timestamp = null)
    {
        var line = new ConsoleLine
        {
            Text = overrideText ?? text,
            Direction = direction,
            Timestamp = timestamp ?? DateTimeOffset.UtcNow
        };

        lock (_sync)
        {
            _lines.Add(line);
            while (_lines.Count > MaxLines)
            {
                _lines.RemoveAt(0);
            }
        }

        LinesChanged?.Invoke(this, EventArgs.Empty);
    }

    private bool PassesFilter(ConsoleLine line) => Filter switch
    {
        ConsoleLogFilter.Rx => line.Direction == ConsoleLineDirection.Rx,
        ConsoleLogFilter.Tx => line.Direction == ConsoleLineDirection.Tx,
        ConsoleLogFilter.Errors => line.Direction == ConsoleLineDirection.System
            || line.Text.Contains("error", StringComparison.OrdinalIgnoreCase),
        _ => true
    };
}
