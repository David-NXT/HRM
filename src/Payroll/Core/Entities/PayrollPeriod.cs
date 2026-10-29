namespace Payroll.Core.Entities;

public class PayrollPeriod
{
    public int payrollPeriodID {get; set;}
    public int year {get; set;}
    public int month {get; set;}
    public string status {get; set;} = "Draft"; // draft, calculated, approved, paid
    public DateTime createdAt {get; set;} = DateTime.UtcNow;

    public List<Payroll> Payrolls {get; set;} = new();
}