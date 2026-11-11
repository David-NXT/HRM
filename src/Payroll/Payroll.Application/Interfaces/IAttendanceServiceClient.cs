using Payroll.Application.DTOs.PayrollAttendance;

namespace Payroll.Application.Interfaces;

public interface IAttendanceServiceClient
{
    Task<AttendancePeriodResponse?> GetLockedAttendanceAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default);
}