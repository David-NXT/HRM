using Attendance.DTOs.LeaveRequest;

namespace Attendance.Services;

public interface ILeaveRequestService
{
    Task<List<LeaveRequestDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<LeaveRequestDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<LeaveRequestDto> CreateAsync(CreateLeaveRequestDto dto, CancellationToken cancellationToken = default);
    Task<LeaveRequestDto?> UpdateAsync(Guid id, UpdateLeaveRequestDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
