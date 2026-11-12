using Microsoft.AspNetCore.Mvc;
using Payroll.Application.DTOs.Payslip;
using Payroll.Application.Services.Payslip;

namespace Payroll.API.Controllers;

[ApiController]
[Route("api/payslips")]
public class PayslipController : ControllerBase
{
    private readonly PayslipService _payslipService;

    public PayslipController(PayslipService payslipService)
    {
        _payslipService = payslipService;
    }

    // Get payslip by Payroll ID
    [HttpGet("{payrollID:guid}")]
    public async Task<ActionResult<PayslipResponse>> GetByPayrollID(
        Guid payrollID,
        CancellationToken cancellationToken)
    {
        var response = await _payslipService.GetByPayrollIDAsync(
            payrollID,
            cancellationToken);

        if (response == null)
            return NotFound("Payroll does not exist.");

        return Ok(response);
    }

    // Get all payslips in a period
    [HttpGet("periods/{periodID:guid}")]
    public async Task<ActionResult<List<PayslipResponse>>> GetByPeriod(
        Guid periodID,
        CancellationToken cancellationToken)
    {
        var response = await _payslipService.GetByPeriodAsync(
            periodID,
            cancellationToken);

        return Ok(response);
    }
}