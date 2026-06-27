using MarlinPrintMiddleware.Core.Enums;
using MarlinPrintMiddleware.Core.Events;
using MarlinPrintMiddleware.Core.Models;

namespace MarlinPrintMiddleware.Core.Interfaces;

/// <summary>
/// Abstraction for serial port communication with a Marlin-compatible printer.
/// </summary>
public interface ISerialEngine
{
    /// <summary>
    /// Gets the current connection state.
    /// </summary>
    ConnectionState State { get; }

    /// <summary>
    /// Gets firmware metadata from the last successful handshake, if available.
    /// </summary>
    FirmwareInfo? FirmwareInfo { get; }

    /// <summary>
    /// Raised when a complete line is received from the printer.
    /// </summary>
    event EventHandler<SerialLineEventArgs>? LineReceived;

    /// <summary>
    /// Raised when the connection state changes.
    /// </summary>
    event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;

    /// <summary>
    /// Raised when the serial connection is lost unexpectedly.
    /// </summary>
    event EventHandler<ConnectionLostEventArgs>? ConnectionLost;

    /// <summary>
    /// Raised as G-code lines are streamed to the printer.
    /// </summary>
    event EventHandler<GCodeStreamProgressEventArgs>? StreamProgress;

    /// <summary>
    /// Opens the serial port and performs firmware handshake.
    /// </summary>
    /// <param name="portName">COM port name (e.g. COM3).</param>
    /// <param name="baudRate">Baud rate for the connection.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ConnectAsync(string portName, int baudRate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Closes the serial port and resets connection state.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DisconnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a single command and waits for the printer response.
    /// </summary>
    /// <param name="command">G-code or M-code command without line number.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The primary response line from the printer.</returns>
    Task<string> SendCommandAsync(string command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Streams G-code lines to the printer with buffer-aware flow control.
    /// </summary>
    /// <param name="lines">Async sequence of G-code lines to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task StreamGCodeAsync(IAsyncEnumerable<GCodeLine> lines, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to restore a lost serial connection and re-handshake with the printer.
    /// Does not auto-resume an interrupted print.
    /// </summary>
    /// <param name="portName">Optional port name; uses the last connected port when null.</param>
    /// <param name="baudRate">Optional baud rate; uses the last connected baud rate when null.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ReconnectAsync(string? portName = null, int? baudRate = null, CancellationToken cancellationToken = default);
}
