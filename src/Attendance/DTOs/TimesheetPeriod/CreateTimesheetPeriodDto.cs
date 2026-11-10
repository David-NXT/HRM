namespace Attendance.DTOs.TimesheetPeriod;

public class CreateTimesheetPeriodDto
{
    public string Name { get; set; } = string.Empty;

    public int Month { get; set; }

    public int Year { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }
}
