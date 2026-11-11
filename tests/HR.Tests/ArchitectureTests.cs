using HR.API.Auth;
using HR.API.Controllers;
using HR.Application;
using HR.Application.Interfaces.Services;
using HR.Application.Services.ContractAlerts;
using HR.Application.Services.Contracts;
using HR.Application.Services.Lifecycle;
using HR.Application.Services.Organization;
using HR.Application.Services.ProfileUpdates;
using HR.Domain.Entities;
using HR.Infrastructure;
using HR.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace HR.Tests;

public sealed class ArchitectureTests
{
    [Fact]
    public void Layers_KeepDomainAndApplicationIndependentOfApiAndPersistence()
    {
        var domain = typeof(Employee).Assembly;
        var application = typeof(IEmployeeService).Assembly;
        var infrastructure = typeof(HrDbContext).Assembly;
        var api = typeof(EmployeesController).Assembly;

        Assert.Equal(4, new[] { domain, application, infrastructure, api }.Distinct().Count());
        Assert.DoesNotContain(domain.GetReferencedAssemblies(), assembly => assembly.Name!.StartsWith("HR."));
        Assert.Equal(new[] { "HR.Domain" }, application.GetReferencedAssemblies()
            .Where(assembly => assembly.Name!.StartsWith("HR.")).Select(assembly => assembly.Name));
        Assert.DoesNotContain(infrastructure.GetReferencedAssemblies(), assembly => assembly.Name == "HR.API");
    }

    [Fact]
    public void Composition_ResolvesAllControllersAndJobsWithoutDatabaseAccess()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:HrDatabase"] = "Host=localhost;Database=not_used",
            ["ContractExpiryJob:Enabled"] = "false"
        }).Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<IWebHostEnvironment>(new TestEnvironment());
        services.AddApplication();
        services.AddInfrastructure(configuration);
        services.AddScoped<EmployeeIdentityFilter>();
        services.AddScoped<HrRoleFilter>();
        services.AddScoped<InternalEmployeeDirectoryFilter>();
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
        using var scope = provider.CreateScope();

        foreach (var controller in typeof(EmployeesController).Assembly.GetTypes()
            .Where(type => !type.IsAbstract && typeof(ControllerBase).IsAssignableFrom(type)))
            Assert.NotNull(ActivatorUtilities.CreateInstance(scope.ServiceProvider, controller));

        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IEmployeeService>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<OrgStructureService>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<ContractService>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<ProfileUpdateService>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<LifecycleService>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<ContractAlertService>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<EmployeeIdentityFilter>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<InternalEmployeeDirectoryFilter>());
        Assert.Single(provider.GetServices<IHostedService>());
    }

    private sealed class TestEnvironment : IWebHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Development";
        public string ApplicationName { get; set; } = "HR.API";
        public string ContentRootPath { get; set; } = Path.GetTempPath();
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = string.Empty;
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
