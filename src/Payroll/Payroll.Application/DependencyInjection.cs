using Microsoft.Extensions.DependencyInjection;
using Payroll.Application.Services.PayrollPeriod;
using Payroll.Application.Services.PayrollManagement;
using Payroll.Application.Services.PayrollAdjustment;
using Payroll.Application.Services.PayrollCalculation;
using Payroll.Application.Services.PayrollAttendance;
using Payroll.Application.Services.PayrollQuery;
using Payroll.Application.Services.PayrollApproval;
using Payroll.Application.Services.PayrollPayment;
using Payroll.Application.Services.PayrollPeriodManagement;
using Payroll.Application.Services.Payslip;
using Payroll.Application.Services.PayrollSalarySync;
using Payroll.Application.Services.PayrollCatalog;

namespace Payroll.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<PayrollPeriodService>();
        
        services.AddScoped<PayrollManagementService>();

        services.AddScoped<PayrollAdjustmentService>();
        services.AddScoped<PayrollCatalogService>();

        services.AddScoped<PayrollCalculator>();
        services.AddScoped<PayrollCalculationService>();

        services.AddScoped<PayrollAttendanceService>();

        services.AddScoped<PayrollQueryService>();

        services.AddScoped<PayrollApprovalService>();

        services.AddScoped<PayrollPaymentService>();

        services.AddScoped<PayrollPeriodManagementService>();

        services.AddScoped<PayslipService>();

        services.AddScoped<PayrollSalarySyncService>();

        return services;
    }
}