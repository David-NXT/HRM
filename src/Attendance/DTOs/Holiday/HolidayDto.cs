namespace Attendance.DTOs.Holiday;

public class HolidayDto
{
    public Guid Id { get; set; }

    public DateOnly Date { get; set; }

    public DateOnly? EndDate { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string HolidayType { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsPaid { get; set; }
}
