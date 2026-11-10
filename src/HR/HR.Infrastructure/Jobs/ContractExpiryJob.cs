using HR.Application.Services.ContractAlerts;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HR.Infrastructure.Jobs;

public sealed class ContractExpiryJob(
    IServiceScopeFactory scopeFactory,
    TimeProvider clock,
    IOptions<ContractExpiryJobOptions> options,
    ILogger<ContractExpiryJob> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Value.Enabled)
        {
            logger.LogInformation("Job cảnh báo hợp đồng đã tắt theo cấu hình.");
            return;
        }
        try
        {
            if (options.Value.RunOnStartup) await RunOnceAsync(stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = clock.GetUtcNow();
                var next = GetNextRunUtc(now, options.Value.DailyRunTimeUtc);
                logger.LogInformation("Lần quét cảnh báo hợp đồng tiếp theo: {NextRunUtc}.", next);
                await Task.Delay(next - now, clock, stoppingToken);
                await RunOnceAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Graceful application shutdown.
        }
    }

    private async Task RunOnceAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Hosted services are singleton; each run needs its own scoped DbContext.
            await using var scope = scopeFactory.CreateAsyncScope();
            var service = scope.ServiceProvider.GetRequiredService<ContractAlertService>();
            var result = await service.GenerateAlertsAsync(cancellationToken);
            if (result.FailedCount > 0)
            {
                logger.LogWarning("Quét ngày {Date}: tạo {Created}, bỏ qua {Skipped}, lỗi {Failed}. Không retry trong lần chạy này.",
                    result.ScanDate, result.CreatedCount, result.SkippedCount, result.FailedCount);
            }
            else
            {
                logger.LogInformation("Quét ngày {Date}: kiểm tra {Scanned}, tạo {Created}, bỏ qua {Skipped}.",
                    result.ScanDate, result.ScannedCount, result.CreatedCount, result.SkippedCount);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Quét cảnh báo hợp đồng thất bại; sẽ xét lại ở lần chạy kế tiếp.");
        }
    }

    public static DateTimeOffset GetNextRunUtc(DateTimeOffset now, TimeOnly runAtUtc)
    {
        var next = new DateTimeOffset(now.UtcDateTime.Date.Add(runAtUtc.ToTimeSpan()), TimeSpan.Zero);
        return next <= now ? next.AddDays(1) : next;
    }
}
