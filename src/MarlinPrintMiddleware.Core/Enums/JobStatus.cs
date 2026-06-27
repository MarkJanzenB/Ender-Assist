namespace MarlinPrintMiddleware.Core.Enums;

/// <summary>
/// Persistence and queue status of an individual print job.
/// </summary>
public enum JobStatus
{
    Pending,
    Printing,
    Completed,
    Failed,
    Cancelled
}
