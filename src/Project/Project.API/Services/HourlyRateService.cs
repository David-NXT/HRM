using Microsoft.EntityFrameworkCore;
using Project.API.Data;
using Project.API.Entities;

namespace Project.API.Services;

public class HourlyRateService : IHourlyRateService
{
    private readonly ProjectManagementDbContext _context;

    public HourlyRateService(ProjectManagementDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<HourlyRate>> GetHourlyRatesAsync(Guid? projectMemberId = null)
    {
        var query = _context.HourlyRates.AsNoTracking();

        if (projectMemberId.HasValue)
            query = query.Where(r => r.ProjectMemberId == projectMemberId.Value);

        return await query.ToListAsync();
    }

    public async Task<HourlyRate?> GetHourlyRateAsync(Guid id)
    {
        return await _context.HourlyRates
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.HourlyRateId == id);
    }

    public async Task<HourlyRate> CreateHourlyRateAsync(HourlyRate input)
    {
        input.HourlyRateId = Guid.NewGuid();

        _context.HourlyRates.Add(input);
        await _context.SaveChangesAsync();

        return input;
    }

    public async Task<bool> UpdateHourlyRateAsync(Guid id, HourlyRate input)
    {
        var rate = await _context.HourlyRates.FindAsync(id);

        if (rate == null)
            return false;

        rate.ProjectMemberId = input.ProjectMemberId;
        rate.Rate = input.Rate;
        rate.EffectiveFrom = input.EffectiveFrom;
        rate.EffectiveTo = input.EffectiveTo;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteHourlyRateAsync(Guid id)
    {
        var rate = await _context.HourlyRates.FindAsync(id);

        if (rate == null)
            return false;

        _context.HourlyRates.Remove(rate);
        await _context.SaveChangesAsync();

        return true;
    }
}