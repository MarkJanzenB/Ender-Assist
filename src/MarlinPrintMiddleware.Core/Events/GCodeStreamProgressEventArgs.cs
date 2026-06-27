namespace MarlinPrintMiddleware.Core.Events;

/// <summary>
/// Event data for G-code streaming progress from the serial engine.
/// </summary>
public class GCodeStreamProgressEventArgs : EventArgs
{
    public GCodeStreamProgressEventArgs(int lineNumber, int linesSent, int totalLines, double progress)
    {
        LineNumber = lineNumber;
        LinesSent = linesSent;
        TotalLines = totalLines;
        Progress = progress;
    }

    public int LineNumber { get; }

    public int LinesSent { get; }

    public int TotalLines { get; }

    /// <summary>
    /// Completion percentage from 0 to 100.
    /// </summary>
    public double Progress { get; }
}
