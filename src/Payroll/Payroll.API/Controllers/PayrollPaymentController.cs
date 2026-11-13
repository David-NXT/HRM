using Microsoft.AspNetCore.Mvc;
using Payroll.Application.DTOs.PayrollPayment;
using Payroll.Application.Services.PayrollPayment;

namespace Payroll.API.Controllers;

[ApiController]
[Route("api/payroll-payment")]
public class PayrollPaymentController : ControllerBase
{
    private readonly PayrollPaymentService _service;

    public PayrollPaymentController(PayrollPaymentService service)
    {
        _service = service;
    }

    [HttpPost("{payrollID:guid}/mark-as-paid")]
    public async Task<ActionResult<MarkPayrollAsPaidResponse>> MarkAsPaid(
        Guid payrollID,
        CancellationToken cancellationToken)
    {
        var response = await _service.MarkAsPaidAsync(
            payrollID,
            cancellationToken
        );

        return Ok(response);
    }
}