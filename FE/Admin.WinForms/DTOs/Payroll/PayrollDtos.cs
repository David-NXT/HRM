using System;
using System.Collections.Generic;

namespace Admin.WinForms.DTOs.Payroll;

public class PayrollPeriodDto
{
    public Guid PayrollPeriodID { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public string DisplayName => $"Kỳ lương Tháng {Month:D2}/{Year} ({Status})";
}

public class CreatePayrollPeriodDto
{
    public int Year { get; set; }
    public int Month { get; set; }
}

public class UpdatePayrollPeriodStatusDto
{
    public Guid PayrollPeriodID { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string Status { get; set; } = string.Empty;
    public int PayrollCount { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class PayrollListDto
{
    public Guid PayrollID { get; set; }
    public Guid EmployeeID { get; set; }
    public string? EmployeeCode { get; set; }
    public string? EmployeeName { get; set; }
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
}

public class PayrollDetailDto
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

    public List<PayrollAllowanceDto> Allowances { get; set; } = new();
    public List<PayrollPayCutDto> PayCuts { get; set; } = new();
}

public class PayrollAllowanceDto
{
    public Guid AllowanceDetailID { get; set; }
    public Guid AllowanceTypeID { get; set; }
    public string AllowanceTypeName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal RateApplied { get; set; }
    public decimal Amount { get; set; }
}

public class PayrollPayCutDto
{
    public Guid PayCutDetailID { get; set; }
    public Guid PayCutReasonID { get; set; }
    public string PayCutReasonName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal RateApplied { get; set; }
    public decimal Amount { get; set; }
}

public class CreateAllowanceDto
{
    public Guid AllowanceTypeID { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal RateApplied { get; set; }
    public decimal Amount { get; set; }
}

public class CreatePayCutDto
{
    public Guid PayCutReasonID { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal RateApplied { get; set; }
    public decimal Amount { get; set; }
}

public class CreatePayrollsDto
{
    public List<Guid> EmployeeIDs { get; set; } = new();
}

public class PayslipDto
{
    public Guid PayrollID { get; set; }
    public Guid EmployeeID { get; set; }
    public string? EmployeeCode { get; set; }
    public string? EmployeeName { get; set; }

    public int Month { get; set; }
    public int Year { get; set; }

    public decimal BaseSalary { get; set; }
    public decimal WorkingDays { get; set; }
    public decimal StandardWorkingDay { get; set; }

    public decimal TotalAllowance { get; set; }
    public decimal TotalPayCut { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal NetSalary { get; set; }

    public string Status { get; set; } = string.Empty;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? PaidAt { get; set; }

    public List<PayslipAllowanceDto> Allowances { get; set; } = new();
    public List<PayslipPayCutDto> PayCuts { get; set; } = new();
}

public class PayslipAllowanceDto
{
    public string AllowanceTypeName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class PayslipPayCutDto
{
    public string PayCutReasonName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class AllowanceTypeItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal DefaultAmount { get; set; }
}

public class PayCutReasonItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal DefaultAmount { get; set; }
}
