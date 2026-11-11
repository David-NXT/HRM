using HR.API.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace HR.Tests;

public sealed class InternalDirectoryAuthTests
{
    private const string TestKey = "test-directory-key-do-not-use-in-production";

    [Fact]
    public async Task ConfiguredKey_AllowsMatchingServiceInProduction()
    {
        var context = Context(); context.HttpContext.Request.Headers[InternalEmployeeDirectoryFilter.ApiKeyHeader] = TestKey;
        await Filter("Production", TestKey).OnAuthorizationAsync(context);
        Assert.Null(context.Result);
    }

    [Fact]
    public async Task ConfiguredKey_CannotBeBypassedByHrRole()
    {
        var context = Context(); context.HttpContext.Request.Headers["X-Role"] = "HR";
        await Filter("Development", TestKey).OnAuthorizationAsync(context);
        Assert.IsType<UnauthorizedObjectResult>(context.Result);
    }

    [Fact]
    public async Task WrongKey_ReturnsForbidden()
    {
        var context = Context(); context.HttpContext.Request.Headers[InternalEmployeeDirectoryFilter.ApiKeyHeader] = "wrong";
        await Filter("Production", TestKey).OnAuthorizationAsync(context);
        Assert.Equal(403, Assert.IsType<ObjectResult>(context.Result).StatusCode);
    }

    [Fact]
    public async Task MissingConfiguration_FailsClosedOutsideDevelopment()
    {
        var context = Context(); context.HttpContext.Request.Headers["X-Role"] = "HR";
        await Filter("Production", null).OnAuthorizationAsync(context);
        Assert.Equal(503, Assert.IsType<ObjectResult>(context.Result).StatusCode);
    }

    [Fact]
    public async Task DevelopmentWithoutKey_PreservesExistingHrHeaderCompatibility()
    {
        var context = Context(); context.HttpContext.Request.Headers["X-Role"] = "HR";
        await Filter("Development", null).OnAuthorizationAsync(context);
        Assert.Null(context.Result);
    }

    [Fact]
    public async Task DevelopmentFallback_StillRequiresHrRole()
    {
        var context = Context();
        await Filter("Development", null).OnAuthorizationAsync(context);
        Assert.IsType<UnauthorizedObjectResult>(context.Result);
    }

    private static AuthorizationFilterContext Context() => new(
        new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), new List<IFilterMetadata>());

    private static InternalEmployeeDirectoryFilter Filter(string environment, string? key) => new(
        new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["InternalServices:EmployeeDirectoryApiKey"] = key }).Build(),
        new TestEnvironment { EnvironmentName = environment }, new HrRoleFilter(), NullLogger<InternalEmployeeDirectoryFilter>.Instance);

    private sealed class TestEnvironment : IWebHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Development";
        public string ApplicationName { get; set; } = "HR.Tests";
        public string ContentRootPath { get; set; } = string.Empty;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = string.Empty;
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
