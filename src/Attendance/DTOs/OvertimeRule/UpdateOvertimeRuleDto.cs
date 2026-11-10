namespace Attendance.DTOs.OvertimeRule;

public class UpdateOvertimeRuleDto
{
    public string DayType { get; set; } = string.Empty;

    public decimal Multiplier { get; set; }

    public decimal MaxHoursPerDay { get; set; }

    public decimal MaxHoursPerMonth { get; set; }

    public decimal MaxHoursPerYear { get; set; }

    public DateOnly EffectiveDate { get; set; }

    public bool IsActive { get; set; }

    public string? Description { get; set; }

    public string? UpdatedBy { get; set; }
}
