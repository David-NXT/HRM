using HR.API.Controllers;
using HR.Application.DTOs.Contracts;
using HR.Application.Services.ContractAlerts;
using HR.Application.Services.Contracts;
using HR.Application.Interfaces.Repositories;
using HR.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace HR.Tests;

public sealed class ContractAlertImmediateTests
{
    private static readonly DateOnly Today = new(2026, 11, 16);
    private static readonly Guid EmployeeId = Guid.NewGuid();

    [Fact]
    public async Task Create_ContractEndingInThreeDays_GeneratesAlertAfterSaving()
    {
        var storage = new Storage();
        var created = await Service(storage).CreateContractAsync(EmployeeId, Request(3), CancellationToken.None);
        var alert = Assert.Single(storage.Alerts);
        Assert.Equal(created.ContractId, alert.ContractId);
        Assert.Equal(Today.AddDays(3), alert.ExpiryDate);
        Assert.Equal("NEW", alert.Status);
        Assert.Equal(0, storage.FullScans);
        Assert.Contains(created.ContractId, storage.SavedContracts);
    }

    [Fact]
    public async Task Update_FromDistantExpiryToThreeDays_GeneratesImmediatelyWithoutDuplicates()
    {
        var storage = new Storage();
        var service = Service(storage);
        var created = await service.CreateContractAsync(EmployeeId, Request(60), CancellationToken.None);
        Assert.Empty(storage.Alerts);
        var update = new UpdateEmploymentContractRequest("HD01", "Fixed", Today.AddDays(-20), Today.AddDays(3), null);
        await service.UpdateContractAsync(created.ContractId, update, CancellationToken.None);
        await service.UpdateContractAsync(created.ContractId, update, CancellationToken.None);
        Assert.Single(storage.Alerts);
        await service.UpdateContractAsync(created.ContractId, update with { EndDate = Today.AddDays(4) }, CancellationToken.None);
        Assert.Equal(2, storage.Alerts.Count);
        Assert.Contains(storage.Alerts, alert => alert.ExpiryDate == Today.AddDays(4));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(31)]
    [InlineData(null)]
    public async Task Create_OutsideWarningWindow_DoesNotCreateAlert(int? remaining)
    {
        var storage = new Storage();
        await Service(storage).CreateContractAsync(EmployeeId, Request(remaining), CancellationToken.None);
        Assert.Empty(storage.Alerts);
    }

    [Fact]
    public async Task FutureDailyOrManualScan_FindsContractThatHasJustEnteredThirtyDayWindow()
    {
        var storage = new Storage();
        var clock = new AdjustableClock();
        var generator = Generator(storage, clock);
        await new ContractService(storage, generator).CreateContractAsync(EmployeeId, Request(31), CancellationToken.None);
        Assert.Empty(storage.Alerts);
        clock.Now = clock.Now.AddDays(1);
        var result = await generator.GenerateAlertsAsync(CancellationToken.None);
        Assert.Equal(1, result.CreatedCount);
        Assert.Single(storage.Alerts);
    }

    [Fact]
    public async Task Renewal_ChecksNewContract_AndLeavesOldAlertHistoryIntact()
    {
        var storage = new Storage();
        var service = Service(storage);
        var old = await service.CreateContractAsync(EmployeeId, Request(0), CancellationToken.None);
        var renewed = await service.CreateContractAsync(EmployeeId,
            new CreateEmploymentContractRequest("HD02", "Fixed", Today.AddDays(1), Today.AddDays(3), null, old.ContractId), CancellationToken.None);
        Assert.Equal("RENEWED", storage.Contracts.Single(contract => contract.ContractId == old.ContractId).Status);
        Assert.Equal(2, storage.Alerts.Count);
        Assert.Contains(storage.Alerts, alert => alert.ContractId == renewed.ContractId);
    }

    [Fact]
    public async Task FailedContractSave_DoesNotGenerateAlert()
    {
        var storage = new Storage { FailSave = true };
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Service(storage).CreateContractAsync(EmployeeId, Request(3), CancellationToken.None));
        Assert.Empty(storage.Alerts);
        Assert.Empty(storage.SavedContracts);
    }

    [Fact]
    public async Task AlertFailure_DoesNotMisreportSavedContractAsFailed_AndScanCanRecover()
    {
        var storage = new Storage { FailAlertInsert = true };
        var created = await Service(storage).CreateContractAsync(EmployeeId, Request(3), CancellationToken.None);
        Assert.Contains(created.ContractId, storage.SavedContracts);
        Assert.Empty(storage.Alerts);
        var generator = Generator(storage);
        var failed = await generator.GenerateAlertsAsync(CancellationToken.None);
        Assert.Equal(1, failed.FailedCount);
        storage.FailAlertInsert = false;
        var recovered = await generator.GenerateAlertsAsync(CancellationToken.None);
        Assert.Equal(1, recovered.CreatedCount);
        Assert.Single(storage.Alerts);
    }

    [Fact]
    public async Task ManualScanEndpoint_ReportsCreatedAndSkippedCounts_WithoutDuplicates()
    {
        var storage = new Storage();
        storage.Contracts.Add(new EmploymentContract { EmployeeId = EmployeeId, StartDate = Today.AddDays(-20), EndDate = Today.AddDays(3) });
        storage.SavedContracts.Add(storage.Contracts[0].ContractId);
        var controller = new ContractAlertsController(Generator(storage));
        var first = Assert.IsType<OkObjectResult>((await controller.Scan(CancellationToken.None)).Result);
        var second = Assert.IsType<OkObjectResult>((await controller.Scan(CancellationToken.None)).Result);
        Assert.Equal(1, Assert.IsType<HR.Application.DTOs.ContractAlerts.ContractAlertScanResult>(first.Value).CreatedCount);
        Assert.Equal(1, Assert.IsType<HR.Application.DTOs.ContractAlerts.ContractAlertScanResult>(second.Value).SkippedCount);
        Assert.Single(storage.Alerts);
    }

    private static CreateEmploymentContractRequest Request(int? days) =>
        new("HD01", "Fixed", Today.AddDays(-20), days is int remaining ? Today.AddDays(remaining) : null, null, null);
    private static ContractService Service(Storage storage) => new(storage, Generator(storage));
    private static ContractAlertService Generator(Storage storage, AdjustableClock? clock = null) =>
        new(storage, clock ?? new AdjustableClock(), NullLogger<ContractAlertService>.Instance);
    private sealed class AdjustableClock : TimeProvider
    {
        public DateTimeOffset Now { get; set; } = new(2026, 11, 16, 12, 0, 0, TimeSpan.Zero);
        public override DateTimeOffset GetUtcNow() => Now;
    }

    private sealed class Storage : IEmploymentRepository, IContractAlertRepository
    {
        public List<EmploymentContract> Contracts { get; } = [];
        public List<ContractAlert> Alerts { get; } = [];
        public HashSet<Guid> SavedContracts { get; } = [];
        public bool FailSave { get; init; }
        public bool FailAlertInsert { get; set; }
        public int FullScans { get; private set; }
        public Task<Employee?> GetEmployeeAsync(Guid id, CancellationToken token) =>
            Task.FromResult<Employee?>(new FullTimeEmployee { EmployeeId = id, Status = "ACTIVE" });
        public Task<EmploymentContract?> GetContractAsync(Guid id, CancellationToken token) =>
            Task.FromResult(Contracts.FirstOrDefault(contract => contract.ContractId == id));
        public Task<IReadOnlyList<EmploymentContract>> GetContractsAsync(Guid id, CancellationToken token) =>
            Task.FromResult<IReadOnlyList<EmploymentContract>>(Contracts.Where(contract => contract.EmployeeId == id).ToArray());
        public Task<bool> ContractNumberExistsAsync(string number, Guid? except, CancellationToken token) =>
            Task.FromResult(Contracts.Any(contract => contract.ContractNumber == number && contract.ContractId != except));
        public Task<bool> RenewalAlreadyExistsAsync(Guid id, CancellationToken token) =>
            Task.FromResult(Contracts.Any(contract => contract.RenewalOfContractId == id));
        public Task AddContractAsync(EmploymentContract contract, CancellationToken token)
        { Contracts.Add(contract); return Task.CompletedTask; }
        public Task SaveChangesAsync(CancellationToken token)
        {
            if (FailSave) throw new InvalidOperationException("Contract save failed");
            SavedContracts.UnionWith(Contracts.Select(contract => contract.ContractId));
            return Task.CompletedTask;
        }
        public Task SaveChangesInTransactionAsync(CancellationToken token) => SaveChangesAsync(token);
        public Task<IReadOnlyList<EmploymentContract>> ListActiveContractsAsync(CancellationToken token)
        {
            FullScans++;
            return Task.FromResult<IReadOnlyList<EmploymentContract>>(Contracts.Where(contract => contract.Status == "ACTIVE").ToArray());
        }
        public Task<bool> AlertExistsAsync(Guid id, DateOnly expiry, CancellationToken token) =>
            Task.FromResult(Alerts.Any(alert => alert.ContractId == id && alert.ExpiryDate == expiry));
        public Task<bool> TryCreateAsync(ContractAlert alert, CancellationToken token)
        {
            Assert.Contains(alert.ContractId, SavedContracts);
            if (FailAlertInsert) throw new InvalidOperationException("Alert insert failed");
            if (Alerts.Any(existing => existing.ContractId == alert.ContractId && existing.ExpiryDate == alert.ExpiryDate)) return Task.FromResult(false);
            Alerts.Add(alert);
            return Task.FromResult(true);
        }
        public Task<IReadOnlyList<WorkHistory>> GetWorkHistoryAsync(Guid id, CancellationToken token) => throw new NotSupportedException();
        public Task<WorkHistory?> GetOpenWorkHistoryAsync(Guid id, CancellationToken token) => throw new NotSupportedException();
        public Task<Department?> GetActiveDepartmentAsync(Guid id, CancellationToken token) => throw new NotSupportedException();
        public Task<Position?> GetActivePositionAsync(Guid id, CancellationToken token) => throw new NotSupportedException();
        public Task AddWorkHistoryAsync(WorkHistory history, CancellationToken token) => throw new NotSupportedException();
        public Task<(IReadOnlyList<ContractAlert> Items, int Total)> SearchAsync(string? status, Guid? employeeId,
            DateOnly? from, DateOnly? to, int page, int pageSize, CancellationToken token) => throw new NotSupportedException();
        public Task<ContractAlert?> GetAsync(Guid id, CancellationToken token) => throw new NotSupportedException();
    }
}
