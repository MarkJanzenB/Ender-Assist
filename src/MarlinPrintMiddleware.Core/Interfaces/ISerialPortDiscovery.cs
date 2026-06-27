using MarlinPrintMiddleware.Core.Models;

namespace MarlinPrintMiddleware.Core.Interfaces;

/// <summary>
/// Discovers available serial ports on the host machine.
/// </summary>
public interface ISerialPortDiscovery
{
    /// <summary>
    /// Returns the cached or freshly discovered list of serial ports.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<IReadOnlyList<SerialPortInfo>> GetPortsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes the internal port cache from the operating system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task RefreshAsync(CancellationToken cancellationToken = default);
}
