namespace Payroll.Application.DTOs.PayrollApproval;

public class ApprovePayrollResponse
{
    public Guid PayrollID { get; set; }
    public Guid EmployeeID { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime ApprovedAt { get; set; }
}