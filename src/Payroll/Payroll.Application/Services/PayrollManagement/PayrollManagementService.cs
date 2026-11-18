using Microsoft.EntityFrameworkCore;
using Payroll.Application.DTOs.PayrollManagement;
using Payroll.Application.Interfaces;
using PayrollEntity = global::Payroll.Domain.Entities.Payroll;
using PayrollPeriodEntity = global::Payroll.Domain.Entities.PayrollPeriod;

namespace Payroll.Application.Services.PayrollManagement;

public class PayrollManagementService
{
    private readonly IApplicationDbContext _context;

    public PayrollManagementService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CreatePayrollsResponse> CreatePayrollsAsync(
        Guid payrollPeriodID,
        CreatePayrollsRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Validate employee IDs
        List<Guid> employeeIDs = request.EmployeeIDs
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        if (employeeIDs.Count == 0)
        {
            throw new ArgumentException(
                "Employee list must contain at least one valid employee ID."
            );
        }

        if (employeeIDs.Count != request.EmployeeIDs.Distinct().Count())
        {
            throw new ArgumentException(
                "Employee IDs must be valid GUIDs."
            );
        }

        // 2. Find payroll period
        PayrollPeriodEntity? payrollPeriod = await _context.PayrollPeriods
            .FirstOrDefaultAsync(
                p => p.PayrollPeriodID == payrollPeriodID,
                cancellationToken
            );

        if (payrollPeriod == null)
        {
            throw new InvalidOperationException(
                "Payroll period does not exist."
            );
        }

        // 3. Only draft period can create payrolls
        if (payrollPeriod.Status != "draft")
        {
            throw new InvalidOperationException(
                "Cannot create payrolls for a period that is not draft."
            );
        }

        // 4. Find employees who already have payroll records
        List<Guid> existingEmployeeIDs = await _context.Payrolls
            .Where(p =>
                p.PayrollPeriodID == payrollPeriodID &&
                employeeIDs.Contains(p.EmployeeID)
            )
            .Select(p => p.EmployeeID)
            .ToListAsync(cancellationToken);
    
        // 5. Get employees who do not have payroll records
        List<Guid> newEmployeeIDs = employeeIDs
            .Except(existingEmployeeIDs)
            .ToList();

        // 6. Create empty payroll records
        List<PayrollEntity> newPayrolls = newEmployeeIDs.Select(employeeID =>
            new PayrollEntity
            {
                PayrollPeriodID = payrollPeriodID,
                EmployeeID = employeeID,

                BaseSalary = 0,
                WorkingDays = 0,
                TotalAllowance = 0,
                TotalPayCut = 0,
                GrossSalary = 0,
                NetSalary = 0,

                Status = "draft"
            }
        ).ToList();

        // 7. Save to database
        if (newPayrolls.Count > 0)
        {
            _context.Payrolls.AddRange(newPayrolls);

            await _context.SaveChangesAsync(cancellationToken);
        }

        // 8. Return result
        return new CreatePayrollsResponse
        {
            PayrollPeriodID = payrollPeriodID,
            CreatedCount = newPayrolls.Count,

            CreatedEmployeeIDs = newEmployeeIDs,

            SkippedEmployeeIDs = existingEmployeeIDs
        };
    }
}
