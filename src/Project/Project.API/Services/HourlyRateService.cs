
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

    // Lấy danh sách đơn giá giờ
    public async Task<IEnumerable<HourlyRate>> GetHourlyRatesAsync(
        Guid? projectMemberId = null)
    {
        var query = _context.HourlyRates
            .AsNoTracking();

        if (projectMemberId.HasValue)
        {
            query = query.Where(r =>
                r.ProjectMemberId == projectMemberId.Value);
        }

        return await query
            .OrderBy(r => r.ProjectMemberId)
            .ThenByDescending(r => r.EffectiveFrom)
            .ToListAsync();
    }

    // Lấy đơn giá giờ theo ID
    public async Task<HourlyRate?> GetHourlyRateAsync(Guid id)
    {
        return await _context.HourlyRates
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.HourlyRateId == id);
    }

    // Tạo đơn giá giờ
    public async Task<HourlyRate> CreateHourlyRateAsync(
        HourlyRate input)
    {
        input.EffectiveFrom = DateTimeNormalizer.ToUtc(input.EffectiveFrom);
        input.EffectiveTo = input.EffectiveTo.HasValue
            ? DateTimeNormalizer.ToUtc(input.EffectiveTo.Value)
            : null;
        ValidateHourlyRate(input);

        var memberExists = await _context.ProjectMembers
            .AnyAsync(m =>
                m.ProjectMemberId == input.ProjectMemberId);

        if (!memberExists)
        {
            throw new ArgumentException(
                "Project member does not exist.");
        }

        var hasOverlap = await HasOverlappingRateAsync(
            input.ProjectMemberId,
            input.EffectiveFrom,
            input.EffectiveTo);

        if (hasOverlap)
        {
            throw new InvalidOperationException(
                "Hourly rate period overlaps with an existing rate.");
        }

        input.HourlyRateId = Guid.NewGuid();

        _context.HourlyRates.Add(input);
        await _context.SaveChangesAsync();

        return input;
    }

    // Cập nhật đơn giá giờ
    
public async Task<bool> UpdateHourlyRateAsync(
    Guid id,
    HourlyRate input)
{
    var rate = await _context.HourlyRates.FindAsync(id);

    if (rate == null)
        return false;

    input.EffectiveFrom = DateTimeNormalizer.ToUtc(input.EffectiveFrom);
    input.EffectiveTo = input.EffectiveTo.HasValue
        ? DateTimeNormalizer.ToUtc(input.EffectiveTo.Value)
        : null;
    ValidateHourlyRate(input);

    var memberExists = await _context.ProjectMembers
        .AnyAsync(m =>
            m.ProjectMemberId == input.ProjectMemberId);

    if (!memberExists)
        throw new ArgumentException(
            "Project member does not exist.");

    var isUsed = await _context.Timesheets
        .AnyAsync(t =>
            t.ProjectMemberId == rate.ProjectMemberId &&
            t.WorkDate.Date >= rate.EffectiveFrom.Date &&
            (rate.EffectiveTo == null ||
             t.WorkDate.Date <= rate.EffectiveTo.Value.Date));

    if (isUsed)
    {
        throw new InvalidOperationException(
            "Cannot update an hourly rate that has been used by timesheets. Create a new hourly rate instead.");
    }

    var hasOverlap = await HasOverlappingRateAsync(
        input.ProjectMemberId,
        input.EffectiveFrom,
        input.EffectiveTo,
        id);

    if (hasOverlap)
    {
        throw new InvalidOperationException(
            "Hourly rate period overlaps with an existing rate.");
    }

    rate.ProjectMemberId = input.ProjectMemberId;
    rate.Rate = input.Rate;
    rate.EffectiveFrom = input.EffectiveFrom;
    rate.EffectiveTo = input.EffectiveTo;

    await _context.SaveChangesAsync();

    return true;
}


    // Xóa đơn giá giờ
    public async Task<bool> DeleteHourlyRateAsync(Guid id)
    {
        var rate = await _context.HourlyRates.FindAsync(id);

        if (rate == null)
            return false;

        var hasTimesheet = await _context.Timesheets
            .AnyAsync(t =>
                t.ProjectMemberId == rate.ProjectMemberId &&
                t.WorkDate >= rate.EffectiveFrom &&
                (rate.EffectiveTo == null ||
                 t.WorkDate <= rate.EffectiveTo.Value));

        if (hasTimesheet)
        {
            throw new InvalidOperationException(
                "Cannot delete an hourly rate that applies to existing timesheets.");
        }

        _context.HourlyRates.Remove(rate);

        await _context.SaveChangesAsync();

        return true;
    }

    // Kiểm tra dữ liệu đơn giá
    private static void ValidateHourlyRate(HourlyRate input)
    {
        if (input.ProjectMemberId == Guid.Empty)
        {
            throw new ArgumentException(
                "ProjectMemberId is required.");
        }

        if (input.Rate <= 0)
        {
            throw new ArgumentException(
                "Hourly rate must be greater than 0.");
        }

        if (input.EffectiveTo.HasValue &&
            input.EffectiveTo.Value.Date < input.EffectiveFrom.Date)
        {
            throw new ArgumentException(
                "EffectiveTo must be greater than or equal to EffectiveFrom.");
        }
    }

    // Kiểm tra khoảng thời gian hiệu lực có bị chồng lấn không
    private async Task<bool> HasOverlappingRateAsync(
        Guid projectMemberId,
        DateTime effectiveFrom,
        DateTime? effectiveTo,
        Guid? excludeRateId = null)
    {
        var query = _context.HourlyRates
            .Where(r => r.ProjectMemberId == projectMemberId);

        if (excludeRateId.HasValue)
        {
            query = query.Where(r =>
                r.HourlyRateId != excludeRateId.Value);
        }

        return await query.AnyAsync(r =>
            (effectiveTo == null ||
             r.EffectiveFrom.Date <= effectiveTo.Value.Date) &&
            (r.EffectiveTo == null ||
             r.EffectiveTo.Value.Date >= effectiveFrom.Date));
    }
}
