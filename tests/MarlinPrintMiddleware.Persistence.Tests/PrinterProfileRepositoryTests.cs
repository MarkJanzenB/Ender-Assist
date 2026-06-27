using FluentAssertions;
using MarlinPrintMiddleware.Core.Models;
using MarlinPrintMiddleware.Persistence.Repositories;

namespace MarlinPrintMiddleware.Persistence.Tests;

public class PrinterProfileRepositoryTests
{
    [Fact]
    public async Task GetDefault_ReturnsEnder3V2Seed()
    {
        using var ctx = new PersistenceTestContext();
        var repo = new PrinterProfileRepository(ctx.ConnectionFactory);

        var profile = await repo.GetDefaultAsync();

        profile.Should().NotBeNull();
        profile!.Name.Should().Be("Ender 3 V2");
        profile.BaudRate.Should().Be(115200);
        profile.BufferSize.Should().Be(4);
    }

    [Fact]
    public async Task SetDefault_DemotesPreviousDefault()
    {
        using var ctx = new PersistenceTestContext();
        var repo = new PrinterProfileRepository(ctx.ConnectionFactory);

        var second = await repo.CreateAsync(new PrinterProfile
        {
            Name = "Custom",
            Port = "COM4",
            IsDefault = true
        });

        var first = await repo.GetByIdAsync(1);
        first!.IsDefault.Should().BeFalse();
        second.IsDefault.Should().BeTrue();
    }
}
