using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payroll.Infrastructure.Persistence;
using Payroll.Application.Interfaces;

namespace Payroll.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("PayrollConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("PayrollConnection connection string is missing.");
        }

        services.AddDbContext<PayrollDbContext>(options => options.UseNpgsql(connectionString));
        
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<PayrollDbContext>());

        return services;
    }
}