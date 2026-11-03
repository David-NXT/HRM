
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
}
