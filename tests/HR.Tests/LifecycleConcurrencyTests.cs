using HR.Domain.Constants;
using HR.Domain.Entities;
using HR.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace HR.Tests;

public sealed class LifecycleConcurrencyTests
{
    [Fact]
    public async Task AddingContract_AdvancesTheTrackedEmployeeVersionWithoutPersonalFieldChanges()
    {
        await using var db = Context();
        var employee = new FullTimeEmployee { EmployeeId = Id(61), Version = 8 };
        db.Attach(employee);
        db.EmploymentContracts.Add(new() { EmployeeId = employee.EmployeeId, Employee = employee, ContractNumber = "HD-NEW" });

        await db.SaveChangesAsync();

        Assert.Equal(9, employee.Version);
    }

    [Fact]
    public async Task UpdatingContract_AlsoAdvancesEmployeeVersionToConflictWithConcurrentOffboarding()
    {
        await using var db = Context();
        var employee = new FullTimeEmployee { EmployeeId = Id(62), Version = 4 };
        var contract = new EmploymentContract { EmployeeId = employee.EmployeeId, Employee = employee, StartDate = new(2026, 1, 1), EndDate = new(2026, 11, 1) };
        db.Attach(contract);
        contract.EndDate = new(2026, 12, 1);

        await db.SaveChangesAsync();

        Assert.Equal(5, employee.Version);
    }

    [Fact]
    public async Task AddedEmployeeAndInitialContract_KeepTheInitialEmployeeVersion()
    {
        await using var db = Context();
        var employee = new FullTimeEmployee { EmployeeId = Id(63), Version = 1 };
        db.Employees.Add(employee);
        db.EmploymentContracts.Add(new() { EmployeeId = employee.EmployeeId, Employee = employee, ContractNumber = "HD-INITIAL" });

        await db.SaveChangesAsync();

        Assert.Equal(1, employee.Version);
    }

    [Fact]
    public async Task SaveChanges_PartTimeUsesTheInheritedConcurrencyVersion()
    {
        await using var db = Context();
        var employee = new PartTimeEmployee { EmployeeId = Id(31), Version = 7 };
        db.Attach(employee);
        employee.Status = EmployeeStatuses.Resigned;
        await db.SaveChangesAsync();

        Assert.Equal(8, employee.Version);
        Assert.Equal(EmploymentTypes.PartTime, employee.EmploymentType);
    }

    [Fact]
    public async Task SaveChanges_EmployeeVersionAdvancesForResignation()
    {
        await using var db = Context();
        var employee = new FullTimeEmployee { EmployeeId = Id(1), Version = 4 };
        db.Attach(employee);
        employee.Status = EmployeeStatuses.Resigned;

        await db.SaveChangesAsync();

        Assert.Equal(5, employee.Version);
        Assert.True(db.Model.FindEntityType(typeof(Employee))!.FindProperty(nameof(Employee.Version))!.IsConcurrencyToken);
    }

    [Fact]
    public async Task ChecklistChangeAdvancesParentVersionEvenWhenTimestampIsUnchanged()
    {
        await using var db = Context();
        var record = new OffboardingRecord { OffboardingRecordId = Id(1), EmployeeId = Id(1), Version = 2,
            Checklist = [new() { OffboardingChecklistItemId = Id(1), OffboardingRecordId = Id(1), Title = "Bàn giao" }] };
        db.Attach(record);
        record.Checklist[0].IsCompleted = true;

        await db.SaveChangesAsync();

        Assert.Equal(3, record.Version);
    }

    [Fact]
    public async Task AlertStatusUpdate_AdvancesVersionAndProtectsContractExpiryPair()
    {
        await using var db = Context();
        var alert = new ContractAlert { ContractAlertId = Id(1), ContractId = Id(1), ExpiryDate = new(2026, 11, 10), Version = 3 };
        db.Attach(alert); alert.Status = ContractAlertStatuses.Viewed;
        await db.SaveChangesAsync();
        Assert.Equal(4, alert.Version);
        var index = db.Model.FindEntityType(typeof(ContractAlert))!.GetIndexes()
            .Single(item => item.GetDatabaseName() == "UX_contract_alert_contract_expiry");
        Assert.True(index.IsUnique);
        Assert.Equal(new[] { "ContractId", "ExpiryDate" }, index.Properties.Select(property => property.Name));
    }

    [Fact]
    public void Model_OnlyAllowsOnePendingOffboardingPerEmployee()
    {
        using var db = Context();
        var index = db.Model.FindEntityType(typeof(OffboardingRecord))!.GetIndexes()
            .Single(item => item.GetDatabaseName() == "UX_offboarding_employee_in_progress");

        Assert.True(index.IsUnique);
        Assert.Equal("EmployeeId", Assert.Single(index.Properties).Name);
        Assert.Equal("status = 'IN_PROGRESS'", index.GetFilter());
    }

    private static HrDbContext Context() => new(new DbContextOptionsBuilder<HrDbContext>()
        .UseNpgsql("Host=localhost;Database=not_used")
        .AddInterceptors(new SuppressDatabaseSave()).Options);

    // Exercises the real change tracker/version hook without contacting a database.
    private sealed class SuppressDatabaseSave : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default) =>
            ValueTask.FromResult(InterceptionResult<int>.SuppressWithResult(1));
    }
}
