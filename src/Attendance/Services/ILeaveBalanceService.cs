using Attendance.DTOs.LeaveBalance;

namespace Attendance.Services;

public interface ILeaveBalanceService
{
    Task<List<LeaveBalanceDto>> GetAllAsync(int? year = null, CancellationToken cancellationToken = default);
    Task<LeaveBalanceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<LeaveBalanceDto?> GetByEmployeeAsync(Guid employeeId, int year, CancellationToken cancellationToken = default);
    Task<LeaveBalanceDto> CreateAsync(CreateLeaveBalanceDto dto, CancellationToken cancellationToken = default);
    Task<LeaveBalanceDto?> UpdateAsync(Guid id, UpdateLeaveBalanceDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
