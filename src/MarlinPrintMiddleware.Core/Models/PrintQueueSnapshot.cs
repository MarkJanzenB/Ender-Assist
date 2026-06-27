namespace MarlinPrintMiddleware.Core.Models;

/// <summary>
/// Immutable view of the print queue at a point in time.
/// </summary>
public class PrintQueueSnapshot
{
    public IReadOnlyList<PrintJob> Jobs { get; init; } = Array.Empty<PrintJob>();

    public long? CurrentJobId { get; init; }
}
