using Attendance.DTOs.AttendanceRule;

namespace Attendance.Services;

public interface IAttendanceRuleService
{
    Task<List<AttendanceRuleDto>> GetAllAsync(bool? isActive = null, CancellationToken cancellationToken = default);
    Task<AttendanceRuleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AttendanceRuleDto?> GetActiveRuleAsync(CancellationToken cancellationToken = default);
    Task<AttendanceRuleDto> CreateAsync(CreateAttendanceRuleDto dto, CancellationToken cancellationToken = default);
    Task<AttendanceRuleDto?> UpdateAsync(Guid id, UpdateAttendanceRuleDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
