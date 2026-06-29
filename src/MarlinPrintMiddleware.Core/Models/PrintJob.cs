using MarlinPrintMiddleware.Core.Enums;

namespace MarlinPrintMiddleware.Core.Models;

/// <summary>
/// A G-code print job tracked by the queue and persistence layers.
/// </summary>
public class PrintJob
{
    public long Id { get; set; }

    public string FilePath { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public JobStatus Status { get; set; } = JobStatus.Pending;

    public JobPriority Priority { get; set; } = JobPriority.Normal;

    /// <summary>
    /// Completion percentage from 0 to 100.
    /// </summary>
    public double Progress { get; set; }

    public int TotalLines { get; set; }

    public int LastLineSent { get; set; }

    public int QueueOrder { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? StartedAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public string? ErrorMessage { get; set; }

    public int? EstimatedDurationSeconds { get; set; }

    public double? FilamentGrams { get; set; }

    public int? TotalLayers { get; set; }
}
