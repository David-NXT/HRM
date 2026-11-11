using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payroll.Infrastructure.Persistence;

namespace Payroll.API.Controllers;

[ApiController]
[Route("health")]
public class DatabaseHealthControllers : ControllerBase
{
    private readonly PayrollDbContext _context;

    public DatabaseHealthControllers(PayrollDbContext context)
    {
        _context = context;
    }

    [HttpGet("database")]
    public async Task<IActionResult> CheckDataBase()
    {
        var canConnect = await _context.Database.CanConnectAsync();

        if (!canConnect)
        {
            return StatusCode(503, new
            {
                Database = "disconnected"
            });
        }

        return Ok(new
        {
            Database = "connected"
        });
    }

    [HttpPost("seed")]
    public async Task<IActionResult> SeedSampleData()
    {
        await PayrollDataSeeder.SeedAsync(_context);

        return Ok(new
        {
            Message = "Sample data seeded successfully.",
            PeriodsCount = await _context.PayrollPeriods.CountAsync(),
            PayrollsCount = await _context.Payrolls.CountAsync(),
            AllowanceTypesCount = await _context.AllowanceTypes.CountAsync(),
            PayCutReasonsCount = await _context.PayCutReasons.CountAsync()
        });
    }
}
