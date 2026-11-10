using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance.Models;
public class ShiftAssignment
{
    public Guid Id {get; set;}

    public Guid EmployeeId {get; set;}

    [ForeignKey(nameof(WorkShift))]
    public Guid ShiftId { get; set; }

    public DateOnly Date {get; set;}

    public string Status {get; set;} = "Pending";

    public DateTime AssignAt {get; set;} = DateTime.UtcNow;

    public string? Note {get; set;}

    public string? ApprovedBy {get; set;}

    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public DateTime? UpdatedAt {get; set;}

    public WorkShift? WorkShift { get; set; }
}