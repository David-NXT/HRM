
using Project.API.DTOs;

namespace Project.API.Services;

public interface ITimesheetService
{
    Task<IEnumerable<TimesheetResponseDto>> GetTimesheetsAsync(
        Guid? projectMemberId = null);

    Task<TimesheetResponseDto?> GetTimesheetAsync(Guid id);

    Task<TimesheetResponseDto> CreateTimesheetAsync(
        CreateTimesheetDto dto);
    Task<bool> UpdateTimesheetAsync(
    Guid id,
    UpdateTimesheetDto dto);

    Task<bool> ApproveTimesheetAsync(
        Guid id,
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
