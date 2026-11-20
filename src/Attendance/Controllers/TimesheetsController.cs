using Attendance.DTOs.TimesheetPeriod;
using Attendance.DTOs.TimesheetSummary;
using Attendance.Services;
using Microsoft.AspNetCore.Mvc;

namespace Attendance.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimesheetsController : ControllerBase
{
    private readonly ITimesheetService _timesheetService;

    public TimesheetsController(ITimesheetService timesheetService)
    {
        _timesheetService = timesheetService;
    }

    /// <summary>
    /// Lấy danh sách các kỳ công (Timesheet Periods)
    /// </summary>
    [HttpGet("periods")]
    public async Task<ActionResult<List<TimesheetPeriodDto>>> GetPeriods(
        [FromQuery] int? year, 
        CancellationToken cancellationToken)
    {
        List<TimesheetPeriodDto> periods = await _timesheetService.GetPeriodsAsync(year, cancellationToken);
        return Ok(periods);
    }

    /// <summary>
    /// Lấy thông tin chi tiết của một kỳ công theo Id
    /// </summary>
    [HttpGet("periods/{periodId:guid}")]
    public async Task<ActionResult<TimesheetPeriodDto>> GetPeriodById(
        Guid periodId, 
        CancellationToken cancellationToken)
    {
        TimesheetPeriodDto? period = await _timesheetService.GetPeriodByIdAsync(periodId, cancellationToken);
        if (period == null)
        {
            return NotFound($"Không tìm thấy kỳ công với Id: {periodId}");
        }

        return Ok(period);
    }

    /// <summary>
    /// Tạo kỳ công mới
    /// </summary>
    [HttpPost("periods")]
    public async Task<ActionResult<TimesheetPeriodDto>> CreatePeriod(
        [FromBody] CreateTimesheetPeriodDto dto, 
        CancellationToken cancellationToken)
    {
        try
        {
            TimesheetPeriodDto result = await _timesheetService.CreatePeriodAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetPeriodById), new { periodId = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Khóa kỳ công (Chốt sổ công để gửi sang bảng lương)
    /// </summary>
    [HttpPut("periods/{periodId:guid}/lock")]
    public async Task<ActionResult<TimesheetPeriodDto>> LockPeriod(
        Guid periodId, 
        [FromQuery] string? lockedBy, 
        CancellationToken cancellationToken)
    {
        TimesheetPeriodDto? result = await _timesheetService.LockPeriodAsync(periodId, lockedBy ?? "HR Manager", cancellationToken);
        if (result == null)
        {
            return NotFound($"Không tìm thấy kỳ công với Id: {periodId}");
        }

        return Ok(result);
    }

    /// <summary>
    /// Lấy toàn bộ danh sách tổng hợp công của tất cả nhân viên trong kỳ công
    /// </summary>
    [HttpGet("periods/{periodId:guid}/summaries")]
    public async Task<ActionResult<List<TimesheetSummaryDto>>> GetSummariesByPeriod(
        Guid periodId, 
        CancellationToken cancellationToken)
    {
        List<TimesheetSummaryDto> summaries = await _timesheetService.GetSummariesByPeriodAsync(periodId, cancellationToken);
        return Ok(summaries);
    }

    /// <summary>
    /// Lấy bảng công của 1 nhân viên theo tháng và năm
    /// VD: /api/timesheets/employee-summary?employeeId=...&month=11&year=2026
    /// </summary>
    [HttpGet("employee-summary")]
    public async Task<ActionResult<EmployeeMonthlyTimesheetDto>> GetEmployeeSummary(
        [FromQuery] Guid employeeId, 
        [FromQuery] int month, 
        [FromQuery] int year, 
        CancellationToken cancellationToken)
    {
        if (employeeId == Guid.Empty || month < 1 || month > 12 || year < 2000)
        {
            return BadRequest(new { message = "Thông số employeeId, month (1-12) hoặc year không hợp lệ." });
        }

        EmployeeMonthlyTimesheetDto? result = await _timesheetService.GetEmployeeSummaryAsync(employeeId, month, year, cancellationToken);
        if (result == null)
        {
            return NotFound(new { message = $"Chưa có bảng công cho nhân viên trong tháng {month}/{year}." });
        }

        return Ok(result);
    }

    /// <summary>
    /// Lấy bảng công của 1 nhân viên theo kỳ công cụ thể
    /// </summary>
    [HttpGet("periods/{periodId:guid}/employees/{employeeId:guid}")]
    public async Task<ActionResult<EmployeeMonthlyTimesheetDto>> GetEmployeeSummaryByPeriod(
        Guid periodId, 
        Guid employeeId, 
        CancellationToken cancellationToken)
    {
        EmployeeMonthlyTimesheetDto? result = await _timesheetService.GetEmployeeSummaryByPeriodAsync(periodId, employeeId, cancellationToken);
        if (result == null)
        {
            return NotFound(new { message = "Không tìm thấy bảng công của nhân viên trong kỳ này." });
        }

        return Ok(result);
    }

    /// <summary>
    /// Tự động quét dữ liệu chấm công (AttendanceRecords, LeaveRequests, OTRequests) để tổng hợp công tháng cho nhân viên
    /// </summary>
    [HttpPost("periods/{periodId:guid}/employees/{employeeId:guid}/calculate")]
    public async Task<ActionResult<TimesheetSummaryDto>> CalculateSummary(
        Guid periodId, 
        Guid employeeId, 
        CancellationToken cancellationToken)
    {
        try
        {
            TimesheetSummaryDto result = await _timesheetService.GenerateOrRecalculateSummaryAsync(periodId, employeeId, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
