using Attendance.DTOs.AttendanceExplanation;

namespace Attendance.Services;

public interface IAttendanceExplanationsService
{
    Task<List<AttendanceExplanationDto>> GetAllAsync (CancellationToken cancellationToken);
    Task<AttendanceExplanationDto?> GetByIdAsync (Guid id, CancellationToken cancellationToken);
    Task<AttendanceExplanationDto> CreateAsync (AttendanceExplanationDto dto, CancellationToken cancellationToken);
    Task<AttendanceExplanationDto?> UpdateAsync (AttendanceExplanationDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync (AttendanceExplanationDto dto, CancellationToken cancellationToken);
}