using Microsoft.EntityFrameworkCore;
using Payroll.Application.DTOs.PayrollPeriodManagement;
using Payroll.Application.Interfaces;
using PayrollPeriodEntity = global::Payroll.Domain.Entities.PayrollPeriod;

namespace Payroll.Application.Services.PayrollPeriodManagement;

public class PayrollPeriodManagementService
{
    private readonly IApplicationDbContext _context;

    public PayrollPeriodManagementService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UpdatePayrollPeriodStatusResponse> UpdateStatusAsync(
        Guid payrollPeriodID,
        CancellationToken cancellationToken = default)
    {
        PayrollPeriodEntity? period = await _context.PayrollPeriods
            .Include(p => p.Payrolls)
            .FirstOrDefaultAsync(
                p => p.PayrollPeriodID == payrollPeriodID,
                cancellationToken
            );

        if (period == null)
            throw new InvalidOperationException("Payroll period does not exist.");

        if (period.Payrolls.Count == 0)
            throw new InvalidOperationException(
                "Cannot update an empty payroll period."
            );

        string newStatus;

        if (period.Payrolls.All(p => p.Status == "paid"))
        {
            newStatus = "paid";
        }
        else if (period.Payrolls.All(p => p.Status == "approved"))
        {
            newStatus = "approved";
        }
        else if (period.Payrolls.All(p => p.Status == "calculated"))
        {
            newStatus = "calculated";
        }
        else
        {
            throw new InvalidOperationException(
                "Not all payrolls have completed the required status."
            );
        }

        period.Status = newStatus;

        await _context.SaveChangesAsync(cancellationToken);

        return new UpdatePayrollPeriodStatusResponse
        {
            PayrollPeriodID = period.PayrollPeriodID,
            Year = period.Year,
            Month = period.Month,
            Status = period.Status,
            PayrollCount = period.Payrolls.Count,
            UpdatedAt = DateTime.UtcNow
        };
    }






















    
}