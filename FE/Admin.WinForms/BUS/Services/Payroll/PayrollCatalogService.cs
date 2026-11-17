using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Admin.WinForms.DAO.Payroll;
using Admin.WinForms.BUS.DTOs.Payroll;

namespace Admin.WinForms.BUS.Services.Payroll;

public sealed class PayrollCatalogService
{
    private readonly PayrollApiClient _apiClient;
    private readonly List<CatalogListCache> _allowanceTypeLists = new();
    private readonly List<CatalogListCache> _payCutReasonLists = new();
    private bool _currentAllowanceTypesActiveOnly;
    private bool _currentPayCutReasonsActiveOnly;

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
        var cachedList = _allowanceTypeLists.FirstOrDefault(item => item.ActiveOnly == activeOnly);
        if (cachedList is null)
        {
            cachedList = new CatalogListCache(
                activeOnly,
                await _apiClient.GetListAsync<PayrollCatalogItemDto>(
                    $"api/payroll-catalog/allowances?activeOnly={activeOnly}"));
            _allowanceTypeLists.Add(cachedList);
        }

        _currentAllowanceTypesActiveOnly = activeOnly;
        AllowanceTypes = cachedList.Items;
        return AllowanceTypes;
    }

    /// <summary>
    /// Lấy danh sách lý do khấu trừ, tái sử dụng list đã nạp nếu cùng chế độ lọc.
    /// </summary>
    public async Task<List<PayrollCatalogItemDto>> GetPayCutReasonsAsync(bool activeOnly = false)
    {
        var cachedList = _payCutReasonLists.FirstOrDefault(item => item.ActiveOnly == activeOnly);
        if (cachedList is null)
        {
            cachedList = new CatalogListCache(
                activeOnly,
                await _apiClient.GetListAsync<PayrollCatalogItemDto>(
                    $"api/payroll-catalog/pay-cuts?activeOnly={activeOnly}"));
            _payCutReasonLists.Add(cachedList);
        }

        _currentPayCutReasonsActiveOnly = activeOnly;
        PayCutReasons = cachedList.Items;
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
        foreach (var cachedList in _allowanceTypeLists)
        {
            cachedList.Items = await _apiClient.GetListAsync<PayrollCatalogItemDto>(
                $"api/payroll-catalog/allowances?activeOnly={cachedList.ActiveOnly}");
        }

        AllowanceTypes = _allowanceTypeLists
            .FirstOrDefault(item => item.ActiveOnly == _currentAllowanceTypesActiveOnly)?.Items
            ?? AllowanceTypes;
    }

    private async Task RefreshPayCutReasonsIfLoadedAsync()
    {
        foreach (var cachedList in _payCutReasonLists)
        {
            cachedList.Items = await _apiClient.GetListAsync<PayrollCatalogItemDto>(
                $"api/payroll-catalog/pay-cuts?activeOnly={cachedList.ActiveOnly}");
        }

        PayCutReasons = _payCutReasonLists
            .FirstOrDefault(item => item.ActiveOnly == _currentPayCutReasonsActiveOnly)?.Items
            ?? PayCutReasons;
    }

    private sealed class CatalogListCache
    {
        public bool ActiveOnly { get; }
        public List<PayrollCatalogItemDto> Items { get; set; }

        public CatalogListCache(bool activeOnly, List<PayrollCatalogItemDto> items)
        {
            ActiveOnly = activeOnly;
            Items = items;
        }
    }
}
