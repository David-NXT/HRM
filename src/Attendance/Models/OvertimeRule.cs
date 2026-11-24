namespace Attendance.Models;

public class OvertimeRule
{
    public Guid Id { get; set; }

    /// <summary>
    /// Loại ngày: Ngày thường (Weekday), Ngày nghỉ (Weekend), Ngày lễ (Holiday)
    /// </summary>
    public string DayType { get; set; } = string.Empty;

    /// <summary>
    /// Hệ số làm thêm giờ: 1.5 (150%), 2.0 (200%), 3.0 (300%)
    /// </summary>
    public decimal Multiplier { get; set; } = 1.5m;

    /// <summary>
    /// Số giờ OT tối đa được phép trong ngày (VD: 4.0h)
    /// </summary>
    public decimal MaxHoursPerDay { get; set; } = 4.0m;

    /// <summary>
    /// Số giờ OT tối đa được phép trong tháng (VD: 40.0h)
    /// </summary>
    public decimal MaxHoursPerMonth { get; set; } = 40.0m;

    /// <summary>
    /// Số giờ OT tối đa được phép trong năm (VD: 200.0h - 300.0h)
    /// </summary>
    public decimal MaxHoursPerYear { get; set; } = 200.0m;

    public DateOnly EffectiveDate { get; set; }

    public bool IsActive { get; set; } = true;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public decimal CalculateOTPay(decimal hours, decimal baseHourlyRate) => hours * baseHourlyRate * Multiplier;
    public bool ValidateOTLimit(decimal hours) => hours >= 0 && (MaxHoursPerDay <= 0 || hours <= MaxHoursPerDay);
}
