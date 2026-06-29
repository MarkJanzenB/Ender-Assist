using MarlinPrintMiddleware.Core.Enums;
using MarlinPrintMiddleware.Core.Models;

namespace MarlinPrintMiddleware.Core.Interfaces;

/// <summary>
/// Manages the print job queue and active print lifecycle.
/// </summary>
public interface IPrintQueueService
{
    /// <summary>
    /// Current print lifecycle state.
    /// </summary>
    PrintState PrintState { get; }

    /// <summary>
    /// Adds a job to the queue.
    /// </summary>
    /// <param name="job">Job to enqueue.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task EnqueueAsync(PrintJob job, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts the next pending job in the queue.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task StartNextAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Pauses the currently active print job.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task PauseAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Resumes a paused print job.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ResumeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels the currently active print job.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task CancelAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts a specific pending job by identifier.
    /// </summary>
    Task StartJobAsync(long jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a pending job from the queue.
    /// </summary>
    Task RemoveJobAsync(long jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all completed and cancelled jobs from persistence.
    /// </summary>
    Task ClearCompletedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets a failed job back to pending for retry.
    /// </summary>
    Task RetryFailedJobAsync(long jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reorders a pending job to a new queue index (0-based).
    /// </summary>
    Task ReorderJobAsync(long jobId, int newIndex, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a point-in-time snapshot of the queue.
    /// </summary>
    PrintQueueSnapshot GetSnapshot();
}
