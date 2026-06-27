using FluentAssertions;
using MarlinPrintMiddleware.Serial.Parsers;

namespace MarlinPrintMiddleware.Serial.Tests;

public class MarlinHandshakeParserTests
{
    private readonly MarlinHandshakeParser _parser = new();

    [Fact]
    public void Parse_SampleM115Response_ExtractsMarlinFirmwareInfo()
    {
        var response = File.ReadAllText(GetFixturePath("sample_m115_response.txt"));

        var result = _parser.Parse(response);

        result.Should().NotBeNull();
        result!.IsMarlin.Should().BeTrue();
        result.FirmwareName.Should().Be("Marlin");
        result.Version.Should().Be("2.0.8");
        result.MachineType.Should().Be("Ender-3 V2");
    }

    [Fact]
    public void Parse_NonMarlinResponse_ReturnsIsMarlinFalse()
    {
        var result = _parser.Parse("FIRMWARE_NAME:Repetier 1.0.0\nok");

        result.Should().NotBeNull();
        result!.IsMarlin.Should().BeFalse();
    }

    [Fact]
    public void Parse_EmptyResponse_ReturnsNull()
    {
        _parser.Parse(string.Empty).Should().BeNull();
        _parser.Parse("   ").Should().BeNull();
    }

    [Fact]
    public void Parse_LinesCollection_ParsesSameAsCombinedText()
    {
        var response = File.ReadAllText(GetFixturePath("sample_m115_response.txt"));
        var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var fromText = _parser.Parse(response);
        var fromLines = _parser.Parse(lines);

        fromLines.Should().BeEquivalentTo(fromText);
    }

    private static string GetFixturePath(string fileName) =>
        Path.Combine(AppContext.BaseDirectory, "Fixtures", fileName);
}
