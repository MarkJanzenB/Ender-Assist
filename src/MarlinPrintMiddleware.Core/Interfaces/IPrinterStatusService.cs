using MarlinPrintMiddleware.Core.Models;

namespace MarlinPrintMiddleware.Core.Interfaces;

/// <summary>
/// Aggregated printer status for UI binding.
/// </summary>
public interface IPrinterStatusService
{
    PrinterStatus CurrentStatus { get; }

    event EventHandler<PrinterStatus>? StatusChanged;

    Task RefreshPositionAsync(CancellationToken cancellationToken = default);
}
