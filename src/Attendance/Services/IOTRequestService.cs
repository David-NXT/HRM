using Attendance.DTOs.OTRequest;

namespace Attendance.Services;

public interface IOTRequestService
{
    Task<List<OTRequestDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OTRequestDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OTRequestDto> CreateAsync(CreateOTRequestDto dto, CancellationToken cancellationToken = default);
    Task<OTRequestDto?> UpdateAsync(Guid id, UpdateOTRequestDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
