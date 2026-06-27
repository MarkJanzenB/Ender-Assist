using MarlinPrintMiddleware.Core.Models;

namespace MarlinPrintMiddleware.Core.Interfaces;

/// <summary>
/// Persistence contract for printer profile records.
/// </summary>
public interface IPrinterProfileRepository
{
    /// <summary>
    /// Retrieves a profile by identifier.
    /// </summary>
    Task<PrinterProfile?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all stored profiles.
    /// </summary>
    Task<IReadOnlyList<PrinterProfile>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the default profile, if one is configured.
    /// </summary>
    Task<PrinterProfile?> GetDefaultAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts a new profile record.
    /// </summary>
    Task<PrinterProfile> CreateAsync(PrinterProfile profile, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing profile record.
    /// </summary>
    Task UpdateAsync(PrinterProfile profile, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a profile record by identifier.
    /// </summary>
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
