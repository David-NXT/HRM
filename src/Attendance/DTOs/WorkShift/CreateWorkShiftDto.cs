using System.ComponentModel.DataAnnotations;

namespace Attendance.DTOs.WorkShift;

public class CreateWorkShiftDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 1440)]
    public int BreakMins { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }

    [Required]
    public TimeOnly EndTime { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateOnly? EffectiveDate { get; set; }
}
