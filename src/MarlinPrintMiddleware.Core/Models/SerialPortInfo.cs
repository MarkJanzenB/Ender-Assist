namespace MarlinPrintMiddleware.Core.Models;

/// <summary>
/// Describes an available serial port discovered on the host.
/// </summary>
public class SerialPortInfo
{
    public string PortName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
