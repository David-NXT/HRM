using HR.Domain.Entities;

namespace HR.Application.Interfaces.Repositories;

public interface IEmploymentRepository
{
    Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken);
    Task<EmploymentContract?> GetContractAsync(Guid contractId, CancellationToken cancellationToken);
    Task<IReadOnlyList<EmploymentContract>> GetContractsAsync(Guid employeeId, CancellationToken cancellationToken);
    Task<IReadOnlyList<WorkHistory>> GetWorkHistoryAsync(Guid employeeId, CancellationToken cancellationToken);
    Task<WorkHistory?> GetOpenWorkHistoryAsync(Guid employeeId, CancellationToken cancellationToken);
    Task<Department?> GetActiveDepartmentAsync(Guid departmentId, CancellationToken cancellationToken);
    Task<Position?> GetActivePositionAsync(Guid positionId, CancellationToken cancellationToken);
    Task<bool> ContractNumberExistsAsync(string contractNumber, Guid? excludingId, CancellationToken cancellationToken);
    Task<bool> RenewalAlreadyExistsAsync(Guid contractId, CancellationToken cancellationToken);
    Task AddContractAsync(EmploymentContract contract, CancellationToken cancellationToken);
    Task AddWorkHistoryAsync(WorkHistory workHistory, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    Task SaveChangesInTransactionAsync(CancellationToken cancellationToken);
}
