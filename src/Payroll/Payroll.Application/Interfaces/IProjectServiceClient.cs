using Payroll.Application.DTOs.ProjectPayroll;

namespace Payroll.Application.Interfaces;

public interface IProjectServiceClient
{
    Task<IReadOnlyList<ProjectAllowanceSummaryResponse>>
        GetProjectAllowanceSummaryAsync(
            DateOnly fromDate,
            DateOnly toDate,
            CancellationToken cancellationToken = default);
}
