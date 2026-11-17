using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Admin.WinForms.DAO.Payroll;
using Admin.WinForms.BUS.DTOs.Payroll;

namespace Admin.WinForms.BUS.Services.Payroll;

public sealed class PayrollPeriodService
{
    private readonly PayrollApiClient _apiClient;

    public List<PayrollPeriodDto>? PayrollPeriods { get; private set; }

    public PayrollPeriodService(PayrollApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    /// <summary>
    /// Lấy danh sách kỳ lương; tái sử dụng danh sách đã tải trong phiên ứng dụng.
    /// </summary>
    public async Task<List<PayrollPeriodDto>> GetPeriodsAsync()
    {
        PayrollPeriods ??= await _apiClient.GetListAsync<PayrollPeriodDto>("api/payroll-periods");
        return PayrollPeriods;
    }

    /// <summary>
    /// Tạo kỳ lương mới và làm mới danh sách kỳ lương nếu đã tải.
    /// </summary>
    public async Task<PayrollPeriodDto?> CreatePeriodAsync(int year, int month)
    {
        var period = await _apiClient.PostForValueAsync<PayrollPeriodDto>(
            "api/payroll-periods",
            new CreatePayrollPeriodDto { Year = year, Month = month },
            "Không thể tạo kỳ lương");
        await RefreshIfLoadedAsync();
        return period;
    }

    /// <summary>
    /// Cập nhật trạng thái kỳ lương và làm mới danh sách kỳ lương nếu đã tải.
    /// </summary>
    public async Task<UpdatePayrollPeriodStatusDto?> UpdatePeriodStatusAsync(Guid periodId)
    {
        var result = await _apiClient.PostForValueAsync<UpdatePayrollPeriodStatusDto>(
            $"api/payroll-period-management/{periodId}/update-status",
            "Không thể cập nhật trạng thái kỳ lương");
        await RefreshIfLoadedAsync();
        return result;
    }

    internal async Task RefreshIfLoadedAsync()
    {
        if (PayrollPeriods is not null)
        {
            PayrollPeriods = await _apiClient.GetListAsync<PayrollPeriodDto>("api/payroll-periods");
        }
    }
}
