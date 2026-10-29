using System.Data;

namespace Payroll.Core.Entities;

public class PayCutDetail
{
    public int PayCutDetailID {get; set;}
    public int PayCutID {get; set;}
    public int PayCutReasonID {get; set;}
    public string Description {get; set;} = "";
    public decimal Amount {get; set;}
    public DateTime CreatedAt {get; set;}

    //quan hệ
    public PayCut? PayCut {get; set;}
    public PayCutReason? PayCutReason {get; set;}
}