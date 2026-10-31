namespace Payroll.Core.Entities;

public class PayCutReason
{
    public int PayCutReasonID {get; set;}
    public string Name {get; set;} = "";
    public string CalculationType {get; set;} = "percentage"; // percentage, fixed, formula
    public decimal DefaultRate {get; set;}
    public string Status {get; set;} = "";

    //Quan hệ
    public List<PayCutDetail> PayCutDetails {get; set;} = new();
}