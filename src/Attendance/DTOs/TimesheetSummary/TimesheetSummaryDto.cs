namespace Attendance.DTOs.TimesheetSummary;

public class TimesheetSummaryDto
{
    public Guid Id { get; set; }

    public Guid PeriodId { get; set; }

    public Guid EmployeeId { get; set; }

    public decimal StandardWorkingDays { get; set; }

    public decimal ActualWorkingDays { get; set; }

    public decimal TotalWorkingHours { get; set; }

    public decimal PaidLeaveDays { get; set; }

    public decimal UnpaidLeaveDays { get; set; }

    public int TotalLateMinutes { get; set; }

    public int LateOccurrences { get; set; }

    public int TotalEarlyLeaveMinutes { get; set; }

    public int EarlyLeaveOccurrences { get; set; }

    public decimal NormalOTHours { get; set; }

    public decimal WeekendOTHours { get; set; }

    public decimal HolidayOTHours { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
