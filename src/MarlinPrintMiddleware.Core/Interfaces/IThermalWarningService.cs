using MarlinPrintMiddleware.Core.Events;

namespace MarlinPrintMiddleware.Core.Interfaces;

/// <summary>
/// Surfaces thermal safety warnings to the UI layer.
/// </summary>
public interface IThermalWarningService
{
    event EventHandler<ThermalWarningEventArgs>? ThermalWarning;
}
