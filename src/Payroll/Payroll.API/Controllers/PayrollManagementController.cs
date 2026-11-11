using Microsoft.AspNetCore.Mvc;
using Payroll.Application.DTOs.PayrollManagement;
using Payroll.Application.Services.PayrollManagement;

namespace Payroll.API.Controllers;

[ApiController]
[Route("api/payroll-management")]
public class PayrollManagementController : ControllerBase
{
    private readonly PayrollManagementService _service;

    public PayrollManagementController(
        PayrollManagementService service)
    {
        _service = service;
    }

    [HttpPost("periods/{payrollPeriodID:guid}/payrolls")]
    public async Task<ActionResult<CreatePayrollsResponse>> CreatePayrolls(
        Guid payrollPeriodID,
        CreatePayrollsRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _service.CreatePayrollsAsync(
            payrollPeriodID,
            request,
            cancellationToken
        );

        return StatusCode(201, response);
    }
}
