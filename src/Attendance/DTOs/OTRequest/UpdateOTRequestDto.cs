namespace Attendance.DTOs.OTRequest;

public class UpdateOTRequestDto
{
    public Guid? EmployeeId { get; set; }

    public DateOnly? Date { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string? Reason { get; set; }

    public string? Status { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
