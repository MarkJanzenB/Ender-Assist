using MarlinPrintMiddleware.Core.Enums;

namespace MarlinPrintMiddleware.Core.Events;

/// <summary>
/// Event data for a serial connection state transition.
/// </summary>
public class ConnectionStateChangedEventArgs : EventArgs
{
    public ConnectionStateChangedEventArgs(ConnectionState previousState, ConnectionState currentState)
    {
        PreviousState = previousState;
        CurrentState = currentState;
    }

    public ConnectionState PreviousState { get; }

    public ConnectionState CurrentState { get; }
}
