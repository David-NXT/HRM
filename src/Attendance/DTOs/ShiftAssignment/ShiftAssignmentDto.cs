namespace Attendance.DTOs.ShiftAssignment;
public class ShiftAssignmentDto
{   
    public Guid Id {get; set;}

    public Guid EmployeeId {get; set;}

    public Guid ShiftId { get; set; }

    public DateOnly Date {get; set;}

    public string Status {get; set;} = string.Empty;
    
    public DateTime AssignAt {get; set;} = DateTime.UtcNow;
    
    public string? Note {get; set;}

    public string? ApprovedBy {get; set;}

    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public DateTime? UpdatedAt {get; set;}

}