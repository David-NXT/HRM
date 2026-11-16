namespace Payroll.Application.DTOs.Payslip;

public class PayslipResponse
{
    // Payslip information
    public Guid PayrollID { get; set; }
    public Guid EmployeeID { get; set; }

    // Payroll period
    public int Month { get; set; }
    public int Year { get; set; }

    // Salary information
    public decimal BaseSalary { get; set; }
    public decimal WorkingDays { get; set; }
    public decimal StandardWorkingDay { get; set; }

    // Salary summary
    public decimal TotalAllowance { get; set; }
    public decimal TotalProjectAllowance { get; set; }
    public decimal TotalPayCut { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal NetSalary { get; set; }

    // Status
    public string Status { get; set; } = string.Empty;
    public DateTime ApprovedAt { get; set; }
    public DateTime? PaidAt { get; set; }

    // Details
    public List<PayslipAllowanceResponse> Allowances { get; set; } = new();
    public List<PayslipPayCutResponse> PayCuts { get; set; } = new();
}

public class PayslipAllowanceResponse
{
    public string AllowanceTypeName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class PayslipPayCutResponse
{
    public string PayCutReasonName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}