using FluentAssertions;
using MarlinPrintMiddleware.Persistence.Repositories;

namespace MarlinPrintMiddleware.Persistence.Tests;

public class SettingsRepositoryTests
{
    [Fact]
    public async Task SetAndGet_RoundTripsTypedValue()
    {
        using var ctx = new PersistenceTestContext();
        var repo = new SettingsRepository(ctx.ConnectionFactory);

        await repo.SetAsync("queue.auto_start", true);
        var value = await repo.GetAsync<bool>("queue.auto_start");

        value.Should().BeTrue();
    }

    [Fact]
    public async Task GetMissingKey_ReturnsDefault()
    {
        using var ctx = new PersistenceTestContext();
        var repo = new SettingsRepository(ctx.ConnectionFactory);

        var value = await repo.GetAsync<int>("missing.key");

        value.Should().Be(0);
    }
}
