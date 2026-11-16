using System.Net.Http.Json;
using Project.DTOs;

namespace Project.Services;

internal sealed class ProjectApiService
{
    private const string ProjectsEndpoint = "api/Projects";
    private readonly HttpClient _httpClient;

    public ProjectApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ProjectDto>> GetAllAsync()
    {
        using var response = await _httpClient.GetAsync(ProjectsEndpoint);
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<List<ProjectDto>>()
            ?? throw new InvalidDataException();
    }

    public async Task<List<EmployeeDirectoryDto>> GetEmployeesAsync()
    {
        var employees = new List<EmployeeDirectoryDto>();
        const int pageSize = 100;

        for (var pageNumber = 1; ; pageNumber++)
        {
            using var response = await _httpClient.GetAsync(
                $"api/employees?page={pageNumber}&pageSize={pageSize}");
            await EnsureSuccessAsync(response);

            var page = await response.Content.ReadFromJsonAsync<EmployeeDirectoryPageDto>()
                ?? throw new InvalidDataException();
            if (page.Items is null)
            {
                throw new InvalidDataException("No employee information was received.");
            }

            employees.AddRange(page.Items);
            if (employees.Count >= page.Total || page.Items.Count == 0)
            {
                return employees;
            }
        }
    }

    public async Task<List<ProjectMemberDto>> GetProjectMembersAsync()
    {
        using var response = await _httpClient.GetAsync("api/ProjectMembers");
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<List<ProjectMemberDto>>()
            ?? throw new InvalidDataException();
    }

    public async Task<List<TaskDto>> GetTasksAsync()
    {
        using var response = await _httpClient.GetAsync("api/Tasks");
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<List<TaskDto>>()
            ?? throw new InvalidDataException();
    }

    public async Task<List<TimesheetResponseDto>> GetTimesheetsAsync(Guid employeeId)
    {
        using var response = await _httpClient.GetAsync($"api/Timesheets?employeeId={employeeId:D}");
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<List<TimesheetResponseDto>>()
            ?? throw new InvalidDataException();
    }

    public async Task CreateTimesheetAsync(CreateTimesheetDto timesheet)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/Timesheets", timesheet);
        await EnsureSuccessAsync(response);
    }

    public async Task CreateAsync(ProjectInputDto project)
    {
        using var response = await _httpClient.PostAsJsonAsync(ProjectsEndpoint, project);
        await EnsureSuccessAsync(response);
    }

    public async Task UpdateAsync(Guid id, ProjectInputDto project)
    {
        using var response = await _httpClient.PutAsJsonAsync($"{ProjectsEndpoint}/{id}", project);
        await EnsureSuccessAsync(response);
    }

    public async Task DeleteAsync(Guid id)
    {
        using var response = await _httpClient.DeleteAsync($"{ProjectsEndpoint}/{id}");
        await EnsureSuccessAsync(response);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var details = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            string.IsNullOrWhiteSpace(details)
                ? "The request could not be completed."
                : details,
            null,
            response.StatusCode);
    }
}
