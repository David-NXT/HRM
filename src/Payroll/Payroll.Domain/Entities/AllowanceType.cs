namespace Payroll.Domain.Entities;

public class AllowanceType
{
    public int AllowanceTypeID {get; set;}
    public string Name {get; set;} = "";
    public string Description {get; set;} = "";
    public string Status {get; set;} = "active";

    //Quan hệ
    public List<AllowanceDetail> AllowanceDetails {get; set;} = new();
}