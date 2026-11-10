namespace Attendance.Models;

public class Holiday
{
    public Guid Id { get; set; }

    /// <summary>
    /// Ngày bắt đầu nghỉ lễ
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Ngày kết thúc nghỉ lễ (đối với kỳ nghỉ lễ nhiều ngày)
    /// </summary>
    public DateOnly? EndDate { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string HolidayType { get; set; } = string.Empty;

    public bool IsPaid { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
