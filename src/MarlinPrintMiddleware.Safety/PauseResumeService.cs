using MarlinPrintMiddleware.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace MarlinPrintMiddleware.Safety;

public sealed class PauseResumeService : IPauseResumeService
{
    private readonly ISerialEngine _serialEngine;
    private readonly IPrintQueueService _queueService;
    private readonly EmergencyStopService _emergencyStopService;
    private readonly ILogger<PauseResumeService> _logger;

    public PauseResumeService(
        ISerialEngine serialEngine,
        IPrintQueueService queueService,
        EmergencyStopService emergencyStopService,
        ILogger<PauseResumeService> logger)
    {
        _serialEngine = serialEngine;
        _queueService = queueService;
        _emergencyStopService = emergencyStopService;
        _logger = logger;
    }

    public async Task PauseAsync(CancellationToken cancellationToken = default)
    {
        if (_emergencyStopService.IsLatched)
        {
            throw new Core.Exceptions.PrintQueueException("Cannot pause after emergency stop until reset.");
        }

        _logger.LogInformation("Pausing print");
        try
        {
            await _serialEngine.SendCommandAsync("M125", cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "M125 not supported, using queue pause only");
        }

        await _queueService.PauseAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task ResumeAsync(CancellationToken cancellationToken = default)
    {
        if (_emergencyStopService.IsLatched)
        {
            throw new Core.Exceptions.PrintQueueException("Cannot resume after emergency stop until reset.");
        }

        _logger.LogInformation("Resuming print");
        await _queueService.ResumeAsync(cancellationToken).ConfigureAwait(false);
    }
}
