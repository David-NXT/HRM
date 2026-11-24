using Microsoft.EntityFrameworkCore;
using Payroll.Application.Interfaces;
using Payroll.Application.DTOs.HR;
using Payroll.Application.DTOs.ProjectPayroll;
using PayrollEntity = global::Payroll.Domain.Entities.Payroll;
using PayrollPeriodEntity = global::Payroll.Domain.Entities.PayrollPeriod;

namespace Payroll.Application.Services.PayrollSalarySync;

public class PayrollSalarySyncService
{
    private readonly IApplicationDbContext _context;
    private readonly IHRServiceClient _hrClient;
    private readonly IProjectServiceClient _projectClient;

    public PayrollSalarySyncService(
        IApplicationDbContext context,
        IHRServiceClient hrClient,
        IProjectServiceClient projectClient)
    {
        _context = context;
        _hrClient = hrClient;
        _projectClient = projectClient;
    }

    public async Task<int> SyncSalaryAsync(
        Guid payrollPeriodID,
        CancellationToken cancellationToken = default)
    {
        // 1. Find payroll period.
        PayrollPeriodEntity? period = await _context.PayrollPeriods
            .FirstOrDefaultAsync(
                p => p.PayrollPeriodID == payrollPeriodID,
                cancellationToken);

        if (period == null)
        {
            throw new InvalidOperationException(
                "Payroll period not found.");
        }

        if (!string.Equals(
                period.Status,
                "draft",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Cannot sync salary for a non-draft payroll period.");
        }

        // 2. Load payroll records.
        List<PayrollEntity> payrolls = await _context.Payrolls
            .Where(p => p.PayrollPeriodID == payrollPeriodID)
            .ToListAsync(cancellationToken);

        if (payrolls.Count == 0)
        {
            throw new InvalidOperationException(
                "No payroll records found for this period.");
        }

        // 3. Validate payroll statuses before calling HR API.
        foreach (PayrollEntity payroll in payrolls)
        {
            if (!string.Equals(
                    payroll.Status,
                    "draft",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Payroll {payroll.PayrollID} is not in draft status.");
            }
        }

        // 4. Retrieve and validate employee salary information.
        List<(Guid EmployeeID,
            string EmploymentType,
            decimal BaseSalary,
            decimal? HourlyRate)> salaryByEmployee =
            new List<(Guid EmployeeID,
                string EmploymentType,
                decimal BaseSalary,
                decimal? HourlyRate)>();

        foreach (PayrollEntity payroll in payrolls)
        {
            cancellationToken.ThrowIfCancellationRequested();

            HREmployeeResponse? employee = await _hrClient.GetEmployeeAsync(
                payroll.EmployeeID,
                cancellationToken);

            if (employee == null)
            {
                throw new InvalidOperationException(
                    $"Employee {payroll.EmployeeID} was not found in HR.");
            }

            if (employee.EmployeeId != payroll.EmployeeID)
            {
                throw new InvalidOperationException(
                    $"HR employee ID does not match payroll employee {payroll.EmployeeID}.");
            }

            if (!string.Equals(
                    employee.Status,
                    "ACTIVE",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Employee {payroll.EmployeeID} is not active.");
            }

            string employmentType = employee.EmploymentType
                .Trim()
                .ToUpperInvariant();

            decimal baseSalary;
            decimal? hourlyRate;

            switch (employmentType)
            {
                case "FULL_TIME":
                    if (!employee.BaseSalary.HasValue ||
                        employee.BaseSalary.Value < 0)
                    {
                        throw new InvalidOperationException(
                            $"Invalid base salary for employee {payroll.EmployeeID}.");
                    }

                    if (employee.PartTimeSalary.HasValue)
                    {
                        throw new InvalidOperationException(
                            $"Unexpected part-time salary for full-time employee {payroll.EmployeeID}.");
                    }

                    baseSalary = employee.BaseSalary.Value;
                    hourlyRate = null;
                    break;

                case "PART_TIME":
                    if (!employee.PartTimeSalary.HasValue ||
                        employee.PartTimeSalary.Value < 0)
                    {
                        throw new InvalidOperationException(
                            $"Invalid part-time salary for employee {payroll.EmployeeID}.");
                    }

                    if (employee.BaseSalary.HasValue)
                    {
                        throw new InvalidOperationException(
                            $"Unexpected base salary for part-time employee {payroll.EmployeeID}.");
                    }

                    baseSalary = 0m;
                    hourlyRate = employee.PartTimeSalary.Value;
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unsupported employment type '{employee.EmploymentType}' for employee {payroll.EmployeeID}.");
            }

            salaryByEmployee.Add(
                (payroll.EmployeeID, employmentType, baseSalary, hourlyRate));
        }

        // 5. Retrieve approved project income for the payroll period.
        DateOnly periodStart = new DateOnly(period.Year, period.Month, 1);
        DateOnly periodEnd = periodStart.AddMonths(1).AddDays(-1);
        IReadOnlyList<ProjectAllowanceSummaryResponse> projectSummaries =
            await _projectClient.GetProjectAllowanceSummaryAsync(
                periodStart,
                periodEnd,
                cancellationToken);

        List<(Guid EmployeeID, decimal TotalProjectIncome)> projectAllowanceByEmployee =
            new List<(Guid EmployeeID, decimal TotalProjectIncome)>();
        foreach (ProjectAllowanceSummaryResponse summary in projectSummaries)
        {
            if (summary.EmployeeId == Guid.Empty ||
                summary.TotalProjectIncome < 0)
            {
                throw new InvalidOperationException(
                    "Project Service returned an invalid project allowance summary.");
            }

            int existingIndex = projectAllowanceByEmployee.FindIndex(
                item => item.EmployeeID == summary.EmployeeId);
            if (existingIndex < 0)
            {
                projectAllowanceByEmployee.Add(
                    (summary.EmployeeId, summary.TotalProjectIncome));
            }
            else
            {
                projectAllowanceByEmployee[existingIndex] = (
                    summary.EmployeeId,
                    projectAllowanceByEmployee[existingIndex].TotalProjectIncome
                    + summary.TotalProjectIncome);
            }
        }

        // 6. Update payroll records only after all external data is valid.
        foreach (PayrollEntity payroll in payrolls)
        {
            (Guid EmployeeID, string EmploymentType, decimal BaseSalary, decimal? HourlyRate) salary =
                salaryByEmployee.First(item => item.EmployeeID == payroll.EmployeeID);

            payroll.EmploymentType = salary.EmploymentType;
            payroll.BaseSalary = salary.BaseSalary;
            payroll.HourlyRate = salary.HourlyRate;
            decimal projectAllowance = projectAllowanceByEmployee
                .Where(item => item.EmployeeID == payroll.EmployeeID)
                .Select(item => item.TotalProjectIncome)
                .FirstOrDefault();
            payroll.TotalProjectAllowance = Math.Round(
                projectAllowance,
                2,
                MidpointRounding.AwayFromZero);
        }

        // 7. Save changes.
        await _context.SaveChangesAsync(cancellationToken);

        return payrolls.Count;
    }
}