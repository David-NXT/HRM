using Microsoft.Extensions.Logging;
using HR.Application.Interfaces.Repositories;
using HR.Domain.Constants;
using HR.Domain.Entities;
using HR.Application.Exceptions;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HR.Infrastructure.Repositories;

public sealed class EmploymentRepository : IEmploymentRepository
{
    private readonly HrDbContext _db;
    private readonly ILogger<EmploymentRepository> _logger;

    public EmploymentRepository(HrDbContext db, ILogger<EmploymentRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken) =>
        _db.Employees.FirstOrDefaultAsync(employee => employee.EmployeeId == employeeId, cancellationToken);

    public Task<EmploymentContract?> GetContractAsync(Guid contractId, CancellationToken cancellationToken) =>
        _db.EmploymentContracts.FirstOrDefaultAsync(contract => contract.ContractId == contractId, cancellationToken);

    public async Task<IReadOnlyList<EmploymentContract>> GetContractsAsync(Guid employeeId, CancellationToken cancellationToken) =>
        await _db.EmploymentContracts.AsNoTracking()
            .Where(contract => contract.EmployeeId == employeeId)
            .OrderByDescending(contract => contract.StartDate)
            .ThenByDescending(contract => contract.ContractId)
            .ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<WorkHistory>> GetWorkHistoryAsync(Guid employeeId, CancellationToken cancellationToken) =>
        await _db.WorkHistories.AsNoTracking()
            .Include(history => history.Department)
            .Include(history => history.Position)
            .Where(history => history.EmployeeId == employeeId)
            .OrderByDescending(history => history.StartDate)
            .ThenByDescending(history => history.WorkHistoryId)
            .ToArrayAsync(cancellationToken);

    public Task<WorkHistory?> GetOpenWorkHistoryAsync(Guid employeeId, CancellationToken cancellationToken) =>
        _db.WorkHistories.FirstOrDefaultAsync(
            history => history.EmployeeId == employeeId && history.EndDate == null,
            cancellationToken);

    public Task<Department?> GetActiveDepartmentAsync(Guid departmentId, CancellationToken cancellationToken) =>
        _db.Departments.FirstOrDefaultAsync(
            department => department.DepartmentId == departmentId && department.Status == OrgStatuses.Active,
            cancellationToken);

    public Task<Position?> GetActivePositionAsync(Guid positionId, CancellationToken cancellationToken) =>
        _db.Positions.FirstOrDefaultAsync(
            position => position.PositionId == positionId && position.Status == OrgStatuses.Active,
            cancellationToken);

    public Task<bool> HasDirectorAssignmentInDepartmentAsync(
        Guid departmentId,
        DateOnly effectiveDate,
        Guid? excludingEmployeeId,
        CancellationToken cancellationToken)
    {
        var query = _db.WorkHistories.Where(history => history.DepartmentId == departmentId
            && history.IsDirector
            && (history.EndDate == null || history.EndDate >= effectiveDate));
        if (excludingEmployeeId.HasValue)
        {
            query = query.Where(history => history.EmployeeId != excludingEmployeeId.Value);
        }
        return query.AnyAsync(cancellationToken);
    }

    public Task<bool> ContractNumberExistsAsync(string contractNumber, Guid? excludingId, CancellationToken cancellationToken)
    {
        var query = _db.EmploymentContracts.Where(contract => contract.ContractNumber == contractNumber);
        if (excludingId.HasValue)
        {
            query = query.Where(contract => contract.ContractId != excludingId.Value);
        }
        return query.AnyAsync(cancellationToken);
    }

    public Task<bool> RenewalAlreadyExistsAsync(Guid contractId, CancellationToken cancellationToken) =>
        _db.EmploymentContracts.AnyAsync(contract => contract.RenewalOfContractId == contractId, cancellationToken);

    public Task AddContractAsync(EmploymentContract contract, CancellationToken cancellationToken) =>
        _db.EmploymentContracts.AddAsync(contract, cancellationToken).AsTask();

    public Task AddWorkHistoryAsync(WorkHistory workHistory, CancellationToken cancellationToken)
    {
        var employee = _db.Employees.Local.FirstOrDefault(item => item.EmployeeId == workHistory.EmployeeId);
        if (employee is not null)
        {
            _db.Entry(employee).Property(item => item.UpdatedAt).IsModified = true;
        }
        return _db.WorkHistories.AddAsync(workHistory, cancellationToken).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => SaveCoreAsync(cancellationToken);

    public async Task SaveChangesInTransactionAsync(CancellationToken cancellationToken)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        await SaveCoreAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task SaveCoreAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            _logger.LogWarning(exception, "Nhân viên đã thay đổi trong khi cập nhật công tác.");
            throw new HrException(409, "EmployeeChanged", "Hồ sơ nhân viên đã thay đổi. Tải lại dữ liệu và thử lại.");
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException postgres
            && postgres.SqlState == PostgresErrorCodes.UniqueViolation
            && postgres.ConstraintName?.Contains("contract_number", StringComparison.OrdinalIgnoreCase) == true)
        {
            _logger.LogWarning(exception, "Trùng số hợp đồng khi lưu.");
            throw new HrException(409, "DuplicateContractNumber", "Số hợp đồng đã tồn tại.");
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException postgres
            && postgres.SqlState == PostgresErrorCodes.UniqueViolation
            && postgres.ConstraintName == "IX_work_history_director_department_open")
        {
            _logger.LogWarning(exception, "Trùng vị trí Giám đốc trong một phòng ban khi lưu.");
            throw new HrException(409, "DepartmentDirectorAlreadyAssigned",
                "Phòng ban đã có Giám đốc được phân công.");
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException postgres
            && postgres.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            _logger.LogWarning(exception, "Xung đột ràng buộc duy nhất khi lưu hợp đồng hoặc quá trình công tác.");
            throw new HrException(409, "EmploymentConflict", "Dữ liệu hợp đồng hoặc quá trình công tác bị xung đột. Tải lại dữ liệu và thử lại.");
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException postgres
            && postgres.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            _logger.LogWarning(exception, "Tham chiếu nhân viên hoặc cơ cấu tổ chức không hợp lệ.");
            throw new HrException(400, "InvalidReference", "Nhân viên, phòng ban hoặc chức vụ không tồn tại.");
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(exception, "Không thể lưu hợp đồng hoặc quá trình công tác.");
            throw new HrException(500, "SaveError", "Không thể lưu dữ liệu. Vui lòng thử lại.");
        }
    }
}
