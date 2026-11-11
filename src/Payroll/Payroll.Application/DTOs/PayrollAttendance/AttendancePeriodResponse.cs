namespace Payroll.Application.DTOs.PayrollAttendance;

public class AttendancePeriodResponse
{
    public int Year { get; set; }

    public int Month { get; set; }

    public bool IsLocked { get; set; }

    public DateTime? LockedAt { get; set; }

    public List<AttendanceRecordResponse> Records { get; set; } = new();
}