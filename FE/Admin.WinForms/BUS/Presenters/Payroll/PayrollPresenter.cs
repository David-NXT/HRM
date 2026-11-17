using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Admin.WinForms.BUS.DTOs.Payroll;
using Admin.WinForms.BUS.Services.Payroll;

namespace Admin.WinForms.BUS.Presenters.Payroll;

public sealed class PayrollPresenter
{
    private readonly PayrollPeriodService _periodService;
    private readonly PayrollManagementService _managementService;
    private readonly PayrollCatalogService _catalogService;
    private readonly PayslipService _payslipService;

    public PayrollPresenter(
        PayrollPeriodService periodService,
        PayrollManagementService managementService,
        PayrollCatalogService catalogService,
        PayslipService payslipService)
    {
        _periodService = periodService;
        _managementService = managementService;
        _catalogService = catalogService;
        _payslipService = payslipService;
    }

    public Task<List<PayrollPeriodDto>> GetPeriodsAsync() =>
        _periodService.GetPeriodsAsync();

    public Task<PayrollPeriodDto?> CreatePeriodAsync(int year, int month) =>
        _periodService.CreatePeriodAsync(year, month);

    public Task<UpdatePayrollPeriodStatusDto?> UpdatePeriodStatusAsync(Guid periodId) =>
        _periodService.UpdatePeriodStatusAsync(periodId);

    public Task<List<PayrollListDto>> GetPayrollsByPeriodAsync(
        Guid periodId,
        bool includeEmployeeDetails = true) =>
        _managementService.GetPayrollsByPeriodAsync(periodId, includeEmployeeDetails);

    public Task RefreshPayrollDataAsync() =>
        _managementService.RefreshPayrollDataAsync();

    public Task<(decimal TotalNetSalary, int PaidPayrollCount)> GetPaidPayrollSummaryForMonthAsync(
        int year,
        int month) =>
        _managementService.GetPaidPayrollSummaryForMonthAsync(year, month);

    public Task<PayrollDetailDto?> GetPayrollDetailAsync(Guid payrollId) =>
        _managementService.GetPayrollDetailAsync(payrollId);

    public Task<int> SyncAttendanceAsync(Guid periodId) =>
        _managementService.SyncAttendanceAsync(periodId);

    public Task<int> SyncSalaryAsync(Guid periodId) =>
        _managementService.SyncSalaryAsync(periodId);

    public Task<bool> CreatePayrollsAsync(Guid periodId, List<Guid> employeeIds) =>
        _managementService.CreatePayrollsAsync(periodId, employeeIds);

    public Task<bool> CalculatePayrollAsync(Guid payrollId) =>
        _managementService.CalculatePayrollAsync(payrollId);

    public Task<bool> ApprovePayrollAsync(Guid payrollId) =>
        _managementService.ApprovePayrollAsync(payrollId);

    public Task<bool> MarkAsPaidAsync(Guid payrollId) =>
        _managementService.MarkAsPaidAsync(payrollId);

    public Task<bool> AddAllowanceAsync(Guid payrollId, CreateAllowanceDto dto) =>
        _managementService.AddAllowanceAsync(payrollId, dto);

    public Task<bool> AddPayCutAsync(Guid payrollId, CreatePayCutDto dto) =>
        _managementService.AddPayCutAsync(payrollId, dto);

    public Task<List<PayrollCatalogItemDto>> GetAllowanceTypesAsync(bool activeOnly = false) =>
        _catalogService.GetAllowanceTypesAsync(activeOnly);

    public Task<List<PayrollCatalogItemDto>> GetPayCutReasonsAsync(bool activeOnly = false) =>
        _catalogService.GetPayCutReasonsAsync(activeOnly);

    public Task<PayrollCatalogItemDto> CreateAllowanceTypeAsync(SavePayrollCatalogItemDto dto) =>
        _catalogService.CreateAllowanceTypeAsync(dto);

    public Task<PayrollCatalogItemDto> UpdateAllowanceTypeAsync(Guid id, SavePayrollCatalogItemDto dto) =>
        _catalogService.UpdateAllowanceTypeAsync(id, dto);

    public Task DeactivateAllowanceTypeAsync(Guid id) =>
        _catalogService.DeactivateAllowanceTypeAsync(id);

    public Task<PayrollCatalogItemDto> CreatePayCutReasonAsync(SavePayrollCatalogItemDto dto) =>
        _catalogService.CreatePayCutReasonAsync(dto);

    public Task<PayrollCatalogItemDto> UpdatePayCutReasonAsync(Guid id, SavePayrollCatalogItemDto dto) =>
        _catalogService.UpdatePayCutReasonAsync(id, dto);

    public Task DeactivatePayCutReasonAsync(Guid id) =>
        _catalogService.DeactivatePayCutReasonAsync(id);

    public Task<PayslipDto?> GetPayslipAsync(Guid payrollId) =>
        _payslipService.GetPayslipAsync(payrollId);

    public Task<List<PayslipDto>> GetPayslipsByPeriodAsync(Guid periodId) =>
        _payslipService.GetPayslipsByPeriodAsync(periodId);
}
