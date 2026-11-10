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

    [HttpPost("{payrollID:int}/allowances")]
    public async Task<ActionResult<CreateAllowanceResponse>> AddAllowance(
        int payrollID,
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

    [HttpPost("{payrollID:int}/paycuts")]
    public async Task<ActionResult<CreatePayCutResponse>> AddPayCut(
        int payrollID,
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