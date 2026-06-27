namespace MarlinPrintMiddleware.Core.Exceptions;

/// <summary>
/// Thrown when a print queue operation cannot be completed.
/// </summary>
public class PrintQueueException : Exception
{
    public PrintQueueException(string message)
        : base(message)
    {
    }

    public PrintQueueException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
