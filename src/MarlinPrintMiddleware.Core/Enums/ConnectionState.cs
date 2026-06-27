namespace MarlinPrintMiddleware.Core.Enums;

/// <summary>
/// Represents the serial connection lifecycle state.
/// </summary>
public enum ConnectionState
{
    Disconnected,
    Connecting,
    Connected,
    Error
}
