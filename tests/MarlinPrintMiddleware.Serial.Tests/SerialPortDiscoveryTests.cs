using FluentAssertions;
using MarlinPrintMiddleware.Serial;

namespace MarlinPrintMiddleware.Serial.Tests;

public class SerialPortDiscoveryTests
{
    [Fact]
    public async Task GetPortsAsync_ReturnsNonNullList()
    {
        var discovery = new SerialPortDiscovery();

        var ports = await discovery.GetPortsAsync();

        ports.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPortsAsync_EmptyOrPopulatedList_DoesNotThrow()
    {
        var discovery = new SerialPortDiscovery();

        var act = async () => await discovery.GetPortsAsync();

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task GetPortsAsync_ReturnsPortNameAndDescription()
    {
        var discovery = new SerialPortDiscovery();
        await discovery.RefreshAsync();

        var ports = await discovery.GetPortsAsync();

        foreach (var port in ports)
        {
            port.PortName.Should().NotBeNullOrWhiteSpace();
            port.Description.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public async Task RefreshAsync_Twice_ReturnsConsistentResults()
    {
        var discovery = new SerialPortDiscovery();

        await discovery.RefreshAsync();
        var first = await discovery.GetPortsAsync();

        await discovery.RefreshAsync();
        var second = await discovery.GetPortsAsync();

        second.Should().BeEquivalentTo(first, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetPortsAsync_UsesCacheUntilRefresh()
    {
        var discovery = new SerialPortDiscovery();

        var initial = await discovery.GetPortsAsync();
        var cached = await discovery.GetPortsAsync();

        cached.Should().BeSameAs(initial);
    }

    [Fact]
    public async Task RefreshAsync_UpdatesCachedReference()
    {
        var discovery = new SerialPortDiscovery();

        await discovery.RefreshAsync();
        var initial = await discovery.GetPortsAsync();
        await discovery.RefreshAsync();
        var refreshed = await discovery.GetPortsAsync();

        refreshed.Should().NotBeSameAs(initial);
        refreshed.Should().BeEquivalentTo(initial, options => options.WithStrictOrdering());
    }
}
