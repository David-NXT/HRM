namespace Payroll.Application.DTOs.PayrollManagement;

public class CreatePayrollsResponse
{
    public Guid PayrollPeriodID { get; set; }

    public int CreatedCount { get; set; }

    public List<Guid> CreatedEmployeeIDs { get; set; } = new();

    public List<Guid> SkippedEmployeeIDs { get; set; } = new();
}
