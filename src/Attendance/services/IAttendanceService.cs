using Attendance.DTOs.AttendanceRecord;

namespace Attendance.Services;

public interface IAttendanceService
{
    Task<List<AttendanceRecordDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AttendanceRecordDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AttendanceRecordDto> CreateAsync(CreateAttendanceRecordDto dto, CancellationToken cancellationToken = default);
    Task<AttendanceRecordDto?> UpdateAsync(Guid id, UpdateAttendanceRecordDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
