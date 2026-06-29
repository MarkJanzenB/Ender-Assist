namespace MarlinPrintMiddleware.Core.Models;

public enum ConsoleLineDirection
{
    Rx,
    Tx,
    System
}

public sealed class ConsoleLine
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    public string Text { get; init; } = string.Empty;

    public ConsoleLineDirection Direction { get; init; }

    public string Display => $"[{Timestamp.LocalDateTime:HH:mm:ss}] {(Direction == ConsoleLineDirection.Tx ? ">> " : Direction == ConsoleLineDirection.Rx ? "<< " : "— ")}{Text}";
}
