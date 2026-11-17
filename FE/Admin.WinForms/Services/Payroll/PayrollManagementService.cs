using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Admin.WinForms.Data.Payroll;
using Admin.WinForms.DTOs.Payroll;
using Admin.WinForms.Models.HR;
using Admin.WinForms.Services.HR;

namespace Admin.WinForms.Services.Payroll;

public sealed class PayrollManagementService
{
    private readonly PayrollApiClient _apiClient;
    private readonly PayrollPeriodService _periodService;
    private readonly PayslipService _payslipService;
    private readonly List<Guid> _loadedPeriods = new();
    private readonly List<Guid> _periodsWithEmployeeDetails = new();

    public List<PayrollListDto> Payrolls { get; private set; } = new();

    public PayrollManagementService(
        PayrollApiClient apiClient,
        PayrollPeriodService periodService,
        PayslipService payslipService)
    {
        _apiClient = apiClient;
        _periodService = periodService;
        _payslipService = payslipService;
    }

    /// <summary>
    /// Lấy bảng lương của một kỳ từ list đang lưu; tải từ API nếu kỳ chưa được nạp.
    /// </summary>
    public async Task<List<PayrollListDto>> GetPayrollsByPeriodAsync(
        Guid periodId,
        bool includeEmployeeDetails = true)
    {
        if (!_loadedPeriods.Contains(periodId))
        {
            return await LoadPayrollsByPeriodAsync(periodId, includeEmployeeDetails);
        }

        if (includeEmployeeDetails && !_periodsWithEmployeeDetails.Contains(periodId))
        {
            await LoadEmployeeDetailsAsync(periodId);
        }

        return Payrolls.Where(payroll => payroll.PayrollPeriodID == periodId).ToList();
    }

    /// <summary>
    /// Tải lại các danh sách kỳ lương, bảng lương và phiếu lương đã nạp trước đó.
    /// </summary>
    public async Task RefreshPayrollDataAsync()
    {
        await _periodService.RefreshIfLoadedAsync();
        foreach (var periodId in _loadedPeriods.ToArray())
        {
            await LoadPayrollsByPeriodAsync(
                periodId,
                _periodsWithEmployeeDetails.Contains(periodId));
        }

        await _payslipService.RefreshLoadedAsync();
    }

    /// <summary>
    /// Tính tổng lương ròng và số phiếu đã chi trong tháng theo thời điểm PaidAt.
    /// </summary>
    public async Task<(decimal TotalNetSalary, int PaidPayrollCount)> GetPaidPayrollSummaryForMonthAsync(
        int year,
        int month)
    {
        if (year < 1 || year > 9999)
        {
            throw new ArgumentOutOfRangeException(nameof(year));
        }

        if (month < 1 || month > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month));
        }

        var periods = await _periodService.GetPeriodsAsync();
        foreach (var period in periods)
        {
            await GetPayrollsByPeriodAsync(period.PayrollPeriodID, includeEmployeeDetails: false);
        }

        var paidPayrolls = Payrolls
            .Where(payroll =>
            {
                if (!string.Equals(payroll.Status, "paid", StringComparison.OrdinalIgnoreCase)
                    || !payroll.PaidAt.HasValue)
                {
                    return false;
                }

                var paidAt = payroll.PaidAt.Value.ToLocalTime();
                return paidAt.Year == year && paidAt.Month == month;
            })
            .ToList();

        return (paidPayrolls.Sum(payroll => payroll.NetSalary), paidPayrolls.Count);
    }

    /// <summary>
    /// Lấy chi tiết một bảng lương theo mã bảng lương.
    /// </summary>
    public Task<PayrollDetailDto?> GetPayrollDetailAsync(Guid payrollId) =>
        _apiClient.GetOptionalAsync<PayrollDetailDto>($"api/payrolls/{payrollId}");

    /// <summary>
    /// Đồng bộ dữ liệu chấm công của kỳ lương sang Payroll API.
    /// </summary>
    public async Task<int> SyncAttendanceAsync(Guid periodId)
    {
        var count = await _apiClient.PostForValueAsync<int>(
            $"api/payroll-attendance/periods/{periodId}/sync",
            "Lỗi đồng bộ chấm công");
        await RefreshPayrollListsForPeriodIfLoadedAsync(periodId);
        return count;
    }

    /// <summary>
    /// Đồng bộ mức lương nhân viên vào kỳ lương.
    /// </summary>
    public async Task<int> SyncSalaryAsync(Guid periodId)
    {
        var count = await _apiClient.PostForValueAsync<int>(
            $"api/payroll-salary/periods/{periodId}/sync",
            "Lỗi đồng bộ mức lương");
        await RefreshPayrollListsForPeriodIfLoadedAsync(periodId);
        return count;
    }

    /// <summary>
    /// Tạo bảng lương cho danh sách nhân viên trong kỳ.
    /// </summary>
    public async Task<bool> CreatePayrollsAsync(Guid periodId, List<Guid> employeeIds)
    {
        await _apiClient.PostAsync(
            $"api/payroll-management/periods/{periodId}/payrolls",
            new CreatePayrollsDto { EmployeeIDs = employeeIds },
            "Khởi tạo bảng lương thất bại");
        await RefreshPayrollListsForPeriodIfLoadedAsync(periodId);
        return true;
    }

    /// <summary>
    /// Yêu cầu Payroll API tính lại bảng lương.
    /// </summary>
    public async Task<bool> CalculatePayrollAsync(Guid payrollId)
    {
        await _apiClient.PostAsync(
            $"api/payroll-calculation/{payrollId}/calculate",
            "Tính lương thất bại");
        await RefreshPayrollListsForPayrollIfLoadedAsync(payrollId);
        return true;
    }

    /// <summary>
    /// Phê duyệt bảng lương.
    /// </summary>
    public async Task<bool> ApprovePayrollAsync(Guid payrollId)
    {
        await _apiClient.PostAsync(
            $"api/payroll-approval/{payrollId}/approve",
            "Phê duyệt lương thất bại");
        await RefreshPayrollListsForPayrollIfLoadedAsync(payrollId);
        return true;
    }

    /// <summary>
    /// Đánh dấu bảng lương đã được chi trả.
    /// </summary>
    public async Task<bool> MarkAsPaidAsync(Guid payrollId)
    {
        await _apiClient.PostAsync(
            $"api/payroll-payment/{payrollId}/mark-as-paid",
            "Đánh dấu chi trả thất bại");
        await RefreshPayrollListsForPayrollIfLoadedAsync(payrollId);
        return true;
    }

    /// <summary>
    /// Thêm khoản phụ cấp vào bảng lương.
    /// </summary>
    public async Task<bool> AddAllowanceAsync(Guid payrollId, CreateAllowanceDto dto)
    {
        await _apiClient.PostAsync(
            $"api/payroll-adjustments/{payrollId}/allowances",
            dto,
            "Thêm phụ cấp thất bại");
        await RefreshPayrollListsForPayrollIfLoadedAsync(payrollId);
        return true;
    }

    /// <summary>
    /// Thêm khoản khấu trừ vào bảng lương.
    /// </summary>
    public async Task<bool> AddPayCutAsync(Guid payrollId, CreatePayCutDto dto)
    {
        await _apiClient.PostAsync(
            $"api/payroll-adjustments/{payrollId}/paycuts",
            dto,
            "Thêm khoản khấu trừ thất bại");
        await RefreshPayrollListsForPayrollIfLoadedAsync(payrollId);
        return true;
    }

    private async Task<List<PayrollListDto>> LoadPayrollsByPeriodAsync(
        Guid periodId,
        bool includeEmployeeDetails)
    {
        var payrolls = await _apiClient.GetListAsync<PayrollListDto>(
            $"api/payrolls/periods/{periodId}");
        Payrolls.RemoveAll(payroll => payroll.PayrollPeriodID == periodId);
        Payrolls.AddRange(payrolls);
        if (!_loadedPeriods.Contains(periodId))
        {
            _loadedPeriods.Add(periodId);
        }

        _periodsWithEmployeeDetails.Remove(periodId);
        if (includeEmployeeDetails)
        {
            await LoadEmployeeDetailsAsync(periodId);
        }

        return Payrolls.Where(payroll => payroll.PayrollPeriodID == periodId).ToList();
    }

    private async Task LoadEmployeeDetailsAsync(Guid periodId)
    {
        var payrolls = Payrolls.Where(payroll => payroll.PayrollPeriodID == periodId).ToList();
        if (payrolls.Count > 0)
        {
            using var hrClient = new HrApiClient();
            var employees = await hrClient.AllEmployeesAsync(CancellationToken.None);
            var missingEmployeeIds = new List<Guid>();

            foreach (var payroll in payrolls)
            {
                var employee = employees.FirstOrDefault(item => item.EmployeeId == payroll.EmployeeID);
                if (employee is null)
                {
                    missingEmployeeIds.Add(payroll.EmployeeID);
                    continue;
                }

                payroll.EmployeeCode = employee.EmployeeCode;
                payroll.EmployeeName = employee.FullName;
            }

            if (missingEmployeeIds.Count > 0)
            {
                var ids = string.Join(", ", missingEmployeeIds.Select(id => id.ToString()));
                throw new InvalidDataException(
                    $"Không tìm thấy {missingEmployeeIds.Count} nhân viên trong HR để đối chiếu bảng lương. EmployeeID: {ids}");
            }
        }

        if (!_periodsWithEmployeeDetails.Contains(periodId))
        {
            _periodsWithEmployeeDetails.Add(periodId);
        }
    }

    private async Task RefreshPayrollListsForPeriodIfLoadedAsync(Guid periodId)
    {
        if (_loadedPeriods.Contains(periodId))
        {
            await LoadPayrollsByPeriodAsync(
                periodId,
                _periodsWithEmployeeDetails.Contains(periodId));
        }

        await _payslipService.RefreshIfLoadedAsync(periodId);
    }

    private async Task RefreshPayrollListsForPayrollIfLoadedAsync(Guid payrollId)
    {
        var periodIds = Payrolls
            .Where(payroll => payroll.PayrollID == payrollId)
            .Select(payroll => payroll.PayrollPeriodID)
            .Distinct()
            .ToArray();

        foreach (var periodId in periodIds)
        {
            await RefreshPayrollListsForPeriodIfLoadedAsync(periodId);
        }
    }
}
