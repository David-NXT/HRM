using Microsoft.Extensions.DependencyInjection;
using Payroll.Application.Services.PayrollPeriod;

namespace Payroll.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<PayrollPeriodService>();

        return services;
    }
}