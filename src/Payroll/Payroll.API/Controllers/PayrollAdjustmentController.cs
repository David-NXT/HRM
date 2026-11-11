using Microsoft.AspNetCore.Mvc;
using Payroll.Application.DTOs.PayrollAdjustment;
using Payroll.Application.Services.PayrollAdjustment;

namespace Payroll.API.Controllers;

[ApiController]
[Route("api/payroll-adjustments")]
public class PayrollAdjustmentController : ControllerBase
{
    private readonly PayrollAdjustmentService _service;

    public PayrollAdjustmentController(
        PayrollAdjustmentService service)
    {
        _service = service;
    }

    [HttpPost("{payrollID:guid}/allowances")]
    public async Task<ActionResult<CreateAllowanceResponse>> AddAllowance(
        Guid payrollID,
        CreateAllowanceRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _service.AddAllowanceAsync(
            payrollID,
            request,
            cancellationToken
        );

        return StatusCode(201, response);
    }

    [HttpPost("{payrollID:guid}/paycuts")]
    public async Task<ActionResult<CreatePayCutResponse>> AddPayCut(
        Guid payrollID,
        CreatePayCutRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _service.AddPayCutAsync(
            payrollID,
            request,
            cancellationToken
        );

        return StatusCode(201, response);
    }
}
