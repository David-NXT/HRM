using Attendance.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Data;

public class AttendanceDbContext : DbContext
{
    public AttendanceDbContext(
        DbContextOptions<AttendanceDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.ConfigureWarnings(w =>
            w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }

    public DbSet<AttendanceRecord> AttendanceRecords
        => Set<AttendanceRecord>();

    public DbSet<WorkShift> WorkShifts
        => Set<WorkShift>();

    public DbSet<ShiftAssignment> ShiftAssignments
        => Set<ShiftAssignment>();

    public DbSet<AttendanceExplanation> AttendanceExplanations
        => Set<AttendanceExplanation>();

    public DbSet<AttendanceExplanationAttachment> AttendanceExplanationAttachments
        => Set<AttendanceExplanationAttachment>();

    public DbSet<OTRequest> OTRequests
        => Set<OTRequest>();

    public DbSet<LeaveRequest> LeaveRequests
        => Set<LeaveRequest>();

    public DbSet<LeaveBalance> LeaveBalances
        => Set<LeaveBalance>();

    public DbSet<OvertimeRule> OvertimeRules
        => Set<OvertimeRule>();

    public DbSet<AttendanceRule> AttendanceRules
        => Set<AttendanceRule>();

    public DbSet<AttendanceConfigAuditLog> AttendanceConfigAuditLogs
        => Set<AttendanceConfigAuditLog>();

    public DbSet<TimesheetPeriod> TimesheetPeriods
        => Set<TimesheetPeriod>();

    public DbSet<TimesheetSummary> TimesheetSummaries
        => Set<TimesheetSummary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AttendanceRecord>()
            .HasOne(x => x.WorkShift)
            .WithMany()
            .HasForeignKey(x => x.ShiftId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ShiftAssignment>()
            .HasOne(x => x.WorkShift)
            .WithMany()
            .HasForeignKey(x => x.ShiftId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AttendanceExplanation>()
            .HasOne(x => x.Attendance)
            .WithMany(x => x.AttendanceExplanations)
            .HasForeignKey(x => x.AttendanceId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AttendanceExplanationAttachment>()
            .HasOne(x => x.Explanation)
            .WithMany(x => x.Attachments)
            .HasForeignKey(x => x.ExplanationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TimesheetSummary>()
            .HasOne(x => x.Period)
            .WithMany(x => x.Summaries)
            .HasForeignKey(x => x.PeriodId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LeaveBalance>(b =>
        {
            b.Property(x => x.TotalDays).HasPrecision(5, 2);
            b.Property(x => x.UsedDays).HasPrecision(5, 2);
            b.Property(x => x.RemainingDays).HasPrecision(5, 2);
            b.Property(x => x.CarriedOverDays).HasPrecision(5, 2);
        });

        modelBuilder.Entity<OvertimeRule>(b =>
        {
            b.Property(x => x.Multiplier).HasPrecision(4, 2);
            b.Property(x => x.MaxHoursPerDay).HasPrecision(4, 2);
            b.Property(x => x.MaxHoursPerMonth).HasPrecision(5, 2);
            b.Property(x => x.MaxHoursPerYear).HasPrecision(6, 2);
        });

        modelBuilder.Entity<OTRequest>(b =>
        {
            b.Property(x => x.Multiplier).HasPrecision(4, 2);
            b.Property(x => x.PlannedHours).HasPrecision(5, 2);
            b.Property(x => x.ActualHours).HasPrecision(5, 2);
        });

        modelBuilder.Entity<LeaveRequest>(b =>
        {
            b.Property(x => x.TotalDays).HasPrecision(5, 2);
        });

        modelBuilder.Entity<TimesheetSummary>(b =>
        {
            b.Property(x => x.StandardWorkingDays).HasPrecision(5, 2);
            b.Property(x => x.ActualWorkingDays).HasPrecision(5, 2);
            b.Property(x => x.TotalWorkingHours).HasPrecision(6, 2);
            b.Property(x => x.PaidLeaveDays).HasPrecision(5, 2);
            b.Property(x => x.UnpaidLeaveDays).HasPrecision(5, 2);
            b.Property(x => x.OTHours).HasPrecision(6, 2);
        });
    }
}
