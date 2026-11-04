using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.API.Data;
using Project.API.Entities;

namespace Project.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HourlyRatesController : ControllerBase
{
    private readonly ProjectManagementDbContext _context;

    public HourlyRatesController(ProjectManagementDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetHourlyRates()
    {
        var rates = await _context.HourlyRates
            .Select(r => new
            {
                r.HourlyRateId,
                r.ProjectMemberId,
                r.Rate,
                r.EffectiveFrom,
                r.EffectiveTo
            })
            .ToListAsync();

        return Ok(rates);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetHourlyRate(Guid id)
    {
        var rate = await _context.HourlyRates
            .Where(r => r.HourlyRateId == id)
            .Select(r => new
            {
                r.HourlyRateId,
                r.ProjectMemberId,
                r.Rate,
                r.EffectiveFrom,
                r.EffectiveTo
            })
            .FirstOrDefaultAsync();

        if (rate == null)
            return NotFound();

        return Ok(rate);
    }

    [HttpPost]
    public async Task<IActionResult> CreateHourlyRate(
        [FromBody] HourlyRate dto)
    {
        var memberExists = await _context.ProjectMembers
            .AnyAsync(m => m.ProjectMemberId == dto.ProjectMemberId);

        if (!memberExists)
            return BadRequest("Project member does not exist.");

        if (dto.Rate < 0)
            return BadRequest("Rate cannot be negative.");

        var rate = new HourlyRate
        {
            HourlyRateId = Guid.NewGuid(),
            ProjectMemberId = dto.ProjectMemberId,
            Rate = dto.Rate,
            EffectiveFrom = dto.EffectiveFrom,
            EffectiveTo = dto.EffectiveTo
        };

        _context.HourlyRates.Add(rate);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetHourlyRate),
            new { id = rate.HourlyRateId },
            new
            {
                rate.HourlyRateId,
                rate.ProjectMemberId,
                rate.Rate,
                rate.EffectiveFrom,
                rate.EffectiveTo
            });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateHourlyRate(
        Guid id, [FromBody] HourlyRate dto)
    {
        var rate = await _context.HourlyRates.FindAsync(id);

        if (rate == null)
            return NotFound();

        var memberExists = await _context.ProjectMembers
            .AnyAsync(m => m.ProjectMemberId == dto.ProjectMemberId);

        if (!memberExists)
            return BadRequest("Project member does not exist.");

        if (dto.Rate < 0)
            return BadRequest("Rate cannot be negative.");

        rate.ProjectMemberId = dto.ProjectMemberId;
        rate.Rate = dto.Rate;
        rate.EffectiveFrom = dto.EffectiveFrom;
        rate.EffectiveTo = dto.EffectiveTo;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteHourlyRate(Guid id)
    {
        var rate = await _context.HourlyRates.FindAsync(id);

        if (rate == null)
            return NotFound();

        _context.HourlyRates.Remove(rate);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}