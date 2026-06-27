using System.IO.Ports;
using System.Text;

namespace MarlinPrintMiddleware.Serial;

/// <summary>
/// Abstraction over System.IO.Ports.SerialPort for testability.
/// </summary>
public interface ISerialPort : IDisposable
{
    bool IsOpen { get; }

    string PortName { get; }

    int BaudRate { get; set; }

    int ReadTimeout { get; set; }

    int WriteTimeout { get; set; }

    void Open();

    void Close();

    void WriteLine(string text);

    Task<string?> ReadLineAsync(CancellationToken cancellationToken);
}

internal sealed class SystemSerialPortWrapper : ISerialPort
{
    private readonly SerialPort _port;

    public SystemSerialPortWrapper(string portName, int baudRate)
    {
        _port = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One)
        {
            NewLine = "\n",
            Encoding = Encoding.ASCII,
            ReadTimeout = 1000,
            WriteTimeout = 5000,
        };
    }

    public bool IsOpen => _port.IsOpen;

    public string PortName => _port.PortName;

    public int BaudRate
    {
        get => _port.BaudRate;
        set => _port.BaudRate = value;
    }

    public int ReadTimeout
    {
        get => _port.ReadTimeout;
        set => _port.ReadTimeout = value;
    }

    public int WriteTimeout
    {
        get => _port.WriteTimeout;
        set => _port.WriteTimeout = value;
    }

    public void Open() => _port.Open();

    public void Close() => _port.Close();

    public void WriteLine(string text) => _port.WriteLine(text);

    public Task<string?> ReadLineAsync(CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                return _port.ReadLine();
            }
            catch (TimeoutException)
            {
                return null;
            }
        }, cancellationToken);
    }

    public void Dispose()
    {
        if (_port.IsOpen)
        {
            _port.Close();
        }

        _port.Dispose();
    }
}
