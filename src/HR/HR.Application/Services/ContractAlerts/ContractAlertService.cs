using HR.Application.DTOs.ContractAlerts;
using HR.Application.Interfaces.Repositories;
using HR.Application.Interfaces.Services;
using HR.Domain.Constants;
using HR.Domain.Entities;
using HR.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace HR.Application.Services.ContractAlerts;

public sealed class ContractAlertService(
    IContractAlertRepository repository, TimeProvider clock, ILogger<ContractAlertService> logger) : IContractAlertGenerator
{
    public async Task<ContractAlertScanResult> GenerateAlertsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // A query failure propagates; the scheduler/manual caller must not report success.
        var contracts = await repository.ListActiveContractsAsync(cancellationToken);
        return await GenerateForContractsAsync(contracts, cancellationToken);
    }

    public async Task GenerateForContractAsync(EmploymentContract contract, CancellationToken cancellationToken)
    {
        // Only check the saved contract; reuse the daily scan's eligibility and duplicate guards.
        await GenerateForContractsAsync([contract], cancellationToken);
    }

    private async Task<ContractAlertScanResult> GenerateForContractsAsync(
        IReadOnlyList<EmploymentContract> contracts, CancellationToken cancellationToken)
    {
        var now = clock.GetUtcNow().UtcDateTime;
        var today = DateOnly.FromDateTime(now);
        var created = 0;
        var skipped = 0;
        var failed = 0;
        foreach (var contract in contracts)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (contract.Status != ContractStatuses.Active || contract.EndDate is null)
            {
                logger.LogDebug("Bỏ qua hợp đồng {ContractId}: không ACTIVE hoặc không có ngày hết hạn.", contract.ContractId);
                skipped++;
                continue;
            }
            var expiryDate = contract.EndDate.Value;
            if (expiryDate <= contract.StartDate)
            {
                logger.LogWarning("Bỏ qua hợp đồng {ContractId}: ngày kết thúc không hợp lệ.", contract.ContractId);
                skipped++;
                continue;
            }
            var daysRemaining = expiryDate.DayNumber - today.DayNumber;
            if (daysRemaining is < 0 or > 30)
            {
                skipped++;
                continue;
            }
            try
            {
                if (await repository.AlertExistsAsync(contract.ContractId, expiryDate, cancellationToken))
                {
                    skipped++;
                    continue;
                }
                var inserted = await repository.TryCreateAsync(new ContractAlert
                {
                    ContractId = contract.ContractId,
                    ExpiryDate = expiryDate,
                    CreatedAt = now,
                    UpdatedAt = now
                }, cancellationToken);
                if (inserted) created++;
                else skipped++; // Another scanner inserted it, or the contract changed meanwhile.
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                failed++;
                logger.LogError(exception, "Không tạo được cảnh báo cho hợp đồng {ContractId}; xét lại ở lần chạy sau.", contract.ContractId);
            }
        }
        return new ContractAlertScanResult(today, contracts.Count, created, skipped, failed);
    }

    public async Task<ContractAlertListDto> ListAsync(
        string? status, Guid? employeeId, DateOnly? expiresFrom, DateOnly? expiresTo,
        int page, int pageSize, CancellationToken cancellationToken)
    {
        var normalizedStatus = string.IsNullOrWhiteSpace(status) ? null : NormalizeStatus(status);
        if (page < 1 || pageSize is < 1 or > 100 || ((long)page - 1) * pageSize > int.MaxValue
            || employeeId == Guid.Empty || (expiresFrom.HasValue && expiresTo.HasValue && expiresFrom > expiresTo))
        {
            throw new HrException(400, "ValidationError", "Bộ lọc hoặc phân trang cảnh báo không hợp lệ.");
        }
        var (items, total) = await repository.SearchAsync(normalizedStatus, employeeId, expiresFrom, expiresTo, page, pageSize, cancellationToken);
        return new ContractAlertListDto(items.Select(ToDto).ToArray(), page, pageSize, total);
    }

    public async Task<ContractAlertDto> GetAsync(Guid id, CancellationToken cancellationToken) =>
        ToDto(await GetEntityAsync(id, cancellationToken));

    public async Task<ContractAlertDto> UpdateStatusAsync(Guid id, UpdateContractAlertStatusRequest request, CancellationToken cancellationToken)
    {
        var status = NormalizeStatus(request.Status);
        var alert = await GetEntityAsync(id, cancellationToken);
        if (request.ExpectedVersion != alert.Version)
        {
            throw new HrException(409, "ContractAlertChanged", "Cảnh báo đã thay đổi. Tải lại dữ liệu và thử lại.");
        }
        if (alert.Status != status)
        {
            alert.Status = status;
            alert.UpdatedAt = clock.GetUtcNow().UtcDateTime;
            await repository.SaveChangesAsync(cancellationToken);
        }
        return ToDto(alert);
    }

    private async Task<ContractAlert> GetEntityAsync(Guid id, CancellationToken cancellationToken) =>
        await repository.GetAsync(id, cancellationToken)
        ?? throw new HrException(404, "ContractAlertNotFound", "Không tìm thấy cảnh báo hợp đồng.");

    private static string NormalizeStatus(string? value)
    {
        var status = (value ?? string.Empty).Trim().ToUpperInvariant();
        if (status is not (ContractAlertStatuses.New or ContractAlertStatuses.Viewed or ContractAlertStatuses.InProgress))
        {
            throw new HrException(400, "ValidationError", "Trạng thái cảnh báo phải là NEW, VIEWED hoặc IN_PROGRESS.");
        }
        return status;
    }

    private ContractAlertDto ToDto(ContractAlert alert)
    {
        var contract = alert.Contract ?? throw new HrException(500, "InvalidContractAlert", "Cảnh báo thiếu dữ liệu hợp đồng.");
        var employee = contract.Employee ?? throw new HrException(500, "InvalidContractAlert", "Cảnh báo thiếu dữ liệu nhân viên.");
        var today = DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime);
        return new ContractAlertDto(alert.ContractAlertId, contract.ContractId,
            employee.EmployeeId, employee.EmployeeCode, employee.FullName, employee.Status,
            contract.ContractNumber, contract.ContractType, contract.Status, contract.StartDate, contract.EndDate,
            alert.ExpiryDate, alert.ExpiryDate.DayNumber - today.DayNumber,
            contract.Status == ContractStatuses.Active && contract.EndDate == alert.ExpiryDate,
            alert.Status, alert.Version, alert.CreatedAt, alert.UpdatedAt);
    }
}
