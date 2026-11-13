namespace Attendance.DTOs.TimesheetSummary;

public class EmployeeMonthlyTimesheetDto
{
    public Guid SummaryId { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid PeriodId { get; set; }

    public string PeriodName { get; set; } = string.Empty;

    public int Month { get; set; }

    public int Year { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string PeriodStatus { get; set; } = string.Empty;

    public decimal StandardWorkingDays { get; set; }

    public decimal ActualWorkingDays { get; set; }

    public decimal TotalWorkingHours { get; set; }

    public decimal PaidLeaveDays { get; set; }

    public decimal UnpaidLeaveDays { get; set; }

    public int TotalLateMinutes { get; set; }

    public int LateOccurrences { get; set; }

    public int TotalEarlyLeaveMinutes { get; set; }

    public int EarlyLeaveOccurrences { get; set; }

    public decimal OTHours { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
