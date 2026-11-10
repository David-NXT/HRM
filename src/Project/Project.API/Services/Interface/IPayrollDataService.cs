using Project.API.DTOs;

namespace Project.API.Services;

public interface IPayrollDataService
{
    Task<IEnumerable<PayrollDataDto>> GetApprovedPayrollDataAsync(
        DateTime from,
        DateTime to);
}