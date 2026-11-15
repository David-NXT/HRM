
using Project.API.DTOs;

namespace Project.API.Services;

public interface ITimesheetService
{
    Task<IEnumerable<TimesheetResponseDto>> GetTimesheetsAsync(
        Guid? projectMemberId = null,
        Guid? projectId = null,
        Guid? employeeId = null,
        DateTime? fromUtc = null,
        DateTime? toExclusiveUtc = null,
        string? status = null);

    Task<TimesheetResponseDto?> GetTimesheetAsync(Guid id);

    Task<TimesheetResponseDto> CreateTimesheetAsync(
        CreateTimesheetDto dto);
    Task<bool> UpdateTimesheetAsync(
    Guid id,
    UpdateTimesheetDto dto);

    Task<bool> ApproveTimesheetAsync(
        Guid id,
        Guid approvedBy);

    Task<BulkApproveTimesheetsResponse> ApproveTimesheetsAsync(
        IReadOnlyCollection<Guid> timesheetIds,
        Guid approvedBy);

    Task<bool> RejectTimesheetAsync(
        Guid id,
        Guid approvedBy,
        string reason);

    Task<bool> ResubmitTimesheetAsync(
        Guid id,
        decimal hoursWorked,
        string? description);

    Task<bool> DeleteTimesheetAsync(Guid id);
}
