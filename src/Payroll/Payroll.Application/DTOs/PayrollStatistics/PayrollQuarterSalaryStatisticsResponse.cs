namespace Payroll.Application.DTOs.PayrollStatistics;

public class PayrollQuarterSalaryStatisticsResponse
{
    public int Year { get; set; }
    public int Quarter { get; set; }
    public int EmployeeCount { get; set; }
    public int PayrollCount { get; set; }
    public decimal TotalBaseSalary { get; set; }
    public decimal TotalAllowance { get; set; }
    public decimal TotalProjectAllowance { get; set; }
    public decimal TotalPayCut { get; set; }
    public decimal TotalGrossSalary { get; set; }
    public decimal TotalNetSalary { get; set; }
}
