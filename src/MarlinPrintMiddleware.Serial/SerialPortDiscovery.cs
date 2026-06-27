using System.IO.Ports;
using MarlinPrintMiddleware.Core.Interfaces;
using MarlinPrintMiddleware.Core.Models;

namespace MarlinPrintMiddleware.Serial;

/// <summary>
/// Enumerates available serial ports via <see cref="SerialPort.GetPortNames"/>.
/// </summary>
public sealed class SerialPortDiscovery : ISerialPortDiscovery
{
    private readonly object _lock = new();
    private bool _hasCachedPorts;
    private IReadOnlyList<SerialPortInfo> _cachedPorts = Array.Empty<SerialPortInfo>();

    public Task<IReadOnlyList<SerialPortInfo>> GetPortsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_lock)
        {
            if (!_hasCachedPorts)
            {
                _cachedPorts = EnumeratePorts();
                _hasCachedPorts = true;
            }

            return Task.FromResult(_cachedPorts);
        }
    }

    public Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_lock)
        {
            _cachedPorts = EnumeratePorts();
            _hasCachedPorts = true;
        }

        return Task.CompletedTask;
    }

    private static IReadOnlyList<SerialPortInfo> EnumeratePorts()
    {
        var portNames = SerialPort.GetPortNames();

        if (portNames.Length == 0)
        {
            return new List<SerialPortInfo>();
        }

        return portNames
            .OrderBy(static name => name, StringComparer.OrdinalIgnoreCase)
            .Select(static portName => new SerialPortInfo
            {
                PortName = portName,
                Description = "USB Serial Device",
            })
            .ToList();
    }
}
