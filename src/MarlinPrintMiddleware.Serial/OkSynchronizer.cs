using MarlinPrintMiddleware.Core.Exceptions;
using MarlinPrintMiddleware.Serial.Parsers;

namespace MarlinPrintMiddleware.Serial;

/// <summary>
/// Waits for Marlin ok responses after a command, handling busy backoff and protocol errors.
/// </summary>
internal sealed class OkSynchronizer
{
    private readonly TimeSpan _commandTimeout;
    private readonly int _busyBackoffMs;
    private readonly int _maxBusyRetries;
    private readonly object _sync = new();

    private TaskCompletionSource<string?>? _pending;
    private int _busyRetries;
    private string? _lastResponseLine;

    public OkSynchronizer(TimeSpan? commandTimeout = null, int busyBackoffMs = 100, int maxBusyRetries = 50)
    {
        _commandTimeout = commandTimeout ?? TimeSpan.FromSeconds(30);
        _busyBackoffMs = busyBackoffMs;
        _maxBusyRetries = maxBusyRetries;
    }

    public void BeginWait()
    {
        lock (_sync)
        {
            _pending = new TaskCompletionSource<string?>(TaskCreationOptions.RunContinuationsAsynchronously);
            _busyRetries = 0;
            _lastResponseLine = null;
        }
    }

    public async Task<string?> WaitForOkAsync(CancellationToken cancellationToken)
    {
        TaskCompletionSource<string?>? pending;
        lock (_sync)
        {
            pending = _pending ?? throw new InvalidOperationException("BeginWait must be called before waiting for ok.");
        }

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(_commandTimeout);

        try
        {
            return await pending.Task.WaitAsync(timeoutCts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            lock (_sync)
            {
                _pending = null;
            }

            throw new MarlinProtocolException($"Timed out waiting for ok after {_commandTimeout.TotalSeconds:0}s.");
        }
    }

    /// <summary>
    /// Processes an incoming line. Returns true when the line was consumed by command synchronization.
    /// </summary>
    public async Task<bool> ProcessLineAsync(string line, CancellationToken cancellationToken)
    {
        TaskCompletionSource<string?>? pending;
        lock (_sync)
        {
            pending = _pending;
        }

        if (pending is null)
        {
            return false;
        }

        if (MarlinResponseParser.IsError(line))
        {
            var message = MarlinResponseParser.GetErrorMessage(line);
            var exception = new MarlinProtocolException(string.IsNullOrWhiteSpace(message) ? line.Trim() : message);
            lock (_sync)
            {
                _pending = null;
            }

            pending.TrySetException(exception);
            throw exception;
        }

        if (MarlinResponseParser.IsBusy(line))
        {
            int retries;
            lock (_sync)
            {
                _busyRetries++;
                retries = _busyRetries;
            }

            if (retries > _maxBusyRetries)
            {
                var exception = new MarlinProtocolException($"Printer remained busy after {_maxBusyRetries} retries.");
                lock (_sync)
                {
                    _pending = null;
                }

                pending.TrySetException(exception);
                throw exception;
            }

            await Task.Delay(_busyBackoffMs, cancellationToken).ConfigureAwait(false);
            return true;
        }

        if (MarlinResponseParser.IsOk(line))
        {
            lock (_sync)
            {
                _lastResponseLine = line;
                _pending = null;
            }

            pending.TrySetResult(line);
            return true;
        }

        _lastResponseLine = line;
        return false;
    }

    public void CancelPending(Exception? exception = null)
    {
        lock (_sync)
        {
            if (_pending is null)
            {
                return;
            }

            if (exception is null)
            {
                _pending.TrySetCanceled();
            }
            else
            {
                _pending.TrySetException(exception);
            }

            _pending = null;
        }
    }

    public bool HasPending => _pending is not null;
}
