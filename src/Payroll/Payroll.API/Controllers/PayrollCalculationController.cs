using Microsoft.AspNetCore.Mvc;
using Payroll.Application.DTOs.PayrollCalculation;
using Payroll.Application.Services.PayrollCalculation;

namespace Payroll.API.Controllers;

[ApiController]
[Route("api/payroll-calculation")]
public class PayrollCalculationController : ControllerBase
{
    private readonly PayrollCalculationService _service;

    public PayrollCalculationController(PayrollCalculationService service)
    {
        _service = service;
    }

    [HttpPost("{payrollID:guid}/calculate")]
    public async Task<ActionResult<CalculatePayrollResponse>> Calculate(
        Guid payrollID,
        CancellationToken cancellationToken)
    {
        var response = await _service.CalculateAsync(
            payrollID,
            cancellationToken
        );

        return Ok(response);
    }
}