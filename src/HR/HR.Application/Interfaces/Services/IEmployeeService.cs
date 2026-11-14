using HR.Application.DTOs.Employees;
using HR.Domain.Entities;
namespace HR.Application.Interfaces.Services;

public interface IEmployeeService
{
    Task<EmployeeListDto> ListAsync(string? keyword, int page, int pageSize, CancellationToken cancellationToken);

    Task<EmployeeStatisticsDto> GetStatisticsAsync(CancellationToken cancellationToken);

    Task<EmployeeDirectoryListDto> ListActiveDirectoryAsync(string? keyword, int page, int pageSize, CancellationToken cancellationToken);

    Task<EmployeeDirectoryEntry> GetActiveDirectoryEntryAsync(Guid employeeId, CancellationToken cancellationToken);

    Task<EmployeeDto> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    HR.Domain.Entities.Employee PrepareNewEmployee(CreateEmployeeRequest request);

    Task<EmployeeDto> UpdateAsync(Guid id, UpdateEmployeeRequest request, CancellationToken cancellationToken);

    Task<EmployeeDto> UpdateSalaryAsync(Guid id, UpdateEmployeeSalaryRequest request, CancellationToken cancellationToken);

    void ValidatePersonalInfoUpdate(UpdateEmployeeRequest request);

    void ApplyPersonalInfoUpdate(HR.Domain.Entities.Employee employee, UpdateEmployeeRequest request);

    Task<EmployeeDto> LockAsync(Guid id, CancellationToken cancellationToken);
}
