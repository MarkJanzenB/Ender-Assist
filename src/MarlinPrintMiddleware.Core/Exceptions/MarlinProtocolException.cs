namespace MarlinPrintMiddleware.Core.Exceptions;

/// <summary>
/// Thrown when the printer returns an unexpected or invalid Marlin protocol response.
/// </summary>
public class MarlinProtocolException : Exception
{
    public MarlinProtocolException()
    {
    }

    public MarlinProtocolException(string message)
        : base(message)
    {
    }

    public MarlinProtocolException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
