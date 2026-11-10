using Project.API.DTOs;

namespace Project.API.Services;

public interface IProjectService
{
    Task<IEnumerable<ProjectResponseDto>> GetProjectsAsync();

    Task<ProjectResponseDto?> GetProjectAsync(Guid id);

    Task<ProjectResponseDto> CreateProjectAsync(CreateProjectDto dto);

    Task<bool> UpdateProjectAsync(Guid id, UpdateProjectDto dto);

    Task<bool> DeleteProjectAsync(Guid id);
}