using MarlinPrintMiddleware.Core.Interfaces;
using MarlinPrintMiddleware.Core.Models;

namespace MarlinPrintMiddleware.App;

internal sealed class StubPrinterProfileRepository : IPrinterProfileRepository
{
    public Task<PrinterProfile> CreateAsync(PrinterProfile profile, CancellationToken cancellationToken = default) =>
        Task.FromResult(profile);

    public Task DeleteAsync(long id, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task<IReadOnlyList<PrinterProfile>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<PrinterProfile>>([]);

    public Task<PrinterProfile?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        Task.FromResult<PrinterProfile?>(null);

    public Task<PrinterProfile?> GetDefaultAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<PrinterProfile?>(new PrinterProfile
        {
            Id = 1,
            Name = "Ender 3 V2",
            BaudRate = 115200,
            BufferSize = 4,
            IsDefault = true
        });

    public Task UpdateAsync(PrinterProfile profile, CancellationToken cancellationToken = default) => Task.CompletedTask;
}
