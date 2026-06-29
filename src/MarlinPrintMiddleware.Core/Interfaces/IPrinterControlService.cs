namespace MarlinPrintMiddleware.Core.Interfaces;

public interface IPrinterControlService
{
    Task HomeAllAsync(CancellationToken cancellationToken = default);

    Task DisableMotorsAsync(CancellationToken cancellationToken = default);

    Task PreheatPlaAsync(CancellationToken cancellationToken = default);

    Task PreheatPetgAsync(CancellationToken cancellationToken = default);

    Task CooldownAsync(CancellationToken cancellationToken = default);

    Task JogAsync(char axis, double deltaMm, CancellationToken cancellationToken = default);
}
