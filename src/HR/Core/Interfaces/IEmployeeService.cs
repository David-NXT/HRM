using HR.Core.DTOs;

namespace HR.Core.Interfaces;

public interface IEmployeeService
{
    Task<EmployeeListDto> ListAsync(string? keyword, int page, int pageSize, CancellationToken cancellationToken);

    Task<EmployeeDto> GetAsync(int id, CancellationToken cancellationToken);

    Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken);

    Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken cancellationToken);

    Task<EmployeeDto> LockAsync(int id, CancellationToken cancellationToken);
}
