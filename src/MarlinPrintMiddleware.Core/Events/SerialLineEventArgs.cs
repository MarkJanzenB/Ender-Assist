namespace MarlinPrintMiddleware.Core.Events;

/// <summary>
/// Event data for a line received from the serial port.
/// </summary>
public class SerialLineEventArgs : EventArgs
{
    public SerialLineEventArgs(string line, DateTimeOffset receivedAt)
    {
        Line = line;
        ReceivedAt = receivedAt;
    }

    public string Line { get; }

    public DateTimeOffset ReceivedAt { get; }
}
