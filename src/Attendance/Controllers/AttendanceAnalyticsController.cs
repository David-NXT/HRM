using Attendance.DTOs.Analytics;
using Attendance.Services;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Controllers;

[ApiController]
[Route("api/attendance/analytics")]
public class AttendanceAnalyticsController : ControllerBase
{
    private readonly IAttendanceAnalyticsService _analyticsService;

    public AttendanceAnalyticsController(IAttendanceAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("overview")]
    public async Task<ActionResult<AttendanceOverviewStatsDto>> GetOverviewStats(
        [FromQuery] DateOnly? date,
        CancellationToken cancellationToken)
    {
        var result = await _analyticsService.GetOverviewStatsAsync(date, cancellationToken);
        return Ok(result);
    }

    [HttpGet("trends")]
    public async Task<ActionResult<List<AttendanceWeeklyTrendDto>>> GetWeeklyTrend(
        [FromQuery] int? month,
        [FromQuery] int? year,
        CancellationToken cancellationToken)
    {
        var m = month ?? DateTime.Today.Month;
        var y = year ?? DateTime.Today.Year;
        var result = await _analyticsService.GetWeeklyTrendAsync(m, y, cancellationToken);
        return Ok(result);
    }

    [HttpGet("departments")]
    public async Task<ActionResult<List<AttendanceDepartmentTrendDto>>> GetDepartmentTrend(
        [FromQuery] int? month,
        [FromQuery] int? year,
        CancellationToken cancellationToken)
    {
        var m = month ?? DateTime.Today.Month;
        var y = year ?? DateTime.Today.Year;
        var result = await _analyticsService.GetDepartmentTrendAsync(m, y, cancellationToken);
        return Ok(result);
    }
}
