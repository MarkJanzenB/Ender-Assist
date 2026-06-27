namespace MarlinPrintMiddleware.Core.Models;

/// <summary>
/// Saved printer connection and protocol settings.
/// </summary>
public class PrinterProfile
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Port { get; set; } = string.Empty;

    public int BaudRate { get; set; } = 115200;

    public int BufferSize { get; set; } = 4;

    public bool IsDefault { get; set; }
}
