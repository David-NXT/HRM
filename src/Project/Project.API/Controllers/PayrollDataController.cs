
using Microsoft.AspNetCore.Mvc;
using Project.API.DTOs;
using Project.API.Services;

namespace Project.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayrollDataController : ControllerBase
{
    private readonly IPayrollDataService _payrollDataService;

    public PayrollDataController(
        IPayrollDataService payrollDataService)
    {
        _payrollDataService = payrollDataService;
    }

    // GET: api/PayrollData?fromDate=2026-11-01&toDate=2026-11-30
    // Lấy chi tiết các Timesheet đã duyệt
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PayrollDataDto>>>
        GetPayrollData(
            [FromQuery] DateOnly? fromDate,
            [FromQuery] DateOnly? toDate)
    {
        if (!TryBuildPeriod(
            fromDate, toDate,
            out var fromUtc, out var toExclusiveUtc))
        {
            return BadRequest(
                "Please provide valid fromDate and toDate.");
        }

        try
        {
            var result =
                await _payrollDataService.GetApprovedPayrollDataAsync(
                    fromUtc, toExclusiveUtc);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // GET: api/PayrollData/summary?fromDate=2026-11-01&toDate=2026-11-30
    // Tổng hợp phụ cấp theo nhân viên và dự án
    [HttpGet("summary")]
    public async Task<ActionResult<IEnumerable<PayrollProjectSummaryDto>>>
        GetPayrollSummary(
            [FromQuery] DateOnly? fromDate,
            [FromQuery] DateOnly? toDate)
    {
        if (!TryBuildPeriod(
            fromDate, toDate,
            out var fromUtc, out var toExclusiveUtc))
        {
            return BadRequest(
                "Please provide valid fromDate and toDate.");
        }

        try
        {
            var result =
                await _payrollDataService.GetProjectAllowanceSummaryAsync(
                    fromUtc,
                    toExclusiveUtc,
                    fromUtc,
                    toExclusiveUtc.AddDays(-1));

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // GET: api/PayrollData/employee/{employeeId}?fromDate=...&toDate=...
    // Lấy dữ liệu chi tiết theo nhân viên
    [HttpGet("employee/{employeeId:guid}")]
    public async Task<ActionResult<IEnumerable<PayrollDataDto>>>
        GetPayrollDataByEmployee(
            Guid employeeId,
            [FromQuery] DateOnly? fromDate,
            [FromQuery] DateOnly? toDate)
    {
        if (!TryBuildPeriod(
            fromDate, toDate,
            out var fromUtc, out var toExclusiveUtc))
        {
            return BadRequest(
                "Please provide valid fromDate and toDate.");
        }

        try
        {
            var result =
                await _payrollDataService.GetApprovedPayrollDataAsync(
                    fromUtc, toExclusiveUtc, employeeId);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // GET: api/PayrollData/employee/{employeeId}/summary?fromDate=...&toDate=...
    // Tổng hợp phụ cấp theo nhân viên
    [HttpGet("employee/{employeeId:guid}/summary")]
    public async Task<ActionResult<IEnumerable<PayrollProjectSummaryDto>>>
        GetPayrollSummaryByEmployee(
            Guid employeeId,
            [FromQuery] DateOnly? fromDate,
            [FromQuery] DateOnly? toDate)
    {
        if (!TryBuildPeriod(
            fromDate, toDate,
            out var fromUtc, out var toExclusiveUtc))
        {
            return BadRequest(
                "Please provide valid fromDate and toDate.");
        }

        try
        {
            var result =
                await _payrollDataService.GetProjectAllowanceSummaryAsync(
                    fromUtc,
                    toExclusiveUtc,
                    fromUtc,
                    toExclusiveUtc.AddDays(-1),
                    employeeId);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    private static bool TryBuildPeriod(
    DateOnly? fromDate,
    DateOnly? toDate,
    out DateTime fromUtc,
    out DateTime toExclusiveUtc)
{
    fromUtc = default;
    toExclusiveUtc = default;

    if (!fromDate.HasValue || !toDate.HasValue)
        return false;

    if (fromDate.Value > toDate.Value)
        return false;

    // 00:00 giờ Việt Nam (UTC+7) = 17:00 UTC ngày hôm trước
    fromUtc = DateTime.SpecifyKind(
        fromDate.Value
            .ToDateTime(TimeOnly.MinValue)
            .AddHours(-7),
        DateTimeKind.Utc);

    // Mốc kết thúc độc quyền: 00:00 ngày tiếp theo theo giờ Việt Nam
    toExclusiveUtc = DateTime.SpecifyKind(
        toDate.Value
            .AddDays(1)
            .ToDateTime(TimeOnly.MinValue)
            .AddHours(-7),
        DateTimeKind.Utc);

    return true;
}
}
