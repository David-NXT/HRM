using Attendance.DTOs.AttendanceExplanation;

namespace Attendance.Services;

public interface IAttendanceExplanationsService
{
    Task<List<AttendanceExplanationDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AttendanceExplanationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AttendanceExplanationDto> CreateAsync(CreateAttendanceExplanationDto dto, CancellationToken cancellationToken = default);
    Task<AttendanceExplanationDto?> UpdateAsync(Guid id, UpdateAttendanceExplanationDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}