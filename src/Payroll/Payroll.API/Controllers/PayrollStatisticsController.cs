using Microsoft.AspNetCore.Mvc;
using Payroll.Application.DTOs.PayrollStatistics;
using Payroll.Application.Services.PayrollStatistics;

namespace Payroll.API.Controllers;

[ApiController]
[Route("api/payroll-statistics")]
public class PayrollStatisticsController : ControllerBase
{
    private readonly PayrollStatisticsService _service;

    public PayrollStatisticsController(PayrollStatisticsService service)
    {
        _service = service;
    }

    [HttpGet("periods/{periodID:guid}")]
    public async Task<ActionResult<PayrollPeriodSalaryStatisticsResponse>> GetPeriod(
        Guid periodID,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetPeriodStatisticsAsync(
            periodID,
            cancellationToken);

        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("periods")]
    public async Task<ActionResult<List<PayrollPeriodSalaryStatisticsResponse>>> GetAllPeriods(
        [FromQuery] int? year,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetAllPeriodStatisticsAsync(
            year,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("periods/{year:int}/{month:int}")]
    public async Task<ActionResult<PayrollPeriodSalaryStatisticsResponse>> GetPeriodByYearMonth(
        int year,
        int month,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetPeriodStatisticsAsync(
            year,
            month,
            cancellationToken);

        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("quarters/{year:int}")]
    public async Task<ActionResult<List<PayrollQuarterSalaryStatisticsResponse>>> GetQuarters(
        int year,
        [FromQuery] int? quarter,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetQuarterStatisticsAsync(
            year,
            quarter,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("employees/{employeeID:guid}/quarters/{year:int}")]
    public async Task<ActionResult<List<EmployeeQuarterSalaryStatisticsResponse>>> GetEmployeeQuarters(
        Guid employeeID,
        int year,
        [FromQuery] int? quarter,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetEmployeeQuarterStatisticsAsync(
            employeeID,
            year,
            quarter,
            cancellationToken);

        return Ok(result);
    }
}
