using Microsoft.EntityFrameworkCore;
using Payroll.Application.DTOs.PayrollApproval;
using Payroll.Application.Interfaces;
using PayrollEntity = global::Payroll.Domain.Entities.Payroll;

namespace Payroll.Application.Services.PayrollApproval;

public class PayrollApprovalService
{
    private readonly IApplicationDbContext _context;

    public PayrollApprovalService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApprovePayrollResponse> ApproveAsync(
        Guid payrollID,
        CancellationToken cancellationToken = default)
    {
        PayrollEntity? payroll = await _context.Payrolls
            .FirstOrDefaultAsync(
                p => p.PayrollID == payrollID,
                cancellationToken
            );

        if (payroll == null)
            throw new InvalidOperationException("Payroll does not exist.");

        payroll.Approve();

        await _context.SaveChangesAsync(cancellationToken);

        return new ApprovePayrollResponse
        {
            PayrollID = payroll.PayrollID,
            EmployeeID = payroll.EmployeeID,
            Status = payroll.Status,
            ApprovedAt = payroll.ApprovedAt
                ?? throw new InvalidOperationException("Approved payroll must have an approval timestamp.")
        };
    }
}
