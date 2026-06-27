using FluentAssertions;
using MarlinPrintMiddleware.Core.Exceptions;
using MarlinPrintMiddleware.Serial;
using MarlinPrintMiddleware.Serial.Parsers;

namespace MarlinPrintMiddleware.Serial.Tests;

public class OkSynchronizerTests
{
    [Fact]
    public async Task WaitForOkAsync_CompletesWhenOkReceived()
    {
        var synchronizer = new OkSynchronizer(TimeSpan.FromSeconds(1));
        synchronizer.BeginWait();

        var waitTask = synchronizer.WaitForOkAsync(CancellationToken.None);
        var processed = await synchronizer.ProcessLineAsync("ok", CancellationToken.None);

        processed.Should().BeTrue();
        (await waitTask).Should().Be("ok");
    }

    [Fact]
    public async Task ProcessLineAsync_RetriesBusyThenCompletes()
    {
        var synchronizer = new OkSynchronizer(TimeSpan.FromSeconds(5), busyBackoffMs: 1, maxBusyRetries: 50);
        synchronizer.BeginWait();

        var waitTask = synchronizer.WaitForOkAsync(CancellationToken.None);

        (await synchronizer.ProcessLineAsync("busy: processing", CancellationToken.None)).Should().BeTrue();
        (await synchronizer.ProcessLineAsync("ok", CancellationToken.None)).Should().BeTrue();

        (await waitTask).Should().Be("ok");
    }

    [Fact]
    public async Task ProcessLineAsync_ThrowsMarlinProtocolExceptionOnError()
    {
        var synchronizer = new OkSynchronizer(TimeSpan.FromSeconds(1));
        synchronizer.BeginWait();
        var waitTask = synchronizer.WaitForOkAsync(CancellationToken.None);

        Func<Task> processAct = () => synchronizer.ProcessLineAsync("Error:Soft reset", CancellationToken.None);
        await processAct.Should().ThrowAsync<MarlinProtocolException>().WithMessage("Soft reset");

        Func<Task> waitAct = () => waitTask;
        await waitAct.Should().ThrowAsync<MarlinProtocolException>();
    }

    [Fact]
    public async Task WaitForOkAsync_ThrowsWhenTimeoutExpires()
    {
        var synchronizer = new OkSynchronizer(TimeSpan.FromMilliseconds(50));
        synchronizer.BeginWait();

        var act = () => synchronizer.WaitForOkAsync(CancellationToken.None);

        await act.Should().ThrowAsync<MarlinProtocolException>()
            .WithMessage("*Timed out waiting for ok*");
    }

    [Fact]
    public async Task ProcessLineAsync_FailsAfterMaxBusyRetries()
    {
        var synchronizer = new OkSynchronizer(TimeSpan.FromSeconds(5), busyBackoffMs: 1, maxBusyRetries: 2);
        synchronizer.BeginWait();

        var waitTask = synchronizer.WaitForOkAsync(CancellationToken.None);

        (await synchronizer.ProcessLineAsync("busy: processing", CancellationToken.None)).Should().BeTrue();
        (await synchronizer.ProcessLineAsync("busy: processing", CancellationToken.None)).Should().BeTrue();

        Func<Task> thirdBusy = () => synchronizer.ProcessLineAsync("busy: processing", CancellationToken.None);
        await thirdBusy.Should().ThrowAsync<MarlinProtocolException>()
            .WithMessage("*remained busy after 2 retries*");

        Func<Task> act = () => waitTask;
        await act.Should().ThrowAsync<MarlinProtocolException>();
    }

    [Fact]
    public async Task ProcessLineAsync_ForwardsNonProtocolLines()
    {
        var synchronizer = new OkSynchronizer(TimeSpan.FromSeconds(1));
        synchronizer.BeginWait();

        var processed = await synchronizer.ProcessLineAsync("echo:some status", CancellationToken.None);
        processed.Should().BeFalse();
    }
}
