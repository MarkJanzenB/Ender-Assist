using MarlinPrintMiddleware.Core.Interfaces;
using MarlinPrintMiddleware.Core.Models;

namespace MarlinPrintMiddleware.App;

internal sealed class StubPrintJobRepository : IPrintJobRepository
{
    public Task<PrintJob> CreateAsync(PrintJob job, CancellationToken cancellationToken = default) =>
        Task.FromResult(job);

    public Task DeleteAsync(long id, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task<IReadOnlyList<PrintJob>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<PrintJob>>([]);

    public Task<PrintJob?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        Task.FromResult<PrintJob?>(null);

    public Task<IReadOnlyList<PrintJob>> GetPendingAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<PrintJob>>([]);

    public Task UpdateAsync(PrintJob job, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task UpsertAsync(PrintJob job, CancellationToken cancellationToken = default) => Task.CompletedTask;
}
