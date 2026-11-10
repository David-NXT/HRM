using HR.Core.Entities;

namespace HR.Core.Interfaces;

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

    Task<Employee?> GetByGuidAsync(Guid employeeGuid, CancellationToken cancellationToken);

    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<bool> CodeExistsAsync(string employeeCode, CancellationToken cancellationToken);

    Task AddAsync(Employee employee, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
