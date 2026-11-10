namespace Payroll.Application.DTOs.PayrollManagement;

public class CreatePayrollsRequest
{
    public List<int> EmployeeIDs { get; set; } = new();
}