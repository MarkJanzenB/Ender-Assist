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

    public bool HasLiveTemperature { get; set; }

    public double? PositionX { get; set; }

    public double? PositionY { get; set; }

    public double? PositionZ { get; set; }

    public double? PositionE { get; set; }

    public bool HasLivePosition { get; set; }

    public double? FanSpeedPercent { get; set; }

    public bool HasLiveFan { get; set; }

    public int? CurrentLayer { get; set; }

    public int? TotalLayers { get; set; }
}
