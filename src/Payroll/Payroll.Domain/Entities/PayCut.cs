namespace Payroll.Domain.Entities;

public class PayCut
{
    public int PayCutID {get; set;}
    public int PayrollID {get; set;}
    public string Status {get; set;} = "active";
    
    //quan hệ
    public Payroll? Payroll {get; set;}
    public List<PayCutDetail> Details {get; set;} = new();

}