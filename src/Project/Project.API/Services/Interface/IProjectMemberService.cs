using Project.API.DTOs;
using Project.API.Entities;

namespace Project.API.Services;

public interface IProjectMemberService
{
    Task<IEnumerable<ProjectMember>> GetProjectMembersAsync(Guid? projectId = null);
    Task<ProjectMember?> GetProjectMemberAsync(Guid id);
    Task<ProjectMember> CreateProjectMemberAsync(CreateProjectMemberDto dto);
    Task<bool> UpdateProjectMemberAsync(Guid id, CreateProjectMemberDto dto);
    Task<bool> DeleteProjectMemberAsync(Guid id);
}