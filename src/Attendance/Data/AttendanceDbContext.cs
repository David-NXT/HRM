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

    public DbSet<AttendanceRecord> AttendanceRecords
        => Set<AttendanceRecord>();

    public DbSet<Holiday> Holidays
        => Set<Holiday>();

    public DbSet<WorkShift> WorkShifts
        => Set<WorkShift>();

    public DbSet<ShiftAssignment> ShiftAssignments
        => Set<ShiftAssignment>();

    public DbSet<AttendanceExplanation> AttendanceExplanations
        => Set<AttendanceExplanation>();

    public DbSet<OTRequest> OTRequests
        => Set<OTRequest>();

    public DbSet<LeaveRequest> LeaveRequests
        => Set<LeaveRequest>();

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
            .OnDelete(DeleteBehavior.Restrict);
    }
}
