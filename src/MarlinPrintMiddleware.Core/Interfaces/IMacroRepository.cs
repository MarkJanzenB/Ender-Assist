using MarlinPrintMiddleware.Core.Models;

namespace MarlinPrintMiddleware.Core.Interfaces;

public interface IMacroRepository
{
    Task<IReadOnlyList<PrinterMacro>> GetAllAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(IReadOnlyList<PrinterMacro> macros, CancellationToken cancellationToken = default);
}
