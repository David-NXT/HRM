namespace Admin.WinForms.BUS.DTOs.Attendance;

public class WorkShiftDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int BreakMins { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateOnly? EffectiveDate { get; set; }
}

public class CreateWorkShiftDto
{
    public string Name { get; set; } = string.Empty;
    public int BreakMins { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateOnly? EffectiveDate { get; set; }
}

public class UpdateWorkShiftDto
{
    public string? Name { get; set; }
    public int? BreakMins { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
    public DateOnly? EffectiveDate { get; set; }
}

public class AttendanceRuleDto
{
    public Guid Id { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public int AllowedLateMinutes { get; set; }
    public int AllowedEarlyLeaveMinutes { get; set; }
    public int MaxViolationsPerMonth { get; set; }
    public DateOnly EffectiveDate { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
}

public class CreateAttendanceRuleDto
{
    public string RuleName { get; set; } = string.Empty;
    public int AllowedLateMinutes { get; set; } = 15;
    public int AllowedEarlyLeaveMinutes { get; set; } = 0;
    public int MaxViolationsPerMonth { get; set; } = 3;
    public DateOnly EffectiveDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
    public string? CreatedBy { get; set; }
}

public class UpdateAttendanceRuleDto
{
    public string? RuleName { get; set; }
    public int? AllowedLateMinutes { get; set; }
    public int? AllowedEarlyLeaveMinutes { get; set; }
    public int? MaxViolationsPerMonth { get; set; }
    public DateOnly? EffectiveDate { get; set; }
    public bool? IsActive { get; set; }
    public string? Description { get; set; }
    public string? UpdatedBy { get; set; }
}

public class OvertimeRuleDto
{
    public Guid Id { get; set; }
    public string DayType { get; set; } = string.Empty;
    public decimal Multiplier { get; set; }
    public decimal MaxHoursPerDay { get; set; }
    public decimal MaxHoursPerMonth { get; set; }
    public decimal MaxHoursPerYear { get; set; }
    public DateOnly EffectiveDate { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
}

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

public class UpdateOvertimeRuleDto
{
    public string? DayType { get; set; }
    public decimal? Multiplier { get; set; }
    public decimal? MaxHoursPerDay { get; set; }
    public decimal? MaxHoursPerMonth { get; set; }
    public decimal? MaxHoursPerYear { get; set; }
    public DateOnly? EffectiveDate { get; set; }
    public bool? IsActive { get; set; }
    public string? Description { get; set; }
    public string? UpdatedBy { get; set; }
}
