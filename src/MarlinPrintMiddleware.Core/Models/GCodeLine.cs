namespace MarlinPrintMiddleware.Core.Models;

/// <summary>
/// A single parsed line from a G-code file or stream.
/// </summary>
public class GCodeLine
{
    public int LineNumber { get; set; }

    public string Content { get; set; } = string.Empty;

    public bool IsComment { get; set; }
}
