using Microsoft.EntityFrameworkCore;
using Payroll.Application.Interfaces;

namespace Payroll.Application.Services.PayrollAttendance;

public class PayrollAttendanceService
{
    private readonly IApplicationDbContext _context;
    private readonly IAttendanceServiceClient _attendanceClient;

    public PayrollAttendanceService(
        IApplicationDbContext context,
        IAttendanceServiceClient attendanceClient)
    {
        _context = context;
        _attendanceClient = attendanceClient;
    }

    public async Task<int> SyncAttendanceAsync(
        Guid payrollPeriodID,
        CancellationToken cancellationToken = default)
    {
        // 1. Find payroll period.
        var period = await _context.PayrollPeriods
            .FirstOrDefaultAsync(
                p => p.PayrollPeriodID == payrollPeriodID,
                cancellationToken);

        if (period == null)
        {
            throw new InvalidOperationException(
                "Payroll period not found.");
        }

        if (period.Status != "draft")
        {
            throw new InvalidOperationException(
                "Cannot sync attendance for a non-draft payroll period.");
        }

        // 2. Load payroll records.
        var payrolls = await _context.Payrolls
            .Where(p => p.PayrollPeriodID == payrollPeriodID)
            .ToListAsync(cancellationToken);

        if (payrolls.Count == 0)
        {
            throw new InvalidOperationException(
                "No payroll records found for this period.");
        }

        // 3. Validate payroll statuses before calling Attendance API.
        foreach (var payroll in payrolls)
        {
            if (payroll.Status != "draft")
            {
                throw new InvalidOperationException(
                    $"Payroll {payroll.PayrollID} is not in draft status.");
            }
        }

        // 4. Retrieve and validate attendance summaries.
        var attendanceByEmployee =
            new Dictionary<Guid, (decimal WorkingDays, decimal StandardWorkingDays)>();

        foreach (var payroll in payrolls)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var attendance =
                await _attendanceClient.GetEmployeeMonthlySummaryAsync(
                    payroll.EmployeeID,
                    period.Month,
                    period.Year,
                    cancellationToken);

            if (attendance == null)
            {
                throw new InvalidOperationException(
                    $"Attendance summary not found for employee {payroll.EmployeeID}.");
            }

            if (attendance.EmployeeId != payroll.EmployeeID)
            {
                throw new InvalidOperationException(
                    $"Attendance employee ID does not match payroll employee {payroll.EmployeeID}.");
            }

            if (attendance.Month != period.Month ||
                attendance.Year != period.Year)
            {
                throw new InvalidOperationException(
                    $"Attendance period does not match payroll period for employee {payroll.EmployeeID}.");
            }

            if (!string.Equals(
                    attendance.PeriodStatus,
                    "LOCKED",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Attendance period is not locked for employee {payroll.EmployeeID}.");
            }

            if (attendance.ActualWorkingDays < 0 ||
                attendance.StandardWorkingDays <= 0)
            {
                throw new InvalidOperationException(
                    $"Invalid attendance data for employee {payroll.EmployeeID}.");
            }

            attendanceByEmployee.Add(
                payroll.EmployeeID,
                (
                    attendance.ActualWorkingDays,
                    attendance.StandardWorkingDays
                ));
        }

        // 5. Update payroll records only after all summaries are valid.
        foreach (var payroll in payrolls)
        {
            var attendance = attendanceByEmployee[payroll.EmployeeID];

            payroll.WorkingDays = attendance.WorkingDays;
            payroll.StandardWorkingDay = attendance.StandardWorkingDays;
        }

        // 6. Save changes.
        await _context.SaveChangesAsync(cancellationToken);

        return payrolls.Count;
    }
}