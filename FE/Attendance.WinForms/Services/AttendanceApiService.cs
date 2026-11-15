using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Attendance.WinForms.DTOs;

namespace Attendance.WinForms.Services
{
    public class AttendanceApiService
    {
        private readonly HttpClient _httpClient;

        public AttendanceApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ==========================================
        // WORK SHIFTS
        // ==========================================
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

        // ==========================================
        // CHECK-IN & CHECK-OUT (ATTENDANCE)
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
                var response = await _httpClient.GetFromJsonAsync<List<AttendanceRecordDto>>("api/Attendance");
                var list = response ?? new List<AttendanceRecordDto>();
                return list.FirstOrDefault(r => r.EmployeeId == employeeId && r.Date == date);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<AttendanceRecordDto>> GetRecentLogsAsync(Guid? employeeId = null)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<AttendanceRecordDto>>("api/Attendance");
                var list = response ?? new List<AttendanceRecordDto>();
                if (employeeId.HasValue && employeeId.Value != Guid.Empty)
                {
                    list = list.Where(r => r.EmployeeId == employeeId.Value).ToList();
                }
                return list.OrderByDescending(r => r.Date).ThenByDescending(r => r.TimeIn).Take(30).ToList();
            }
            catch
            {
                return new List<AttendanceRecordDto>();
            }
        }

        // ==========================================
        // LEAVE REQUESTS (NGHỈ PHÉP)
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
                return list.OrderByDescending(l => l.CreatedAt).ToList();
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
        // OT REQUESTS (LÀM THÊM GIỜ)
        // ==========================================
        public async Task<List<OTRequestDto>> GetOTRequestsAsync(Guid? employeeId = null)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<OTRequestDto>>("api/OTRequests");
                var list = response ?? new List<OTRequestDto>();
                if (employeeId.HasValue && employeeId.Value != Guid.Empty)
                {
                    list = list.Where(l => l.EmployeeId == employeeId.Value).ToList();
                }
                return list.OrderByDescending(l => l.CreatedAt).ToList();
            }
            catch
            {
                return new List<OTRequestDto>();
            }
        }

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
        // ATTENDANCE EXPLANATIONS (GIẢI TRÌNH)
        // ==========================================
        public async Task<List<AttendanceExplanationDto>> GetExplanationsAsync(Guid? employeeId = null)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<AttendanceExplanationDto>>("api/AttendanceExplanations");
                var list = response ?? new List<AttendanceExplanationDto>();
                if (employeeId.HasValue && employeeId.Value != Guid.Empty)
                {
                    list = list.Where(l => l.EmployeeId == employeeId.Value).ToList();
                }
                return list.OrderByDescending(l => l.CreatedAt).ToList();
            }
            catch
            {
                return new List<AttendanceExplanationDto>();
            }
        }

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
}
