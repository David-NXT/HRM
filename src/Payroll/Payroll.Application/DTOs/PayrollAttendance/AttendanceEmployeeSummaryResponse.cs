namespace Payroll.Application.DTOs.PayrollAttendance;

public class AttendanceEmployeeSummaryResponse
{
    public Guid SummaryId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid PeriodId { get; set; }

    public int Month { get; set; }
    public int Year { get; set; }

    public string PeriodStatus { get; set; } = string.Empty;

    public decimal StandardWorkingDays { get; set; }
    public decimal ActualWorkingDays { get; set; }
    public decimal TotalWorkingHours { get; set; }
    public decimal OTHours { get; set; }
    public int LateOccurrences { get; set; }
    public int TotalLateMinutes { get; set; }
}