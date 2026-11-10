using Attendance.DTOs.ShiftAssignment;

namespace Attendance.Services;

public interface IShiftAssignmentService
{
    Task<List<ShiftAssignmentDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ShiftAssignmentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ShiftAssignmentDto> CreateAsync(CreateShiftAssignmentDto dto, CancellationToken cancellationToken = default);
    Task<ShiftAssignmentDto?> UpdateAsync(Guid id, UpdateShiftAssignmentDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
