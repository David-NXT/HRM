using Microsoft.EntityFrameworkCore;
using Payroll.Application.DTOs.PayrollCalculation;
using Payroll.Application.Interfaces;

namespace Payroll.Application.Services.PayrollCalculation;

public class PayrollCalculationService
{
    private readonly IApplicationDbContext _context;
    private readonly PayrollCalculator _calculator;

    public PayrollCalculationService(
        IApplicationDbContext context,
        PayrollCalculator calculator)
    {
        _context = context;
        _calculator = calculator;
    }

    public async Task<CalculatePayrollResponse> CalculateAsync(
        Guid payrollID,
        CancellationToken cancellationToken = default)
    {
        // 1. Find payroll and load its allowances and pay cuts.
        var payroll = await _context.Payrolls
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

        // 3. Calculate total allowance from active groups.
        var totalAllowance = payroll.Allowances
            .Where(a => a.Status == "active")
            .SelectMany(a => a.AllowanceDetails)
            .Sum(ad => ad.Amount);

        // 4. Calculate total pay cut from active groups.
        var totalPayCut = payroll.PayCuts
            .Where(pc => pc.Status == "active")
            .SelectMany(pc => pc.Details)
            .Sum(pd => pd.Amount);

        // 5. Validate attendance and overtime synchronization.
        if (!payroll.OTHours.HasValue ||
            !payroll.OTMultiplier.HasValue)
        {
            throw new InvalidOperationException(
                "Attendance and overtime data must be synchronized before payroll calculation.");
        }

        // 6. Calculate salary.
        var result = _calculator.Calculate(
            payroll.EmploymentType ?? string.Empty,
            payroll.BaseSalary,
            payroll.HourlyRate,
            payroll.WorkingDays,
            payroll.StandardWorkingDay,
            payroll.TotalWorkingHours,
            payroll.OTHours.Value,
            payroll.OTMultiplier.Value,
            totalAllowance,
            totalPayCut);

        // 7. Update payroll.
        payroll.TotalAllowance = result.TotalAllowance;
        payroll.TotalPayCut = result.TotalPayCut;
        payroll.OTPay = result.OTPay;
        payroll.GrossSalary = result.GrossSalary;
        payroll.NetSalary = result.NetSalary;

        payroll.Status = "calculated";
        payroll.CalculatedAt = DateTime.UtcNow;

        // 8. Save changes.
        await _context.SaveChangesAsync(cancellationToken);

        // 9. Return calculation response.
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
            TotalPayCut = result.TotalPayCut,

            GrossSalary = result.GrossSalary,
            NetSalary = result.NetSalary,

            Status = payroll.Status,
            CalculatedAt = payroll.CalculatedAt
        };
    }
}