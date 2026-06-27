namespace MarlinPrintMiddleware.Core.Events;

/// <summary>
/// Raised when thermal anomaly is detected during a print.
/// </summary>
public class ThermalWarningEventArgs : EventArgs
{
    public ThermalWarningEventArgs(string message)
    {
        Message = message;
    }

    public string Message { get; }
}
