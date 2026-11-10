using HR.Core.Entities;
using HR.Core.Exceptions;
using HR.Core.Interfaces;
using HR.Infrastructure.Data;
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
        var query = _db.Employees.AsNoTracking();
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

        return (items, total);
    }

    public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        _db.Employees.FirstOrDefaultAsync(employee => employee.EmployeeId == id, cancellationToken);

    public Task<bool> CodeExistsAsync(string employeeCode, CancellationToken cancellationToken) =>
        _db.Employees.AnyAsync(employee => employee.EmployeeCode == employeeCode, cancellationToken);

    public Task AddAsync(Employee employee, CancellationToken cancellationToken) =>
        _db.Employees.AddAsync(employee, cancellationToken).AsTask();

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            _logger.LogWarning(exception, "Trùng mã nhân viên khi lưu hồ sơ.");
            throw new HrException(409, "DuplicateEmployeeCode", "Mã nhân viên đã tồn tại. Nhập mã khác.");
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

    private static string EscapeLike(string value) =>
        value.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
}
