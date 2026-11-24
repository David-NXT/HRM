using HR.Application.DTOs.Employees;
using HR.Application.DTOs.ProfileUpdates;
using HR.Application.Exceptions;
using HR.Application.Interfaces.Repositories;
using HR.Application.Interfaces.Storage;
using HR.Application.Services.Employees;
using HR.Application.Services.ProfileUpdates;
using HR.Domain.Constants;
using HR.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace HR.Tests;

public sealed class ProfileUpdateVersionTests
{
    private static UpdateEmployeeRequest Changes(string email = "proposed@example.com") =>
        new("Nguyễn Văn A", new(1995, 1, 1), "Male", email, "0901234567", "079195001234", "Hà Nội");

    private static ProfileUpdateService Service(Store store) => new(store, store,
        new EmployeeService(store), new NoEvidenceStore(), NullLogger<ProfileUpdateService>.Instance);

    [Fact]
    public async Task Create_RecordsTheEmployeeVersionThatWasRead()
    {
        var store = new Store();
        var created = await Service(store).CreateAsync(store.Employee.EmployeeId, Changes(), true, [], CancellationToken.None, 7);
        Assert.Equal(7, created.BaseEmployeeVersion);
        Assert.Equal(1, created.Version);
        Assert.Equal(ProfileUpdateStatuses.Pending, created.Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(6)]
    public async Task Create_MissingOrStaleBaseline_DoesNotSave(int? version)
    {
        var store = new Store();
        var error = await Assert.ThrowsAsync<HrException>(() => Service(store)
            .CreateAsync(store.Employee.EmployeeId, Changes(), true, [], CancellationToken.None, version));
        Assert.Equal("EmployeeProfileChanged", error.Code);
        Assert.Null(store.Request);
        Assert.Equal(0, store.SaveCount);
    }

    [Fact]
    public async Task Review_ChangedProfile_IsNotOverwrittenOrApproved()
    {
        var store = new Store();
        var service = Service(store);
        var created = await service.CreateAsync(store.Employee.EmployeeId, Changes(), true, [], CancellationToken.None, 7);
        store.Employee.Email = "newer@example.com";
        store.Employee.Version++;
        var error = await Assert.ThrowsAsync<HrException>(() => service.ReviewAsync(
            created.ProfileUpdateRequestId, new("APPROVE", null, created.Version), CancellationToken.None));
        Assert.Equal("EmployeeProfileChanged", error.Code);
        Assert.Equal("newer@example.com", store.Employee.Email);
        Assert.Equal(ProfileUpdateStatuses.Pending, store.Request!.Status);
        Assert.Empty(store.Notifications);
        Assert.Equal(1, store.SaveCount);
    }

    [Fact]
    public async Task ReturnForMoreInfo_ResubmitAgainstCurrentProfile_ThenApprove()
    {
        var store = new Store();
        var service = Service(store);
        var created = await service.CreateAsync(store.Employee.EmployeeId, Changes(), true, [], CancellationToken.None, 7);
        store.Employee.Version = 8;
        var returned = await service.ReviewAsync(created.ProfileUpdateRequestId,
            new("REQUEST_MORE_INFO", "Đối chiếu hồ sơ mới và gửi lại.", created.Version), CancellationToken.None);
        var submitted = await service.UpdateAsync(store.Employee.EmployeeId, created.ProfileUpdateRequestId,
            Changes("verified@example.com"), true, [], CancellationToken.None, 8, returned.Version);
        var approved = await service.ReviewAsync(created.ProfileUpdateRequestId,
            new("APPROVE", null, submitted.Version), CancellationToken.None);
        Assert.Equal(8, submitted.BaseEmployeeVersion);
        Assert.Equal(ProfileUpdateStatuses.Approved, approved.Status);
        Assert.Equal("verified@example.com", store.Employee.Email);
        Assert.Equal(2, store.Notifications.Count);
    }

    [Fact]
    public async Task Review_StaleRequestVersion_IsRejected()
    {
        var store = new Store();
        var service = Service(store);
        var created = await service.CreateAsync(store.Employee.EmployeeId, Changes(), true, [], CancellationToken.None, 7);
        store.Request!.Version++;
        var error = await Assert.ThrowsAsync<HrException>(() => service.ReviewAsync(created.ProfileUpdateRequestId,
            new("APPROVE", null, created.Version), CancellationToken.None));
        Assert.Equal("ProfileUpdateChanged", error.Code);
        Assert.Empty(store.Notifications);
    }

    [Fact]
    public async Task Update_StaleDraftVersion_IsRejectedBeforeReplacingItsContents()
    {
        var store = new Store();
        var service = Service(store);
        var created = await service.CreateAsync(store.Employee.EmployeeId, Changes(), false, [], CancellationToken.None, 7);
        store.Request!.Version++;
        var original = store.Request.ChangesJson;
        var error = await Assert.ThrowsAsync<HrException>(() => service.UpdateAsync(store.Employee.EmployeeId,
            created.ProfileUpdateRequestId, Changes("stale@example.com"), true, [], CancellationToken.None, 7, created.Version));
        Assert.Equal("ProfileUpdateChanged", error.Code);
        Assert.Equal(original, store.Request.ChangesJson);
    }

    [Fact]
    public async Task Review_LegacyRequestWithoutBaseline_RequiresResubmission()
    {
        var store = new Store();
        var service = Service(store);
        var created = await service.CreateAsync(store.Employee.EmployeeId, Changes(), true, [], CancellationToken.None, 7);
        store.Request!.BaseEmployeeVersion = null;
        var error = await Assert.ThrowsAsync<HrException>(() => service.ReviewAsync(created.ProfileUpdateRequestId,
            new("APPROVE", null, created.Version), CancellationToken.None));
        Assert.Equal("EmployeeProfileChanged", error.Code);
    }

    [Theory]
    [InlineData(EmployeeStatuses.Locked)]
    [InlineData(EmployeeStatuses.Resigned)]
    public async Task Review_InactiveEmployeeCannotBeApproved(string status)
    {
        var store = new Store();
        var service = Service(store);
        var created = await service.CreateAsync(store.Employee.EmployeeId, Changes(), true, [], CancellationToken.None, 7);
        store.Employee.Status = status;
        var error = await Assert.ThrowsAsync<HrException>(() => service.ReviewAsync(created.ProfileUpdateRequestId,
            new("APPROVE", null, created.Version), CancellationToken.None));
        Assert.Equal("EmployeeInactive", error.Code);
        Assert.Empty(store.Notifications);
    }

    [Theory]
    [InlineData("ALL", null)]
    [InlineData("all", null)]
    [InlineData(null, "PENDING")]
    [InlineData("", "PENDING")]
    [InlineData("NEED_INFO", "NEED_INFO")]
    public async Task ReviewFilter_AllExplicitlyRemovesStatusFilter(string? input, string? expected)
    {
        var store = new Store();
        await Service(store).ListForReviewAsync(input, CancellationToken.None);
        Assert.Equal(expected, store.LastFilter);
    }

    private sealed class Store : IEmployeeRepository, IProfileUpdateRepository
    {
        public Employee Employee { get; } = new FullTimeEmployee
        {
            FullName = "Nguyễn Văn A", DateOfBirth = new(1995, 1, 1), Version = 7, Email = "current@example.com"
        };
        public ProfileUpdateRequest? Request { get; set; }
        public List<EmployeeNotification> Notifications { get; } = [];
        public int SaveCount { get; private set; }
        public string? LastFilter { get; private set; }
        public Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct) => Task.FromResult<Employee?>(id == Employee.EmployeeId ? Employee : null);
        public Task<Employee?> GetReadOnlyByIdAsync(Guid id, CancellationToken ct) => GetByIdAsync(id, ct);
        public Task<ProfileUpdateRequest?> GetRequestAsync(Guid id, CancellationToken ct) => Task.FromResult(Request?.ProfileUpdateRequestId == id ? Request : null);
        public Task<ProfileUpdateRequest?> GetOwnedRequestAsync(Guid id, Guid employeeId, CancellationToken ct) => GetRequestAsync(id, ct);
        public Task AddRequestAsync(ProfileUpdateRequest request, CancellationToken ct) { Request = request; return Task.CompletedTask; }
        public Task AddNotificationAsync(EmployeeNotification notification, CancellationToken ct) { Notifications.Add(notification); return Task.CompletedTask; }
        public Task SaveChangesAsync(CancellationToken ct)
        {
            if (SaveCount > 0 && Request is not null) Request.Version++;
            SaveCount++;
            return Task.CompletedTask;
        }
        public Task SaveChangesInTransactionAsync(CancellationToken ct) => SaveChangesAsync(ct);
        public Task<IReadOnlyList<ProfileUpdateRequest>> ListForReviewAsync(string? status, CancellationToken ct)
        {
            LastFilter = status;
            return Task.FromResult<IReadOnlyList<ProfileUpdateRequest>>([]);
        }
        public Task<IReadOnlyList<ProfileUpdateRequest>> ListOwnedRequestsAsync(Guid id, CancellationToken ct) => throw new NotSupportedException();
        public Task<IReadOnlyList<EmployeeNotification>> ListNotificationsAsync(Guid id, CancellationToken ct) => throw new NotSupportedException();
        public Task<(IReadOnlyList<Employee> Items, int Total)> SearchAsync(string? keyword, int page, int pageSize, CancellationToken ct) => throw new NotSupportedException();
        public Task<(IReadOnlyList<Employee> Items, int Total)> SearchActiveAsync(string? keyword, int page, int pageSize, CancellationToken ct) => throw new NotSupportedException();
        public Task<EmployeeStatisticsDto> GetStatisticsAsync(CancellationToken ct) => throw new NotSupportedException();
        public Task<bool> HasRelatedRecordsAsync(Guid id, CancellationToken ct) => throw new NotSupportedException();
        public Task<bool> CodeExistsAsync(string code, CancellationToken ct) => throw new NotSupportedException();
        public Task AddAsync(Employee employee, CancellationToken ct) => throw new NotSupportedException();
        public void Remove(Employee employee) => throw new NotSupportedException();
    }

    private sealed class NoEvidenceStore : IEmployeeUpdateEvidenceStore
    {
        public Task<StoredEvidence> SaveAsync(EvidenceUpload upload, CancellationToken ct) => throw new NotSupportedException();
        public Task DeleteAsync(string key, CancellationToken ct) => throw new NotSupportedException();
        public Task<EvidenceDownload> OpenReadAsync(string key, string name, string type, CancellationToken ct) => throw new NotSupportedException();
    }
}
