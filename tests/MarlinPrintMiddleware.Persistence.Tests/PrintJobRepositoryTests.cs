using FluentAssertions;
using MarlinPrintMiddleware.Core.Enums;
using MarlinPrintMiddleware.Core.Models;
using MarlinPrintMiddleware.Persistence.Repositories;

namespace MarlinPrintMiddleware.Persistence.Tests;

public class PrintJobRepositoryTests
{
    [Fact]
    public async Task CreateAndGetById_RoundTripsJob()
    {
        using var ctx = new PersistenceTestContext();
        var repo = new PrintJobRepository(ctx.ConnectionFactory);

        var job = new PrintJob
        {
            FilePath = @"C:\prints\test.gcode",
            Name = "test.gcode",
            Status = JobStatus.Pending,
            Priority = JobPriority.High,
            TotalLines = 100,
            QueueOrder = 0
        };

        await repo.CreateAsync(job);
        var loaded = await repo.GetByIdAsync(job.Id);

        loaded.Should().NotBeNull();
        loaded!.Name.Should().Be("test.gcode");
        loaded.Priority.Should().Be(JobPriority.High);
    }

    [Fact]
    public async Task GetPending_OrdersByPriorityThenQueueOrder()
    {
        using var ctx = new PersistenceTestContext();
        var repo = new PrintJobRepository(ctx.ConnectionFactory);

        await repo.CreateAsync(new PrintJob
        {
            FilePath = "a.gcode",
            Name = "low",
            Priority = JobPriority.Low,
            QueueOrder = 0,
            Status = JobStatus.Pending
        });
        await repo.CreateAsync(new PrintJob
        {
            FilePath = "b.gcode",
            Name = "high",
            Priority = JobPriority.High,
            QueueOrder = 0,
            Status = JobStatus.Pending
        });

        var pending = await repo.GetPendingAsync();
        pending[0].Name.Should().Be("high");
        pending[1].Name.Should().Be("low");
    }

    [Fact]
    public async Task UpdateProgress_Persists()
    {
        using var ctx = new PersistenceTestContext();
        var repo = new PrintJobRepository(ctx.ConnectionFactory);
        var job = await repo.CreateAsync(new PrintJob
        {
            FilePath = "a.gcode",
            Name = "a",
            Status = JobStatus.Printing,
            Progress = 0,
            LastLineSent = 0
        });

        job.Progress = 42.5;
        job.LastLineSent = 50;
        await repo.UpdateAsync(job);

        var loaded = await repo.GetByIdAsync(job.Id);
        loaded!.Progress.Should().Be(42.5);
        loaded.LastLineSent.Should().Be(50);
    }
}
