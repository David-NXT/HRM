namespace Admin.WinForms.DTOs.Attendance;

public class TimesheetSummaryDto
{
    public Guid Id { get; set; }

    public Guid PeriodId { get; set; }

    public Guid EmployeeId { get; set; }

    public string? EmployeeCode { get; set; }

    public string? EmployeeName { get; set; }

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

    public decimal NormalOTHours { get; set; }

    public decimal WeekendOTHours { get; set; }

    public decimal HolidayOTHours { get; set; }

    public DateTime CreatedAt { get; set; }
}
