using System.ComponentModel.DataAnnotations;

namespace Attendance.DTOs.ShiftAssignment;
public class CreateShiftAssignmentDto
{   
    [Required]
    public Guid EmployeeId {get; set;}
    [Required]
    public Guid ShiftId { get; set; }
    [Required]
    public DateOnly Date {get; set;}

    public string Status {get; set;} = "Pending";
    [Required]
    public DateTime AssignAt {get; set;} = DateTime.UtcNow;
    
    public string? Note {get; set;}

    public string? ApprovedBy {get; set;}

    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public DateTime? UpdatedAt {get; set;}

}