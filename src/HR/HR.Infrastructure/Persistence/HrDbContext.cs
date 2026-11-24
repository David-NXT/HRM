using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence;

public class HrDbContext : DbContext
{
    public HrDbContext(DbContextOptions<HrDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Position> Positions => Set<Position>();
    public DbSet<EmploymentContract> EmploymentContracts => Set<EmploymentContract>();
    public DbSet<WorkHistory> WorkHistories => Set<WorkHistory>();
    public DbSet<ProfileUpdateRequest> ProfileUpdateRequests => Set<ProfileUpdateRequest>();
    public DbSet<ProfileUpdateAttachment> ProfileUpdateAttachments => Set<ProfileUpdateAttachment>();
    public DbSet<EmployeeNotification> EmployeeNotifications => Set<EmployeeNotification>();
    public DbSet<OffboardingRecord> OffboardingRecords => Set<OffboardingRecord>();
    public DbSet<OffboardingChecklistItem> OffboardingChecklistItems => Set<OffboardingChecklistItem>();
    public DbSet<ContractAlert> ContractAlerts => Set<ContractAlert>();

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        PrepareConcurrencyVersions();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        PrepareConcurrencyVersions();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    // Dùng version nhân viên chung để bảo vệ cả ghi hồ sơ, hợp đồng và xác nhận nghỉ việc đồng thời.
    private void PrepareConcurrencyVersions()
    {
        ChangeTracker.DetectChanges();
        var contractEmployeeIds = ChangeTracker.Entries<EmploymentContract>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified)
            .Select(entry => entry.Entity.EmployeeId).ToHashSet();
        foreach (var entry in ChangeTracker.Entries<Employee>()
            .Where(entry => entry.State != EntityState.Added && contractEmployeeIds.Contains(entry.Entity.EmployeeId)))
        {
            entry.Property(employee => employee.UpdatedAt).IsModified = true;
        }
        var changedChecklistRecords = ChangeTracker.Entries<OffboardingChecklistItem>()
            .Where(entry => entry.State == EntityState.Modified)
            .Select(entry => entry.Entity.OffboardingRecordId).ToHashSet();
        foreach (var entry in ChangeTracker.Entries<OffboardingRecord>()
            .Where(entry => entry.State != EntityState.Added && changedChecklistRecords.Contains(entry.Entity.OffboardingRecordId)))
        {
            entry.Property(record => record.UpdatedAt).IsModified = true;
        }
        foreach (var entry in ChangeTracker.Entries<Employee>().Where(entry => entry.State == EntityState.Modified))
        {
            entry.Entity.Version = checked(entry.Property(employee => employee.Version).OriginalValue + 1);
        }
        foreach (var entry in ChangeTracker.Entries<OffboardingRecord>().Where(entry => entry.State == EntityState.Modified))
        {
            entry.Entity.Version = checked(entry.Property(record => record.Version).OriginalValue + 1);
        }
        foreach (var entry in ChangeTracker.Entries<ContractAlert>().Where(entry => entry.State == EntityState.Modified))
        {
            entry.Entity.Version = checked(entry.Property(alert => alert.Version).OriginalValue + 1);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HrDbContext).Assembly);
    }
}
