using System.ComponentModel.DataAnnotations;

namespace Attendance.DTOs.Holiday;

public class CreateHolidayDto
{
    [Required]
    public DateOnly Date { get; set; }

    public DateOnly? EndDate { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public string HolidayType { get; set; } = string.Empty;

    public bool IsPaid { get; set; } = true;
}
