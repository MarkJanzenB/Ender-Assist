namespace MarlinPrintMiddleware.Core.Enums;

/// <summary>
/// Represents the active print job lifecycle state.
/// </summary>
public enum PrintState
{
    Idle,
    Preparing,
    Printing,
    Paused,
    Completed,
    Failed,
    Cancelled
}
