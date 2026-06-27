using MarlinPrintMiddleware.Core.Enums;

namespace MarlinPrintMiddleware.Core.Models;

/// <summary>
/// Aggregated snapshot of printer connection, temperature, and print progress.
/// </summary>
public class PrinterStatus
{
    public ConnectionState ConnectionState { get; set; } = ConnectionState.Disconnected;

    public PrintState PrintState { get; set; } = PrintState.Idle;

    public double HotendTemp { get; set; }

    public double BedTemp { get; set; }

    public double TargetHotend { get; set; }

    public double TargetBed { get; set; }

    /// <summary>
    /// Active job completion percentage from 0 to 100.
    /// </summary>
    public double Progress { get; set; }

    public string? CurrentJobName { get; set; }

    public TimeSpan Elapsed { get; set; }

    public TimeSpan? Eta { get; set; }
}
