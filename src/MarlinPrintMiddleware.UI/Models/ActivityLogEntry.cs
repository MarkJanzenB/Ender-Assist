namespace MarlinPrintMiddleware.UI.Models;

public sealed class ActivityLogEntry
{
    public ActivityLogEntry(string message, bool isError = false)
    {
        Timestamp = DateTime.Now;
        Message = message;
        IsError = isError;
    }

    public DateTime Timestamp { get; }

    public string Message { get; }

    public bool IsError { get; }

    public string Display => $"[{Timestamp:HH:mm:ss}] {Message}";
}
