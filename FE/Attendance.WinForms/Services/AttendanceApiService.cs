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

        // Biến mảng tạm (in-memory cache) trên Client để đọc/lọc nhanh mà không cần gọi thẳng xuống DB mỗi lần
        private static List<WorkShiftDto>? _cacheShifts;
        private static List<EmployeeDirectoryDto>? _cacheEmployees;
        private static List<AttendanceRecordDto> _cacheRecords = new();
        private static List<LeaveRequestDto> _cacheLeaves = new();
        private static List<OTRequestDto> _cacheOTRequests = new();
        private static List<AttendanceExplanationDto> _cacheExplanations = new();

        public AttendanceApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ==========================================
        // WORK SHIFTS
        // ==========================================
        public async Task<List<WorkShiftDto>> GetWorkShiftsAsync(bool forceRefresh = false)
        {
            if (!forceRefresh && _cacheShifts != null && _cacheShifts.Count > 0)
            {
                return _cacheShifts;
            }

            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<WorkShiftDto>>("api/WorkShifts");
                _cacheShifts = response ?? new List<WorkShiftDto>();
                return _cacheShifts;
            }
            catch
            {
                return _cacheShifts ?? new List<WorkShiftDto>();
            }
        }

        // ==========================================
        // EMPLOYEE DIRECTORY
        // ==========================================
        public async Task<List<EmployeeDirectoryDto>> GetEmployeesAsync(string? keyword = null, bool forceRefresh = false)
        {
            if (!forceRefresh && _cacheEmployees != null && _cacheEmployees.Count > 0)
            {
                if (string.IsNullOrWhiteSpace(keyword)) return _cacheEmployees;
                var kw = keyword.Trim().ToLower();
                return _cacheEmployees.Where(e =>
                    (!string.IsNullOrEmpty(e.FullName) && e.FullName.ToLower().Contains(kw)) ||
                    (!string.IsNullOrEmpty(e.EmployeeCode) && e.EmployeeCode.ToLower().Contains(kw))).ToList();
            }

            try
            {
                var url = string.IsNullOrWhiteSpace(keyword)
                    ? "api/employees?page=1&pageSize=100"
                    : $"api/employees?keyword={Uri.EscapeDataString(keyword.Trim())}&page=1&pageSize=50";
                var response = await _httpClient.GetFromJsonAsync<EmployeeDirectoryPageDto>(url);
                var items = response?.Items ?? new List<EmployeeDirectoryDto>();
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    _cacheEmployees = items;
                }
                return items;
            }
            catch
            {
                return _cacheEmployees ?? new List<EmployeeDirectoryDto>();
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
                var created = await response.Content.ReadFromJsonAsync<AttendanceRecordDto>();
                if (created != null)
                {
                    _cacheRecords.RemoveAll(r => r.EmployeeId == created.EmployeeId && r.Date == created.Date);
                    _cacheRecords.Insert(0, created);
                }
                return created;
            }
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Check-in thất bại: {err}");
        }

        public async Task<AttendanceRecordDto?> UpdateAttendanceRecordAsync(Guid id, UpdateAttendanceRecordDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Attendance/{id}", dto);
            if (response.IsSuccessStatusCode)
            {
                var updated = await response.Content.ReadFromJsonAsync<AttendanceRecordDto>();
                if (updated != null)
                {
                    var idx = _cacheRecords.FindIndex(r => r.Id == id);
                    if (idx >= 0) _cacheRecords[idx] = updated;
                    else _cacheRecords.Insert(0, updated);
                }
                return updated;
            }
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Check-out thất bại: {err}");
        }

        public async Task<AttendanceRecordDto?> GetTodayRecordAsync(Guid employeeId, DateOnly date, bool forceRefresh = false)
        {
            if (!forceRefresh && _cacheRecords.Count > 0)
            {
                var cached = _cacheRecords.FirstOrDefault(r => r.EmployeeId == employeeId && r.Date == date);
                if (cached != null) return cached;
            }

            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<AttendanceRecordDto>>("api/Attendance");
                _cacheRecords = response ?? new List<AttendanceRecordDto>();
                return _cacheRecords.FirstOrDefault(r => r.EmployeeId == employeeId && r.Date == date);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<AttendanceRecordDto>> GetRecentLogsAsync(Guid? employeeId = null, bool forceRefresh = false)
        {
            if (!forceRefresh && _cacheRecords.Count > 0)
            {
                var list = _cacheRecords;
                if (employeeId.HasValue && employeeId.Value != Guid.Empty)
                {
                    list = list.Where(r => r.EmployeeId == employeeId.Value).ToList();
                }
                return list.OrderByDescending(r => r.Date).ThenByDescending(r => r.TimeIn).Take(30).ToList();
            }

            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<AttendanceRecordDto>>("api/Attendance");
                _cacheRecords = response ?? new List<AttendanceRecordDto>();
                var list = _cacheRecords;
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
        public async Task<List<LeaveRequestDto>> GetLeaveRequestsAsync(Guid? employeeId = null, bool forceRefresh = false)
        {
            if (!forceRefresh && _cacheLeaves.Count > 0)
            {
                var list = _cacheLeaves;
                if (employeeId.HasValue && employeeId.Value != Guid.Empty)
                {
                    list = list.Where(l => l.EmployeeId == employeeId.Value).ToList();
                }
                return list.OrderByDescending(l => l.CreatedAt).ToList();
            }

            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<LeaveRequestDto>>("api/LeaveRequests");
                _cacheLeaves = response ?? new List<LeaveRequestDto>();
                var list = _cacheLeaves;
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
                var created = await response.Content.ReadFromJsonAsync<LeaveRequestDto>();
                if (created != null)
                {
                    _cacheLeaves.Insert(0, created);
                }
                return created;
            }
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Gửi đơn xin nghỉ phép thất bại: {err}");
        }

        // ==========================================
        // OT REQUESTS (LÀM THÊM GIỜ)
        // ==========================================
        public async Task<List<OTRequestDto>> GetOTRequestsAsync(Guid? employeeId = null, bool forceRefresh = false)
        {
            if (!forceRefresh && _cacheOTRequests.Count > 0)
            {
                var list = _cacheOTRequests;
                if (employeeId.HasValue && employeeId.Value != Guid.Empty)
                {
                    list = list.Where(l => l.EmployeeId == employeeId.Value).ToList();
                }
                return list.OrderByDescending(l => l.CreatedAt).ToList();
            }

            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<OTRequestDto>>("api/OTRequests");
                _cacheOTRequests = response ?? new List<OTRequestDto>();
                var list = _cacheOTRequests;
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
                var created = await response.Content.ReadFromJsonAsync<OTRequestDto>();
                if (created != null)
                {
                    _cacheOTRequests.Insert(0, created);
                }
                return created;
            }
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Gửi đơn làm thêm giờ thất bại: {err}");
        }

        // ==========================================
        // ATTENDANCE EXPLANATIONS (GIẢI TRÌNH)
        // ==========================================
        public async Task<List<AttendanceExplanationDto>> GetExplanationsAsync(Guid? employeeId = null, bool forceRefresh = false)
        {
            if (!forceRefresh && _cacheExplanations.Count > 0)
            {
                var list = _cacheExplanations;
                if (employeeId.HasValue && employeeId.Value != Guid.Empty)
                {
                    list = list.Where(l => l.EmployeeId == employeeId.Value).ToList();
                }
                return list.OrderByDescending(l => l.CreatedAt).ToList();
            }

            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<AttendanceExplanationDto>>("api/AttendanceExplanations");
                _cacheExplanations = response ?? new List<AttendanceExplanationDto>();
                var list = _cacheExplanations;
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
                var created = await response.Content.ReadFromJsonAsync<AttendanceExplanationDto>();
                if (created != null)
                {
                    _cacheExplanations.Insert(0, created);
                }
                return created;
            }
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Gửi đơn giải trình thất bại: {err}");
        }
    }
}
