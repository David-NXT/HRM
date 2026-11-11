using HR.Application.Interfaces.Services;
using HR.Application.Services.ContractAlerts;
using HR.Application.Services.Contracts;
using HR.Application.Services.Employees;
using HR.Application.Services.Lifecycle;
using HR.Application.Services.Organization;
using HR.Application.Services.ProfileUpdates;
using Microsoft.Extensions.DependencyInjection;

namespace HR.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<OrgStructureService>();
        services.AddScoped<ContractService>();
        services.AddScoped<ProfileUpdateService>();
        services.AddScoped<LifecycleService>();
        services.AddScoped<ContractAlertService>();
        services.AddSingleton<TimeProvider>(TimeProvider.System);
        return services;
    }
}
