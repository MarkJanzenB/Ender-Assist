using MarlinPrintMiddleware.Core.Models;

namespace MarlinPrintMiddleware.Core.Interfaces;

public enum ConsoleLogFilter
{
    All,
    Rx,
    Tx,
    Errors
}

public interface ISerialConsoleService
{
    IReadOnlyList<ConsoleLine> Lines { get; }

    ConsoleLogFilter Filter { get; set; }

    event EventHandler? LinesChanged;

    Task SendAsync(string command, CancellationToken cancellationToken = default);

    void Clear();
}
