namespace MarlinPrintMiddleware.Serial;

/// <summary>
/// Configurable timeouts for serial communication.
/// </summary>
public class SerialOptions
{
    public int HandshakeTimeoutMs { get; set; } = 5000;

    public int CommandTimeoutMs { get; set; } = 30000;
}
