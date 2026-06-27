using FluentAssertions;
using MarlinPrintMiddleware.App;
using MarlinPrintMiddleware.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MarlinPrintMiddleware.Integration.Tests;

public class HostBootstrapTests
{
    [Fact]
    public void ConfigureServices_ResolvesSerialEngine()
    {
        var services = new ServiceCollection();
        HostBootstrap.ConfigureServices(services);

        var provider = services.BuildServiceProvider();
        provider.GetRequiredService<ISerialEngine>().Should().NotBeNull();
    }

    [Fact]
    public async Task Host_StartsAndStopsCleanly()
    {
        using var host = HostBootstrap.BuildHost();
        await host.StartAsync();
        await host.StopAsync();
    }
}
