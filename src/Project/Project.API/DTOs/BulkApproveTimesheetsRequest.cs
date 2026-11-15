namespace Project.API.DTOs;

public sealed class BulkApproveTimesheetsRequest
{
    public List<Guid> TimesheetIds { get; set; } = [];

    public Guid ApprovedBy { get; set; }
}
