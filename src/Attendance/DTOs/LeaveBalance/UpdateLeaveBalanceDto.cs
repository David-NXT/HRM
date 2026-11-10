namespace Attendance.DTOs.LeaveBalance;

public class UpdateLeaveBalanceDto
{
    public decimal TotalDays { get; set; }

    public decimal UsedDays { get; set; }

    public decimal RemainingDays { get; set; }

    public decimal CarriedOverDays { get; set; }
}
