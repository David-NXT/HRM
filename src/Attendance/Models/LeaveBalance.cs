namespace Attendance.Models;

public class LeaveBalance
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public int Year { get; set; }

    public decimal TotalDays { get; set; }

    public decimal UsedDays { get; set; }

    public decimal RemainingDays { get; set; }

    public decimal CarriedOverDays { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
