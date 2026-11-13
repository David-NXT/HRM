namespace Payroll.Domain.Entities;

public class Payroll
{
    public Guid PayrollID { get; set; } = Guid.NewGuid();
    public Guid PayrollPeriodID { get; set; }
    public Guid EmployeeID { get; set; }
    public string? EmploymentType { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal? HourlyRate { get; set; }
    public decimal WorkingDays { get; set; }
    public decimal TotalWorkingHours { get; set; }
    public decimal? OTHours { get; set; }
    public decimal? OTMultiplier { get; set; }
    public decimal OTPay { get; set; }
    public decimal StandardWorkingDay { get; set; } = 22m;
    public decimal TotalAllowance { get; set; }
    public decimal TotalPayCut { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal NetSalary { get; set; }
    public string Status { get; set; } = "draft"; // draft, calculated, approved, paid
    public DateTime? CalculatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? PaidAt { get; set; }

    // Relationships
    public PayrollPeriod? PayrollPeriod { get; set; }
    public List<PayCut> PayCuts { get; set; } = new();
    public List<Allowance> Allowances { get; set; } = new();
}
