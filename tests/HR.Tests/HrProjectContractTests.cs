    using HR.Application.DTOs.Employees;
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Text;
    using System.Text.Json;
    using Microsoft.EntityFrameworkCore;
    using Project.API.Data;
    using Project.API.DTOs;
    using Project.API.Services;
    using Xunit;
    using ProjectHourlyRate = Project.API.Entities.HourlyRate;

    namespace HR.Tests;

    public sealed class HrProjectContractTests
    {
        [Fact]
        public async Task HrPayload_IsReadByRealProjectClientWithGuidAndOrganization()
        {
            var guid = Guid.NewGuid();
            var payload = new HR.Application.DTOs.Employees.EmployeeDirectoryEntry(guid, "NV001", "Nguyễn Văn A", "ACTIVE", "Nhân sự", "Chuyên viên", "PART_TIME");
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

        [Fact]
        public void HourlyRate_IsEffectiveAt_UsesCalendarDateOnly()
        {
            var rate = new ProjectHourlyRate
            {
                EffectiveFrom = new DateTime(2026, 11, 14, 17, 0, 0, DateTimeKind.Utc),
                EffectiveTo = new DateTime(2026, 11, 15, 17, 0, 0, DateTimeKind.Utc)
            };

            Assert.True(rate.IsEffectiveAt(new DateTime(2026, 11, 14, 0, 0, 0, DateTimeKind.Utc)));
            Assert.True(rate.IsEffectiveAt(new DateTime(2026, 11, 15, 0, 0, 0, DateTimeKind.Utc)));
            Assert.False(rate.IsEffectiveAt(new DateTime(2026, 11, 13, 23, 59, 0, DateTimeKind.Utc)));
        }

        [Fact]
        public void ProjectDto_RequiresEndDate()
        {
            var dto = new CreateProjectDto
            {
                ProjectName = "Test project",
                StartDate = new DateOnly(2026, 11, 1)
            };
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(
                dto,
                new ValidationContext(dto),
                results,
                validateAllProperties: true);

            Assert.False(isValid);
            Assert.Contains(results, result =>
                result.MemberNames.Contains(nameof(CreateProjectDto.EndDate)));
        }

        [Fact]
        public void ProjectMemberDto_RequiresLeaveDate()
        {
            var dto = new CreateProjectMemberDto
            {
                ProjectId = Guid.NewGuid(),
                EmployeeId = Guid.NewGuid(),
                ProjectRoleId = Guid.NewGuid(),
                JoinDate = new DateTime(2026, 11, 1)
            };
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(
                dto,
                new ValidationContext(dto),
                results,
                validateAllProperties: true);

            Assert.False(isValid);
            Assert.Contains(results, result =>
                result.MemberNames.Contains(nameof(CreateProjectMemberDto.LeaveDate)));
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

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task ProjectService_RejectsNonPositiveInitialHourlyRate(decimal hourlyRate)
        {
            await using var db = new ProjectManagementDbContext(new DbContextOptionsBuilder<ProjectManagementDbContext>()
                .UseNpgsql("Host=localhost;Database=not_used").Options);
            var client = Client(_ => throw new InvalidOperationException("HR lookup should not be reached."));
            var service = new ProjectMemberService(db, client);

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateProjectMemberAsync(new CreateProjectMemberDto
            {
                EmployeeId = Guid.NewGuid(),
                ProjectId = Guid.NewGuid(),
                ProjectRoleId = Guid.NewGuid(),
                InitialHourlyRate = hourlyRate
            }));

            Assert.Empty(db.ChangeTracker.Entries());
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(10)]
        public async Task ProjectService_RejectsOutOfRangeNewRoleCoefficient(decimal coefficient)
        {
            await using var db = new ProjectManagementDbContext(new DbContextOptionsBuilder<ProjectManagementDbContext>()
                .UseNpgsql("Host=localhost;Database=not_used").Options);
            var client = Client(_ => throw new InvalidOperationException("HR lookup should not be reached."));
            var service = new ProjectMemberService(db, client);

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateProjectMemberAsync(new CreateProjectMemberDto
            {
                EmployeeId = Guid.NewGuid(),
                ProjectId = Guid.NewGuid(),
                ProjectRoleName = "Test Role",
                NewRoleCoefficient = coefficient
            }));

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
