namespace Payroll.Application.DTOs.PayrollManagement;

public class CreatePayrollsResponse
{
    public int PayrollPeriodID { get; set; }

    public int CreatedCount { get; set; }

    public List<int> CreatedEmployeeIDs { get; set; } = new();

    public List<int> SkippedEmployeeIDs { get; set; } = new();
}