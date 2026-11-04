using Attendance.DTOs.Holiday;

namespace Attendance.Services;

public interface IHolidayService
{
    Task<List<HolidayDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<HolidayDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<HolidayDto> CreateAsync(CreateHolidayDto dto, CancellationToken cancellationToken = default);
    Task<HolidayDto?> UpdateAsync(Guid id, UpdateHolidayDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
