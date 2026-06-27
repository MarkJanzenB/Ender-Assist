using MarlinPrintMiddleware.Core.Enums;

namespace MarlinPrintMiddleware.Core.Events;

/// <summary>
/// Event data when the serial connection is lost unexpectedly.
/// </summary>
public class ConnectionLostEventArgs : EventArgs
{
    public ConnectionLostEventArgs(ConnectionState lastState, int lastAcknowledgedLine, Exception? cause = null)
    {
        LastState = lastState;
        LastAcknowledgedLine = lastAcknowledgedLine;
        Cause = cause;
    }

    public ConnectionState LastState { get; }

    public int LastAcknowledgedLine { get; }

    public Exception? Cause { get; }
}
