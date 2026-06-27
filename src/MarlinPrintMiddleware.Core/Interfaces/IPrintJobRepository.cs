using MarlinPrintMiddleware.Core.Models;

namespace MarlinPrintMiddleware.Core.Interfaces;

/// <summary>
/// Persistence contract for print job records.
/// </summary>
public interface IPrintJobRepository
{
    /// <summary>
    /// Retrieves a job by identifier.
    /// </summary>
    Task<PrintJob?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all stored jobs.
    /// </summary>
    Task<IReadOnlyList<PrintJob>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves jobs awaiting execution.
    /// </summary>
    Task<IReadOnlyList<PrintJob>> GetPendingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts a new job record.
    /// </summary>
    Task<PrintJob> CreateAsync(PrintJob job, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing job record.
    /// </summary>
    Task UpdateAsync(PrintJob job, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts or updates a job record.
    /// </summary>
    Task UpsertAsync(PrintJob job, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a job record by identifier.
    /// </summary>
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
