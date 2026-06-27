using MarlinPrintMiddleware.Core.Models;

namespace MarlinPrintMiddleware.Core.Interfaces;

/// <summary>
/// Manages the print job queue and active print lifecycle.
/// </summary>
public interface IPrintQueueService
{
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
    /// Returns a point-in-time snapshot of the queue.
    /// </summary>
    PrintQueueSnapshot GetSnapshot();
}
