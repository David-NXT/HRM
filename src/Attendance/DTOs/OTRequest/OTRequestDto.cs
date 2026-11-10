namespace Attendance.DTOs.OTRequest;

public class OTRequestDto
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public decimal PlannedHours { get; set; }

    public decimal? ActualHours { get; set; }

    public string? DayType { get; set; }

    public decimal Multiplier { get; set; }

    public bool IsUnplanned { get; set; }

    public string? Reason { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public string? RejectionReason { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
