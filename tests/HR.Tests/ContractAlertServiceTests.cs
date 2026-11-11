using HR.Application.Interfaces.Repositories;
using HR.Application.Services.ContractAlerts;
using HR.Domain.Constants;
using HR.Domain.Entities;
using HR.Application.Exceptions;
using HR.Infrastructure.Jobs;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace HR.Tests;

public sealed class ContractAlertServiceTests
{
    private static readonly DateOnly Today = new(2026, 11, 10);

    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, true)]
    [InlineData(30, true)]
    [InlineData(31, false)]
    public async Task Scan_UsesInclusiveZeroToThirtyDayWindow(int days, bool expectedCreation)
    {
        var repository = new FakeRepository();
        repository.Contracts.Add(Contract(1, days));
        var result = await Service(repository).GenerateAlertsAsync(CancellationToken.None);

        Assert.Equal(expectedCreation ? 1 : 0, result.CreatedCount);
        Assert.Equal(0, result.FailedCount);
    }

    [Fact]
    public async Task Scan_SkipsIndefiniteInvalidAndRenewedContracts()
    {
        var repository = new FakeRepository();
        var indefinite = Contract(1, 10); indefinite.EndDate = null;
        var invalid = Contract(2, 10); invalid.StartDate = invalid.EndDate!.Value;
        var renewed = Contract(3, 10); renewed.Status = ContractStatuses.Renewed;
        repository.Contracts.AddRange([indefinite, invalid, renewed]);

        var result = await Service(repository).GenerateAlertsAsync(CancellationToken.None);

        Assert.Equal(3, result.SkippedCount);
        Assert.Equal(0, result.CreatedCount);
        Assert.Empty(repository.InsertCalls);
    }

    [Fact]
    public async Task Scan_IsIdempotentForContractAndExpiryButKeepsNewExpiryCycle()
    {
        var repository = new FakeRepository();
        var contract = Contract(1, 10); repository.Contracts.Add(contract);
        var service = Service(repository);

        Assert.Equal(1, (await service.GenerateAlertsAsync(CancellationToken.None)).CreatedCount);
        Assert.Equal(0, (await service.GenerateAlertsAsync(CancellationToken.None)).CreatedCount);
        contract.EndDate = Today.AddDays(11);
        Assert.Equal(1, (await service.GenerateAlertsAsync(CancellationToken.None)).CreatedCount);
        Assert.Equal(2, repository.Alerts.Count);
    }

    [Fact]
    public async Task Scan_LogsIndividualFailureAndContinuesWithoutRetry()
    {
        var repository = new FakeRepository();
        repository.Contracts.AddRange([Contract(1, 10), Contract(2, 20)]);
        repository.FailingContractIds.Add(Id(1));

        var result = await Service(repository).GenerateAlertsAsync(CancellationToken.None);

        Assert.Equal(1, result.FailedCount);
        Assert.Equal(1, result.CreatedCount);
        Assert.Equal(1, repository.InsertCalls[Id(1)]);
        Assert.Equal(1, repository.InsertCalls[Id(2)]);
    }

    [Fact]
    public async Task Scan_QueryFailureDoesNotReportSuccess()
    {
        var repository = new FakeRepository { FailQuery = true };
        await Assert.ThrowsAsync<InvalidOperationException>(() => Service(repository).GenerateAlertsAsync(CancellationToken.None));
        Assert.Empty(repository.InsertCalls);
    }

    [Fact]
    public async Task Scan_ConcurrentInsertOrChangedContractIsSkipped()
    {
        var repository = new FakeRepository { RejectInsert = true };
        repository.Contracts.Add(Contract(1, 10));
        var result = await Service(repository).GenerateAlertsAsync(CancellationToken.None);

        Assert.Equal(1, result.SkippedCount);
        Assert.Equal(0, result.FailedCount);
        Assert.Empty(repository.Alerts);
    }

    [Fact]
    public async Task Scan_CancellationPropagates()
    {
        var repository = new FakeRepository(); repository.Contracts.Add(Contract(1, 10));
        using var cancellation = new CancellationTokenSource(); cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Service(repository).GenerateAlertsAsync(cancellation.Token));
    }

    [Fact]
    public async Task StatusUpdate_StaleVersionCannotOverwriteAnotherReviewer()
    {
        var repository = WithAlert();
        var error = await Assert.ThrowsAsync<HrException>(() => Service(repository).UpdateStatusAsync(
            Id(1), new(ContractAlertStatuses.Viewed, 1), CancellationToken.None));

        Assert.Equal(409, error.StatusCode);
        Assert.Equal(ContractAlertStatuses.New, repository.Alerts[0].Status);
        Assert.Equal(0, repository.Saves);
    }

    [Fact]
    public async Task StatusUpdate_ValidStateIsSaved()
    {
        var repository = WithAlert();
        var result = await Service(repository).UpdateStatusAsync(Id(1), new("viewed", 2), CancellationToken.None);
        Assert.Equal(ContractAlertStatuses.Viewed, result.Status);
        Assert.Equal(1, repository.Saves);
    }

    [Fact]
    public async Task StatusUpdate_UnknownStateReturns400()
    {
        var repository = WithAlert();
        var error = await Assert.ThrowsAsync<HrException>(() => Service(repository).UpdateStatusAsync(Id(1), new("DONE", 2), CancellationToken.None));
        Assert.Equal(400, error.StatusCode);
        Assert.Equal(0, repository.Saves);
    }

    [Fact]
    public async Task Detail_OldCycleRemainsVisibleAfterRenewal()
    {
        var repository = WithAlert();
        repository.Alerts[0].Contract!.Status = ContractStatuses.Renewed;
        var result = await Service(repository).GetAsync(Id(1), CancellationToken.None);
        Assert.False(result.IsCurrentCycle);
        Assert.Equal(Today.AddDays(10), result.ExpiryDate);
        Assert.Equal(ContractStatuses.Renewed, result.ContractStatus);
    }

    [Fact]
    public async Task List_InvalidDateRangeReturns400BeforeQuery()
    {
        var repository = new FakeRepository();
        var error = await Assert.ThrowsAsync<HrException>(() => Service(repository).ListAsync(null, null,
            Today.AddDays(1), Today, 1, 20, CancellationToken.None));
        Assert.Equal(400, error.StatusCode);
        Assert.Equal(0, repository.SearchCalls);
    }

    [Fact]
    public async Task Detail_MissingAlertReturns404()
    {
        var error = await Assert.ThrowsAsync<HrException>(() => Service(new FakeRepository()).GetAsync(Id(99), CancellationToken.None));
        Assert.Equal(404, error.StatusCode);
    }

    [Theory]
    [InlineData(0, 0, 10)]
    [InlineData(1, 0, 11)]
    [InlineData(2, 0, 11)]
    public void Schedule_ChoosesNextDailyUtcSlot(int hour, int minute, int expectedDay)
    {
        var now = new DateTimeOffset(2026, 11, 10, hour, minute, 0, TimeSpan.Zero);
        var next = ContractExpiryJob.GetNextRunUtc(now, new(1, 0));
        Assert.Equal(new DateTimeOffset(2026, 11, expectedDay, 1, 0, 0, TimeSpan.Zero), next);
    }

    private static ContractAlertService Service(FakeRepository repository) => new(repository, new FixedClock(), NullLogger<ContractAlertService>.Instance);
    private sealed class FixedClock : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => new(2026, 11, 10, 12, 0, 0, TimeSpan.Zero);
    }

    private static EmploymentContract Contract(int seed, int days) => new()
    {
        ContractId = Id(seed), EmployeeId = Id(1), ContractNumber = "HD" + seed, ContractType = "Fixed term",
        StartDate = Today.AddYears(-1), EndDate = Today.AddDays(days), Status = ContractStatuses.Active,
        Employee = new FullTimeEmployee { EmployeeId = Id(1), EmployeeCode = "NV001", FullName = "Nguyễn Văn A", Status = EmployeeStatuses.Active }
    };

    private static FakeRepository WithAlert()
    {
        var repository = new FakeRepository();
        var contract = Contract(1, 10);
        repository.Alerts.Add(new ContractAlert { ContractAlertId = Id(1), ContractId = Id(1), Contract = contract,
            ExpiryDate = contract.EndDate!.Value, Version = 2 });
        return repository;
    }

    private sealed class FakeRepository : IContractAlertRepository
    {
        public List<EmploymentContract> Contracts { get; } = [];
        public List<ContractAlert> Alerts { get; } = [];
        public HashSet<Guid> FailingContractIds { get; } = [];
        public Dictionary<Guid, int> InsertCalls { get; } = [];
        public bool FailQuery { get; init; }
        public bool RejectInsert { get; init; }
        public int Saves { get; private set; }
        public int SearchCalls { get; private set; }
        public Task<IReadOnlyList<EmploymentContract>> ListActiveContractsAsync(CancellationToken ct) =>
            FailQuery ? throw new InvalidOperationException("Database unavailable") : Task.FromResult<IReadOnlyList<EmploymentContract>>(Contracts);
        public Task<bool> AlertExistsAsync(Guid id, DateOnly date, CancellationToken ct) =>
            Task.FromResult(Alerts.Any(alert => alert.ContractId == id && alert.ExpiryDate == date));
        public Task<bool> TryCreateAsync(ContractAlert alert, CancellationToken ct)
        {
            InsertCalls[alert.ContractId] = InsertCalls.GetValueOrDefault(alert.ContractId) + 1;
            if (FailingContractIds.Contains(alert.ContractId)) throw new InvalidOperationException("Insert failed");
            if (RejectInsert) return Task.FromResult(false);
            Alerts.Add(alert); return Task.FromResult(true);
        }
        public Task<(IReadOnlyList<ContractAlert> Items, int Total)> SearchAsync(string? status, Guid? id,
            DateOnly? from, DateOnly? to, int page, int pageSize, CancellationToken ct)
        { SearchCalls++; return Task.FromResult(((IReadOnlyList<ContractAlert>)Alerts, Alerts.Count)); }
        public Task<ContractAlert?> GetAsync(Guid id, CancellationToken ct) => Task.FromResult(Alerts.FirstOrDefault(alert => alert.ContractAlertId == id));
        public Task SaveChangesAsync(CancellationToken ct) { Saves++; return Task.CompletedTask; }
    }
}
