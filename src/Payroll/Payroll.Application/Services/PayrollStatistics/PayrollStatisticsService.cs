using Microsoft.EntityFrameworkCore;
using Payroll.Application.DTOs.PayrollStatistics;
using Payroll.Application.Interfaces;

namespace Payroll.Application.Services.PayrollStatistics;

public class PayrollStatisticsService
{
    private readonly IApplicationDbContext _context;

    public PayrollStatisticsService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PayrollPeriodSalaryStatisticsResponse?> GetPeriodStatisticsAsync(
        Guid payrollPeriodID,
        CancellationToken cancellationToken = default)
    {
        var period = await _context.PayrollPeriods
            .AsNoTracking()
            .Where(p => p.PayrollPeriodID == payrollPeriodID)
            .Select(p => new { p.PayrollPeriodID, p.Year, p.Month })
            .FirstOrDefaultAsync(cancellationToken);

        if (period == null)
            return null;

        var payrolls = await GetPeriodPayrollRowsAsync(
            payrollPeriodID,
            cancellationToken);

        return CreatePeriodResponse(
            period.PayrollPeriodID,
            period.Year,
            period.Month,
            payrolls);
    }

    public async Task<PayrollPeriodSalaryStatisticsResponse?> GetPeriodStatisticsAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        ValidateYear(year);
        ValidateMonth(month);

        var period = await _context.PayrollPeriods
            .AsNoTracking()
            .Where(p => p.Year == year && p.Month == month)
            .Select(p => new { p.PayrollPeriodID, p.Year, p.Month })
            .FirstOrDefaultAsync(cancellationToken);

        if (period == null)
            return null;

        var payrolls = await GetPeriodPayrollRowsAsync(
            period.PayrollPeriodID,
            cancellationToken);

        return CreatePeriodResponse(
            period.PayrollPeriodID,
            period.Year,
            period.Month,
            payrolls);
    }

    public async Task<List<PayrollPeriodSalaryStatisticsResponse>> GetAllPeriodStatisticsAsync(
        int? year = null,
        CancellationToken cancellationToken = default)
    {
        if (year.HasValue)
            ValidateYear(year.Value);

        var periods = await _context.PayrollPeriods
            .AsNoTracking()
            .Where(p => !year.HasValue || p.Year == year.Value)
            .OrderBy(p => p.Year)
            .ThenBy(p => p.Month)
            .Select(p => new { p.PayrollPeriodID, p.Year, p.Month })
            .ToListAsync(cancellationToken);

        var response = new List<PayrollPeriodSalaryStatisticsResponse>();
        foreach (var period in periods)
        {
            var payrolls = await GetPeriodPayrollRowsAsync(
                period.PayrollPeriodID,
                cancellationToken);

            response.Add(CreatePeriodResponse(
                period.PayrollPeriodID,
                period.Year,
                period.Month,
                payrolls));
        }

        return response;
    }

    public async Task<List<PayrollQuarterSalaryStatisticsResponse>> GetQuarterStatisticsAsync(
        int year,
        int? quarter = null,
        CancellationToken cancellationToken = default)
    {
        ValidateYear(year);
        if (quarter.HasValue)
            ValidateQuarter(quarter.Value);

        var payrolls = await _context.Payrolls
            .AsNoTracking()
            .Where(p => p.PayrollPeriod!.Year == year)
            .Where(p => !quarter.HasValue ||
                (p.PayrollPeriod!.Month - 1) / 3 + 1 == quarter.Value)
            .Select(p => new
            {
                p.EmployeeID,
                p.PayrollPeriod!.Month,
                p.BaseSalary,
                p.TotalAllowance,
                p.TotalProjectAllowance,
                p.TotalPayCut,
                p.GrossSalary,
                p.NetSalary
            })
            .ToListAsync(cancellationToken);

        return payrolls
            .GroupBy(p => (p.Month - 1) / 3 + 1)
            .OrderBy(g => g.Key)
            .Select(g => new PayrollQuarterSalaryStatisticsResponse
            {
                Year = year,
                Quarter = g.Key,
                EmployeeCount = g.Select(p => p.EmployeeID).Distinct().Count(),
                PayrollCount = g.Count(),
                TotalBaseSalary = g.Sum(p => p.BaseSalary),
                TotalAllowance = g.Sum(p => p.TotalAllowance),
                TotalProjectAllowance = g.Sum(p => p.TotalProjectAllowance),
                TotalPayCut = g.Sum(p => p.TotalPayCut),
                TotalGrossSalary = g.Sum(p => p.GrossSalary),
                TotalNetSalary = g.Sum(p => p.NetSalary)
            })
            .ToList();
    }

    public async Task<List<EmployeeQuarterSalaryStatisticsResponse>> GetEmployeeQuarterStatisticsAsync(
        Guid employeeID,
        int year,
        int? quarter = null,
        CancellationToken cancellationToken = default)
    {
        ValidateYear(year);
        if (quarter.HasValue)
            ValidateQuarter(quarter.Value);

        var payrolls = await _context.Payrolls
            .AsNoTracking()
            .Where(p => p.EmployeeID == employeeID && p.PayrollPeriod!.Year == year)
            .Where(p => !quarter.HasValue ||
                (p.PayrollPeriod!.Month - 1) / 3 + 1 == quarter.Value)
            .Select(p => new
            {
                p.EmployeeID,
                p.PayrollPeriod!.Month,
                p.BaseSalary,
                p.TotalAllowance,
                p.TotalProjectAllowance,
                p.TotalPayCut,
                p.GrossSalary,
                p.NetSalary
            })
            .ToListAsync(cancellationToken);

        return payrolls
            .GroupBy(p => (p.Month - 1) / 3 + 1)
            .OrderBy(g => g.Key)
            .Select(g => new EmployeeQuarterSalaryStatisticsResponse
            {
                EmployeeID = employeeID,
                Year = year,
                Quarter = g.Key,
                PayrollCount = g.Count(),
                TotalBaseSalary = g.Sum(p => p.BaseSalary),
                TotalAllowance = g.Sum(p => p.TotalAllowance),
                TotalProjectAllowance = g.Sum(p => p.TotalProjectAllowance),
                TotalPayCut = g.Sum(p => p.TotalPayCut),
                TotalGrossSalary = g.Sum(p => p.GrossSalary),
                TotalNetSalary = g.Sum(p => p.NetSalary)
            })
            .ToList();
    }

    public Task<List<PayrollQuarterSalaryStatisticsResponse>> GetSalaryStatisticsByQuarterAsync(
        int year,
        int? quarter = null,
        CancellationToken cancellationToken = default)
        => GetQuarterStatisticsAsync(year, quarter, cancellationToken);

    public Task<List<EmployeeQuarterSalaryStatisticsResponse>> GetEmployeeSalaryStatisticsByQuarterAsync(
        Guid employeeID,
        int year,
        int? quarter = null,
        CancellationToken cancellationToken = default)
        => GetEmployeeQuarterStatisticsAsync(employeeID, year, quarter, cancellationToken);

    private static PayrollPeriodSalaryStatisticsResponse CreatePeriodResponse(
        Guid periodID,
        int year,
        int month,
        IReadOnlyCollection<PeriodPayrollRow> payrolls)
    {
        return new PayrollPeriodSalaryStatisticsResponse
        {
            PayrollPeriodID = periodID,
            Year = year,
            Month = month,
            EmployeeCount = payrolls
                .Select(p => p.EmployeeID)
                .Distinct()
                .Count(),
            TotalBaseSalary = payrolls.Sum(p => p.BaseSalary),
            TotalAllowance = payrolls.Sum(p => p.TotalAllowance),
            TotalProjectAllowance = payrolls.Sum(p => p.TotalProjectAllowance),
            TotalPayCut = payrolls.Sum(p => p.TotalPayCut),
            TotalGrossSalary = payrolls.Sum(p => p.GrossSalary),
            TotalNetSalary = payrolls.Sum(p => p.NetSalary)
        };
    }

    private async Task<List<PeriodPayrollRow>> GetPeriodPayrollRowsAsync(
        Guid periodID,
        CancellationToken cancellationToken)
    {
        return await _context.Payrolls
            .AsNoTracking()
            .Where(p => p.PayrollPeriodID == periodID)
            .Select(p => new PeriodPayrollRow
            {
                EmployeeID = p.EmployeeID,
                BaseSalary = p.BaseSalary,
                TotalAllowance = p.TotalAllowance,
                TotalProjectAllowance = p.TotalProjectAllowance,
                TotalPayCut = p.TotalPayCut,
                GrossSalary = p.GrossSalary,
                NetSalary = p.NetSalary
            })
            .ToListAsync(cancellationToken);
    }

    private static void ValidateYear(int year)
    {
        if (year < 1)
            throw new ArgumentOutOfRangeException(nameof(year), "Year must be greater than zero.");
    }

    private static void ValidateMonth(int month)
    {
        if (month is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12.");
    }

    private static void ValidateQuarter(int quarter)
    {
        if (quarter is < 1 or > 4)
            throw new ArgumentOutOfRangeException(nameof(quarter), "Quarter must be between 1 and 4.");
    }

}

public class PeriodPayrollRow
{
    public Guid EmployeeID { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal TotalAllowance { get; set; }
    public decimal TotalProjectAllowance { get; set; }
    public decimal TotalPayCut { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal NetSalary { get; set; }
}
