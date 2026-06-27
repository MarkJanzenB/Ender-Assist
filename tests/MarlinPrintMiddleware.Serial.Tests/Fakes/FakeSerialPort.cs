using System.Collections.Concurrent;
using MarlinPrintMiddleware.Serial;

namespace MarlinPrintMiddleware.Serial.Tests.Fakes;

/// <summary>
/// Scriptable in-memory serial port for unit tests.
/// </summary>
public sealed class FakeSerialPort : ISerialPort
{
    private readonly ConcurrentQueue<string> _responses = new();
    private readonly List<string> _writtenLines = new();
    private readonly object _sync = new();
    private bool _isOpen;
    private bool _throwOnRead;
    private bool _throwOnWrite;

    public FakeSerialPort(string portName = "COM_FAKE", int baudRate = 115200)
    {
        PortName = portName;
        BaudRate = baudRate;
    }

    public bool IsOpen => _isOpen;

    public string PortName { get; }

    public int BaudRate { get; set; }

    public int ReadTimeout { get; set; } = 500;

    public int WriteTimeout { get; set; } = 5000;

    public IReadOnlyList<string> WrittenLines
    {
        get
        {
            lock (_sync)
            {
                return _writtenLines.ToArray();
            }
        }
    }

    public void EnqueueResponse(params string[] lines)
    {
        foreach (var line in lines)
        {
            _responses.Enqueue(line);
        }
    }

    public void EnqueueOk() => EnqueueResponse("ok");

    public void ScriptCommand(string command, params string[] responses)
    {
        _commandScripts[command.Trim()] = responses;
    }

    private readonly Dictionary<string, string[]> _commandScripts = new(StringComparer.OrdinalIgnoreCase);

    public void SimulateDisconnectOnNextRead() => _throwOnRead = true;

    public void SimulateDisconnectOnNextWrite() => _throwOnWrite = true;

    public void Open() => _isOpen = true;

    public void Close() => _isOpen = false;

    public void WriteLine(string text)
    {
        if (_throwOnWrite)
        {
            _throwOnWrite = false;
            throw new IOException("Simulated write failure.");
        }

        lock (_sync)
        {
            _writtenLines.Add(text);
        }

        if (_commandScripts.TryGetValue(text.Trim(), out var scripted))
        {
            EnqueueResponse(scripted);
            return;
        }

        EnqueueOk();
    }

    public async Task<string?> ReadLineAsync(CancellationToken cancellationToken)
    {
        if (_throwOnRead)
        {
            _throwOnRead = false;
            throw new IOException("Simulated read failure.");
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            if (_throwOnRead)
            {
                _throwOnRead = false;
                throw new IOException("Simulated read failure.");
            }

            if (_responses.TryDequeue(out var line))
            {
                return line;
            }

            await Task.Delay(10, cancellationToken).ConfigureAwait(false);
        }

        cancellationToken.ThrowIfCancellationRequested();
        return null;
    }

    public void Dispose() => Close();
}
