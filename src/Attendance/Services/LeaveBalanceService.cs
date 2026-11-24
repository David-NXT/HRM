using Attendance.Data;
using Attendance.DTOs.LeaveBalance;
using Attendance.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Services;

public class LeaveBalanceService : ILeaveBalanceService
{
    private readonly AttendanceDbContext _dbContext;

    public LeaveBalanceService(AttendanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<LeaveBalanceDto>> GetAllAsync(int? year = null, CancellationToken cancellationToken = default)
    {
        IQueryable<LeaveBalance> query = _dbContext.LeaveBalances.AsNoTracking();
        if (year.HasValue)
        {
            query = query.Where(x => x.Year == year.Value);
        }

        List<LeaveBalance> list = await query.OrderBy(x => x.Year).ToListAsync(cancellationToken);
        return list.Select(MapToDto).ToList();
    }

    public async Task<LeaveBalanceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        LeaveBalance? balance = await _dbContext.LeaveBalances
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return balance == null ? null : MapToDto(balance);
    }

    public async Task<LeaveBalanceDto?> GetByEmployeeAsync(Guid employeeId, int year, CancellationToken cancellationToken = default)
    {
        LeaveBalance? balance = await _dbContext.LeaveBalances
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.Year == year, cancellationToken);

        return balance == null ? null : MapToDto(balance);
    }

    public async Task<LeaveBalanceDto> CreateAsync(CreateLeaveBalanceDto dto, CancellationToken cancellationToken = default)
    {
        bool existing = await _dbContext.LeaveBalances
            .AnyAsync(x => x.EmployeeId == dto.EmployeeId && x.Year == dto.Year, cancellationToken);

        if (existing)
        {
            throw new ArgumentException($"Dữ liệu ngày phép năm {dto.Year} của nhân viên đã tồn tại.");
        }

        LeaveBalance balance = new LeaveBalance
        {
            Id = Guid.NewGuid(),
            EmployeeId = dto.EmployeeId,
            Year = dto.Year,
            TotalDays = dto.TotalDays,
            UsedDays = dto.UsedDays,
            RemainingDays = dto.RemainingDays,
            CarriedOverDays = dto.CarriedOverDays,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.LeaveBalances.Add(balance);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(balance);
    }

    public async Task<LeaveBalanceDto?> UpdateAsync(Guid id, UpdateLeaveBalanceDto dto, CancellationToken cancellationToken = default)
    {
        LeaveBalance? balance = await _dbContext.LeaveBalances.FindAsync([id], cancellationToken);
        if (balance == null)
        {
            return null;
        }

        balance.TotalDays = dto.TotalDays;
        balance.UsedDays = dto.UsedDays;
        balance.RemainingDays = dto.RemainingDays;
        balance.CarriedOverDays = dto.CarriedOverDays;
        balance.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapToDto(balance);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        LeaveBalance? balance = await _dbContext.LeaveBalances.FindAsync([id], cancellationToken);
        if (balance == null)
        {
            return false;
        }

        _dbContext.LeaveBalances.Remove(balance);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static LeaveBalanceDto MapToDto(LeaveBalance model) => new()
    {
        Id = model.Id,
        EmployeeId = model.EmployeeId,
        Year = model.Year,
        TotalDays = model.TotalDays,
        UsedDays = model.UsedDays,
        RemainingDays = model.RemainingDays,
        CarriedOverDays = model.CarriedOverDays,
        CreatedAt = model.CreatedAt,
        UpdatedAt = model.UpdatedAt
    };
}
