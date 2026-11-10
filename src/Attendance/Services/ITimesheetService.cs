using Attendance.DTOs.TimesheetPeriod;
using Attendance.DTOs.TimesheetSummary;

namespace Attendance.Services;

public interface ITimesheetService
{
    Task<List<TimesheetPeriodDto>> GetPeriodsAsync(int? year = null, CancellationToken cancellationToken = default);
    Task<TimesheetPeriodDto?> GetPeriodByIdAsync(Guid periodId, CancellationToken cancellationToken = default);
    Task<TimesheetPeriodDto> CreatePeriodAsync(CreateTimesheetPeriodDto dto, CancellationToken cancellationToken = default);
    Task<TimesheetPeriodDto?> LockPeriodAsync(Guid periodId, string lockedBy, CancellationToken cancellationToken = default);

    Task<List<TimesheetSummaryDto>> GetSummariesByPeriodAsync(Guid periodId, CancellationToken cancellationToken = default);
    Task<EmployeeMonthlyTimesheetDto?> GetEmployeeSummaryAsync(Guid employeeId, int month, int year, CancellationToken cancellationToken = default);
    Task<EmployeeMonthlyTimesheetDto?> GetEmployeeSummaryByPeriodAsync(Guid periodId, Guid employeeId, CancellationToken cancellationToken = default);
    Task<TimesheetSummaryDto> GenerateOrRecalculateSummaryAsync(Guid periodId, Guid employeeId, CancellationToken cancellationToken = default);
}
