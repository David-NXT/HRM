using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Admin.WinForms.DTOs.Payroll;

namespace Admin.WinForms.Services.Payroll;

public class PayrollApiService
{
    private readonly HttpClient _httpClient;

    public PayrollApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // 1. Quản lý kỳ lương
    public async Task<List<PayrollPeriodDto>> GetPeriodsAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<PayrollPeriodDto>>("api/payroll-periods");
            return response ?? new List<PayrollPeriodDto>();
        }
        catch
        {
            return new List<PayrollPeriodDto>();
        }
    }

    public async Task<PayrollPeriodDto?> CreatePeriodAsync(int year, int month)
    {
        var response = await _httpClient.PostAsJsonAsync("api/payroll-periods", new CreatePayrollPeriodDto { Year = year, Month = month });
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<PayrollPeriodDto>();
        }
        var err = await response.Content.ReadAsStringAsync();
        throw new Exception($"Không thể tạo kỳ lương: {err}");
    }

    public async Task<UpdatePayrollPeriodStatusDto?> UpdatePeriodStatusAsync(Guid periodId)
    {
        var response = await _httpClient.PostAsync($"api/payroll-period-management/{periodId}/update-status", null);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<UpdatePayrollPeriodStatusDto>();
        }
        var err = await response.Content.ReadAsStringAsync();
        throw new Exception($"Không thể cập nhật trạng thái kỳ lương: {err}");
    }

    // 2. Tra cứu bảng lương theo kỳ
    public async Task<List<PayrollListDto>> GetPayrollsByPeriodAsync(Guid periodId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<PayrollListDto>>($"api/payrolls/periods/{periodId}");
            return response ?? new List<PayrollListDto>();
        }
        catch
        {
            return new List<PayrollListDto>();
        }
    }

    public async Task<PayrollDetailDto?> GetPayrollDetailAsync(Guid payrollId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PayrollDetailDto>($"api/payrolls/{payrollId}");
        }
        catch
        {
            return null;
        }
    }

    // 3. Đồng bộ Chấm công & Lương
    public async Task<int> SyncAttendanceAsync(Guid periodId)
    {
        var response = await _httpClient.PostAsync($"api/payroll-attendance/periods/{periodId}/sync", null);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<int>();
        }
        var err = await response.Content.ReadAsStringAsync();
        throw new Exception($"Lỗi đồng bộ chấm công: {err}");
    }

    public async Task<int> SyncSalaryAsync(Guid periodId)
    {
        var response = await _httpClient.PostAsync($"api/payroll-salary/periods/{periodId}/sync", null);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<int>();
        }
        var err = await response.Content.ReadAsStringAsync();
        throw new Exception($"Lỗi đồng bộ mức lương: {err}");
    }

    // 4. Khởi tạo & Tính lương
    public async Task<bool> CreatePayrollsAsync(Guid periodId, List<Guid> employeeIds)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/payroll-management/periods/{periodId}/payrolls", new CreatePayrollsDto { EmployeeIDs = employeeIds });
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Khởi tạo bảng lương thất bại: {err}");
        }
        return true;
    }

    public async Task<bool> CalculatePayrollAsync(Guid payrollId)
    {
        var response = await _httpClient.PostAsync($"api/payroll-calculation/{payrollId}/calculate", null);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Tính lương thất bại: {err}");
        }
        return true;
    }

    // 5. Phê duyệt & Quyết toán chi trả
    public async Task<bool> ApprovePayrollAsync(Guid payrollId)
    {
        var response = await _httpClient.PostAsync($"api/payroll-approval/{payrollId}/approve", null);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Phê duyệt lương thất bại: {err}");
        }
        return true;
    }

    public async Task<bool> MarkAsPaidAsync(Guid payrollId)
    {
        var response = await _httpClient.PostAsync($"api/payroll-payment/{payrollId}/mark-as-paid", null);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Đánh dấu chi trả thất bại: {err}");
        }
        return true;
    }

    // 6. Điều chỉnh phụ cấp & khấu trừ
    public async Task<bool> AddAllowanceAsync(Guid payrollId, CreateAllowanceDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/payroll-adjustments/{payrollId}/allowances", dto);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Thêm phụ cấp thất bại: {err}");
        }
        return true;
    }

    public async Task<bool> AddPayCutAsync(Guid payrollId, CreatePayCutDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/payroll-adjustments/{payrollId}/paycuts", dto);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            throw new Exception($"Thêm khoản khấu trừ thất bại: {err}");
        }
        return true;
    }

    // 7. Phiếu lương (Payslips)
    public async Task<PayslipDto?> GetPayslipAsync(Guid payrollId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PayslipDto>($"api/payslips/{payrollId}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<PayslipDto>> GetPayslipsByPeriodAsync(Guid periodId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<PayslipDto>>($"api/payslips/periods/{periodId}");
            return response ?? new List<PayslipDto>();
        }
        catch
        {
            return new List<PayslipDto>();
        }
    }

    // 8. Seed dữ liệu mẫu nếu cần
    public async Task SeedSampleDataAsync()
    {
        try
        {
            await _httpClient.PostAsync("health/seed", null);
        }
        catch { }
    }
}
