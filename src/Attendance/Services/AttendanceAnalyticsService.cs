using Attendance.Data;
using Attendance.DTOs.Analytics;
using Attendance.Models;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Services;

public interface IAttendanceAnalyticsService
{
    Task<AttendanceOverviewStatsDto> GetOverviewStatsAsync(DateOnly? date, CancellationToken cancellationToken = default);
    Task<List<AttendanceWeeklyTrendDto>> GetWeeklyTrendAsync(int month, int year, CancellationToken cancellationToken = default);
    Task<List<AttendanceDepartmentTrendDto>> GetDepartmentTrendAsync(int month, int year, CancellationToken cancellationToken = default);
}

public class AttendanceAnalyticsService : IAttendanceAnalyticsService
{
    private readonly AttendanceDbContext _context;
    private readonly IHrEmployeeDirectoryClient _employees;

    public AttendanceAnalyticsService(AttendanceDbContext context, IHrEmployeeDirectoryClient employees)
    {
        _context = context;
        _employees = employees;
    }

    public async Task<AttendanceOverviewStatsDto> GetOverviewStatsAsync(DateOnly? date, CancellationToken cancellationToken = default)
    {
        DateOnly targetDate = date ?? DateOnly.FromDateTime(DateTime.Today);

        // Lay danh sach nhan vien tu HR
        int totalEmployees = 0;
        try
        {
            List<EmployeeDirectoryEntry> allEmployees = await _employees.GetAllActiveEmployeesAsync(cancellationToken);
            totalEmployees = allEmployees.Count;
        }
        catch
        {
            // Bo qua neu HR API tam thoi offline
        }

        // Lay du lieu quet the trong ngay
        List<AttendanceRecord> records = await _context.AttendanceRecords
            .AsNoTracking()
            .Where(r => r.Date == targetDate)
            .ToListAsync(cancellationToken);

        int presentCount = records.Count(r => r.WorkHour > 0 || r.Status.Equals("Present", StringComparison.OrdinalIgnoreCase) || r.TimeIn.HasValue);
        int lateCount = records.Count(r => r.IsLate || r.LateMins > 0 || r.Status.Equals("Late", StringComparison.OrdinalIgnoreCase));

        // Lay so nguoi co don nghi phep da duyet
        int onLeaveCount = 0;
        try
        {
            List<LeaveRequest> leaves = await _context.LeaveRequests
                .AsNoTracking()
                .Where(l => l.Status == "Approved" && l.StartDate <= targetDate && l.EndDate >= targetDate)
                .ToListAsync(cancellationToken);
            onLeaveCount = leaves.Select(l => l.EmployeeId).Distinct().Count();
        }
        catch
        {
        }

        if (totalEmployees == 0)
        {
            int uniqueEmployees = await _context.AttendanceRecords
                .Select(r => r.EmployeeId)
                .Distinct()
                .CountAsync(cancellationToken);
            totalEmployees = Math.Max(uniqueEmployees, presentCount + onLeaveCount);
        }

        int absentCount = records.Count(r => r.Status.Equals("Absent", StringComparison.OrdinalIgnoreCase));
        if (absentCount == 0 && totalEmployees > (presentCount + onLeaveCount))
        {
            absentCount = totalEmployees - (presentCount + onLeaveCount);
        }

        return new AttendanceOverviewStatsDto
        {
            Date = targetDate,
            TotalEmployees = totalEmployees,
            PresentCount = presentCount,
            LateCount = lateCount,
            AbsentCount = Math.Max(0, absentCount),
            OnLeaveCount = onLeaveCount
        };
    }

    public async Task<List<AttendanceWeeklyTrendDto>> GetWeeklyTrendAsync(int month, int year, CancellationToken cancellationToken = default)
    {
        List<AttendanceWeeklyTrendDto> result = new List<AttendanceWeeklyTrendDto>();
        int daysInMonth = DateTime.DaysInMonth(year, month);

        (string Name, int StartDay, int EndDay)[] weeks = new[]
        {
            (Name: "Week 1", StartDay: 1, EndDay: 7),
            (Name: "Week 2", StartDay: 8, EndDay: 14),
            (Name: "Week 3", StartDay: 15, EndDay: 21),
            (Name: "Week 4", StartDay: 22, EndDay: daysInMonth)
        };

        foreach ((string Name, int StartDay, int EndDay) w in weeks)
        {
            DateOnly start = new DateOnly(year, month, w.StartDay);
            DateOnly end = new DateOnly(year, month, w.EndDay);

            List<AttendanceRecord> records = await _context.AttendanceRecords
                .AsNoTracking()
                .Where(r => r.Date >= start && r.Date <= end)
                .ToListAsync(cancellationToken);

            int presentPct = 0;
            if (records.Count > 0)
            {
                int present = records.Count(r => r.WorkHour > 0 || r.Status.Equals("Present", StringComparison.OrdinalIgnoreCase));
                presentPct = (int)Math.Round((double)present / records.Count * 100);
            }
            else
            {
                // Fallback khi khong co du lieu: 0% co mat (100% vang)
                presentPct = 0;
            }

            presentPct = Math.Clamp(presentPct, 0, 100);
            result.Add(new AttendanceWeeklyTrendDto
            {
                Name = w.Name,
                Present = presentPct,
                Absent = 100 - presentPct
            });
        }

        return result;
    }

    public async Task<List<AttendanceDepartmentTrendDto>> GetDepartmentTrendAsync(int month, int year, CancellationToken cancellationToken = default)
    {
        List<AttendanceDepartmentTrendDto> result = new List<AttendanceDepartmentTrendDto>();

        try
        {
            IReadOnlyList<HrDepartmentDto> departments = await _employees.GetDepartmentsAsync(cancellationToken);
            List<EmployeeDirectoryEntry> employees = await _employees.GetAllActiveEmployeesAsync(cancellationToken);

            List<string> deptNames = new List<string>();
            if (departments.Count > 0)
            {
                deptNames = departments.Select(d => d.Name).Distinct().ToList();
            }
            else if (employees.Count > 0)
            {
                deptNames = employees
                    .Where(e => !string.IsNullOrWhiteSpace(e.DepartmentName))
                    .Select(e => e.DepartmentName!)
                    .Distinct()
                    .ToList();
            }

            if (deptNames.Count > 0)
            {
                foreach (string deptName in deptNames)
                {
                    List<Guid> deptEmployees = employees
                        .Where(e => string.Equals(e.DepartmentName, deptName, StringComparison.OrdinalIgnoreCase))
                        .Select(e => e.EmployeeId)
                        .ToList();

                    int pct = 0;

                    if (deptEmployees.Count > 0)
                    {
                        List<AttendanceRecord> records = await _context.AttendanceRecords
                            .AsNoTracking()
                            .Where(r => deptEmployees.Contains(r.EmployeeId) && r.Date.Month == month && r.Date.Year == year)
                            .ToListAsync(cancellationToken);

                        if (records.Count > 0)
                        {
                            int present = records.Count(r => r.WorkHour > 0 || r.Status.Equals("Present", StringComparison.OrdinalIgnoreCase));
                            pct = (int)Math.Round((double)present / records.Count * 100);
                        }
                        else
                        {
                            pct = 0;
                        }
                    }
                    else
                    {
                        pct = 0;
                    }

                    result.Add(new AttendanceDepartmentTrendDto
                    {
                        Name = deptName,
                        Percentage = Math.Clamp(pct, 0, 100),
                        TotalEmployees = deptEmployees.Count
                    });
                }

                return result;
            }
        }
        catch
        {
            // HR API offline hoac co loi
        }

        // Fallback khi khong the ket noi HR API: tra ve 0% theo yeu cau
        result.AddRange(new[]
        {
            new AttendanceDepartmentTrendDto { Name = "Phòng Công Nghệ Thông Tin", Percentage = 0, TotalEmployees = 0 },
            new AttendanceDepartmentTrendDto { Name = "Phòng Hành Chính Nhân Sự", Percentage = 0, TotalEmployees = 0 },
            new AttendanceDepartmentTrendDto { Name = "Phòng Marketing", Percentage = 0, TotalEmployees = 0 }
        });

        return result;
    }
}
