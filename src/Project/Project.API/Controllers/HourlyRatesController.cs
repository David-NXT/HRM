
using Microsoft.AspNetCore.Mvc;
using Project.API.Entities;
using Project.API.Services;

namespace Project.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HourlyRatesController : ControllerBase
{
    private readonly IHourlyRateService _hourlyRateService;

    public HourlyRatesController(
        IHourlyRateService hourlyRateService)
    {
        _hourlyRateService = hourlyRateService;
    }

    // GET: api/HourlyRates?projectMemberId={guid}
    [HttpGet]
    public async Task<IActionResult> GetHourlyRates(
        [FromQuery] Guid? projectMemberId = null)
    {
        var rates = await _hourlyRateService.GetHourlyRatesAsync(
            projectMemberId);

        return Ok(rates);
    }

    // GET: api/HourlyRates/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetHourlyRate(Guid id)
    {
        var rate = await _hourlyRateService.GetHourlyRateAsync(id);

        if (rate == null)
            return NotFound();

        return Ok(rate);
    }

    // POST: api/HourlyRates
    [HttpPost]
    public async Task<IActionResult> CreateHourlyRate(
        [FromBody] HourlyRate dto)
    {
        try
        {
            var rate = await _hourlyRateService.CreateHourlyRateAsync(dto);

            return CreatedAtAction(
                nameof(GetHourlyRate),
                new { id = rate.HourlyRateId },
                rate);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // PUT: api/HourlyRates/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateHourlyRate(
        Guid id,
        [FromBody] HourlyRate dto)
    {
        try
        {
            var result = await _hourlyRateService.UpdateHourlyRateAsync(
                id, dto);

            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // DELETE: api/HourlyRates/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteHourlyRate(Guid id)
    {
        try
        {
            var result = await _hourlyRateService.DeleteHourlyRateAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
