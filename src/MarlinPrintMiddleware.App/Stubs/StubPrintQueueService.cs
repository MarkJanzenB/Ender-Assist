using MarlinPrintMiddleware.Core.Interfaces;
using MarlinPrintMiddleware.Core.Models;

namespace MarlinPrintMiddleware.App;

internal sealed class StubPrintQueueService : IPrintQueueService
{
    public Task CancelAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public PrintQueueSnapshot GetSnapshot() => new() { Jobs = [], CurrentJobId = null };

    public Task EnqueueAsync(PrintJob job, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task PauseAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task ResumeAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task StartNextAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
