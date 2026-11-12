using Microsoft.AspNetCore.Mvc;
using Payroll.Application.Services.PayrollAttendance;

namespace Payroll.API.Controllers;

[ApiController]
[Route("api/payroll-attendance")]
public class PayrollAttendanceController : ControllerBase
{
    private readonly PayrollAttendanceService _service;

    public PayrollAttendanceController(
        PayrollAttendanceService service)
    {
        _service = service;
    }

    [HttpPost("periods/{payrollPeriodID:guid}/sync")]
    public async Task<ActionResult<int>> SyncAttendance(
        Guid payrollPeriodID,
        CancellationToken cancellationToken)
    {
        var syncedCount = await _service.SyncAttendanceAsync(
            payrollPeriodID,
            cancellationToken
        );

        return Ok(syncedCount);
    }
}