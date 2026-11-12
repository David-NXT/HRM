using Payroll.Application.DTOs.PayrollAttendance;

namespace Payroll.Application.Interfaces;

public interface IAttendanceServiceClient
{
    Task<AttendanceEmployeeSummaryResponse?> GetEmployeeMonthlySummaryAsync(
        Guid employeeID,
        int month,
        int year,
        CancellationToken cancellationToken = default);
}