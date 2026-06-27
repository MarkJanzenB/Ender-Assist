using FluentAssertions;
using MarlinPrintMiddleware.Core.Enums;
using MarlinPrintMiddleware.Core.Interfaces;
using MarlinPrintMiddleware.Core.Models;
using MarlinPrintMiddleware.Persistence.Repositories;
using MarlinPrintMiddleware.Queue;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace MarlinPrintMiddleware.Queue.Tests;

public class PrintQueueServiceTests
{
    private static string SamplePath =>
        Path.Combine(AppContext.BaseDirectory, "SampleGCode", "sample.gcode");

    [Fact]
    public async Task EnqueueAsync_PersistsPendingJob()
    {
        using var ctx = new QueueTestDatabase();
        var service = CreateService(ctx, ConnectionState.Connected);

        await service.EnqueueAsync(new PrintJob { FilePath = SamplePath });

        var snapshot = service.GetSnapshot();
        snapshot.Jobs.Should().ContainSingle(j => j.Status == JobStatus.Pending);
    }

    [Fact]
    public async Task StartNextAsync_WhenDisconnected_Throws()
    {
        using var ctx = new QueueTestDatabase();
        var service = CreateService(ctx, ConnectionState.Disconnected);

        await service.EnqueueAsync(new PrintJob { FilePath = SamplePath });

        await service.Invoking(s => s.StartNextAsync())
            .Should().ThrowAsync<MarlinPrintMiddleware.Core.Exceptions.PrintQueueException>()
            .WithMessage("*not connected*");
    }

    [Fact]
    public async Task AutoStart_ChainsSecondJobWhenEnabled()
    {
        using var ctx = new QueueTestDatabase();
        var settings = new SettingsRepository(ctx.ConnectionFactory);
        await settings.SetAsync(PrintQueueService.AutoStartSettingKey, true);

        var serial = new Mock<ISerialEngine>();
        serial.SetupGet(s => s.State).Returns(ConnectionState.Connected);
        serial.Setup(s => s.StreamGCodeAsync(It.IsAny<IAsyncEnumerable<GCodeLine>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new PrintQueueService(
            new PrintJobRepository(ctx.ConnectionFactory),
            serial.Object,
            settings,
            new GCodeParser(),
            NullLogger<PrintQueueService>.Instance);

        await service.EnqueueAsync(new PrintJob { FilePath = SamplePath, Name = "job1" });
        await service.EnqueueAsync(new PrintJob { FilePath = SamplePath, Name = "job2" });

        await service.StartNextAsync();
        await Task.Delay(500);

        serial.Verify(
            s => s.StreamGCodeAsync(It.IsAny<IAsyncEnumerable<GCodeLine>>(), It.IsAny<CancellationToken>()),
            Times.AtLeast(1));
    }

    private static PrintQueueService CreateService(QueueTestDatabase ctx, ConnectionState state)
    {
        var serial = new Mock<ISerialEngine>();
        serial.SetupGet(s => s.State).Returns(state);

        return new PrintQueueService(
            new PrintJobRepository(ctx.ConnectionFactory),
            serial.Object,
            new SettingsRepository(ctx.ConnectionFactory),
            new GCodeParser(),
            NullLogger<PrintQueueService>.Instance);
    }
}
