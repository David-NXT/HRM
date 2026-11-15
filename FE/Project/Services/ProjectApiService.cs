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

        throw new HttpRequestException("The project request could not be completed.", null, response.StatusCode);
    }
}
