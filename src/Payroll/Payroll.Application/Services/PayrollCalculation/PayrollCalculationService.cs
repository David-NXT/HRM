using Microsoft.EntityFrameworkCore;
using Payroll.Application.DTOs.HR;
using Payroll.Application.DTOs.PayrollCalculation;
using Payroll.Application.Interfaces;
using Payroll.Domain.Entities;
using PayrollEntity = global::Payroll.Domain.Entities.Payroll;
using PayrollPeriodEntity = global::Payroll.Domain.Entities.PayrollPeriod;

namespace Payroll.Application.Services.PayrollCalculation;

public class PayrollCalculationService
{
    private readonly IApplicationDbContext _context;
    private readonly PayrollCalculator _calculator;
    private readonly IHRServiceClient _hrClient;
    private readonly IAttendanceServiceClient _attendanceClient;

    public PayrollCalculationService(
        IApplicationDbContext context,
        PayrollCalculator calculator,
        IHRServiceClient hrClient,
        IAttendanceServiceClient attendanceClient)
    {
        _context = context;
        _calculator = calculator;
        _hrClient = hrClient;
        _attendanceClient = attendanceClient;
    }

    public async Task<CalculatePayrollResponse> CalculateAsync(
        Guid payrollID,
        CancellationToken cancellationToken = default)
    {
        // 1. Find payroll and load its allowances and pay cuts.
        PayrollEntity? payroll = await _context.Payrolls
            .Include(p => p.PayrollPeriod)
            .Include(p => p.Allowances)
                .ThenInclude(a => a.AllowanceDetails)
            .Include(p => p.PayCuts)
                .ThenInclude(pc => pc.Details)
            .FirstOrDefaultAsync(
                p => p.PayrollID == payrollID,
                cancellationToken);

        if (payroll == null)
        {
            throw new InvalidOperationException(
                "Payroll does not exist.");
        }

        // 2. Only draft payroll can be calculated.
        if (payroll.Status != "draft")
        {
            throw new InvalidOperationException(
                "Only draft payroll can be calculated.");
        }

        PayrollPeriodEntity payrollPeriod = payroll.PayrollPeriod
            ?? throw new InvalidOperationException(
                "Payroll period does not exist.");

        if (!payroll.OTHours.HasValue ||
            !payroll.OTMultiplier.HasValue)
        {
            throw new InvalidOperationException(
                "Attendance and overtime data must be synchronized before payroll calculation.");
        }

        await ApplyAutomaticAdjustmentsAsync(
            payroll,
            payrollPeriod,
            cancellationToken);

        // 3. Calculate total allowance from active groups.
        decimal totalAllowance = payroll.Allowances
            .Where(a => a.Status == "active")
            .SelectMany(a => a.AllowanceDetails)
            .Sum(ad => ad.Amount);

        // 4. Calculate total pay cut from active groups.
        decimal totalPayCut = payroll.PayCuts
            .Where(pc => pc.Status == "active")
            .SelectMany(pc => pc.Details)
            .Sum(pd => pd.Amount);

        // 5. Calculate salary.
        PayrollCalculationResult result = _calculator.Calculate(
            payroll.EmploymentType ?? string.Empty,
            payroll.BaseSalary,
            payroll.HourlyRate,
            payroll.WorkingDays,
            payroll.StandardWorkingDay,
            payroll.TotalWorkingHours,
            payroll.OTHours.Value,
            payroll.OTMultiplier.Value,
            totalAllowance,
            totalPayCut,
            payroll.TotalProjectAllowance);

        // 6. Update payroll.
        payroll.TotalAllowance = result.TotalAllowance;
        payroll.TotalPayCut = result.TotalPayCut;
        payroll.OTPay = result.OTPay;
        payroll.GrossSalary = result.GrossSalary;
        payroll.NetSalary = result.NetSalary;

        payroll.Status = "calculated";
        payroll.CalculatedAt = DateTime.UtcNow;

        // 7. Save changes.
        await _context.SaveChangesAsync(cancellationToken);

        // 8. Return calculation response.
        return new CalculatePayrollResponse
        {
            PayrollID = payroll.PayrollID,
            EmployeeID = payroll.EmployeeID,

            EmploymentType = payroll.EmploymentType,
            BaseSalary = payroll.BaseSalary,
            HourlyRate = payroll.HourlyRate,

            WorkingDays = payroll.WorkingDays,
            StandardWorkingDay = payroll.StandardWorkingDay,
            TotalWorkingHours = payroll.TotalWorkingHours,

            ProratedSalary = result.ProratedSalary,

            OTHours = payroll.OTHours.Value,
            OTMultiplier = payroll.OTMultiplier.Value,
            OTPay = result.OTPay,

            TotalAllowance = result.TotalAllowance,
            TotalProjectAllowance = payroll.TotalProjectAllowance,
            TotalPayCut = result.TotalPayCut,

            GrossSalary = result.GrossSalary,
            NetSalary = result.NetSalary,

            Status = payroll.Status,
            CalculatedAt = payroll.CalculatedAt
        };
    }

    private async Task ApplyAutomaticAdjustmentsAsync(
        PayrollEntity payroll,
        PayrollPeriodEntity payrollPeriod,
        CancellationToken cancellationToken)
    {
        DateOnly periodEndDate = new(
            payrollPeriod.Year,
            payrollPeriod.Month,
            DateTime.DaysInMonth(payrollPeriod.Year, payrollPeriod.Month));

        var attendance = await _attendanceClient.GetEmployeeMonthlySummaryAsync(
            payroll.EmployeeID,
            payrollPeriod.Month,
            payrollPeriod.Year,
            cancellationToken);

        if (attendance == null ||
            attendance.EmployeeId != payroll.EmployeeID ||
            attendance.Month != payrollPeriod.Month ||
            attendance.Year != payrollPeriod.Year ||
            !string.Equals(
                attendance.PeriodStatus,
                "LOCKED",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"A locked attendance summary for {payrollPeriod.Month}/{payrollPeriod.Year} is required for employee {payroll.EmployeeID}.");
        }

        if (attendance.LateOccurrences < 0 || attendance.TotalLateMinutes < 0)
        {
            throw new InvalidOperationException(
                $"Invalid lateness data for employee {payroll.EmployeeID}.");
        }

        if (attendance.LateOccurrences > 0)
        {
            PayCutReason? lateReason = (await _context.PayCutReasons
                    .Where(reason => reason.Status == "active")
                    .ToListAsync(cancellationToken))
                .FirstOrDefault(reason => string.Equals(
                    reason.Name,
                    "Phạt đi muộn / về sớm",
                    StringComparison.OrdinalIgnoreCase));

            if (lateReason == null)
            {
                throw new InvalidOperationException(
                    "Active pay-cut reason 'Phạt đi muộn / về sớm' is required when the employee has late occurrences.");
            }

            if (!string.Equals(
                    lateReason.CalculationType,
                    "fixed",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "The late pay-cut reason must use the fixed calculation type.");
            }

            decimal lateDeduction = _calculator.CalculateLateDeduction(
                attendance.LateOccurrences,
                lateReason.DefaultRate);

            if (lateDeduction > 0)
            {
                PayCut payCut = payroll.PayCuts.FirstOrDefault(group => group.Status == "active")
                    ?? new PayCut { PayrollID = payroll.PayrollID, Status = "active" };

                if (!payroll.PayCuts.Contains(payCut))
                {
                    payroll.PayCuts.Add(payCut);
                    _context.PayCuts.Add(payCut);
                }

                payCut.Details.Add(new PayCutDetail
                {
                    PayCutReasonID = lateReason.PayCutReasonID,
                    Description = $"Tự động khấu trừ đi trễ ({attendance.LateOccurrences} lần)",
                    RateApplied = lateReason.DefaultRate,
                    Amount = lateDeduction
                });
            }
        }

        AllowanceType? seniorityAllowanceType = (await _context.AllowanceTypes
                .Where(type => type.Status == "active")
                .ToListAsync(cancellationToken))
            .FirstOrDefault(type => string.Equals(
                type.Name,
                "Phụ cấp thâm niên",
                StringComparison.OrdinalIgnoreCase));

        if (seniorityAllowanceType == null)
        {
            return;
        }

        if (!string.Equals(
                seniorityAllowanceType.CalculationType,
                "fixed",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "The seniority allowance type must use the fixed calculation type.");
        }

        HREmployeeResponse? employee = await _hrClient.GetEmployeeAsync(
            payroll.EmployeeID,
            cancellationToken);

        if (employee == null || employee.EmployeeId != payroll.EmployeeID)
        {
            throw new InvalidOperationException(
                $"HR employee data was not found or did not match payroll employee {payroll.EmployeeID}.");
        }

        if (!employee.EmploymentStartDate.HasValue)
        {
            throw new InvalidOperationException(
                $"Employment start date is missing for employee {payroll.EmployeeID}; seniority allowance cannot be calculated.");
        }

        decimal seniorityAllowance = _calculator.CalculateSeniorityAllowance(
            employee.EmploymentStartDate,
            periodEndDate,
            seniorityAllowanceType.DefaultRate);

        if (seniorityAllowance <= 0)
        {
            return;
        }

        int completedYears = _calculator.CalculateCompletedServiceYears(
            employee.EmploymentStartDate,
            periodEndDate);

        Allowance allowance = payroll.Allowances.FirstOrDefault(group => group.Status == "active")
            ?? new Allowance { PayrollID = payroll.PayrollID, Status = "active" };

        if (!payroll.Allowances.Contains(allowance))
        {
            payroll.Allowances.Add(allowance);
            _context.Allowances.Add(allowance);
        }

        allowance.AllowanceDetails.Add(new AllowanceDetail
        {
            AllowanceTypeID = seniorityAllowanceType.AllowanceTypeID,
            Description = $"Tự động phụ cấp thâm niên ({completedYears} năm)",
            RateApplied = seniorityAllowanceType.DefaultRate,
            Amount = seniorityAllowance
        });
    }
}