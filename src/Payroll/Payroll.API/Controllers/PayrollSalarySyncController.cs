using Microsoft.AspNetCore.Mvc;
using Payroll.Application.Services.PayrollSalarySync;

namespace Payroll.API.Controllers;

[ApiController]
[Route("api/payroll-salary")]
public class PayrollSalarySyncController : ControllerBase
{
    private readonly PayrollSalarySyncService _service;

    public PayrollSalarySyncController(
        PayrollSalarySyncService service)
    {
        _service = service;
    }

    [HttpPost("periods/{payrollPeriodID:guid}/sync")]
    public async Task<ActionResult<int>> SyncSalary(
        Guid payrollPeriodID,
        CancellationToken cancellationToken)
    {
        var syncedCount = await _service.SyncSalaryAsync(
            payrollPeriodID,
            cancellationToken);

        return Ok(syncedCount);
    }
}