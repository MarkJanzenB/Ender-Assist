using MarlinPrintMiddleware.Core.Enums;
using MarlinPrintMiddleware.Core.Events;
using MarlinPrintMiddleware.Core.Exceptions;
using MarlinPrintMiddleware.Core.Interfaces;
using MarlinPrintMiddleware.Core.Models;
using Microsoft.Extensions.Logging;

namespace MarlinPrintMiddleware.Queue;

public sealed class PrintQueueService : IPrintQueueService
{
    public const string AutoStartSettingKey = "queue.auto_start";

    private readonly IPrintJobRepository _jobRepository;
    private readonly ISerialEngine _serialEngine;
    private readonly ISettingsRepository _settingsRepository;
    private readonly GCodeParser _gcodeParser;
    private readonly ILogger<PrintQueueService> _logger;
    private readonly PrintStateMachine _stateMachine = new();
    private readonly SemaphoreSlim _gate = new(1, 1);

    private PrintJob? _currentJob;
    private CancellationTokenSource? _printCts;

    public PrintQueueService(
        IPrintJobRepository jobRepository,
        ISerialEngine serialEngine,
        ISettingsRepository settingsRepository,
        GCodeParser gcodeParser,
        ILogger<PrintQueueService> logger)
    {
        _jobRepository = jobRepository;
        _serialEngine = serialEngine;
        _settingsRepository = settingsRepository;
        _gcodeParser = gcodeParser;
        _logger = logger;

        _serialEngine.StreamProgress += OnStreamProgress;
    }

    public PrintState PrintState => _stateMachine.State;

    public async Task EnqueueAsync(PrintJob job, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var info = await _gcodeParser.AnalyzeAsync(job.FilePath, cancellationToken).ConfigureAwait(false);
            job.Name = string.IsNullOrWhiteSpace(job.Name) ? info.Name : job.Name;
            job.Status = JobStatus.Pending;
            job.Progress = 0;
            job.TotalLines = info.PrintableLines;
            job.LastLineSent = 0;
            job.CreatedAt = DateTimeOffset.UtcNow;

            var pending = await _jobRepository.GetPendingAsync(cancellationToken).ConfigureAwait(false);
            job.QueueOrder = pending.Count;

            await _jobRepository.CreateAsync(job, cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Enqueued job {Name} ({Lines} lines)", job.Name, job.TotalLines);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task StartNextAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_serialEngine.State != ConnectionState.Connected)
            {
                throw new PrintQueueException("Cannot start print: printer is not connected.");
            }

            if (_stateMachine.State is PrintState.Printing or PrintState.Preparing or PrintState.Paused)
            {
                throw new PrintQueueException("A print is already active.");
            }

            var pending = await _jobRepository.GetPendingAsync(cancellationToken).ConfigureAwait(false);
            if (pending.Count == 0)
            {
                throw new PrintQueueException("No pending jobs in queue.");
            }

            var job = pending[0];
            _currentJob = job;
            _printCts = new CancellationTokenSource();

            _ = Task.Run(() => RunPrintAsync(job, _printCts.Token), CancellationToken.None);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task PauseAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_stateMachine.State != PrintState.Printing)
            {
                throw new PrintQueueException("Cannot pause: no active print.");
            }

            _printCts?.Cancel();
            _stateMachine.TransitionTo(PrintState.Paused);
            _logger.LogInformation("Print paused for job {JobId}", _currentJob?.Id);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task ResumeAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_stateMachine.State != PrintState.Paused || _currentJob is null)
            {
                throw new PrintQueueException("Cannot resume: no paused print.");
            }

            if (_serialEngine.State != ConnectionState.Connected)
            {
                throw new PrintQueueException("Cannot resume: printer is not connected.");
            }

            _printCts = new CancellationTokenSource();
            var job = _currentJob;
            _ = Task.Run(() => RunPrintAsync(job, _printCts.Token, resume: true), CancellationToken.None);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task CancelAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            _printCts?.Cancel();

            if (_currentJob is not null)
            {
                _currentJob.Status = JobStatus.Cancelled;
                _currentJob.CompletedAt = DateTimeOffset.UtcNow;
                await _jobRepository.UpdateAsync(_currentJob, cancellationToken).ConfigureAwait(false);
            }

            _stateMachine.TransitionTo(PrintState.Cancelled);
            _stateMachine.Reset();
            _currentJob = null;
            _logger.LogInformation("Print cancelled");
        }
        finally
        {
            _gate.Release();
        }
    }

    public PrintQueueSnapshot GetSnapshot()
    {
        var jobs = _jobRepository.GetAllAsync().GetAwaiter().GetResult();
        return new PrintQueueSnapshot
        {
            Jobs = jobs,
            CurrentJobId = _currentJob?.Id
        };
    }

    public async Task ReloadFromPersistenceAsync(CancellationToken cancellationToken = default)
    {
        await _jobRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        _logger.LogDebug("Queue reloaded from persistence");
    }

    private async Task RunPrintAsync(PrintJob job, CancellationToken cancellationToken, bool resume = false)
    {
        try
        {
            if (!resume)
            {
                _stateMachine.TransitionTo(PrintState.Preparing);
                job.Status = JobStatus.Printing;
                job.StartedAt = DateTimeOffset.UtcNow;
                await _jobRepository.UpdateAsync(job, cancellationToken).ConfigureAwait(false);
            }

            _stateMachine.TransitionTo(PrintState.Printing);

            var lines = FilterLinesForResume(_gcodeParser.ReadLinesAsync(job.FilePath, cancellationToken), job.LastLineSent);
            await _serialEngine.StreamGCodeAsync(lines, cancellationToken).ConfigureAwait(false);

            job.Status = JobStatus.Completed;
            job.Progress = 100;
            job.CompletedAt = DateTimeOffset.UtcNow;
            await _jobRepository.UpdateAsync(job, cancellationToken).ConfigureAwait(false);

            _stateMachine.TransitionTo(PrintState.Completed);
            _stateMachine.Reset();
            _currentJob = null;

            await TryAutoStartNextAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            if (_stateMachine.State == PrintState.Paused)
            {
                await _jobRepository.UpdateAsync(job, CancellationToken.None).ConfigureAwait(false);
                return;
            }

            job.Status = JobStatus.Cancelled;
            job.CompletedAt = DateTimeOffset.UtcNow;
            await _jobRepository.UpdateAsync(job, CancellationToken.None).ConfigureAwait(false);
            _stateMachine.TransitionTo(PrintState.Cancelled);
            _stateMachine.Reset();
            _currentJob = null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Print failed for job {JobId}", job.Id);
            job.Status = JobStatus.Failed;
            job.ErrorMessage = ex.Message;
            job.CompletedAt = DateTimeOffset.UtcNow;
            await _jobRepository.UpdateAsync(job, CancellationToken.None).ConfigureAwait(false);
            _stateMachine.TransitionTo(PrintState.Failed);
            _stateMachine.Reset();
            _currentJob = null;
        }
    }

    private async Task TryAutoStartNextAsync(CancellationToken cancellationToken)
    {
        var autoStart = await _settingsRepository
            .GetAsync<bool>(AutoStartSettingKey, cancellationToken)
            .ConfigureAwait(false);

        if (autoStart != true)
        {
            return;
        }

        var pending = await _jobRepository.GetPendingAsync(cancellationToken).ConfigureAwait(false);
        if (pending.Count == 0 || _serialEngine.State != ConnectionState.Connected)
        {
            return;
        }

        try
        {
            await StartNextAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (PrintQueueException ex)
        {
            _logger.LogWarning(ex, "Auto-start skipped");
        }
    }

    private void OnStreamProgress(object? sender, GCodeStreamProgressEventArgs e)
    {
        if (_currentJob is null)
        {
            return;
        }

        _currentJob.LastLineSent = e.LineNumber;
        _currentJob.Progress = e.Progress;
        _ = _jobRepository.UpdateAsync(_currentJob, CancellationToken.None);
    }

    private static async IAsyncEnumerable<GCodeLine> FilterLinesForResume(
        IAsyncEnumerable<GCodeLine> source,
        int lastLineSent)
    {
        await foreach (var line in source.ConfigureAwait(false))
        {
            if (line.LineNumber <= lastLineSent)
            {
                continue;
            }

            yield return line;
        }
    }
}
