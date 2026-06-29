namespace MarlinPrintMiddleware.Monitoring;

public sealed class MonitoringOptions
{
    public int TemperaturePollIntervalMs { get; set; } = 2000;

    public int StatusRefreshIntervalMs { get; set; } = 250;

    public int PositionPollIntervalMs { get; set; } = 5000;
}
