using HR.Application.DTOs.Employees;
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Project.API.Data;
using Project.API.DTOs;
using Project.API.Services;
using Xunit;

namespace HR.Tests;

public sealed class HrProjectContractTests
{
    [Fact]
    public async Task HrPayload_IsReadByRealProjectClientWithGuidAndOrganization()
    {
        var guid = Guid.NewGuid();
        var payload = new HR.Application.DTOs.Employees.EmployeeDirectoryEntry(guid, "NV001", "Nguyễn Văn A", "ACTIVE", "Nhân sự", "Chuyên viên");
        var client = Client(_ => Task.FromResult(JsonResponse(JsonSerializer.Serialize(payload, new JsonSerializerOptions(JsonSerializerDefaults.Web)))));
        var employee = await client.GetActiveEmployeeAsync(guid, CancellationToken.None);

        Assert.NotNull(employee);
        Assert.Equal(guid, employee.EmployeeId);
        Assert.Equal("Nhân sự", employee.DepartmentName);
        Assert.Equal("Chuyên viên", employee.PositionName);
    }

    [Fact]
    public async Task MissingOrNonActiveHrEmployee404_IsNotAssignable()
    {
        var client = Client(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)));
        Assert.Null(await client.GetActiveEmployeeAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Theory]
    [InlineData(401)]
    [InlineData(403)]
    [InlineData(500)]
    [InlineData(503)]
    public async Task HrFailure_DoesNotBecomeValidEmployee(int statusCode)
    {
        var client = Client(_ => Task.FromResult(new HttpResponseMessage((HttpStatusCode)statusCode)));
        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetActiveEmployeeAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task Timeout_IsNormalizedToDirectoryUnavailable()
    {
        var client = Client(_ => throw new TaskCanceledException("Simulated HttpClient timeout"));
        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetActiveEmployeeAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task InvalidJson_IsNotAssignable()
    {
        var client = Client(_ => Task.FromResult(JsonResponse("not JSON")));
        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetActiveEmployeeAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Theory]
    [InlineData("LOCKED")]
    [InlineData("RESIGNED")]
    public async Task UnexpectedNonActive200Response_IsRejected(string status)
    {
        var guid = Guid.NewGuid();
        var body = JsonSerializer.Serialize(new { employeeId = guid, employeeCode = "NV001", fullName = "A", status });
        var client = Client(_ => Task.FromResult(JsonResponse(body)));
        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetActiveEmployeeAsync(guid, CancellationToken.None));
    }

    [Fact]
    public async Task ProjectService_RejectsUnknownHrEmployeeBeforeTrackingAssignment()
    {
        await using var db = new ProjectManagementDbContext(new DbContextOptionsBuilder<ProjectManagementDbContext>()
            .UseNpgsql("Host=localhost;Database=not_used").Options);
        var client = Client(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)));
        var service = new ProjectMemberService(db, client);
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateProjectMemberAsync(new CreateProjectMemberDto
        { EmployeeId = Guid.NewGuid(), ProjectId = Guid.NewGuid(), ProjectRoleId = Guid.NewGuid() }));
        Assert.Empty(db.ChangeTracker.Entries());
    }

    [Fact]
    public async Task WrongGuid200Response_IsRejected()
    {
        var body = JsonSerializer.Serialize(new { employeeId = Guid.NewGuid(), employeeCode = "NV001", fullName = "A", status = "ACTIVE" });
        var client = Client(_ => Task.FromResult(JsonResponse(body)));
        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetActiveEmployeeAsync(Guid.NewGuid(), CancellationToken.None));
    }

    private static HrEmployeeDirectoryClient Client(Func<CancellationToken, Task<HttpResponseMessage>> response) =>
        new(new HttpClient(new StubHandler(response)) { BaseAddress = new Uri("http://hr-contract-test/") });

    private static HttpResponseMessage JsonResponse(string body) => new(HttpStatusCode.OK)
    { Content = new StringContent(body, Encoding.UTF8, "application/json") };

    private sealed class StubHandler(Func<CancellationToken, Task<HttpResponseMessage>> response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => response(cancellationToken);
    }
}
