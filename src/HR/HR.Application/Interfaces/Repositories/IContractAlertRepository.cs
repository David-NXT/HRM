using HR.Domain.Entities;

namespace HR.Application.Interfaces.Repositories;

public interface IContractAlertRepository
{
    Task<IReadOnlyList<EmploymentContract>> ListActiveContractsAsync(CancellationToken cancellationToken);
    Task<bool> AlertExistsAsync(Guid contractId, DateOnly expiryDate, CancellationToken cancellationToken);
    Task<bool> TryCreateAsync(ContractAlert alert, CancellationToken cancellationToken);
    Task<(IReadOnlyList<ContractAlert> Items, int Total)> SearchAsync(
        string? status, Guid? employeeId, DateOnly? expiresFrom, DateOnly? expiresTo,
        int page, int pageSize, CancellationToken cancellationToken);
    Task<ContractAlert?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
