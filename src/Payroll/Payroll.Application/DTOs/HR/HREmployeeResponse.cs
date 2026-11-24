namespace Payroll.Application.DTOs.HR;

public sealed class HREmployeeResponse
{
    public Guid EmployeeId { get; set; }

    public string EmploymentType { get; set; } = string.Empty;

    public decimal? BaseSalary { get; set; }

    public decimal? PartTimeSalary { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateOnly? EmploymentStartDate { get; set; }
}