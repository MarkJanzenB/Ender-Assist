using FluentAssertions;
using MarlinPrintMiddleware.Queue;

namespace MarlinPrintMiddleware.Queue.Tests;

public class GCodeParserTests
{
  private static string SamplePath =>
      Path.Combine(AppContext.BaseDirectory, "SampleGCode", "sample.gcode");

  [Fact]
  public async Task AnalyzeAsync_ParsesMetadataAndLineCounts()
  {
    var parser = new GCodeParser();
    var info = await parser.AnalyzeAsync(SamplePath);

    info.Name.Should().Be("sample.gcode");
    info.SlicerName.Should().Be("Cura");
    info.EstimatedDuration.Should().Be(TimeSpan.FromSeconds(3600));
    info.PrintableLines.Should().Be(3);
  }

  [Fact]
  public async Task ReadLinesAsync_YieldsAllLinesWithCommentFlags()
  {
    var parser = new GCodeParser();
    var lines = new List<MarlinPrintMiddleware.Core.Models.GCodeLine>();

    await foreach (var line in parser.ReadLinesAsync(SamplePath))
    {
      lines.Add(line);
    }

    lines.Should().HaveCount(6);
    lines.Count(l => l.IsComment).Should().Be(3);
  }

  [Fact]
  public void ValidateFilePath_ThrowsForMissingFile()
  {
    var parser = new GCodeParser();
    parser.Invoking(p => p.ValidateFilePath(@"C:\missing.gcode"))
        .Should().Throw<MarlinPrintMiddleware.Core.Exceptions.PrintQueueException>();
  }
}
