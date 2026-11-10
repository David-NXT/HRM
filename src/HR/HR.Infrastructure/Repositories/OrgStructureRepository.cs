using Microsoft.Extensions.Logging;
using HR.Application.Interfaces.Repositories;
using HR.Domain.Entities;
using HR.Domain.Constants;
using HR.Application.Exceptions;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HR.Infrastructure.Repositories;

public sealed class OrgStructureRepository : IOrgStructureRepository
{
    private readonly HrDbContext _db;
    private readonly ILogger<OrgStructureRepository> _logger;

    public OrgStructureRepository(HrDbContext db, ILogger<OrgStructureRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Department>> ListDepartmentsAsync(CancellationToken cancellationToken) =>
        await _db.Departments.AsNoTracking().OrderBy(item => item.Code).ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<Position>> ListPositionsAsync(CancellationToken cancellationToken) =>
        await _db.Positions.AsNoTracking().OrderBy(item => item.Code).ToArrayAsync(cancellationToken);

    public Task<Department?> GetDepartmentAsync(Guid id, CancellationToken cancellationToken) =>
        _db.Departments.FirstOrDefaultAsync(item => item.DepartmentId == id, cancellationToken);

    public Task<Position?> GetPositionAsync(Guid id, CancellationToken cancellationToken) =>
        _db.Positions.FirstOrDefaultAsync(item => item.PositionId == id, cancellationToken);

    public Task AddDepartmentAsync(Department department, CancellationToken cancellationToken) =>
        _db.Departments.AddAsync(department, cancellationToken).AsTask();

    public Task AddPositionAsync(Position position, CancellationToken cancellationToken) =>
        _db.Positions.AddAsync(position, cancellationToken).AsTask();

    public Task<bool> DepartmentCodeExistsAsync(string code, Guid? excludingId, CancellationToken cancellationToken)
    {
        var query = _db.Departments.Where(item => item.Code == code);
        if (excludingId.HasValue)
        {
            query = query.Where(item => item.DepartmentId != excludingId.Value);
        }
        return query.AnyAsync(cancellationToken);
    }

    public Task<bool> PositionCodeExistsAsync(string code, Guid? excludingId, CancellationToken cancellationToken)
    {
        var query = _db.Positions.Where(item => item.Code == code);
        if (excludingId.HasValue)
        {
            query = query.Where(item => item.PositionId != excludingId.Value);
        }
        return query.AnyAsync(cancellationToken);
    }

    public Task<bool> DepartmentHasActiveChildrenAsync(Guid id, CancellationToken cancellationToken) =>
        _db.Departments.AnyAsync(item => item.ParentDepartmentId == id && item.Status == OrgStatuses.Active, cancellationToken);

    public Task<bool> HasActiveEmployeesInDepartmentsAsync(
        IReadOnlyCollection<Guid> departmentIds,
        CancellationToken cancellationToken) =>
        _db.Employees.AnyAsync(
            employee => employee.Status == EmployeeStatuses.Active
                && employee.DepartmentId.HasValue
                && !_db.WorkHistories.Any(history => history.EmployeeId == employee.EmployeeId)
                && departmentIds.Contains(employee.DepartmentId.Value),
            cancellationToken);

    public Task<bool> HasActiveEmployeesInPositionAsync(Guid positionId, CancellationToken cancellationToken) =>
        _db.Employees.AnyAsync(
            employee => employee.Status == EmployeeStatuses.Active
                && employee.PositionId == positionId
                && !_db.WorkHistories.Any(history => history.EmployeeId == employee.EmployeeId),
            cancellationToken);

    public Task<bool> HasOpenWorkHistoryInDepartmentsAsync(
        IReadOnlyCollection<Guid> departmentIds,
        CancellationToken cancellationToken) =>
        _db.WorkHistories.AnyAsync(
            history => history.EndDate == null && departmentIds.Contains(history.DepartmentId),
            cancellationToken);

    public Task<bool> HasOpenWorkHistoryInPositionAsync(Guid positionId, CancellationToken cancellationToken) =>
        _db.WorkHistories.AnyAsync(
            history => history.EndDate == null && history.PositionId == positionId,
            cancellationToken);

    public Task<bool> HasCurrentWorkHistoryInDepartmentsAsync(
        IReadOnlyCollection<Guid> departmentIds,
        DateOnly effectiveOn,
        CancellationToken cancellationToken) =>
        _db.WorkHistories.AnyAsync(
            history => departmentIds.Contains(history.DepartmentId)
                && history.Employee != null && history.Employee.Status != EmployeeStatuses.Resigned
                && history.StartDate <= effectiveOn
                && (history.EndDate == null || history.EndDate >= effectiveOn),
            cancellationToken);

    public Task<bool> HasCurrentWorkHistoryInPositionAsync(
        Guid positionId,
        DateOnly effectiveOn,
        CancellationToken cancellationToken) =>
        _db.WorkHistories.AnyAsync(
            history => history.PositionId == positionId
                && history.Employee != null && history.Employee.Status != EmployeeStatuses.Resigned
                && history.StartDate <= effectiveOn
                && (history.EndDate == null || history.EndDate >= effectiveOn),
            cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException postgres
            && postgres.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            _logger.LogWarning(exception, "Trùng mã phòng ban hoặc chức vụ khi lưu cơ cấu tổ chức.");
            throw new HrException(409, "DuplicateCode", "Mã phòng ban hoặc chức vụ đã tồn tại.");
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException postgres
            && postgres.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            _logger.LogWarning(exception, "Tham chiếu cơ cấu tổ chức không hợp lệ.");
            throw new HrException(400, "InvalidReference", "Phòng ban cấp trên không tồn tại.");
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(exception, "Không thể lưu cơ cấu tổ chức.");
            throw new HrException(500, "SaveError", "Không thể lưu cơ cấu tổ chức. Vui lòng thử lại.");
        }
    }
}
