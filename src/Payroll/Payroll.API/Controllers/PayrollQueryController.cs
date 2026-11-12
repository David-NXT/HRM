using Microsoft.AspNetCore.Mvc;
using Payroll.Application.DTOs.PayrollQuery;
using Payroll.Application.Services.PayrollQuery;

namespace Payroll.API.Controllers;

[ApiController]
[Route("api/payrolls")]
public class PayrollQueryController : ControllerBase
{
    private readonly PayrollQueryService _service;

    public PayrollQueryController(PayrollQueryService service)
    {
        _service = service;
    }

    [HttpGet("periods/{periodID:guid}")]
    public async Task<ActionResult<List<PayrollListResponse>>> GetByPeriod(
        Guid periodID,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByPeriodAsync(
            periodID,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{payrollID:guid}")]
    public async Task<ActionResult<PayrollDetailResponse>> GetByID(
        Guid payrollID,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetByIDAsync(
            payrollID,
            cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("employees/{employeeID:guid}")]
    public async Task<ActionResult<List<PayrollHistoryResponse>>> GetEmployeeHistory(
        Guid employeeID,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetEmployeeHistoryAsync(
            employeeID,
            cancellationToken);

        return Ok(result);
    }
}