namespace Project.API.DTOs;

public sealed class BulkApproveTimesheetsResponse
{
    public List<TimesheetReviewResultDto> Results { get; set; } = [];

    public int SucceededCount => Results.Count(result => result.Succeeded);

    public int FailedCount => Results.Count - SucceededCount;
}

public sealed class TimesheetReviewResultDto
{
    public Guid TimesheetId { get; set; }

    public bool Succeeded { get; set; }

    public string? Status { get; set; }

    public string? Message { get; set; }
}
