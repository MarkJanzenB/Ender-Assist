using FluentAssertions;
using MarlinPrintMiddleware.Serial.Parsers;

namespace MarlinPrintMiddleware.Serial.Tests;

public class MarlinResponseParserTests
{
    [Theory]
    [InlineData("ok")]
    [InlineData("OK")]
    [InlineData("N123 G28 X0 Y0 ok")]
    [InlineData("  ok  ")]
    public void IsOk_DetectsStandaloneAndTrailingOk(string line)
    {
        MarlinResponseParser.IsOk(line).Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("echo:busy")]
    [InlineData("okish")]
    public void IsOk_RejectsNonOkLines(string line)
    {
        MarlinResponseParser.IsOk(line).Should().BeFalse();
    }

    [Theory]
    [InlineData("busy: processing")]
    [InlineData("BUSY doing something")]
    public void IsBusy_DetectsBusyResponses(string line)
    {
        MarlinResponseParser.IsBusy(line).Should().BeTrue();
    }

    [Theory]
    [InlineData("Error:Line Number is not Last Line Number+1, Last Line: 4")]
    [InlineData("error:Soft reset")]
    public void IsError_DetectsErrorResponses(string line)
    {
        MarlinResponseParser.IsError(line).Should().BeTrue();
    }

    [Fact]
    public void GetErrorMessage_ExtractsMessageAfterColon()
    {
        const string line = "Error:Line Number is not Last Line Number+1, Last Line: 4";
        MarlinResponseParser.GetErrorMessage(line)
            .Should()
            .Be("Line Number is not Last Line Number+1, Last Line: 4");
    }
}
