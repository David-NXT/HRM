namespace HR.Infrastructure.Jobs;

public sealed class ContractExpiryJobOptions
{
    public bool Enabled { get; set; } = true;
    public bool RunOnStartup { get; set; } = true;
    public TimeOnly DailyRunTimeUtc { get; set; } = new(1, 0);
}
