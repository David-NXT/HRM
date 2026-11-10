using Microsoft.Extensions.Logging;
using HR.Application.Interfaces.Repositories;
using HR.Domain.Entities;
using HR.Application.Exceptions;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HR.Infrastructure.Repositories;

public sealed class ProfileUpdateRepository : IProfileUpdateRepository
{
    private readonly HrDbContext _db;
    private readonly ILogger<ProfileUpdateRepository> _logger;

    public ProfileUpdateRepository(HrDbContext db, ILogger<ProfileUpdateRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public Task<ProfileUpdateRequest?> GetOwnedRequestAsync(Guid requestId, Guid employeeId, CancellationToken cancellationToken) =>
        _db.ProfileUpdateRequests.Include(request => request.Attachments)
            .FirstOrDefaultAsync(request => request.ProfileUpdateRequestId == requestId && request.EmployeeId == employeeId, cancellationToken);

    public Task<ProfileUpdateRequest?> GetRequestAsync(Guid requestId, CancellationToken cancellationToken) =>
        _db.ProfileUpdateRequests.Include(request => request.Attachments)
            .FirstOrDefaultAsync(request => request.ProfileUpdateRequestId == requestId, cancellationToken);

    public async Task<IReadOnlyList<ProfileUpdateRequest>> ListOwnedRequestsAsync(Guid employeeId, CancellationToken cancellationToken) =>
        await _db.ProfileUpdateRequests.AsNoTracking().Include(request => request.Attachments)
            .Where(request => request.EmployeeId == employeeId)
            .OrderByDescending(request => request.CreatedAt)
            .ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<ProfileUpdateRequest>> ListForReviewAsync(string? status, CancellationToken cancellationToken)
    {
        var query = _db.ProfileUpdateRequests.AsNoTracking().Include(request => request.Attachments).AsQueryable();
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(request => request.Status == status);
        }
        return await query.OrderByDescending(request => request.SubmittedAt ?? request.CreatedAt).ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EmployeeNotification>> ListNotificationsAsync(Guid employeeId, CancellationToken cancellationToken) =>
        await _db.EmployeeNotifications.AsNoTracking()
            .Where(notification => notification.EmployeeId == employeeId)
            .OrderByDescending(notification => notification.CreatedAt)
            .Take(100)
            .ToArrayAsync(cancellationToken);

    public Task AddRequestAsync(ProfileUpdateRequest request, CancellationToken cancellationToken) =>
        _db.ProfileUpdateRequests.AddAsync(request, cancellationToken).AsTask();

    public Task AddNotificationAsync(EmployeeNotification notification, CancellationToken cancellationToken) =>
        _db.EmployeeNotifications.AddAsync(notification, cancellationToken).AsTask();

    public Task SaveChangesAsync(CancellationToken cancellationToken) => SaveCoreAsync(cancellationToken);

    public async Task SaveChangesInTransactionAsync(CancellationToken cancellationToken)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        await SaveCoreAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task SaveCoreAsync(CancellationToken cancellationToken)
    {
        try
        {
            foreach (var entry in _db.ChangeTracker.Entries<ProfileUpdateRequest>()
                .Where(entry => entry.State == EntityState.Modified))
            {
                entry.Entity.Version++;
            }
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            _logger.LogWarning(exception, "Yêu cầu hồ sơ đã được thay đổi đồng thời.");
            throw new HrException(409, "AlreadyProcessed", "Yêu cầu đã được người khác xử lý hoặc cập nhật. Tải lại dữ liệu.");
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException postgres
            && postgres.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            _logger.LogWarning(exception, "Tham chiếu không hợp lệ khi lưu yêu cầu cập nhật hồ sơ.");
            throw new HrException(404, "ProfileUpdateTargetNotFound", "Không tìm thấy hồ sơ hoặc yêu cầu cập nhật.");
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(exception, "Không thể lưu yêu cầu cập nhật hồ sơ.");
            throw new HrException(500, "SaveError", "Không thể lưu yêu cầu cập nhật. Vui lòng thử lại.");
        }
    }
}
