using Microsoft.Extensions.Logging;
using HR.Application.DTOs.Employees;
using HR.Application.Interfaces.Repositories;
using HR.Domain.Entities;
using HR.Domain.Constants;
using HR.Application.Exceptions;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HR.Infrastructure.Repositories;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly HrDbContext _db;
    private readonly ILogger<EmployeeRepository> _logger;

    public EmployeeRepository(HrDbContext db, ILogger<EmployeeRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<(IReadOnlyList<Employee> Items, int Total)> SearchAsync(
        string? keyword,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        IQueryable<Employee> query = _db.Employees.AsNoTracking()
            .Include(employee => employee.Department)
            .Include(employee => employee.Position);
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var pattern = "%" + EscapeLike(keyword.Trim()) + "%";
            query = query.Where(employee =>
                EF.Functions.ILike(employee.EmployeeCode, pattern, "\\")
                || EF.Functions.ILike(employee.FullName, pattern, "\\")
                || EF.Functions.ILike(employee.Email, pattern, "\\")
                || EF.Functions.ILike(employee.Phone, pattern, "\\")
                || EF.Functions.ILike(employee.CitizenId, pattern, "\\"));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(employee => employee.EmployeeCode)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        await ApplyCurrentWorkHistoryAsync(items, cancellationToken);

        return (items, total);
    }

    public async Task<(IReadOnlyList<Employee> Items, int Total)> SearchActiveAsync(
        string? keyword,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        IQueryable<Employee> query = _db.Employees.AsNoTracking()
            .Include(employee => employee.Department)
            .Include(employee => employee.Position)
            .Where(employee => employee.Status == "ACTIVE");
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var pattern = "%" + EscapeLike(keyword.Trim()) + "%";
            query = query.Where(employee =>
                EF.Functions.ILike(employee.EmployeeCode, pattern, "\\")
                || EF.Functions.ILike(employee.FullName, pattern, "\\"));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(employee => employee.EmployeeCode)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        await ApplyCurrentWorkHistoryAsync(items, cancellationToken);

        return (items, total);
    }

    public async Task<EmployeeStatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken)
    {
        var groups = await _db.Employees.AsNoTracking()
            .GroupBy(employee => new { employee.Status, employee.EmploymentType })
            .Select(group => new
            {
                group.Key.Status,
                group.Key.EmploymentType,
                Count = group.Count()
            })
            .ToListAsync(cancellationToken);

        int CountByStatus(string status) => groups
            .Where(group => group.Status == status)
            .Sum(group => group.Count);

        int CountByEmploymentType(string employmentType) => groups
            .Where(group => group.EmploymentType == employmentType)
            .Sum(group => group.Count);

        return new EmployeeStatisticsDto(
            groups.Sum(group => group.Count),
            CountByStatus(EmployeeStatuses.Active),
            CountByStatus(EmployeeStatuses.Locked),
            CountByStatus(EmployeeStatuses.Resigned),
            CountByEmploymentType(EmploymentTypes.FullTime),
            CountByEmploymentType(EmploymentTypes.PartTime));
    }

    public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var employee = await _db.Employees.Include(item => item.Department).Include(item => item.Position)
            .FirstOrDefaultAsync(item => item.EmployeeId == id, cancellationToken);
        if (employee is not null)
        {
            await ApplyCurrentWorkHistoryAsync([employee], cancellationToken);
        }
        return employee;
    }

    public async Task<bool> HasRelatedRecordsAsync(Guid employeeId, CancellationToken cancellationToken) =>
        await _db.WorkHistories.AnyAsync(item => item.EmployeeId == employeeId, cancellationToken)
        || await _db.EmploymentContracts.AnyAsync(item => item.EmployeeId == employeeId, cancellationToken)
        || await _db.ProfileUpdateRequests.AnyAsync(item => item.EmployeeId == employeeId, cancellationToken)
        || await _db.EmployeeNotifications.AnyAsync(item => item.EmployeeId == employeeId, cancellationToken)
        || await _db.OffboardingRecords.AnyAsync(item => item.EmployeeId == employeeId, cancellationToken);

    public Task<bool> CodeExistsAsync(string employeeCode, CancellationToken cancellationToken) =>
        _db.Employees.AnyAsync(employee => employee.EmployeeCode == employeeCode, cancellationToken);

    public Task AddAsync(Employee employee, CancellationToken cancellationToken) =>
        _db.Employees.AddAsync(employee, cancellationToken).AsTask();

    public void Remove(Employee employee) => _db.Employees.Remove(employee);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            _logger.LogWarning(exception, "Hồ sơ nhân viên đã thay đổi đồng thời.");
            throw new HrException(409, "EmployeeChanged", "Hồ sơ nhân viên đã thay đổi. Tải lại dữ liệu và thử lại.");
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            _logger.LogWarning(exception, "Trùng mã nhân viên khi lưu hồ sơ.");
            throw new HrException(409, "DuplicateEmployeeCode", "Mã nhân viên đã tồn tại. Nhập mã khác.");
        }
        catch (DbUpdateException exception) when (IsForeignKeyViolation(exception))
        {
            _logger.LogWarning(exception, "Không thể xóa hồ sơ nhân viên đang được dữ liệu khác tham chiếu.");
            throw new HrException(409, "EmployeeHasRelatedRecords",
                "Không thể xóa nhân viên đang được dữ liệu khác tham chiếu.");
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(exception, "Không lưu được hồ sơ nhân viên.");
            throw new HrException(500, "SaveError", "Không thể lưu hồ sơ. Vui lòng thử lại.");
        }
    }

    private static bool IsUniqueViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException postgres
        && postgres.SqlState == PostgresErrorCodes.UniqueViolation;

    private static bool IsForeignKeyViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException postgres
        && postgres.SqlState == PostgresErrorCodes.ForeignKeyViolation;

    private static string EscapeLike(string value) =>
        value.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");

    private async Task ApplyCurrentWorkHistoryAsync(IReadOnlyList<Employee> employees, CancellationToken cancellationToken)
    {
        if (employees.Count == 0)
        {
            return;
        }
        var employeeIds = employees.Select(item => item.EmployeeId).ToArray();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var assignments = await _db.WorkHistories.AsNoTracking()
            .Include(history => history.Department)
            .Include(history => history.Position)
            .Where(history => employeeIds.Contains(history.EmployeeId)
                && history.StartDate <= today
                && (history.EndDate == null || history.EndDate >= today))
            .OrderByDescending(history => history.StartDate)
            .ToArrayAsync(cancellationToken);
        var currentByEmployee = assignments.GroupBy(history => history.EmployeeId)
            .ToDictionary(group => group.Key, group => group.First());
        foreach (var employee in employees)
        {
            if (!currentByEmployee.TryGetValue(employee.EmployeeId, out var current))
            {
                continue;
            }
            if (_db.Entry(employee).State != EntityState.Detached)
            {
                var department = _db.Departments.Local.FirstOrDefault(item => item.DepartmentId == current.DepartmentId)
                    ?? _db.Attach(current.Department!).Entity;
                var position = _db.Positions.Local.FirstOrDefault(item => item.PositionId == current.PositionId)
                    ?? _db.Attach(current.Position!).Entity;
                employee.Department = department;
                employee.Position = position;
            }
            else
            {
                employee.Department = current.Department;
                employee.Position = current.Position;
            }
            employee.DepartmentId = current.DepartmentId;
            employee.PositionId = current.PositionId;
        }
    }
}
