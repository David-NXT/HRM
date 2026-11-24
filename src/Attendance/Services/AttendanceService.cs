using Attendance.Data;
using Attendance.DTOs.AttendanceRecord;
using Attendance.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Services;

public class AttendanceService : IAttendanceService
{
    private readonly AttendanceDbContext _context;
    private readonly IHrEmployeeDirectoryClient _employees;

    public AttendanceService(AttendanceDbContext context, IHrEmployeeDirectoryClient employees)
    {
        _context = context;
        _employees = employees;
    }

    public async Task<List<AttendanceRecordDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<AttendanceRecordDto> records = await _context.AttendanceRecords
            .AsNoTracking()
            .OrderByDescending(x => x.Date)
            .Select(x => new AttendanceRecordDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                ShiftId = x.ShiftId,
                Date = x.Date,
                TimeIn = x.TimeIn,
                TimeOut = x.TimeOut,
                Status = x.Status,
                WorkHour = x.WorkHour,
                LateMins = x.LateMins,
                EarlyLeaveMins = x.EarlyLeaveMins,
                OTMins = x.OTMins,
                ActualOTMins = x.ActualOTMins,
                IsLate = x.IsLate,
                IsEarlyLeave = x.IsEarlyLeave,
                CheckInSource = x.CheckInSource,
                CheckOutSource = x.CheckOutSource,
                CheckInLatitude = x.CheckInLatitude,
                CheckInLongitude = x.CheckInLongitude,
                CheckInLocation = x.CheckInLocation,
                CheckOutLatitude = x.CheckOutLatitude,
                CheckOutLongitude = x.CheckOutLongitude,
                CheckOutLocation = x.CheckOutLocation,
                DeviceId = x.DeviceId,
                Note = x.Note,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        await EnrichEmployeeInfoAsync(records, cancellationToken);
        return records;
    }

    public async Task<AttendanceWorkHoursDto?> GetWorkHoursAsync(
        Guid employeeId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        return await _context.AttendanceRecords
            .AsNoTracking()
            .Where(record => record.EmployeeId == employeeId && record.Date == date)
            .GroupBy(record => record.EmployeeId)
            .Select(records => new AttendanceWorkHoursDto
            {
                EmployeeId = records.Key,
                Date = date,
                TotalWorkHours = records.Sum(record => record.WorkHour),
                RecordCount = records.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<AttendanceRecordDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        AttendanceRecord? entity = await _context.AttendanceRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            return null;
        }

        return MapToDto(entity);
    }

    public async Task<AttendanceRecordDto> CreateAsync(CreateAttendanceRecordDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.EmployeeId == Guid.Empty)
        {
            throw new ArgumentException("EmployeeId is required.", nameof(dto));
        }

        await EnsureActiveEmployeeAsync(dto.EmployeeId, cancellationToken);

        if (dto.ShiftId == Guid.Empty)
        {
            throw new ArgumentException("ShiftId is required.", nameof(dto));
        }

        // 1. Kiểm tra không cho phép duplicate check-in cùng ca trong ngày
        AttendanceRecord? existing = await _context.AttendanceRecords
            .FirstOrDefaultAsync(x => x.EmployeeId == dto.EmployeeId && x.Date == dto.Date && x.ShiftId == dto.ShiftId, cancellationToken);

        if (existing != null && existing.TimeIn.HasValue)
        {
            throw new InvalidOperationException("Nhân viên đã check-in ca này trong ngày.");
        }

        // Đảm bảo có phân ca (ShiftAssignment) cho nhân viên vào ngày này
        ShiftAssignment? assignment = await _context.ShiftAssignments
            .FirstOrDefaultAsync(sa => sa.EmployeeId == dto.EmployeeId && sa.Date == dto.Date, cancellationToken);

        if (assignment == null)
        {
            assignment = new ShiftAssignment
            {
                Id = Guid.NewGuid(),
                EmployeeId = dto.EmployeeId,
                ShiftId = dto.ShiftId,
                Date = dto.Date,
                Status = "Approved",
                AssignAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            _context.ShiftAssignments.Add(assignment);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // 2. Tra cứu ca làm việc & quy tắc đi trễ
        WorkShift? shift = await _context.WorkShifts
            .FirstOrDefaultAsync(s => s.Id == dto.ShiftId, cancellationToken);

        AttendanceRule? rule = await _context.AttendanceRules
            .Where(r => r.IsActive)
            .OrderByDescending(r => r.EffectiveDate)
            .FirstOrDefaultAsync(cancellationToken);

        int lateMins = dto.LateMins;
        bool isLate = dto.IsLate;

        if (shift != null && dto.TimeIn.HasValue)
        {
            AttendanceRecord check = new() { TimeIn = dto.TimeIn, TimeOut = dto.TimeOut };
            check.EvaluateLateAndEarlyLeave(shift, rule ?? new AttendanceRule { AllowedLateMinutes = 15, AllowedEarlyLeaveMinutes = 15 });
            lateMins = check.LateMins;
            isLate = check.IsLate;
        }

        string status = !string.IsNullOrWhiteSpace(dto.Status) && dto.Status != "Pending"
            ? dto.Status
            : (isLate ? "Late" : "Present");

        AttendanceRecord entity;
        if (existing != null)
        {
            entity = existing;
            entity.TimeIn = dto.TimeIn;
            entity.TimeOut = dto.TimeOut;
            entity.Status = status;
            entity.WorkHour = dto.WorkHour;
            entity.LateMins = lateMins;
            entity.EarlyLeaveMins = dto.EarlyLeaveMins;
            entity.OTMins = dto.OTMins;
            entity.ActualOTMins = dto.ActualOTMins;
            entity.IsLate = isLate;
            entity.IsEarlyLeave = dto.IsEarlyLeave;
            entity.CheckInSource = dto.CheckInSource;
            entity.CheckInLatitude = dto.CheckInLatitude;
            entity.CheckInLongitude = dto.CheckInLongitude;
            entity.CheckInLocation = dto.CheckInLocation;
            entity.DeviceId = dto.DeviceId;
            entity.Note = dto.Note;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            entity = new AttendanceRecord
            {
                Id = Guid.NewGuid(),
                EmployeeId = dto.EmployeeId,
                ShiftId = dto.ShiftId,
                ShiftAssignmentId = assignment.Id,
                Date = dto.Date,
                TimeIn = dto.TimeIn,
                TimeOut = dto.TimeOut,
                Status = status,
                WorkHour = dto.WorkHour,
                LateMins = lateMins,
                EarlyLeaveMins = dto.EarlyLeaveMins,
                OTMins = dto.OTMins,
                ActualOTMins = dto.ActualOTMins,
                IsLate = isLate,
                IsEarlyLeave = dto.IsEarlyLeave,
                CheckInSource = dto.CheckInSource,
                CheckOutSource = dto.CheckOutSource,
                CheckInLatitude = dto.CheckInLatitude,
                CheckInLongitude = dto.CheckInLongitude,
                CheckInLocation = dto.CheckInLocation,
                CheckOutLatitude = dto.CheckOutLatitude,
                CheckOutLongitude = dto.CheckOutLongitude,
                CheckOutLocation = dto.CheckOutLocation,
                DeviceId = dto.DeviceId,
                Note = dto.Note,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.AttendanceRecords.Add(entity);
        }

        if (dto.TimeIn.HasValue) entity.CheckIn(dto.TimeIn.Value, dto.CheckInLocation, dto.CheckInSource);
        if (dto.TimeOut.HasValue) entity.CheckOut(dto.TimeOut.Value, dto.CheckOutLocation, dto.CheckOutSource);

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public async Task<AttendanceRecordDto?> UpdateAsync(Guid id, UpdateAttendanceRecordDto dto, CancellationToken cancellationToken = default)
    {
        AttendanceRecord? entity = await _context.AttendanceRecords
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            return null;
        }

        if (dto.EmployeeId.HasValue)
        {
            await EnsureActiveEmployeeAsync(dto.EmployeeId.Value, cancellationToken);
            entity.EmployeeId = dto.EmployeeId.Value;
        }

        if (dto.ShiftId.HasValue)
        {
            entity.ShiftId = dto.ShiftId.Value;
        }

        if (dto.Date.HasValue)
        {
            entity.Date = dto.Date.Value;
        }

        if (dto.TimeIn.HasValue)
        {
            entity.CheckIn(dto.TimeIn.Value, dto.CheckInLocation ?? entity.CheckInLocation, dto.CheckInSource ?? entity.CheckInSource);
        }

        if (dto.TimeOut.HasValue)
        {
            entity.CheckOut(dto.TimeOut.Value, dto.CheckOutLocation ?? entity.CheckOutLocation, dto.CheckOutSource ?? entity.CheckOutSource);

            WorkShift? shift = await _context.WorkShifts
                .FirstOrDefaultAsync(s => s.Id == entity.ShiftId, cancellationToken);

            AttendanceRule? rule = await _context.AttendanceRules
                .Where(r => r.IsActive)
                .OrderByDescending(r => r.EffectiveDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (shift != null)
            {
                entity.EvaluateLateAndEarlyLeave(shift, rule ?? new AttendanceRule { AllowedLateMinutes = 15, AllowedEarlyLeaveMinutes = 15 });
                entity.CalculateWorkHours(shift);
            }

            // Đối soát với đơn làm thêm giờ (OTRequest) đã duyệt
            OTRequest? otRequest = await _context.OTRequests
                .FirstOrDefaultAsync(ot => ot.EmployeeId == entity.EmployeeId && ot.Date == entity.Date && ot.Status == "Approved", cancellationToken);

            if (otRequest != null && shift != null && dto.TimeOut.Value > shift.EndTime)
            {
                int otMinutes = (int)(dto.TimeOut.Value.ToTimeSpan() - shift.EndTime.ToTimeSpan()).TotalMinutes;
                if (otMinutes > 0)
                {
                    entity.ActualOTMins = otMinutes;
                    otRequest.ReconcileWithRecord(entity);
                    otRequest.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(dto.Status))
        {
            entity.Status = dto.Status;
        }

        if (dto.WorkHour.HasValue)
        {
            entity.WorkHour = dto.WorkHour.Value;
        }

        if (dto.LateMins.HasValue)
        {
            entity.LateMins = dto.LateMins.Value;
        }

        if (dto.EarlyLeaveMins.HasValue)
        {
            entity.EarlyLeaveMins = dto.EarlyLeaveMins.Value;
        }

        if (dto.OTMins.HasValue)
        {
            entity.OTMins = dto.OTMins.Value;
        }

        if (dto.ActualOTMins.HasValue)
        {
            entity.ActualOTMins = dto.ActualOTMins.Value;
        }

        if (dto.IsLate.HasValue)
        {
            entity.IsLate = dto.IsLate.Value;
        }

        if (dto.IsEarlyLeave.HasValue)
        {
            entity.IsEarlyLeave = dto.IsEarlyLeave.Value;
        }

        if (dto.CheckInSource != null)
        {
            entity.CheckInSource = dto.CheckInSource;
        }

        if (dto.CheckOutSource != null)
        {
            entity.CheckOutSource = dto.CheckOutSource;
        }

        if (dto.CheckInLatitude.HasValue)
        {
            entity.CheckInLatitude = dto.CheckInLatitude.Value;
        }

        if (dto.CheckInLongitude.HasValue)
        {
            entity.CheckInLongitude = dto.CheckInLongitude.Value;
        }

        if (dto.CheckInLocation != null)
        {
            entity.CheckInLocation = dto.CheckInLocation;
        }

        if (dto.CheckOutLatitude.HasValue)
        {
            entity.CheckOutLatitude = dto.CheckOutLatitude.Value;
        }

        if (dto.CheckOutLongitude.HasValue)
        {
            entity.CheckOutLongitude = dto.CheckOutLongitude.Value;
        }

        if (dto.CheckOutLocation != null)
        {
            entity.CheckOutLocation = dto.CheckOutLocation;
        }

        if (dto.DeviceId != null)
        {
            entity.DeviceId = dto.DeviceId;
        }

        if (dto.Note != null)
        {
            entity.Note = dto.Note;
        }

        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        AttendanceRecord? entity = await _context.AttendanceRecords
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity == null)
        {
            return false;
        }

        _context.AttendanceRecords.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<List<AttendanceRecordDto>> SearchAsync(
        AttendanceSearchRequestDto request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<AttendanceRecord> query = _context.AttendanceRecords
            .AsNoTracking()
            .AsQueryable();

        // 1. Lọc theo ngày
        if (request.Date.HasValue)
        {
            query = query.Where(x => x.Date == request.Date.Value);
        }

        // 2. Lọc theo trạng thái
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(x => x.Status == request.Status);
        }

        // 3. Lọc theo thông tin nhân viên (gọi API từ HR qua IHrEmployeeDirectoryClient)
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            try
            {
                EmployeeDirectoryPage? employeesPage = await _employees.GetActiveEmployeesAsync(request.Keyword, 1, 100, cancellationToken);
                if (employeesPage?.Items != null)
                {
                    List<Guid> matchingEmployeeIds = employeesPage.Items.Select(x => x.EmployeeId).Distinct().ToList();
                    query = query.Where(x => matchingEmployeeIds.Contains(x.EmployeeId));
                }
            }
            catch (HttpRequestException)
            {
                // Bỏ qua nếu HR API tạm thời không phản hồi
            }
        }

        // 4. Trả về kết quả
        List<AttendanceRecordDto> records = await query
            .OrderByDescending(x => x.Date)
            .Select(x => new AttendanceRecordDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                ShiftId = x.ShiftId,
                Date = x.Date,
                TimeIn = x.TimeIn,
                TimeOut = x.TimeOut,
                Status = x.Status,
                WorkHour = x.WorkHour,
                LateMins = x.LateMins,
                EarlyLeaveMins = x.EarlyLeaveMins,
                OTMins = x.OTMins,
                ActualOTMins = x.ActualOTMins,
                IsLate = x.IsLate,
                IsEarlyLeave = x.IsEarlyLeave,
                CheckInSource = x.CheckInSource,
                CheckOutSource = x.CheckOutSource,
                CheckInLatitude = x.CheckInLatitude,
                CheckInLongitude = x.CheckInLongitude,
                CheckInLocation = x.CheckInLocation,
                CheckOutLatitude = x.CheckOutLatitude,
                CheckOutLongitude = x.CheckOutLongitude,
                CheckOutLocation = x.CheckOutLocation,
                DeviceId = x.DeviceId,
                Note = x.Note,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        await EnrichEmployeeInfoAsync(records, cancellationToken);
        return records;
    }

    private async Task EnrichEmployeeInfoAsync(List<AttendanceRecordDto> dtos, CancellationToken cancellationToken)
    {
        if (dtos.Count == 0) return;
        try
        {
            EmployeeDirectoryPage? page = await _employees.GetActiveEmployeesAsync(null, 1, 100, cancellationToken);
            if (page?.Items != null && page.Items.Count > 0)
            {
                Dictionary<Guid, EmployeeDirectoryEntry> map = page.Items.ToDictionary(e => e.EmployeeId);
                foreach (AttendanceRecordDto dto in dtos)
                {
                    if (map.TryGetValue(dto.EmployeeId, out EmployeeDirectoryEntry? emp))
                    {
                        dto.EmployeeName = emp.FullName;
                        dto.EmployeeCode = emp.EmployeeCode;
                    }
                }
            }
        }
        catch
        {
            // Bỏ qua nếu service HR chưa sẵn sàng
        }
    }
    private async Task EnsureActiveEmployeeAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        if (await _employees.GetActiveEmployeeAsync(employeeId, cancellationToken) is null)
        {
            throw new EmployeeNotAvailableException(employeeId);
        }
    }

    private static AttendanceRecordDto MapToDto(AttendanceRecord entity)
    {
        return new AttendanceRecordDto
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            ShiftId = entity.ShiftId,
            Date = entity.Date,
            TimeIn = entity.TimeIn,
            TimeOut = entity.TimeOut,
            Status = entity.Status,
            WorkHour = entity.WorkHour,
            LateMins = entity.LateMins,
            EarlyLeaveMins = entity.EarlyLeaveMins,
            OTMins = entity.OTMins,
            ActualOTMins = entity.ActualOTMins,
            IsLate = entity.IsLate,
            IsEarlyLeave = entity.IsEarlyLeave,
            CheckInSource = entity.CheckInSource,
            CheckOutSource = entity.CheckOutSource,
            CheckInLatitude = entity.CheckInLatitude,
            CheckInLongitude = entity.CheckInLongitude,
            CheckInLocation = entity.CheckInLocation,
            CheckOutLatitude = entity.CheckOutLatitude,
            CheckOutLongitude = entity.CheckOutLongitude,
            CheckOutLocation = entity.CheckOutLocation,
            DeviceId = entity.DeviceId,
            Note = entity.Note,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
