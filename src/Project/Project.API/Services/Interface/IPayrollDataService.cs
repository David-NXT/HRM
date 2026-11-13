
using Project.API.DTOs;

namespace Project.API.Services;

public interface IPayrollDataService
{
    Task<IEnumerable<PayrollDataDto>> GetApprovedPayrollDataAsync(
        DateTime fromUtc,
        DateTime toExclusiveUtc,
        Guid? employeeId = null);

    Task<IEnumerable<PayrollProjectSummaryDto>>
        GetProjectAllowanceSummaryAsync(
            DateTime fromUtc,
            DateTime toExclusiveUtc,
            DateTime fromDate,
            DateTime toDate,
            Guid? employeeId = null);
}
