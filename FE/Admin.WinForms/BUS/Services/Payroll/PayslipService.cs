using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Admin.WinForms.DAO.Payroll;
using Admin.WinForms.BUS.DTOs.Payroll;

namespace Admin.WinForms.BUS.Services.Payroll;

public sealed class PayslipService
{
    private readonly PayrollApiClient _apiClient;
    private readonly PayrollPeriodService _periodService;
    private readonly List<Guid> _loadedPeriods = new();

    public List<PayslipDto> Payslips { get; private set; } = new();

    public PayslipService(PayrollApiClient apiClient, PayrollPeriodService periodService)
    {
        _apiClient = apiClient;
        _periodService = periodService;
    }

    /// <summary>
    /// Lấy phiếu lương của một bảng lương.
    /// </summary>
    public Task<PayslipDto?> GetPayslipAsync(Guid payrollId) =>
        _apiClient.GetOptionalAsync<PayslipDto>($"api/payslips/{payrollId}");

    /// <summary>
    /// Lấy phiếu lương của kỳ và tái sử dụng list đã nạp cho kỳ đó.
    /// </summary>
    public async Task<List<PayslipDto>> GetPayslipsByPeriodAsync(Guid periodId)
    {
        var periods = await _periodService.GetPeriodsAsync();
        var period = periods.FirstOrDefault(item => item.PayrollPeriodID == periodId)
            ?? throw new InvalidDataException($"Không tìm thấy kỳ lương {periodId}.");

        if (!_loadedPeriods.Contains(periodId))
        {
            await LoadByPeriodAsync(periodId);
        }

        return Payslips
            .Where(payslip => payslip.Month == period.Month && payslip.Year == period.Year)
            .ToList();
    }

    internal async Task RefreshIfLoadedAsync(Guid periodId)
    {
        if (_loadedPeriods.Contains(periodId))
        {
            await LoadByPeriodAsync(periodId);
        }
    }

    internal async Task RefreshLoadedAsync()
    {
        foreach (var periodId in _loadedPeriods.ToArray())
        {
            await LoadByPeriodAsync(periodId);
        }
    }

    private async Task LoadByPeriodAsync(Guid periodId)
    {
        var periods = await _periodService.GetPeriodsAsync();
        var period = periods.FirstOrDefault(item => item.PayrollPeriodID == periodId)
            ?? throw new InvalidDataException($"Không tìm thấy kỳ lương {periodId}.");
        var payslips = await _apiClient.GetListAsync<PayslipDto>($"api/payslips/periods/{periodId}");

        Payslips.RemoveAll(payslip => payslip.Month == period.Month && payslip.Year == period.Year);
        Payslips.AddRange(payslips);
        if (!_loadedPeriods.Contains(periodId))
        {
            _loadedPeriods.Add(periodId);
        }
    }
}
