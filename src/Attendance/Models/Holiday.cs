namespace Attendance.Models;
public class Holiday
{
    public Guid Id {get; set;}
    
    public DateOnly Date {get; set;}
    
    public string Name {get; set;} = string.Empty;
    
    public string? Description {get; set;}
    
    public string HolidayType {get; set;} = string.Empty;
    
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    
    public bool IsPaid {get; set;}
    
}