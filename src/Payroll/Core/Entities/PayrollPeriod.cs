namespace Payroll.Core.Entities;

public class PayrollPeriod
{
    public int PayrollPeriodID {get; set;}
    public int Year {get; set;}
    public int Month {get; set;}
    public string Status {get; set;} = "draft"; // draft, calculated, approved, paid
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public List<Payroll> Payrolls {get; set;} = new();
}