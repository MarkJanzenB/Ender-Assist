using MarlinPrintMiddleware.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace MarlinPrintMiddleware.Safety;

public sealed class EmergencyStopService : IEmergencyStopService
{
    private readonly ISerialEngine _serialEngine;
    private readonly IPrintQueueService _queueService;
    private readonly ILogger<EmergencyStopService> _logger;
    private bool _latched;

    public EmergencyStopService(
        ISerialEngine serialEngine,
        IPrintQueueService queueService,
        ILogger<EmergencyStopService> logger)
    {
        _serialEngine = serialEngine;
        _queueService = queueService;
        _logger = logger;
    }

    public bool IsLatched => _latched;

    public async Task TriggerAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Emergency stop triggered");
        _latched = true;

        try
        {
            await _serialEngine.SendCommandAsync("M112", cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send M112");
        }

        await _queueService.CancelAsync(cancellationToken).ConfigureAwait(false);
    }

    public void ResetLatch() => _latched = false;
}
