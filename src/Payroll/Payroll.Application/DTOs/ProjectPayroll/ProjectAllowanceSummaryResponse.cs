namespace Payroll.Application.DTOs.ProjectPayroll;

public class ProjectAllowanceSummaryResponse
{
    public Guid EmployeeId { get; set; }
    public decimal TotalProjectIncome { get; set; }
}
