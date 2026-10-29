namespace Payroll.Core.Entities;

public class PayCut
{
    public int PayCutID {get; set;}
    public int PayrollID {get; set;}
    public string Status {get; set;} = "draft";
    
    //quan hệ
    public Payroll? payroll {get; set;}
    public List<PayCutDetail> Details {get; set;} = new();

}