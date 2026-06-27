namespace MarlinPrintMiddleware.Core.Models;

/// <summary>
/// Firmware metadata returned from a Marlin handshake (M115).
/// </summary>
public class FirmwareInfo
{
    public string FirmwareName { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    public string MachineType { get; set; } = string.Empty;

    public bool IsMarlin { get; set; }
}
