using Microsoft.EntityFrameworkCore;
using Payroll.Application.DTOs.Payslip;
using Payroll.Application.Interfaces;

namespace Payroll.Application.Services.Payslip;

public class PayslipService
{
    private readonly IApplicationDbContext _context;

    public PayslipService(IApplicationDbContext context)
    {
        _context = context;
    }

    // Get payslip by Payroll ID
    public async Task<PayslipResponse?> GetByPayrollIDAsync(
        Guid payrollID,
        CancellationToken cancellationToken = default)
    {
        var payroll = await _context.Payrolls
            .AsNoTracking()
            .Include(p => p.PayrollPeriod)
            .Include(p => p.Allowances)
                .ThenInclude(a => a.AllowanceDetails)
                    .ThenInclude(ad => ad.AllowanceType)
            .Include(p => p.PayCuts)
                .ThenInclude(pc => pc.Details)
                    .ThenInclude(pd => pd.PayCutReason)
            .FirstOrDefaultAsync(
                p => p.PayrollID == payrollID,
                cancellationToken);

        if (payroll == null)
            return null;

        if (payroll.Status != "approved" && payroll.Status != "paid")
            return null;

        if (payroll.ApprovedAt == null)
            throw new InvalidOperationException(
                "Approved payroll must have an approval timestamp.");

        return new PayslipResponse
        {
            PayrollID = payroll.PayrollID,
            EmployeeID = payroll.EmployeeID,

            Month = payroll.PayrollPeriod!.Month,
            Year = payroll.PayrollPeriod.Year,

            BaseSalary = payroll.BaseSalary,
            WorkingDays = payroll.WorkingDays,
            StandardWorkingDay = payroll.StandardWorkingDay,

            TotalAllowance = payroll.TotalAllowance,
            TotalProjectAllowance = payroll.TotalProjectAllowance,
            TotalPayCut = payroll.TotalPayCut,
            GrossSalary = payroll.GrossSalary,
            NetSalary = payroll.NetSalary,

            Status = payroll.Status,
            ApprovedAt = payroll.ApprovedAt.Value,
            PaidAt = payroll.PaidAt,

            Allowances = payroll.Allowances
                .Where(a => a.Status == "active")
                .SelectMany(a => a.AllowanceDetails)
                .Select(ad => new PayslipAllowanceResponse
                {
                    AllowanceTypeName = ad.AllowanceType!.Name,
                    Description = ad.Description,
                    Amount = ad.Amount
                })
                .ToList(),

            PayCuts = payroll.PayCuts
                .Where(pc => pc.Status == "active")
                .SelectMany(pc => pc.Details)
                .Select(pd => new PayslipPayCutResponse
                {
                    PayCutReasonName = pd.PayCutReason!.Name,
                    Description = pd.Description,
                    Amount = pd.Amount
                })
                .ToList()
        };
    }

    // Get all payslips in a payroll period
    public async Task<List<PayslipResponse>> GetByPeriodAsync(
        Guid payrollPeriodID,
        CancellationToken cancellationToken = default)
    {
        var payrollIDs = await _context.Payrolls
            .AsNoTracking()
            .Where(p => p.PayrollPeriodID == payrollPeriodID)
            .Select(p => p.PayrollID)
            .ToListAsync(cancellationToken);

        var payslips = new List<PayslipResponse>();

        foreach (var payrollID in payrollIDs)
        {
            var payslip = await GetByPayrollIDAsync(
                payrollID,
                cancellationToken);

            if (payslip != null)
                payslips.Add(payslip);
        }

        return payslips;
    }
}