using FluentAssertions;
using MarlinPrintMiddleware.Core.Enums;
using MarlinPrintMiddleware.Core.Events;
using MarlinPrintMiddleware.Core.Exceptions;
using MarlinPrintMiddleware.Core.Models;
using MarlinPrintMiddleware.Serial;
using MarlinPrintMiddleware.Serial.Tests.Fakes;
using Microsoft.Extensions.Logging.Abstractions;

namespace MarlinPrintMiddleware.Serial.Tests;

public class SerialEngineTests
{
    [Fact]
    public async Task ConnectAsync_PerformsHandshakeAndSetsConnected()
    {
        var fakePort = CreateFakePortWithM115Fixture();
        await using var handle = await StartEngineAsync(fakePort);

        await handle.Engine.ConnectAsync("COM_FAKE", 115200);

        handle.Engine.State.Should().Be(ConnectionState.Connected);
        handle.Engine.FirmwareInfo.Should().NotBeNull();
        handle.Engine.FirmwareInfo!.IsMarlin.Should().BeTrue();
        handle.Engine.FirmwareInfo.MachineType.Should().Be("Ender-3 V2");
        fakePort.WrittenLines.Should().Contain("M115");
    }

    [Fact]
    public async Task SendCommandAsync_WaitsForOk()
    {
        var fakePort = CreateFakePortWithM115Fixture();
        fakePort.ScriptCommand("G28", "ok");
        await using var handle = await StartEngineAsync(fakePort);
        await handle.Engine.ConnectAsync("COM_FAKE", 115200);

        var response = await handle.Engine.SendCommandAsync("G28");

        response.Should().Be("G28");
        fakePort.WrittenLines.Should().Contain("G28");
    }

    [Fact]
    public async Task SendCommandAsync_M112BypassesQueue()
    {
        var fakePort = CreateFakePortWithM115Fixture();
        fakePort.ScriptCommand("M112", "ok");
        await using var handle = await StartEngineAsync(fakePort);
        await handle.Engine.ConnectAsync("COM_FAKE", 115200);

        await handle.Engine.SendCommandAsync("M112");

        fakePort.WrittenLines.Should().Contain("M112");
    }

    [Fact]
    public async Task StreamGCodeAsync_RespectsBufferAndSkipsComments()
    {
        var fakePort = CreateFakePortWithM115Fixture();
        await using var handle = await StartEngineAsync(fakePort, plannerBufferSize: 2);
        await handle.Engine.ConnectAsync("COM_FAKE", 115200);

        var progressEvents = new List<GCodeStreamProgressEventArgs>();
        handle.Engine.StreamProgress += (_, args) => progressEvents.Add(args);

        var lines = ToAsyncEnumerable(
            new GCodeLine { LineNumber = 1, Content = "; comment", IsComment = true },
            new GCodeLine { LineNumber = 2, Content = "G1 X10" },
            new GCodeLine { LineNumber = 3, Content = string.Empty },
            new GCodeLine { LineNumber = 4, Content = "G1 Y10" },
            new GCodeLine { LineNumber = 5, Content = "G1 Z5" });

        await handle.Engine.StreamGCodeAsync(lines);

        fakePort.WrittenLines.Should().Contain(new[] { "G1 X10", "G1 Y10", "G1 Z5" });
        fakePort.WrittenLines.Should().NotContain(line => line.StartsWith(';'));
        progressEvents.Should().HaveCount(3);
        progressEvents.Last().Progress.Should().Be(100);
    }

    [Fact]
    public async Task StreamGCodeAsync_CancellationStopsSending()
    {
        var fakePort = CreateFakePortWithM115Fixture();
        await using var handle = await StartEngineAsync(fakePort, plannerBufferSize: 1);
        await handle.Engine.ConnectAsync("COM_FAKE", 115200);

        using var cts = new CancellationTokenSource();
        var lines = ToAsyncEnumerable(
            Enumerable.Range(1, 50)
                .Select(i => new GCodeLine { LineNumber = i, Content = $"G1 X{i}" })
                .ToArray());

        cts.CancelAfter(TimeSpan.FromMilliseconds(50));

        var act = async () => await handle.Engine.StreamGCodeAsync(lines, cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ReadLoop_IOExceptionRaisesConnectionLost()
    {
        var fakePort = CreateFakePortWithM115Fixture();
        await using var handle = await StartEngineAsync(fakePort);
        await handle.Engine.ConnectAsync("COM_FAKE", 115200);

        ConnectionLostEventArgs? lostArgs = null;
        handle.Engine.ConnectionLost += (_, args) => lostArgs = args;

        fakePort.SimulateDisconnectOnNextRead();

        for (var attempt = 0; attempt < 20; attempt++)
        {
            if (handle.Engine.State == ConnectionState.Error)
            {
                break;
            }

            await Task.Delay(50);
        }

        handle.Engine.State.Should().Be(ConnectionState.Error);
        lostArgs.Should().NotBeNull();
        lostArgs!.Cause.Should().BeOfType<IOException>();
    }

    [Fact]
    public async Task ReconnectAsync_RetriesUntilConnected()
    {
        var fakePort = CreateFakePortWithM115Fixture();
        await using var handle = await StartEngineAsync(fakePort);
        await handle.Engine.ConnectAsync("COM_FAKE", 115200);
        await handle.Engine.DisconnectAsync();

        await handle.Engine.ReconnectAsync("COM_FAKE", 115200);

        handle.Engine.State.Should().Be(ConnectionState.Connected);
        handle.Engine.FirmwareInfo.Should().NotBeNull();
    }

    [Fact]
    public async Task SendCommandAsync_ErrorResponseThrowsMarlinProtocolException()
    {
        var fakePort = CreateFakePortWithM115Fixture();
        fakePort.ScriptCommand("G999", "Error:unknown command", "ok");
        await using var handle = await StartEngineAsync(fakePort);
        await handle.Engine.ConnectAsync("COM_FAKE", 115200);

        var act = async () => await handle.Engine.SendCommandAsync("G999");

        await act.Should().ThrowAsync<MarlinProtocolException>().WithMessage("unknown command");
    }

    private static FakeSerialPort CreateFakePortWithM115Fixture()
    {
        var fakePort = new FakeSerialPort();
        var fixturePath = Path.Combine(AppContext.BaseDirectory, "Fixtures", "sample_m115_response.txt");
        var lines = File.ReadAllLines(fixturePath);
        fakePort.ScriptCommand("M115", lines);
        return fakePort;
    }

    private static async Task<TestEngineHandle> StartEngineAsync(
        FakeSerialPort fakePort,
        int plannerBufferSize = 4,
        TimeSpan? commandTimeout = null)
    {
        var engine = new SerialEngine(
            NullLogger<SerialEngine>.Instance,
            (_, _) => fakePort,
            plannerBufferSize,
            commandTimeout ?? TimeSpan.FromSeconds(5));

        await engine.StartAsync(CancellationToken.None);
        return new TestEngineHandle(engine);
    }

    private static async IAsyncEnumerable<GCodeLine> ToAsyncEnumerable(params GCodeLine[] lines)
    {
        foreach (var line in lines)
        {
            yield return line;
            await Task.Yield();
        }
    }

    private sealed class TestEngineHandle : IAsyncDisposable
    {
        public TestEngineHandle(SerialEngine engine) => Engine = engine;

        public SerialEngine Engine { get; }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await Engine.StopAsync(CancellationToken.None);
            }
            catch (SerialConnectionException)
            {
            }

            Engine.Dispose();
        }
    }
}
