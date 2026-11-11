using HR.Application.Interfaces.Repositories;
using HR.Application.Interfaces.Storage;
using HR.Infrastructure.Jobs;
using HR.Infrastructure.Persistence;
using HR.Infrastructure.Repositories;
using HR.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HR.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("HrDatabase");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Missing ConnectionStrings:HrDatabase.");

        services.AddDbContext<HrDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IOrgStructureRepository, OrgStructureRepository>();
        services.AddScoped<IEmploymentRepository, EmploymentRepository>();
        services.AddScoped<IProfileUpdateRepository, ProfileUpdateRepository>();
        services.AddScoped<ILifecycleRepository, LifecycleRepository>();
        services.AddScoped<IContractAlertRepository, ContractAlertRepository>();
        services.AddSingleton<IEmployeeUpdateEvidenceStore, LocalEmployeeUpdateEvidenceStore>();
        services.AddOptions<ContractExpiryJobOptions>().Bind(configuration.GetSection("ContractExpiryJob"));
        services.AddHostedService<ContractExpiryJob>();
        return services;
    }
}
