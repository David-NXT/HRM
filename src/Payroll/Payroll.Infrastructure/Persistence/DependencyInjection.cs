using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payroll.Application.Interfaces;
using Payroll.Infrastructure.Persistence;
using Payroll.Infrastructure.Services;

namespace Payroll.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register Payroll database.
        var connectionString =
            configuration.GetConnectionString("PayrollConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "PayrollConnection connection string is missing.");
        }

        services.AddDbContext<PayrollDbContext>(
            options => options.UseNpgsql(connectionString));

        services.AddScoped<IApplicationDbContext>(
            provider => provider.GetRequiredService<PayrollDbContext>());

        // Register Attendance HTTP client.
        var attendanceBaseUrl =
            configuration["AttendanceService:BaseUrl"];

        if (string.IsNullOrWhiteSpace(attendanceBaseUrl))
        {
            throw new InvalidOperationException(
                "AttendanceService:BaseUrl is missing.");
        }

        if (!Uri.TryCreate(
                attendanceBaseUrl,
                UriKind.Absolute,
                out var attendanceUri))
        {
            throw new InvalidOperationException(
                "AttendanceService:BaseUrl is not a valid absolute URL.");
        }

        services.AddHttpClient<
            IAttendanceServiceClient,
            AttendanceServiceClient>(client =>
        {
            client.BaseAddress = attendanceUri;
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Register HR HTTP client.
        var hrBaseUrl = configuration["HRService:BaseUrl"];

        if (string.IsNullOrWhiteSpace(hrBaseUrl))
        {
            throw new InvalidOperationException(
                "HRService:BaseUrl is missing.");
        }

        if (!Uri.TryCreate(
                hrBaseUrl,
                UriKind.Absolute,
                out var hrUri))
        {
            throw new InvalidOperationException(
                "HRService:BaseUrl is not a valid absolute URL.");
        }

        services.AddHttpClient<IHRServiceClient, HRServiceClient>(client =>
        {
            client.BaseAddress = hrUri;
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("X-Role", "HR");
        });

        // Register Project HTTP client.
        var projectBaseUrl = configuration["ProjectService:BaseUrl"];

        if (string.IsNullOrWhiteSpace(projectBaseUrl))
        {
            throw new InvalidOperationException(
                "ProjectService:BaseUrl is missing.");
        }

        if (!Uri.TryCreate(
                projectBaseUrl,
                UriKind.Absolute,
                out var projectUri)
            || projectUri.Scheme is not ("http" or "https"))
        {
            throw new InvalidOperationException(
                "ProjectService:BaseUrl is not a valid HTTP or HTTPS URL.");
        }

        services.AddHttpClient<IProjectServiceClient, ProjectServiceClient>(
            client =>
            {
                client.BaseAddress = new Uri(
                    projectUri.AbsoluteUri.TrimEnd('/') + "/");
                client.Timeout = TimeSpan.FromSeconds(30);
            });

        return services;
    }
}