using HR.Domain.Entities;

namespace HR.Application.Interfaces.Repositories;

public interface IProfileUpdateRepository
{
    Task<ProfileUpdateRequest?> GetOwnedRequestAsync(Guid requestId, Guid employeeId, CancellationToken cancellationToken);
    Task<ProfileUpdateRequest?> GetRequestAsync(Guid requestId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProfileUpdateRequest>> ListOwnedRequestsAsync(Guid employeeId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProfileUpdateRequest>> ListForReviewAsync(string? status, CancellationToken cancellationToken);
    Task<IReadOnlyList<EmployeeNotification>> ListNotificationsAsync(Guid employeeId, CancellationToken cancellationToken);
    Task AddRequestAsync(ProfileUpdateRequest request, CancellationToken cancellationToken);
    Task AddNotificationAsync(EmployeeNotification notification, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    Task SaveChangesInTransactionAsync(CancellationToken cancellationToken);
}
