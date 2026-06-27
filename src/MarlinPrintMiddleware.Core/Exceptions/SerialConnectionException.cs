namespace MarlinPrintMiddleware.Core.Exceptions;

/// <summary>
/// Thrown when a serial port cannot be opened or communication fails.
/// </summary>
public class SerialConnectionException : Exception
{
    public SerialConnectionException()
    {
    }

    public SerialConnectionException(string message)
        : base(message)
    {
    }

    public SerialConnectionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
