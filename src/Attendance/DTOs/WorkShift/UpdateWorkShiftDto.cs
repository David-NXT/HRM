using System.ComponentModel.DataAnnotations;

namespace Attendance.DTOs.WorkShift;

public class UpdateWorkShiftDto
{
    [StringLength(100)]
    public string? Name { get; set; }

    [Range(0, 1440)]
    public int? BreakMins { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public DateOnly? EffectiveDate { get; set; }
}
