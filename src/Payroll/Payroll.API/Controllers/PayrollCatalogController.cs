using Microsoft.AspNetCore.Mvc;
using Payroll.Application.DTOs.PayrollCatalog;
using Payroll.Application.Services.PayrollCatalog;

namespace Payroll.API.Controllers;

[ApiController]
[Route("api/payroll-catalog")]
public class PayrollCatalogController : ControllerBase
{
    private readonly PayrollCatalogService _service;

    public PayrollCatalogController(PayrollCatalogService service)
    {
        _service = service;
    }

    [HttpGet("allowances")]
    public async Task<ActionResult<List<PayrollCatalogItemResponse>>> GetAllowanceTypes(
        [FromQuery] bool activeOnly = false,
        CancellationToken cancellationToken = default) =>
        Ok(await _service.GetAllowanceTypesAsync(activeOnly, cancellationToken));

    [HttpPost("allowances")]
    public async Task<ActionResult<PayrollCatalogItemResponse>> CreateAllowanceType(
        SavePayrollCatalogItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _service.CreateAllowanceTypeAsync(request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, item);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails { Detail = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails { Detail = ex.Message });
        }
    }

    [HttpPut("allowances/{id:guid}")]
    public async Task<ActionResult<PayrollCatalogItemResponse>> UpdateAllowanceType(
        Guid id,
        SavePayrollCatalogItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _service.UpdateAllowanceTypeAsync(id, request, cancellationToken));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails { Detail = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails { Detail = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails { Detail = ex.Message });
        }
    }

    [HttpDelete("allowances/{id:guid}")]
    public async Task<IActionResult> DeactivateAllowanceType(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _service.DeactivateAllowanceTypeAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails { Detail = ex.Message });
        }
    }

    [HttpGet("pay-cuts")]
    public async Task<ActionResult<List<PayrollCatalogItemResponse>>> GetPayCutReasons(
        [FromQuery] bool activeOnly = false,
        CancellationToken cancellationToken = default) =>
        Ok(await _service.GetPayCutReasonsAsync(activeOnly, cancellationToken));

    [HttpPost("pay-cuts")]
    public async Task<ActionResult<PayrollCatalogItemResponse>> CreatePayCutReason(
        SavePayrollCatalogItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _service.CreatePayCutReasonAsync(request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, item);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails { Detail = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails { Detail = ex.Message });
        }
    }

    [HttpPut("pay-cuts/{id:guid}")]
    public async Task<ActionResult<PayrollCatalogItemResponse>> UpdatePayCutReason(
        Guid id,
        SavePayrollCatalogItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _service.UpdatePayCutReasonAsync(id, request, cancellationToken));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails { Detail = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails { Detail = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails { Detail = ex.Message });
        }
    }

    [HttpDelete("pay-cuts/{id:guid}")]
    public async Task<IActionResult> DeactivatePayCutReason(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _service.DeactivatePayCutReasonAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails { Detail = ex.Message });
        }
    }
}
