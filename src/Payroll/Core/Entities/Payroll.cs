namespace Payroll.Core.Entities;

public class Payroll
{
    public int PayrollID {get; set;}
    public int PayrollPeriodID {get; set;}
    public int EmployeeID {get; set;}
    public decimal BaseSalary {get; set;}
    public decimal WorkingDay {get; set;}
    public decimal StandardWorkingDay {get; set;} = 22m;
    public decimal TotalAllowance {get; set;}
    public decimal ToTalPayCut {get; set;}
    public decimal GrossSalary {get; set;}
    public decimal NetSalary  {get; set;}
    public string Status {get; set;} = "draft"; // draft, calculated, approved, paid
    public DateTime? CalculatedAt {get; set;}
    public DateTime? ApprovedAt {get; set;}
    public DateTime? PaidAt {get; set;}

    //Quan hệ
    public PayrollPeriod? PayrollPeriod {get; set;}
    public List<PayCut> PayCuts {get; set;} = new();
    public List<Allowance> Allowances {get; set;} = new();
    
}