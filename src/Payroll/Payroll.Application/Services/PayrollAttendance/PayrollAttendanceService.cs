using Microsoft.EntityFrameworkCore;
using Payroll.Application.Interfaces;
using Payroll.Application.DTOs.PayrollAttendance;
using PayrollEntity = global::Payroll.Domain.Entities.Payroll;
using PayrollPeriodEntity = global::Payroll.Domain.Entities.PayrollPeriod;

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
        PayrollPeriodEntity? period = await _context.PayrollPeriods
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
        List<PayrollEntity> payrolls = await _context.Payrolls
            .Where(p => p.PayrollPeriodID == payrollPeriodID)
            .ToListAsync(cancellationToken);

        if (payrolls.Count == 0)
        {
            throw new InvalidOperationException(
                "No payroll records found for this period.");
        }

        // 3. Validate payroll statuses.
        foreach (PayrollEntity payroll in payrolls)
        {
            if (payroll.Status != "draft")
            {
                throw new InvalidOperationException(
                    $"Payroll {payroll.PayrollID} is not in draft status.");
            }
        }

        // 4. Retrieve overtime multiplier once per payroll period.
        decimal overtimeMultiplier =
            await _attendanceClient.GetOvertimeMultiplierAsync(
                cancellationToken);

        if (overtimeMultiplier <= 0)
        {
            throw new InvalidOperationException(
                "Invalid overtime multiplier.");
        }

        // 5. Retrieve and validate attendance summaries.
        List<(Guid EmployeeID,
            decimal WorkingDays,
            decimal StandardWorkingDays,
            decimal TotalWorkingHours,
            decimal OTHours)> attendanceByEmployee =
            new List<(Guid EmployeeID,
                decimal WorkingDays,
                decimal StandardWorkingDays,
                decimal TotalWorkingHours,
                decimal OTHours)>();

        foreach (PayrollEntity payroll in payrolls)
        {
            cancellationToken.ThrowIfCancellationRequested();

            AttendanceEmployeeSummaryResponse? attendance =
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
                attendance.StandardWorkingDays <= 0 ||
                attendance.TotalWorkingHours < 0 ||
                attendance.OTHours < 0)
            {
                throw new InvalidOperationException(
                    $"Invalid attendance data for employee {payroll.EmployeeID}.");
            }

            attendanceByEmployee.Add(
                (
                    payroll.EmployeeID,
                    attendance.ActualWorkingDays,
                    attendance.StandardWorkingDays,
                    attendance.TotalWorkingHours,
                    attendance.OTHours
                ));
        }

        // 6. Update payroll records after all summaries are valid.
        foreach (PayrollEntity payroll in payrolls)
        {
            (Guid EmployeeID, decimal WorkingDays, decimal StandardWorkingDays, decimal TotalWorkingHours, decimal OTHours) attendance =
                attendanceByEmployee.First(item => item.EmployeeID == payroll.EmployeeID);

            payroll.WorkingDays = attendance.WorkingDays;
            payroll.StandardWorkingDay = attendance.StandardWorkingDays;
            payroll.TotalWorkingHours = attendance.TotalWorkingHours;

            payroll.OTHours = attendance.OTHours;
            payroll.OTMultiplier = overtimeMultiplier;
        }

        // 7. Save changes once.
        await _context.SaveChangesAsync(cancellationToken);

        return payrolls.Count;
    }
}