using Microsoft.AspNetCore.Mvc;
using Payroll.Application.DTOs.PayrollPeriodManagement;
using Payroll.Application.Services.PayrollPeriodManagement;

namespace Payroll.API.Controllers;

[ApiController]
[Route("api/payroll-period-management")]
public class PayrollPeriodManagementController : ControllerBase
{
    private readonly PayrollPeriodManagementService _service;

    public PayrollPeriodManagementController(
        PayrollPeriodManagementService service)
    {
        _service = service;
    }

    [HttpPost("{payrollPeriodID:guid}/update-status")]
    public async Task<ActionResult<UpdatePayrollPeriodStatusResponse>> UpdateStatus(
        Guid payrollPeriodID,
        CancellationToken cancellationToken)
    {
        var response = await _service.UpdateStatusAsync(
            payrollPeriodID,
            cancellationToken
        );

        return Ok(response);
    }
}