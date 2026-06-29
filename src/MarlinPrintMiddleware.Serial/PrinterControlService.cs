using MarlinPrintMiddleware.Core.Enums;
using MarlinPrintMiddleware.Core.Exceptions;
using MarlinPrintMiddleware.Core.Interfaces;

namespace MarlinPrintMiddleware.Serial;

public sealed class PrinterControlService : IPrinterControlService
{
    private readonly ISerialEngine _serialEngine;

    public PrinterControlService(ISerialEngine serialEngine)
    {
        _serialEngine = serialEngine;
    }

    public Task HomeAllAsync(CancellationToken cancellationToken = default) =>
        SendWhenConnected("G28", cancellationToken);

    public Task DisableMotorsAsync(CancellationToken cancellationToken = default) =>
        SendWhenConnected("M84", cancellationToken);

    public async Task PreheatPlaAsync(CancellationToken cancellationToken = default)
    {
        await SendWhenConnected("M140 S60", cancellationToken).ConfigureAwait(false);
        await SendWhenConnected("M104 S200", cancellationToken).ConfigureAwait(false);
    }

    public async Task PreheatPetgAsync(CancellationToken cancellationToken = default)
    {
        await SendWhenConnected("M140 S80", cancellationToken).ConfigureAwait(false);
        await SendWhenConnected("M104 S230", cancellationToken).ConfigureAwait(false);
    }

    public async Task CooldownAsync(CancellationToken cancellationToken = default)
    {
        await SendWhenConnected("M104 S0", cancellationToken).ConfigureAwait(false);
        await SendWhenConnected("M140 S0", cancellationToken).ConfigureAwait(false);
    }

    public async Task JogAsync(char axis, double deltaMm, CancellationToken cancellationToken = default)
    {
        var normalized = char.ToUpperInvariant(axis);
        if (normalized is not ('X' or 'Y' or 'Z'))
        {
            throw new PrintQueueException($"Unsupported jog axis: {axis}");
        }

        await SendWhenConnected("G91", cancellationToken).ConfigureAwait(false);
        await SendWhenConnected($"G1 {normalized}{deltaMm:F2} F3000", cancellationToken).ConfigureAwait(false);
        await SendWhenConnected("G90", cancellationToken).ConfigureAwait(false);
    }

    private Task SendWhenConnected(string command, CancellationToken cancellationToken)
    {
        if (_serialEngine.State != ConnectionState.Connected)
        {
            throw new PrintQueueException("Printer is not connected.");
        }

        return _serialEngine.SendCommandAsync(command, cancellationToken);
    }
}
