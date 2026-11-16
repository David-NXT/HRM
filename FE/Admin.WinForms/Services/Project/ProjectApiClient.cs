using Admin.WinForms.Models.Project;
using System.Net.Http.Json;

namespace Admin.WinForms.Services.Project;

internal sealed class ProjectApiClient
{
    private static readonly HttpClient HttpClient = CreateHttpClient();
    public Uri BaseAddress => HttpClient.BaseAddress!;

    public async Task<List<ProjectDto>> GetProjectsAsync(CancellationToken cancellationToken)
    {
        return await GetAsync<List<ProjectDto>>("api/Projects", cancellationToken);
    }

    public Task<ProjectDto> CreateProjectAsync(ProjectDto project, CancellationToken cancellationToken)
    {
        return PostAsync<ProjectDto, ProjectDto>("api/Projects", project, cancellationToken);
    }

    public Task UpdateProjectAsync(Guid projectId, ProjectDto project, CancellationToken cancellationToken)
    {
        return PutAsync($"api/Projects/{projectId}", project, cancellationToken);
    }

    public Task DeleteProjectAsync(Guid projectId, CancellationToken cancellationToken)
    {
        return DeleteAsync($"api/Projects/{projectId}", cancellationToken);
    }

    public async Task<List<TaskDto>> GetTasksAsync(CancellationToken cancellationToken)
    {
        return await GetAsync<List<TaskDto>>("api/Tasks", cancellationToken);
    }

    public Task<TaskDto> CreateTaskAsync(TaskDto task, CancellationToken cancellationToken)
    {
        return PostAsync<TaskDto, TaskDto>("api/Tasks", task, cancellationToken);
    }

    public Task UpdateTaskAsync(Guid taskId, TaskDto task, CancellationToken cancellationToken)
    {
        return PutAsync($"api/Tasks/{taskId}", task, cancellationToken);
    }

    public Task DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken)
    {
        return DeleteAsync($"api/Tasks/{taskId}", cancellationToken);
    }

    public async Task<List<ProjectMemberDto>> GetProjectMembersAsync(
        Guid? projectId,
        CancellationToken cancellationToken)
    {
        var path = projectId.HasValue
            ? $"api/ProjectMembers?projectId={projectId.Value}"
            : "api/ProjectMembers";
        return await GetAsync<List<ProjectMemberDto>>(path, cancellationToken);
    }

    public async Task<List<ProjectRoleDto>> GetProjectRolesAsync(CancellationToken cancellationToken)
    {
        return await GetAsync<List<ProjectRoleDto>>("api/ProjectRoles", cancellationToken);
    }

    public async Task<List<HourlyRateDto>> GetHourlyRatesAsync(CancellationToken cancellationToken)
    {
        return await GetAsync<List<HourlyRateDto>>("api/HourlyRates", cancellationToken);
    }

    public Task<ProjectMemberCreateResultDto> CreateProjectMemberAsync(
        ProjectMemberDto member,
        CancellationToken cancellationToken)
    {
        return PostAsync<ProjectMemberDto, ProjectMemberCreateResultDto>("api/ProjectMembers", member, cancellationToken);
    }

    public Task UpdateProjectMemberAsync(
        Guid memberId,
        ProjectMemberDto member,
        CancellationToken cancellationToken)
    {
        return PutAsync($"api/ProjectMembers/{memberId}", member, cancellationToken);
    }

    public Task DeleteProjectMemberAsync(Guid memberId, CancellationToken cancellationToken)
    {
        return DeleteAsync($"api/ProjectMembers/{memberId}", cancellationToken);
    }

    public async Task<List<TimesheetDto>> GetTimesheetsAsync(
        Guid? projectId,
        string? status,
        CancellationToken cancellationToken)
    {
        var query = new List<string>();
        if (projectId.HasValue)
        {
            query.Add($"projectId={projectId.Value}");
        }
        if (!string.IsNullOrWhiteSpace(status))
        {
            query.Add($"status={Uri.EscapeDataString(status)}");
        }

        var path = "api/Timesheets";
        if (query.Count > 0)
        {
            path += "?" + string.Join("&", query);
        }

        return await GetAsync<List<TimesheetDto>>(path, cancellationToken);
    }

    public async Task<EmployeeDirectoryDto> GetEmployeeAsync(
        Guid employeeId,
        CancellationToken cancellationToken)
    {
        return await GetAsync<EmployeeDirectoryDto>($"api/employees/{employeeId}", cancellationToken);
    }

    public Task<EmployeeDirectoryPageDto> GetEmployeesAsync(CancellationToken cancellationToken)
    {
        return GetEmployeesAsync(null, cancellationToken);
    }

    public async Task<EmployeeDirectoryPageDto> GetEmployeesAsync(
        string? keyword,
        CancellationToken cancellationToken)
    {
        return await GetEmployeesPageAsync(keyword, 1, 100, cancellationToken);
    }

    public async Task<List<EmployeeDirectoryDto>> GetAllEmployeesAsync(
        CancellationToken cancellationToken)
    {
        const int pageSize = 100;
        var employees = new List<EmployeeDirectoryDto>();
        var page = 1;

        while (true)
        {
            var result = await GetEmployeesPageAsync(null, page, pageSize, cancellationToken);
            employees.AddRange(result.Items);
            if (employees.Count >= result.Total || result.Items.Count == 0)
            {
                return employees;
            }

            page++;
        }
    }

    private Task<EmployeeDirectoryPageDto> GetEmployeesPageAsync(
        string? keyword,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var path = $"api/employees?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            path += $"&keyword={Uri.EscapeDataString(keyword.Trim())}";
        }

        return GetAsync<EmployeeDirectoryPageDto>(path, cancellationToken);
    }

    public Task<TimesheetDto> CreateTimesheetAsync(
        CreateTimesheetRequest request,
        CancellationToken cancellationToken)
    {
        return PostAsync<CreateTimesheetRequest, TimesheetDto>("api/Timesheets", request, cancellationToken);
    }

    public Task UpdateTimesheetAsync(
        Guid timesheetId,
        UpdateTimesheetRequest request,
        CancellationToken cancellationToken)
    {
        return PutAsync($"api/Timesheets/{timesheetId}", request, cancellationToken);
    }

    public Task DeleteTimesheetAsync(Guid timesheetId, CancellationToken cancellationToken)
    {
        return DeleteAsync($"api/Timesheets/{timesheetId}", cancellationToken);
    }

    public Task ReviewTimesheetAsync(
        Guid timesheetId,
        bool approved,
        Guid approvedBy,
        string? reason,
        CancellationToken cancellationToken)
    {
        var path = $"api/Timesheets/{timesheetId}/review?approved={approved.ToString().ToLowerInvariant()}&approvedBy={approvedBy}";
        if (!string.IsNullOrWhiteSpace(reason))
        {
            path += $"&reason={Uri.EscapeDataString(reason.Trim())}";
        }

        return PutAsync(path, new { }, cancellationToken);
    }

    public async Task<List<ApprovedPayrollDataDto>> GetApprovedPayrollDataAsync(
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken,
        Guid? projectId = null)
    {
        var path = $"api/PayrollData?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}";
        if (projectId.HasValue)
        {
            path += $"&projectId={projectId.Value}";
        }
        return await GetAsync<List<ApprovedPayrollDataDto>>(path, cancellationToken);
    }

    private static async Task<T> GetAsync<T>(string path, CancellationToken cancellationToken)
    {
        using var response = await HttpClient.GetAsync(path, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            await ThrowApiErrorAsync(response, cancellationToken);
        }

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken)
            ?? throw new InvalidDataException("Project API returned an empty response.");
    }

    private static async Task<TResponse> PostAsync<TRequest, TResponse>(
        string path,
        TRequest request,
        CancellationToken cancellationToken)
    {
        using var response = await HttpClient.PostAsJsonAsync(path, request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken)
            ?? throw new InvalidDataException("Project API returned an empty response.");
    }

    private static async Task PutAsync<TRequest>(
        string path,
        TRequest request,
        CancellationToken cancellationToken)
    {
        using var response = await HttpClient.PutAsJsonAsync(path, request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    private static async Task DeleteAsync(string path, CancellationToken cancellationToken)
    {
        using var response = await HttpClient.DeleteAsync(path, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    private static Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            return ThrowApiErrorAsync(response, cancellationToken);
        }

        return Task.CompletedTask;
    }

    private static async Task ThrowApiErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var details = await response.Content.ReadAsStringAsync(cancellationToken);
        var message = string.IsNullOrWhiteSpace(details)
            ? $"Project API returned HTTP {(int)response.StatusCode}."
            : $"Project API returned HTTP {(int)response.StatusCode}: {details}";
        throw new HttpRequestException(message, null, response.StatusCode);
    }

    private static HttpClient CreateHttpClient()
    {
        var address = Environment.GetEnvironmentVariable("Services__Project__BaseUrl")
            ?? "http://localhost:5032/";
        if (!Uri.TryCreate(address, UriKind.Absolute, out var uri)
            || uri.Scheme is not ("http" or "https"))
        {
            throw new InvalidOperationException(
                "Project API URL is invalid. Check Services__Project__BaseUrl.");
        }

        return new HttpClient
        {
            BaseAddress = new Uri(uri.AbsoluteUri.TrimEnd('/') + "/"),
            Timeout = TimeSpan.FromSeconds(20)
        };
    }
}
