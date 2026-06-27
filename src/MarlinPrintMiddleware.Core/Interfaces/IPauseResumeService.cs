namespace MarlinPrintMiddleware.Core.Interfaces;

/// <summary>
/// Safe pause and resume for active prints.
/// </summary>
public interface IPauseResumeService
{
    Task PauseAsync(CancellationToken cancellationToken = default);

    Task ResumeAsync(CancellationToken cancellationToken = default);
}
