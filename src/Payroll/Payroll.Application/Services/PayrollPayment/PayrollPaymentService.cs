using Microsoft.EntityFrameworkCore;
using Payroll.Application.DTOs.PayrollPayment;
using Payroll.Application.Interfaces;
using PayrollEntity = global::Payroll.Domain.Entities.Payroll;

namespace Payroll.Application.Services.PayrollPayment;

public class PayrollPaymentService
{
    private readonly IApplicationDbContext _context;

    public PayrollPaymentService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MarkPayrollAsPaidResponse> MarkAsPaidAsync(
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

        if (payroll.Status != "approved")
            throw new InvalidOperationException(
                "Only approved payroll can be marked as paid."
            );

        payroll.Status = "paid";
        payroll.PaidAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new MarkPayrollAsPaidResponse
        {
            PayrollID = payroll.PayrollID,
            EmployeeID = payroll.EmployeeID,
            NetSalary = payroll.NetSalary,
            Status = payroll.Status,
            PaidAt = payroll.PaidAt.Value
        };
    }
}