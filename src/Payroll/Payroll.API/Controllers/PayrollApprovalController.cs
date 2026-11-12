using Microsoft.AspNetCore.Mvc;
using Payroll.Application.DTOs.PayrollApproval;
using Payroll.Application.Services.PayrollApproval;

namespace Payroll.API.Controllers;

[ApiController]
[Route("api/payroll-approval")]
public class PayrollApprovalController : ControllerBase
{
    private readonly PayrollApprovalService _service;

    public PayrollApprovalController(PayrollApprovalService service)
    {
        _service = service;
    }

    [HttpPost("{payrollID:guid}/approve")]
    public async Task<ActionResult<ApprovePayrollResponse>> Approve(
        Guid payrollID,
        CancellationToken cancellationToken)
    {
        var response = await _service.ApproveAsync(
            payrollID,
            cancellationToken
        );

        return Ok(response);
    }
}