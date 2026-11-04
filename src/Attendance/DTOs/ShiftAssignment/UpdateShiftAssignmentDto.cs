namespace Attendance.DTOs.ShiftAssignment;

public class UpdateShiftAssignmentDto
{   

    public Guid? EmployeeId {get; set;}

    public Guid? ShiftId { get; set; }

    public DateOnly? Date {get; set;}

    public string? Status {get; set;}

    public DateTime? AssignAt {get; set;}

    public string? Note {get; set;}

    public string? ApprovedBy {get; set;}

    public DateTime? UpdatedAt {get; set;}

}