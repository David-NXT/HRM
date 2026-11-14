using HR.Application.DTOs.Employees;
using HR.Domain.Entities;

namespace HR.Application.Interfaces.Repositories;

public interface IEmployeeRepository
{
    Task<(IReadOnlyList<Employee> Items, int Total)> SearchAsync(
        string? keyword,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<(IReadOnlyList<Employee> Items, int Total)> SearchActiveAsync(
        string? keyword,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<EmployeeStatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken);

    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> HasRelatedRecordsAsync(Guid employeeId, CancellationToken cancellationToken);

    Task<bool> CodeExistsAsync(string employeeCode, CancellationToken cancellationToken);

    Task AddAsync(Employee employee, CancellationToken cancellationToken);

    void Remove(Employee employee);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
