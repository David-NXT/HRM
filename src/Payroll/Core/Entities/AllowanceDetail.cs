namespace Payroll.Core.Entities;

public class AllowanceDetail
{
    public int AllowanceDetailID {get; set;}
    public int AllowanceID {get; set;}
    public int AllowanceTypeID {get; set;}
    public string Description {get; set;} = "";
    public decimal RateApplied {get; set;} 
    public decimal Amount {get; set;}

    //Quan hệ
    public Allowance? Allowance {get; set;}
    public AllowanceType? AllowanceType {get; set;}
}