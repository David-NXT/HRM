namespace Payroll.Application.DTOs.PayrollManagement;

public class CreatePayrollsRequest
{
    public List<Guid> EmployeeIDs { get; set; } = new();
}
