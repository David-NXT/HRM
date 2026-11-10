namespace Attendance.DTOs.LeaveBalance;

public class CreateLeaveBalanceDto
{
    public Guid EmployeeId { get; set; }

    public int Year { get; set; }

    public decimal TotalDays { get; set; } = 12.0m;

    public decimal UsedDays { get; set; } = 0.0m;

    public decimal RemainingDays { get; set; } = 12.0m;

    public decimal CarriedOverDays { get; set; } = 0.0m;
}
