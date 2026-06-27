namespace MarlinPrintMiddleware.Core.Interfaces;

/// <summary>
/// Emergency stop control for the active printer.
/// </summary>
public interface IEmergencyStopService
{
    Task TriggerAsync(CancellationToken cancellationToken = default);
}
