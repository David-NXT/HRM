using System.Net.Http.Json;
using Admin.WinForms.DTOs.Attendance;
using Admin.WinForms.DTOs.Department;

namespace Admin.WinForms.Services.Attendance;

public class AttendanceApiService
{
    private readonly HttpClient _httpClient;

    public AttendanceApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // GET: api/Attendance
    public async Task<List<AttendanceRecordDto>> GetAllAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<List<AttendanceRecordDto>>("api/Attendance");
        return response ?? new List<AttendanceRecordDto>();
    }

    // GET: api/Attendance/{id}
    public async Task<AttendanceRecordDto?> GetByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<AttendanceRecordDto>($"api/Attendance/{id}");
    }

    // POST: api/Attendance/search
    public async Task<List<AttendanceRecordDto>> SearchAsync(AttendanceSearchRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Attendance/search", request);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<List<AttendanceRecordDto>>();
            return result ?? new List<AttendanceRecordDto>();
        }
        return new List<AttendanceRecordDto>();
    }

    // GET: api/AttendanceExplanations
    public async Task<List<AttendanceExplanationDto>> GetExplanationsAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<List<AttendanceExplanationDto>>("api/AttendanceExplanations");
        return response ?? new List<AttendanceExplanationDto>();
    }

    // PUT: api/AttendanceExplanations/{id}
    public async Task<bool> UpdateExplanationAsync(Guid id, UpdateAttendanceExplanationDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/AttendanceExplanations/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    // GET: api/OTRequests
    public async Task<List<OTRequestDto>> GetOTRequestsAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<List<OTRequestDto>>("api/OTRequests");
        return response ?? new List<OTRequestDto>();
    }

    // PUT: api/OTRequests/{id}
    public async Task<bool> UpdateOTRequestAsync(Guid id, UpdateOTRequestDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/OTRequests/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    // GET: api/Timesheets/periods
    public async Task<List<TimesheetPeriodDto>> GetTimesheetPeriodsAsync(int? year = null)
    {
        var url = year.HasValue ? $"api/Timesheets/periods?year={year.Value}" : "api/Timesheets/periods";
        var response = await _httpClient.GetFromJsonAsync<List<TimesheetPeriodDto>>(url);
        return response ?? new List<TimesheetPeriodDto>();
    }

    // GET: api/Timesheets/periods/{periodId}/summaries
    public async Task<List<TimesheetSummaryDto>> GetTimesheetSummariesByPeriodAsync(Guid periodId)
    {
        var response = await _httpClient.GetFromJsonAsync<List<TimesheetSummaryDto>>($"api/Timesheets/periods/{periodId}/summaries");
        return response ?? new List<TimesheetSummaryDto>();
    }

    // GET: api/Attendance/departments
    public async Task<List<DepartmentDto>> GetDepartmentsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<DepartmentDto>>("api/Attendance/departments");
            return response ?? new List<DepartmentDto>();
        }
        catch
        {
            return new List<DepartmentDto>();
        }
    }

    // GET: api/attendance/analytics/overview
    public async Task<AttendanceOverviewStatsDto?> GetOverviewStatsAsync(DateOnly? date = null)
    {
        try
        {
            var url = date.HasValue
                ? $"api/attendance/analytics/overview?date={date.Value:yyyy-MM-dd}"
                : "api/attendance/analytics/overview";
            return await _httpClient.GetFromJsonAsync<AttendanceOverviewStatsDto>(url);
        }
        catch
        {
            return null;
        }
    }

    // GET: api/attendance/analytics/trends
    public async Task<List<AttendanceWeeklyTrendDto>> GetWeeklyTrendsAsync(int? month = null, int? year = null)
    {
        try
        {
            var url = (month.HasValue && year.HasValue)
                ? $"api/attendance/analytics/trends?month={month.Value}&year={year.Value}"
                : "api/attendance/analytics/trends";
            var response = await _httpClient.GetFromJsonAsync<List<AttendanceWeeklyTrendDto>>(url);
            return response ?? new List<AttendanceWeeklyTrendDto>();
        }
        catch
        {
            return new List<AttendanceWeeklyTrendDto>();
        }
    }

    // GET: api/attendance/analytics/departments
    public async Task<List<AttendanceDepartmentTrendDto>> GetDepartmentTrendsAsync(int? month = null, int? year = null)
    {
        try
        {
            var url = (month.HasValue && year.HasValue)
                ? $"api/attendance/analytics/departments?month={month.Value}&year={year.Value}"
                : "api/attendance/analytics/departments";
            var response = await _httpClient.GetFromJsonAsync<List<AttendanceDepartmentTrendDto>>(url);
            return response ?? new List<AttendanceDepartmentTrendDto>();
        }
        catch
        {
            return new List<AttendanceDepartmentTrendDto>();
        }
    }
}
