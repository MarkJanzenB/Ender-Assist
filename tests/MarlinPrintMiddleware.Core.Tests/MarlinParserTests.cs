using FluentAssertions;
using MarlinPrintMiddleware.Monitoring.Parsers;

namespace MarlinPrintMiddleware.Core.Tests;

public class MarlinParserTests
{
    [Fact]
    public void TemperatureParser_ParsesM105()
    {
        var ok = MarlinTemperatureParser.TryParse(
            "ok T:210.12 /215.00 B:60.00 /65.00",
            out var hotend, out var targetHotend, out var bed, out var targetBed);

        ok.Should().BeTrue();
        hotend.Should().BeApproximately(210.12, 0.01);
        targetHotend.Should().BeApproximately(215, 0.01);
        bed.Should().BeApproximately(60, 0.01);
    }

    [Fact]
    public void PositionParser_ParsesM114()
    {
        var ok = MarlinPositionParser.TryParse(
            "X:125.40 Y:130.20 Z:0.20 E:3372.84 Count A:12345",
            out var x, out var y, out var z);

        ok.Should().BeTrue();
        x.Should().BeApproximately(125.4, 0.01);
        y.Should().BeApproximately(130.2, 0.01);
        z.Should().BeApproximately(0.2, 0.01);
    }
}
