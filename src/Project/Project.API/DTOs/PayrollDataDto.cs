namespace Project.API.DTOs;

public class PayrollDataDto
{
    public Guid TimesheetId { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid TaskId { get; set; }

    public DateTime WorkDate { get; set; }

    public decimal HoursWorked { get; set; }

    public decimal HourlyRate { get; set; }

    public decimal TotalAmount => HoursWorked * HourlyRate;
}