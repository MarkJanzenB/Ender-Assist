namespace MarlinPrintMiddleware.Queue;

/// <summary>
/// Metadata extracted from a G-code file without loading all lines into memory.
/// </summary>
public sealed class GCodeFileInfo
{
    public string FilePath { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public int TotalLines { get; init; }

    public int PrintableLines { get; init; }

    public string? SlicerName { get; init; }

    public TimeSpan? EstimatedDuration { get; init; }

    public int? TotalLayers { get; init; }

    public double? FilamentGrams { get; init; }
}
