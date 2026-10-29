namespace Payroll.Core.Entities;

public class Allowance
{
    public int AllowanceID {get; set;}
    public int PayrollID {get; set;}
    public string Status {get; set;} = "draft";

    //Quan hệ
    public Payroll? Payroll {get; set;}
    public List<AllowanceDetail> AllowanceDetails {get; set;} = new();
}