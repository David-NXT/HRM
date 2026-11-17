using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Admin.WinForms.Data.Payroll;
using Admin.WinForms.DTOs.Payroll;

namespace Admin.WinForms.Services.Payroll;

public sealed class PayrollCatalogService
{
    private readonly PayrollApiClient _apiClient;
    private bool _allowanceTypesLoaded;
    private bool _allowanceTypesActiveOnly;
    private bool _payCutReasonsLoaded;
    private bool _payCutReasonsActiveOnly;

    public List<PayrollCatalogItemDto> AllowanceTypes { get; private set; } = new();
    public List<PayrollCatalogItemDto> PayCutReasons { get; private set; } = new();

    public PayrollCatalogService(PayrollApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    /// <summary>
    /// Lấy danh sách loại phụ cấp, tái sử dụng list đã nạp nếu cùng chế độ lọc.
    /// </summary>
    public async Task<List<PayrollCatalogItemDto>> GetAllowanceTypesAsync(bool activeOnly = false)
    {
        if (!_allowanceTypesLoaded || _allowanceTypesActiveOnly != activeOnly)
        {
            AllowanceTypes = await _apiClient.GetListAsync<PayrollCatalogItemDto>(
                $"api/payroll-catalog/allowances?activeOnly={activeOnly}");
            _allowanceTypesActiveOnly = activeOnly;
            _allowanceTypesLoaded = true;
        }

        return AllowanceTypes;
    }

    /// <summary>
    /// Lấy danh sách lý do khấu trừ, tái sử dụng list đã nạp nếu cùng chế độ lọc.
    /// </summary>
    public async Task<List<PayrollCatalogItemDto>> GetPayCutReasonsAsync(bool activeOnly = false)
    {
        if (!_payCutReasonsLoaded || _payCutReasonsActiveOnly != activeOnly)
        {
            PayCutReasons = await _apiClient.GetListAsync<PayrollCatalogItemDto>(
                $"api/payroll-catalog/pay-cuts?activeOnly={activeOnly}");
            _payCutReasonsActiveOnly = activeOnly;
            _payCutReasonsLoaded = true;
        }

        return PayCutReasons;
    }

    /// <summary>
    /// Tạo loại phụ cấp mới và làm mới list danh mục đã nạp.
    /// </summary>
    public async Task<PayrollCatalogItemDto> CreateAllowanceTypeAsync(SavePayrollCatalogItemDto dto)
    {
        var item = await _apiClient.SaveAsync<PayrollCatalogItemDto>(
            "api/payroll-catalog/allowances",
            dto,
            HttpMethod.Post);
        await RefreshAllowanceTypesIfLoadedAsync();
        return item;
    }

    /// <summary>
    /// Cập nhật loại phụ cấp và làm mới list danh mục đã nạp.
    /// </summary>
    public async Task<PayrollCatalogItemDto> UpdateAllowanceTypeAsync(
        Guid id,
        SavePayrollCatalogItemDto dto)
    {
        var item = await _apiClient.SaveAsync<PayrollCatalogItemDto>(
            $"api/payroll-catalog/allowances/{id}",
            dto,
            HttpMethod.Put);
        await RefreshAllowanceTypesIfLoadedAsync();
        return item;
    }

    /// <summary>
    /// Ngừng kích hoạt loại phụ cấp và làm mới list danh mục đã nạp.
    /// </summary>
    public async Task DeactivateAllowanceTypeAsync(Guid id)
    {
        await _apiClient.DeleteAsync($"api/payroll-catalog/allowances/{id}");
        await RefreshAllowanceTypesIfLoadedAsync();
    }

    /// <summary>
    /// Tạo lý do khấu trừ mới và làm mới list danh mục đã nạp.
    /// </summary>
    public async Task<PayrollCatalogItemDto> CreatePayCutReasonAsync(SavePayrollCatalogItemDto dto)
    {
        var item = await _apiClient.SaveAsync<PayrollCatalogItemDto>(
            "api/payroll-catalog/pay-cuts",
            dto,
            HttpMethod.Post);
        await RefreshPayCutReasonsIfLoadedAsync();
        return item;
    }

    /// <summary>
    /// Cập nhật lý do khấu trừ và làm mới list danh mục đã nạp.
    /// </summary>
    public async Task<PayrollCatalogItemDto> UpdatePayCutReasonAsync(
        Guid id,
        SavePayrollCatalogItemDto dto)
    {
        var item = await _apiClient.SaveAsync<PayrollCatalogItemDto>(
            $"api/payroll-catalog/pay-cuts/{id}",
            dto,
            HttpMethod.Put);
        await RefreshPayCutReasonsIfLoadedAsync();
        return item;
    }

    /// <summary>
    /// Ngừng kích hoạt lý do khấu trừ và làm mới list danh mục đã nạp.
    /// </summary>
    public async Task DeactivatePayCutReasonAsync(Guid id)
    {
        await _apiClient.DeleteAsync($"api/payroll-catalog/pay-cuts/{id}");
        await RefreshPayCutReasonsIfLoadedAsync();
    }

    private async Task RefreshAllowanceTypesIfLoadedAsync()
    {
        if (_allowanceTypesLoaded)
        {
            AllowanceTypes = await _apiClient.GetListAsync<PayrollCatalogItemDto>(
                $"api/payroll-catalog/allowances?activeOnly={_allowanceTypesActiveOnly}");
        }
    }

    private async Task RefreshPayCutReasonsIfLoadedAsync()
    {
        if (_payCutReasonsLoaded)
        {
            PayCutReasons = await _apiClient.GetListAsync<PayrollCatalogItemDto>(
                $"api/payroll-catalog/pay-cuts?activeOnly={_payCutReasonsActiveOnly}");
        }
    }
}
