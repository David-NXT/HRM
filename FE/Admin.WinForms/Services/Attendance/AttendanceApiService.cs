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

    // GET: api/WorkShifts
    public async Task<List<WorkShiftDto>> GetWorkShiftsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<WorkShiftDto>>("api/WorkShifts");
            return response ?? new List<WorkShiftDto>();
        }
        catch
        {
            return new List<WorkShiftDto>();
        }
    }

    // POST: api/WorkShifts
    public async Task<WorkShiftDto?> CreateWorkShiftAsync(CreateWorkShiftDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/WorkShifts", dto);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<WorkShiftDto>();
        var err = await response.Content.ReadAsStringAsync();
        throw new Exception($"Tạo ca thất bại: {err}");
    }

    // PUT: api/WorkShifts/{id}
    public async Task<bool> UpdateWorkShiftAsync(Guid id, UpdateWorkShiftDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/WorkShifts/{id}", dto);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Cập nhật ca thất bại: {err}");
        }
        return true;
    }

    // DELETE: api/WorkShifts/{id}
    public async Task<bool> DeleteWorkShiftAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/WorkShifts/{id}");
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Xóa ca thất bại: {err}");
        }
        return true;
    }

    // GET: api/AttendanceRules
    public async Task<List<AttendanceRuleDto>> GetAttendanceRulesAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<AttendanceRuleDto>>("api/AttendanceRules");
            return response ?? new List<AttendanceRuleDto>();
        }
        catch
        {
            return new List<AttendanceRuleDto>();
        }
    }

    // POST: api/AttendanceRules
    public async Task<AttendanceRuleDto?> CreateAttendanceRuleAsync(CreateAttendanceRuleDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/AttendanceRules", dto);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<AttendanceRuleDto>();
        var err = await response.Content.ReadAsStringAsync();
        throw new Exception($"Tạo quy tắc thất bại: {err}");
    }

    // PUT: api/AttendanceRules/{id}
    public async Task<bool> UpdateAttendanceRuleAsync(Guid id, UpdateAttendanceRuleDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/AttendanceRules/{id}", dto);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Cập nhật quy tắc thất bại: {err}");
        }
        return true;
    }

    // DELETE: api/AttendanceRules/{id}
    public async Task<bool> DeleteAttendanceRuleAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/AttendanceRules/{id}");
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Xóa quy tắc thất bại: {err}");
        }
        return true;
    }

    // GET: api/OvertimeRules
    public async Task<List<OvertimeRuleDto>> GetOvertimeRulesAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<OvertimeRuleDto>>("api/OvertimeRules");
            return response ?? new List<OvertimeRuleDto>();
        }
        catch
        {
            return new List<OvertimeRuleDto>();
        }
    }

    // POST: api/OvertimeRules
    public async Task<OvertimeRuleDto?> CreateOvertimeRuleAsync(CreateOvertimeRuleDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/OvertimeRules", dto);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<OvertimeRuleDto>();
        var err = await response.Content.ReadAsStringAsync();
        throw new Exception($"Tạo quy tắc OT thất bại: {err}");
    }

    // PUT: api/OvertimeRules/{id}
    public async Task<bool> UpdateOvertimeRuleAsync(Guid id, UpdateOvertimeRuleDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/OvertimeRules/{id}", dto);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Cập nhật quy tắc OT thất bại: {err}");
        }
        return true;
    }

    // DELETE: api/OvertimeRules/{id}
    public async Task<bool> DeleteOvertimeRuleAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/OvertimeRules/{id}");
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Xóa quy tắc OT thất bại: {err}");
        }
        return true;
    }

    // ==========================================
    // EMPLOYEE DIRECTORY
    // ==========================================
    public async Task<List<EmployeeDirectoryDto>> GetEmployeesAsync(string? keyword = null)
    {
        try
        {
            var url = string.IsNullOrWhiteSpace(keyword)
                ? "api/employees?page=1&pageSize=100"
                : $"api/employees?keyword={Uri.EscapeDataString(keyword.Trim())}&page=1&pageSize=50";
            var response = await _httpClient.GetFromJsonAsync<EmployeeDirectoryPageDto>(url);
            return response?.Items ?? new List<EmployeeDirectoryDto>();
        }
        catch
        {
            return new List<EmployeeDirectoryDto>();
        }
    }

    public async Task<List<EmployeeDirectoryDto>> SearchEmployeesAsync(string keyword)
    {
        var path = $"api/employees?keyword={Uri.EscapeDataString(keyword.Trim())}&page=1&pageSize=100";
        using var response = await _httpClient.GetAsync(path);
        if (!response.IsSuccessStatusCode)
        {
            var details = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(details)
                    ? $"Attendance API trả về HTTP {(int)response.StatusCode} khi tìm nhân viên."
                    : $"Attendance API trả về HTTP {(int)response.StatusCode}: {details}",
                null,
                response.StatusCode);
        }

        var page = await response.Content.ReadFromJsonAsync<EmployeeDirectoryPageDto>()
            ?? throw new InvalidDataException("Attendance API trả về dữ liệu tìm kiếm nhân viên rỗng.");
        return page.Items;
    }

    public async Task<EmployeeDirectoryDto?> GetEmployeeByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<EmployeeDirectoryDto>($"api/employees/{id}");
        }
        catch
        {
            return null;
        }
    }

    // ==========================================
    // CHECK-IN & CHECK-OUT ATTENDANCE RECORD
    // ==========================================
    public async Task<AttendanceRecordDto?> CreateAttendanceRecordAsync(CreateAttendanceRecordDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Attendance", dto);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AttendanceRecordDto>();
        }
        var err = await response.Content.ReadAsStringAsync();
        throw new Exception($"Check-in thất bại: {err}");
    }

    public async Task<AttendanceRecordDto?> UpdateAttendanceRecordAsync(Guid id, UpdateAttendanceRecordDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/Attendance/{id}", dto);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AttendanceRecordDto>();
        }
        var err = await response.Content.ReadAsStringAsync();
        throw new Exception($"Check-out thất bại: {err}");
    }

    public async Task<AttendanceRecordDto?> GetTodayRecordAsync(Guid employeeId, DateOnly date)
    {
        try
        {
            var search = new AttendanceSearchRequest
            {
                Date = date
            };
            var list = await SearchAsync(search);
            return list.FirstOrDefault(r => r.EmployeeId == employeeId && r.Date == date);
        }
        catch
        {
            return null;
        }
    }

    // ==========================================
    // LEAVE REQUESTS (UC-ATT03)
    // ==========================================
    public async Task<List<LeaveRequestDto>> GetLeaveRequestsAsync(Guid? employeeId = null)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<LeaveRequestDto>>("api/LeaveRequests");
            var list = response ?? new List<LeaveRequestDto>();
            if (employeeId.HasValue && employeeId.Value != Guid.Empty)
            {
                list = list.Where(l => l.EmployeeId == employeeId.Value).ToList();
            }
            return list;
        }
        catch
        {
            return new List<LeaveRequestDto>();
        }
    }

    public async Task<LeaveRequestDto?> CreateLeaveRequestAsync(CreateLeaveRequestDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/LeaveRequests", dto);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<LeaveRequestDto>();
        }
        var err = await response.Content.ReadAsStringAsync();
        throw new Exception($"Gửi đơn xin nghỉ phép thất bại: {err}");
    }

    // ==========================================
    // OT REQUESTS (UC-ATT04)
    // ==========================================
    public async Task<OTRequestDto?> CreateOTRequestAsync(CreateOTRequestDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/OTRequests", dto);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<OTRequestDto>();
        }
        var err = await response.Content.ReadAsStringAsync();
        throw new Exception($"Gửi đơn làm thêm giờ thất bại: {err}");
    }

    // ==========================================
    // ATTENDANCE EXPLANATIONS (UC-ATT05)
    // ==========================================
    public async Task<AttendanceExplanationDto?> CreateAttendanceExplanationAsync(CreateAttendanceExplanationDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/AttendanceExplanations", dto);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AttendanceExplanationDto>();
        }
        var err = await response.Content.ReadAsStringAsync();
        throw new Exception($"Gửi đơn giải trình thất bại: {err}");
    }
}
