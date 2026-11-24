using Microsoft.Extensions.Logging;
using HR.Application.Interfaces.Repositories;
using HR.Domain.Constants;
using HR.Domain.Entities;
using HR.Application.Exceptions;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HR.Infrastructure.Repositories;

public sealed class LifecycleRepository(HrDbContext db, ILogger<LifecycleRepository> logger) : ILifecycleRepository
{
    public Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken) =>
        db.Employees.FirstOrDefaultAsync(employee => employee.EmployeeId == employeeId, cancellationToken);

    public async Task<IReadOnlyList<WorkHistory>> GetWorkHistoryAsync(Guid employeeId, CancellationToken cancellationToken) =>
        await db.WorkHistories.Where(history => history.EmployeeId == employeeId)
            .OrderBy(history => history.StartDate).ToArrayAsync(cancellationToken);

    // Giữ tracking để thay đổi trạng thái hợp đồng nằm trong giao dịch xác nhận nghỉ việc.
    public async Task<IReadOnlyList<EmploymentContract>> GetContractsAsync(Guid employeeId, CancellationToken cancellationToken) =>
        await db.EmploymentContracts.Where(contract => contract.EmployeeId == employeeId)
            .ToArrayAsync(cancellationToken);

    public Task<bool> HasPendingOffboardingAsync(Guid employeeId, CancellationToken cancellationToken) =>
        db.OffboardingRecords.AnyAsync(record => record.EmployeeId == employeeId
            && record.Status == LifecycleStatuses.InProgress, cancellationToken);

    public Task<OffboardingRecord?> GetOffboardingAsync(Guid id, CancellationToken cancellationToken) =>
        db.OffboardingRecords.Include(record => record.Employee).Include(record => record.Checklist)
            .FirstOrDefaultAsync(record => record.OffboardingRecordId == id, cancellationToken);

    public async Task<IReadOnlyList<OffboardingRecord>> ListOffboardingsAsync(
        Guid? employeeId, string? status, CancellationToken cancellationToken)
    {
        IQueryable<OffboardingRecord> query = db.OffboardingRecords.AsNoTracking()
            .Include(record => record.Employee).Include(record => record.Checklist);
        if (employeeId.HasValue)
        {
            query = query.Where(record => record.EmployeeId == employeeId.Value);
        }
        if (status is not null)
        {
            query = query.Where(record => record.Status == status);
        }
        return await query.OrderByDescending(record => record.CreatedAt)
            .ThenByDescending(record => record.OffboardingRecordId).ToArrayAsync(cancellationToken);
    }

    public async Task AddOnboardingAsync(Employee employee, WorkHistory firstAssignment, CancellationToken cancellationToken)
    {
        await db.Employees.AddAsync(employee, cancellationToken);
        firstAssignment.Employee = employee;
        await db.WorkHistories.AddAsync(firstAssignment, cancellationToken);
    }

    public Task AddOffboardingAsync(OffboardingRecord record, CancellationToken cancellationToken)
    {
        if (record.Employee is not null)
        {
            db.Entry(record.Employee).Property(employee => employee.UpdatedAt).IsModified = true;
        }
        return db.OffboardingRecords.AddAsync(record, cancellationToken).AsTask();
    }

    public async Task SaveChangesInTransactionAsync(CancellationToken cancellationToken)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            logger.LogWarning(exception, "Hồ sơ nhân viên hoặc nghỉ việc đã thay đổi đồng thời.");
            throw new HrException(409, "LifecycleChanged", "Hồ sơ đã thay đổi. Tải lại dữ liệu và thử lại.");
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException postgres
            && postgres.SqlState == PostgresErrorCodes.UniqueViolation
            && postgres.ConstraintName == "IX_employee_employee_code")
        {
            throw new HrException(409, "DuplicateEmployeeCode", "Mã nhân viên đã tồn tại.");
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException postgres
            && postgres.SqlState == PostgresErrorCodes.UniqueViolation
            && postgres.ConstraintName == "IX_work_history_director_department_open")
        {
            throw new HrException(409, "DepartmentDirectorAlreadyAssigned",
                "Phòng ban đã có Giám đốc được phân công.");
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException postgres
            && postgres.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            logger.LogWarning(exception, "Trùng hồ sơ nghỉ việc hoặc công tác đang mở.");
            throw new HrException(409, "LifecycleConflict", "Nhân viên đã có hồ sơ nghỉ việc hoặc công tác đang mở.");
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException postgres
            && postgres.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            throw new HrException(400, "InvalidReference", "Nhân viên, phòng ban hoặc chức vụ không tồn tại.");
        }
        catch (DbUpdateException exception)
        {
            logger.LogError(exception, "Không thể lưu quy trình tiếp nhận hoặc nghỉ việc.");
            throw new HrException(500, "SaveError", "Không thể lưu quy trình nhân sự. Vui lòng thử lại.");
        }
    }
}
