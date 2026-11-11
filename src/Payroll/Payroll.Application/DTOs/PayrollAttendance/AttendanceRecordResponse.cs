namespace Payroll.Application.DTOs.PayrollAttendance;

public class AttendanceRecordResponse
{
    public Guid EmployeeID { get; set; }

    public decimal TotalWorkingDays { get; set; }

    public decimal StandardWorkingDays { get; set; }
}
