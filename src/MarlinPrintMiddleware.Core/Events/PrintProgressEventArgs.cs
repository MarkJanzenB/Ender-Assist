namespace MarlinPrintMiddleware.Core.Events;

/// <summary>
/// Event data for print job progress updates.
/// </summary>
public class PrintProgressEventArgs : EventArgs
{
    public PrintProgressEventArgs(long jobId, double progress, int lastLineSent, int totalLines)
    {
        JobId = jobId;
        Progress = progress;
        LastLineSent = lastLineSent;
        TotalLines = totalLines;
    }

    public long JobId { get; }

    /// <summary>
    /// Completion percentage from 0 to 100.
    /// </summary>
    public double Progress { get; }

    public int LastLineSent { get; }

    public int TotalLines { get; }
}
