namespace Payroll.Application.DTOs.PayrollQuery;

public class PayrollDetailResponse
{
    public Guid PayrollID { get; set; }
    public Guid EmployeeID { get; set; }
    public Guid PayrollPeriodID { get; set; }

    public decimal BaseSalary { get; set; }
    public decimal WorkingDays { get; set; }
    public decimal StandardWorkingDay { get; set; }

    public decimal TotalAllowance { get; set; }
    public decimal TotalPayCut { get; set; }

    public decimal GrossSalary { get; set; }
    public decimal NetSalary { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime? CalculatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? PaidAt { get; set; }

    public List<PayrollAllowanceResponse> Allowances { get; set; } = new();
    public List<PayrollPayCutResponse> PayCuts { get; set; } = new();
}

public class PayrollAllowanceResponse
{
    public Guid AllowanceDetailID { get; set; }
    public Guid AllowanceTypeID { get; set; }
    public string AllowanceTypeName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public decimal RateApplied { get; set; }
    public decimal Amount { get; set; }
}

public class PayrollPayCutResponse
{
    public Guid PayCutDetailID { get; set; }
    public Guid PayCutReasonID { get; set; }
    public string PayCutReasonName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public decimal RateApplied { get; set; }
    public decimal Amount { get; set; }
}