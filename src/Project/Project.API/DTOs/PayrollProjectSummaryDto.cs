
namespace Project.API.DTOs;

public class PayrollProjectSummaryDto
{
    public Guid EmployeeId { get; set; }

    public Guid ProjectId { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public int TimesheetCount { get; set; }

    public decimal TotalApprovedHours { get; set; }


    public decimal TotalProjectIncome { get; set; }
}
