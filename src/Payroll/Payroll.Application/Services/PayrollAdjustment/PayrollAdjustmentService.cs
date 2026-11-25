using Microsoft.EntityFrameworkCore;
using Payroll.Application.DTOs.PayrollAdjustment;
using Payroll.Application.Interfaces;
using Payroll.Domain.Entities;
using PayrollEntity = global::Payroll.Domain.Entities.Payroll;

namespace Payroll.Application.Services.PayrollAdjustment;

public class PayrollAdjustmentService
{
    private readonly IApplicationDbContext _context;

    public PayrollAdjustmentService(IApplicationDbContext context)
    {
        _context = context;
    }

    // Add allowance or bonus
    public async Task<CreateAllowanceResponse> AddAllowanceAsync(
        Guid payrollID,
        CreateAllowanceRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Validate request
        if (request.Amount <= 0)
        {
            throw new ArgumentException(
                "Allowance amount must be greater than zero."
            );
        }

        if (request.RateApplied < 0)
        {
            throw new ArgumentException(
                "Rate applied cannot be negative."
            );
        }

        // 2. Find payroll
        PayrollEntity? payroll = await _context.Payrolls
            .FirstOrDefaultAsync(
                p => p.PayrollID == payrollID,
                cancellationToken
            );

        if (payroll == null)
        {
            throw new InvalidOperationException(
                "Payroll does not exist."
            );
        }

        // 3. Only draft payroll can be adjusted
        if (payroll.Status != "draft")
        {
            throw new InvalidOperationException(
                "Cannot adjust a payroll that is not draft."
            );
        }

        // 4. Check allowance type
        AllowanceType? allowanceType = await _context.AllowanceTypes
            .FirstOrDefaultAsync(
                at => at.AllowanceTypeID == request.AllowanceTypeID,
                cancellationToken
            );

        if (allowanceType == null || !allowanceType.IsActive())
        {
            throw new InvalidOperationException(
                "Allowance type does not exist or is inactive."
            );
        }

        // 5. Find active allowance group
        Allowance? allowance = await _context.Allowances
            .FirstOrDefaultAsync(
                a => a.PayrollID == payrollID &&
                     a.Status == "active",
                cancellationToken
            );

        // 6. Create allowance group if it does not exist
        if (allowance == null)
        {
            allowance = new Allowance
            {
                PayrollID = payrollID,
                Status = "active"
            };

            _context.Allowances.Add(allowance);
        }

        // 7. Create allowance detail
        AllowanceDetail detail = new AllowanceDetail
        {
            AllowanceTypeID = request.AllowanceTypeID,
            Description = request.Description.Trim(),
            RateApplied = request.RateApplied,
            Amount = request.Amount
        };

        allowance.AddDetail(detail);

        // 8. Save
        await _context.SaveChangesAsync(cancellationToken);

        // 9. Return response
        return new CreateAllowanceResponse
        {
            AllowanceDetailID = detail.AllowanceDetailID,
            PayrollID = payrollID,
            AllowanceTypeID = detail.AllowanceTypeID,
            Description = detail.Description,
            RateApplied = detail.RateApplied,
            Amount = detail.Amount
        };
    }

    // Add deduction
    public async Task<CreatePayCutResponse> AddPayCutAsync(
        Guid payrollID,
        CreatePayCutRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Validate request
        if (request.Amount <= 0)
        {
            throw new ArgumentException(
                "Pay cut amount must be greater than zero."
            );
        }

        if (request.RateApplied < 0)
        {
            throw new ArgumentException(
                "Rate applied cannot be negative."
            );
        }

        // 2. Find payroll
        PayrollEntity? payroll = await _context.Payrolls
            .FirstOrDefaultAsync(
                p => p.PayrollID == payrollID,
                cancellationToken
            );

        if (payroll == null)
        {
            throw new InvalidOperationException(
                "Payroll does not exist."
            );
        }

        // 3. Only draft payroll can be adjusted
        if (payroll.Status != "draft")
        {
            throw new InvalidOperationException(
                "Cannot adjust a payroll that is not draft."
            );
        }

        // 4. Check pay cut reason
        PayCutReason? reason = await _context.PayCutReasons
            .FirstOrDefaultAsync(
                r => r.PayCutReasonID == request.PayCutReasonID,
                cancellationToken
            );

        if (reason == null || !reason.IsActive())
        {
            throw new InvalidOperationException(
                "Pay cut reason does not exist or is inactive."
            );
        }

        // 5. Find active pay cut group
        PayCut? payCut = await _context.PayCuts
            .FirstOrDefaultAsync(
                pc => pc.PayrollID == payrollID &&
                      pc.Status == "active",
                cancellationToken
            );

        // 6. Create pay cut group if it does not exist
        if (payCut == null)
        {
            payCut = new PayCut
            {
                PayrollID = payrollID,
                Status = "active"
            };

            _context.PayCuts.Add(payCut);
        }

        // 7. Create pay cut detail
        PayCutDetail detail = new PayCutDetail
        {
            PayCutReasonID = request.PayCutReasonID,
            Description = request.Description.Trim(),
            RateApplied = request.RateApplied,
            Amount = request.Amount
        };

        payCut.AddDetail(detail);

        // 8. Save
        await _context.SaveChangesAsync(cancellationToken);

        // 9. Return response
        return new CreatePayCutResponse
        {
            PayCutDetailID = detail.PayCutDetailID,
            PayrollID = payrollID,
            PayCutReasonID = detail.PayCutReasonID,
            Description = detail.Description,
            RateApplied = detail.RateApplied,
            Amount = detail.Amount
        };
    }
}
