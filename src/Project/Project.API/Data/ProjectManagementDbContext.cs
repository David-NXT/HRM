
using Microsoft.EntityFrameworkCore;
using Project.API.Entities;

using ProjectEntity = Project.API.Entities.Project;
using ProjectTask = Project.API.Entities.Task;

namespace Project.API.Data;

public class ProjectManagementDbContext : DbContext
{
    public ProjectManagementDbContext(
        DbContextOptions<ProjectManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProjectEntity> Projects { get; set; }
    public DbSet<ProjectTask> Tasks { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    public DbSet<ProjectRole> ProjectRoles { get; set; }
    public DbSet<HourlyRate> HourlyRates { get; set; }
    public DbSet<Timesheet> Timesheets { get; set; }

    // Thêm OnModelCreating ở đây
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Timesheet>(entity =>
        {
            entity.HasKey(t => t.TimesheetId);

            entity.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(t => t.RejectReason)
                .HasMaxLength(500);

            entity.Property(t => t.Description)
                .HasMaxLength(1000);

            entity.Property(t => t.HoursWorked)
                .HasPrecision(5, 2);

            entity.HasOne(t => t.ProjectMember)
                .WithMany(pm => pm.Timesheets)
                .HasForeignKey(t => t.ProjectMemberId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Task)
                .WithMany(task => task.Timesheets)
                .HasForeignKey(t => t.TaskId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<HourlyRate>(entity =>
        {
            entity.HasKey(h => h.HourlyRateId);

            entity.Property(h => h.Rate)
                .HasPrecision(18, 2);

            entity.HasOne(h => h.ProjectMember)
                .WithMany(pm => pm.HourlyRates)
                .HasForeignKey(h => h.ProjectMemberId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProjectRole>(entity =>
        {
            entity.Property(r => r.RoleCoefficient)
                .HasPrecision(5, 4);
        });
    }
}
