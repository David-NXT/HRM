using PayrollPeriodEntity = Payroll.Domain.Entities.PayrollPeriod;
using Payroll.Application.Interfaces;
using Payroll.Application.DTOs.PayrollPeriod;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Payroll.Application.Services.PayrollPeriod;

public class PayrollPeriodService
{
    private readonly IApplicationDbContext _context;

    public PayrollPeriodService(IApplicationDbContext context)
    {
        _context = context;
    }

    private static void Validate(CreatePayrollPeriodRequest request)
    {
        if (request.Month < 1 || request.Month > 12)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.Month),
                "Month must be between 1 and 12."
            );
        }
    
        int currentYear = DateTime.UtcNow.Year;
    
        if (request.Year < 2000 || request.Year > currentYear)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request.Year),
                "Year must be between 2000 and the current year."
            );
        }
    }

    private async Task<bool> IsDuplicateAsync(CreatePayrollPeriodRequest request, CancellationToken cancellationToken = default)
    {
        return await _context.PayrollPeriods.AnyAsync(p => p.Month == request.Month && p.Year == request.Year, cancellationToken);
        
    }

    public async Task<PayrollPeriodResponse> CreateAsync(CreatePayrollPeriodRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request);
        if(await IsDuplicateAsync(request,cancellationToken)) throw new InvalidOperationException("Payroll period already exists.");

        var payrollPeriod = new PayrollPeriodEntity
        {
            Month = request.Month,
            Year = request.Year  
        };

        _context.PayrollPeriods.Add(payrollPeriod);

        await _context.SaveChangesAsync(cancellationToken);

        return new PayrollPeriodResponse
        {
            PayrollPeriodID = payrollPeriod.PayrollPeriodID,
            Year = payrollPeriod.Year,
            Month = payrollPeriod.Month,
            Status = payrollPeriod.Status,
            CreatedAt = payrollPeriod.CreatedAt
        };
    }

    public async Task<List<PayrollPeriodResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var periods = await _context.PayrollPeriods
            .Include(p => p.Payrolls)
            .OrderByDescending(p => p.Year)
            .ThenByDescending(p => p.Month)
            .ToListAsync(cancellationToken);

        return periods.Select(p => new PayrollPeriodResponse
        {
            PayrollPeriodID = p.PayrollPeriodID,
            Year = p.Year,
            Month = p.Month,
            Status = GetCurrentStatus(p),
            CreatedAt = p.CreatedAt
        }).ToList();
    }

    private static string GetCurrentStatus(PayrollPeriodEntity period)
    {
        if (period.Payrolls.Count == 0)
        {
            return period.Status;
        }

        var statuses = period.Payrolls
            .Select(payroll => payroll.Status)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (statuses.Count == 1)
        {
            return statuses[0];
        }

        return statuses.Contains("draft", StringComparer.OrdinalIgnoreCase)
            ? "draft"
            : "in_progress";
    }
}