namespace Attendance.DTOs.Holiday;

public class UpdateHolidayDto
{
    public DateOnly? Date { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? HolidayType { get; set; }

    public bool? IsPaid { get; set; }
}
