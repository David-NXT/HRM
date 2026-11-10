using Attendance.DTOs.OvertimeRule;

namespace Attendance.Services;

public interface IOvertimeRuleService
{
    Task<List<OvertimeRuleDto>> GetAllAsync(bool? isActive = null, CancellationToken cancellationToken = default);
    Task<OvertimeRuleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OvertimeRuleDto?> GetByDayTypeAsync(string dayType, CancellationToken cancellationToken = default);
    Task<OvertimeRuleDto> CreateAsync(CreateOvertimeRuleDto dto, CancellationToken cancellationToken = default);
    Task<OvertimeRuleDto?> UpdateAsync(Guid id, UpdateOvertimeRuleDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
