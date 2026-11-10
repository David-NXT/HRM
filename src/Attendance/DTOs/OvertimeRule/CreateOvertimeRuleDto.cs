namespace Attendance.DTOs.OvertimeRule;

public class CreateOvertimeRuleDto
{
    public string DayType { get; set; } = string.Empty;

    public decimal Multiplier { get; set; } = 1.5m;

    public decimal MaxHoursPerDay { get; set; } = 4.0m;

    public decimal MaxHoursPerMonth { get; set; } = 40.0m;

    public decimal MaxHoursPerYear { get; set; } = 200.0m;

    public DateOnly EffectiveDate { get; set; }

    public bool IsActive { get; set; } = true;

    public string? Description { get; set; }

    public string? CreatedBy { get; set; }
}
