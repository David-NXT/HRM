using Microsoft.Extensions.Logging;
using HR.Application.Interfaces.Repositories;
using HR.Domain.Constants;
using HR.Domain.Entities;
using HR.Application.Exceptions;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Repositories;

public sealed class ContractAlertRepository(HrDbContext db, ILogger<ContractAlertRepository> logger) : IContractAlertRepository
{
    public async Task<IReadOnlyList<EmploymentContract>> ListActiveContractsAsync(CancellationToken cancellationToken) =>
        await db.EmploymentContracts.AsNoTracking().Where(contract => contract.Status == ContractStatuses.Active)
            .OrderBy(contract => contract.ContractId).ToArrayAsync(cancellationToken);

    public Task<bool> AlertExistsAsync(Guid contractId, DateOnly expiryDate, CancellationToken cancellationToken) =>
        db.ContractAlerts.AnyAsync(alert => alert.ContractId == contractId && alert.ExpiryDate == expiryDate, cancellationToken);

    public async Task<bool> TryCreateAsync(ContractAlert alert, CancellationToken cancellationToken)
    {
        // Check current contract state in the same SQL statement as the insert.
        // The unique index also protects against another instance scanning simultaneously.
        var inserted = await db.Database.ExecuteSqlInterpolatedAsync($"""
            INSERT INTO contract_alert (contract_id, expiry_date, status, version, created_at, updated_at)
            SELECT contract_id, {alert.ExpiryDate}, {ContractAlertStatuses.New}, 1, {alert.CreatedAt}, {alert.UpdatedAt}
            FROM employment_contract
            WHERE contract_id = {alert.ContractId}
                AND status = {ContractStatuses.Active}
                AND end_date = {alert.ExpiryDate}
                AND start_date < end_date
            ON CONFLICT (contract_id, expiry_date) DO NOTHING
            """, cancellationToken);
        return inserted == 1;
    }

    public async Task<(IReadOnlyList<ContractAlert> Items, int Total)> SearchAsync(
        string? status, Guid? employeeId, DateOnly? expiresFrom, DateOnly? expiresTo,
        int page, int pageSize, CancellationToken cancellationToken)
    {
        IQueryable<ContractAlert> query = db.ContractAlerts.AsNoTracking()
            .Include(alert => alert.Contract).ThenInclude(contract => contract!.Employee);
        if (status is not null) query = query.Where(alert => alert.Status == status);
        if (employeeId.HasValue) query = query.Where(alert => alert.Contract!.EmployeeId == employeeId.Value);
        if (expiresFrom.HasValue) query = query.Where(alert => alert.ExpiryDate >= expiresFrom.Value);
        if (expiresTo.HasValue) query = query.Where(alert => alert.ExpiryDate <= expiresTo.Value);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(alert => alert.ExpiryDate).ThenBy(alert => alert.ContractAlertId)
            .Skip((page - 1) * pageSize).Take(pageSize).ToArrayAsync(cancellationToken);
        return (items, total);
    }

    public Task<ContractAlert?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        db.ContractAlerts.Include(alert => alert.Contract).ThenInclude(contract => contract!.Employee)
            .FirstOrDefaultAsync(alert => alert.ContractAlertId == id, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            logger.LogWarning(exception, "Cảnh báo hợp đồng đã thay đổi đồng thời.");
            throw new HrException(409, "ContractAlertChanged", "Cảnh báo đã thay đổi. Tải lại dữ liệu và thử lại.");
        }
        catch (DbUpdateException exception)
        {
            logger.LogError(exception, "Không thể lưu trạng thái cảnh báo hợp đồng.");
            throw new HrException(500, "SaveError", "Không thể lưu cảnh báo hợp đồng. Vui lòng thử lại.");
        }
    }
}
