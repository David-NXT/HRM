using System;
using System.Collections.Generic;
using System.IO;
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
        return await GetListAsync<PayrollPeriodDto>("api/payroll-periods");
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
        return await GetListAsync<PayrollListDto>($"api/payrolls/periods/{periodId}");
    }

    public async Task<PayrollDetailDto?> GetPayrollDetailAsync(Guid payrollId)
    {
        return await GetOptionalAsync<PayrollDetailDto>($"api/payrolls/{payrollId}");
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

    public Task<List<PayrollCatalogItemDto>> GetAllowanceTypesAsync(bool activeOnly = false) =>
        GetListAsync<PayrollCatalogItemDto>($"api/payroll-catalog/allowances?activeOnly={activeOnly}");

    public Task<List<PayrollCatalogItemDto>> GetPayCutReasonsAsync(bool activeOnly = false) =>
        GetListAsync<PayrollCatalogItemDto>($"api/payroll-catalog/pay-cuts?activeOnly={activeOnly}");

    public Task<PayrollCatalogItemDto> CreateAllowanceTypeAsync(SavePayrollCatalogItemDto dto) =>
        SaveCatalogItemAsync("api/payroll-catalog/allowances", dto, HttpMethod.Post);

    public Task<PayrollCatalogItemDto> UpdateAllowanceTypeAsync(Guid id, SavePayrollCatalogItemDto dto) =>
        SaveCatalogItemAsync($"api/payroll-catalog/allowances/{id}", dto, HttpMethod.Put);

    public Task DeactivateAllowanceTypeAsync(Guid id) =>
        DeactivateCatalogItemAsync($"api/payroll-catalog/allowances/{id}");

    public Task<PayrollCatalogItemDto> CreatePayCutReasonAsync(SavePayrollCatalogItemDto dto) =>
        SaveCatalogItemAsync("api/payroll-catalog/pay-cuts", dto, HttpMethod.Post);

    public Task<PayrollCatalogItemDto> UpdatePayCutReasonAsync(Guid id, SavePayrollCatalogItemDto dto) =>
        SaveCatalogItemAsync($"api/payroll-catalog/pay-cuts/{id}", dto, HttpMethod.Put);

    public Task DeactivatePayCutReasonAsync(Guid id) =>
        DeactivateCatalogItemAsync($"api/payroll-catalog/pay-cuts/{id}");

    // 7. Phiếu lương (Payslips)
    public async Task<PayslipDto?> GetPayslipAsync(Guid payrollId)
    {
        return await GetOptionalAsync<PayslipDto>($"api/payslips/{payrollId}");
    }

    public async Task<List<PayslipDto>> GetPayslipsByPeriodAsync(Guid periodId)
    {
        return await GetListAsync<PayslipDto>($"api/payslips/periods/{periodId}");
    }

    private async Task<List<T>> GetListAsync<T>(string path)
    {
        using var response = await _httpClient.GetAsync(path);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<List<T>>()
            ?? throw new InvalidDataException("Payroll API trả về danh sách rỗng hoặc không hợp lệ.");
    }

    private async Task<T?> GetOptionalAsync<T>(string path) where T : class
    {
        using var response = await _httpClient.GetAsync(path);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<T>()
            ?? throw new InvalidDataException("Payroll API trả về dữ liệu rỗng hoặc không hợp lệ.");
    }

    private async Task<PayrollCatalogItemDto> SaveCatalogItemAsync(
        string path,
        SavePayrollCatalogItemDto dto,
        HttpMethod method)
    {
        using var request = new HttpRequestMessage(method, path)
        {
            Content = JsonContent.Create(dto)
        };
        using var response = await _httpClient.SendAsync(request);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<PayrollCatalogItemDto>()
            ?? throw new InvalidDataException("Payroll API trả về danh mục rỗng hoặc không hợp lệ.");
    }

    private async Task DeactivateCatalogItemAsync(string path)
    {
        using var response = await _httpClient.DeleteAsync(path);
        await EnsureSuccessAsync(response);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var details = await response.Content.ReadAsStringAsync();
        var message = string.IsNullOrWhiteSpace(details)
            ? $"Payroll API trả về HTTP {(int)response.StatusCode}."
            : $"Payroll API trả về HTTP {(int)response.StatusCode}: {details}";
        throw new HttpRequestException(message, null, response.StatusCode);
    }
}
