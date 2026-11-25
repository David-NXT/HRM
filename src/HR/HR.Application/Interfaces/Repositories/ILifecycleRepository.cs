using HR.Domain.Entities;

namespace HR.Application.Interfaces.Repositories;

public interface ILifecycleRepository
{
    Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken);
    Task<IReadOnlyList<WorkHistory>> GetWorkHistoryAsync(Guid employeeId, CancellationToken cancellationToken);
    // Lấy hợp đồng có tracking để lưu đồng bộ cùng nhân viên, công tác và hồ sơ nghỉ việc.
    Task<IReadOnlyList<EmploymentContract>> GetContractsAsync(Guid employeeId, CancellationToken cancellationToken);
    Task<bool> HasPendingOffboardingAsync(Guid employeeId, CancellationToken cancellationToken);
    Task<OffboardingRecord?> GetOffboardingAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<OffboardingRecord>> ListOffboardingsAsync(Guid? employeeId, string? status, CancellationToken cancellationToken);
    Task AddOnboardingAsync(Employee employee, WorkHistory firstAssignment, CancellationToken cancellationToken);
    Task AddOffboardingAsync(OffboardingRecord record, CancellationToken cancellationToken);
    Task SaveChangesInTransactionAsync(CancellationToken cancellationToken);
}
