using Microsoft.EntityFrameworkCore;
using Payroll.Application.DTOs.PayrollQuery;
using Payroll.Application.Interfaces;
using PayrollEntity = global::Payroll.Domain.Entities.Payroll;

namespace Payroll.Application.Services.PayrollQuery;

public class PayrollQueryService
{
    private readonly IApplicationDbContext _context;

    public PayrollQueryService(IApplicationDbContext context)
    {
        _context = context;
    }

    // Get all payrolls in a period
    public async Task<List<PayrollListResponse>> GetByPeriodAsync(
        Guid payrollPeriodID,
        CancellationToken cancellationToken = default)
    {
        return await _context.Payrolls
            .AsNoTracking()
            .Where(p => p.PayrollPeriodID == payrollPeriodID)
            .OrderBy(p => p.EmployeeID)
            .Select(p => new PayrollListResponse
            {
                PayrollID = p.PayrollID,
                EmployeeID = p.EmployeeID,
                PayrollPeriodID = p.PayrollPeriodID,

                EmploymentType = p.EmploymentType,
                BaseSalary = p.BaseSalary,
                WorkingDays = p.WorkingDays,
                StandardWorkingDay = p.StandardWorkingDay,
                TotalWorkingHours = p.TotalWorkingHours,

                TotalAllowance = p.TotalAllowance,
                TotalProjectAllowance = p.TotalProjectAllowance,
                TotalPayCut = p.TotalPayCut,

                GrossSalary = p.GrossSalary,
                NetSalary = p.NetSalary,

                Status = p.Status,
                CalculatedAt = p.CalculatedAt,
                ApprovedAt = p.ApprovedAt,
                PaidAt = p.PaidAt
            })
            .ToListAsync(cancellationToken);
    }

    // Get payroll detail
    public async Task<PayrollDetailResponse?> GetByIDAsync(
        Guid payrollID,
        CancellationToken cancellationToken = default)
    {
        PayrollEntity? payroll = await _context.Payrolls
            .AsNoTracking()
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

        return new PayrollDetailResponse
        {
            PayrollID = payroll.PayrollID,
            EmployeeID = payroll.EmployeeID,
            PayrollPeriodID = payroll.PayrollPeriodID,

            BaseSalary = payroll.BaseSalary,
            WorkingDays = payroll.WorkingDays,
            StandardWorkingDay = payroll.StandardWorkingDay,

            TotalAllowance = payroll.TotalAllowance,
            TotalProjectAllowance = payroll.TotalProjectAllowance,
            TotalPayCut = payroll.TotalPayCut,

            GrossSalary = payroll.GrossSalary,
            NetSalary = payroll.NetSalary,

            Status = payroll.Status,
            CalculatedAt = payroll.CalculatedAt,
            ApprovedAt = payroll.ApprovedAt,
            PaidAt = payroll.PaidAt,

            Allowances = payroll.Allowances
                .Where(a => a.Status == "active")
                .SelectMany(a => a.AllowanceDetails)
                .Select(ad => new PayrollAllowanceResponse
                {
                    AllowanceDetailID = ad.AllowanceDetailID,
                    AllowanceTypeID = ad.AllowanceTypeID,
                    AllowanceTypeName = ad.AllowanceType!.Name,
                    Description = ad.Description,
                    RateApplied = ad.RateApplied,
                    Amount = ad.Amount
                })
                .ToList(),

            PayCuts = payroll.PayCuts
                .Where(pc => pc.Status == "active")
                .SelectMany(pc => pc.Details)
                .Select(pd => new PayrollPayCutResponse
                {
                    PayCutDetailID = pd.PayCutDetailID,
                    PayCutReasonID = pd.PayCutReasonID,
                    PayCutReasonName = pd.PayCutReason!.Name,
                    Description = pd.Description,
                    RateApplied = pd.RateApplied,
                    Amount = pd.Amount
                })
                .ToList()
        };
    }

    // Get employee payroll history
    public async Task<List<PayrollHistoryResponse>> GetEmployeeHistoryAsync(
        Guid employeeID,
        CancellationToken cancellationToken = default)
    {
        return await _context.Payrolls
            .AsNoTracking()
            .Where(p => p.EmployeeID == employeeID)
            .OrderByDescending(p => p.PayrollPeriod!.Year)
            .ThenByDescending(p => p.PayrollPeriod!.Month)
            .Select(p => new PayrollHistoryResponse
            {
                PayrollID = p.PayrollID,
                PayrollPeriodID = p.PayrollPeriodID,

                Month = p.PayrollPeriod!.Month,
                Year = p.PayrollPeriod!.Year,

                BaseSalary = p.BaseSalary,
                TotalAllowance = p.TotalAllowance,
                TotalProjectAllowance = p.TotalProjectAllowance,
                TotalPayCut = p.TotalPayCut,
                GrossSalary = p.GrossSalary,
                NetSalary = p.NetSalary,

                Status = p.Status,
                PaidAt = p.PaidAt
            })
            .ToListAsync(cancellationToken);
    }
}