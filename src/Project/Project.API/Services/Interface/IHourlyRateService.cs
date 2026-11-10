using Project.API.Entities;

namespace Project.API.Services;

public interface IHourlyRateService
{
    Task<IEnumerable<HourlyRate>> GetHourlyRatesAsync(Guid? projectMemberId = null);
    Task<HourlyRate?> GetHourlyRateAsync(Guid id);
    Task<HourlyRate> CreateHourlyRateAsync(HourlyRate input);
    Task<bool> UpdateHourlyRateAsync(Guid id, HourlyRate input);
    Task<bool> DeleteHourlyRateAsync(Guid id);
}