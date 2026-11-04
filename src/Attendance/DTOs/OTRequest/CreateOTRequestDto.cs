using System.ComponentModel.DataAnnotations;
namespace Attendance.DTOs.OTRequest;

public class CreateOTRequestDto
{
    [Required]
    public Guid EmployeeId { get; set; }
    [Required]
    public DateOnly Date { get; set; }
    [Required]  
    public TimeOnly StartTime { get; set; }
    [Required] 
    public TimeOnly EndTime { get; set; }

    public string? Reason { get; set; }

    public string Status { get; set; } = "Pending";

    public string? ApprovedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
