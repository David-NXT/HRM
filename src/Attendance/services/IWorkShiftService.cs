using Attendance.DTOs.WorkShift;

namespace Attendance.Services;

public interface IWorkShiftService
{
    Task<List<WorkShiftDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<WorkShiftDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkShiftDto> CreateAsync(CreateWorkShiftDto dto, CancellationToken cancellationToken = default);
    Task<WorkShiftDto?> UpdateAsync(Guid id, UpdateWorkShiftDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
