using Microsoft.AspNetCore.Mvc;
using Payroll.Application.DTOs.PayrollPeriod;
using Payroll.Application.Services.PayrollPeriod;

namespace Payroll.API.Controllers;

[ApiController]
[Route("api/payroll-periods")]
public class PayrollPeriodController : ControllerBase
{
    private readonly PayrollPeriodService _payrollPeriodService;

    public PayrollPeriodController(
        PayrollPeriodService payrollPeriodService)
    {
        _payrollPeriodService = payrollPeriodService;
    }

    [HttpPost]
    public async Task<ActionResult<PayrollPeriodResponse>> Create(
        CreatePayrollPeriodRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _payrollPeriodService.CreateAsync(
            request,
            cancellationToken
        );
    
        return StatusCode(201, response);
    }
}