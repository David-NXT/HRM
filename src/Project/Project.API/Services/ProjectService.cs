using Microsoft.EntityFrameworkCore;
using Project.API.Data;
using Project.API.DTOs;
using ProjectEntity = Project.API.Entities.Project;

namespace Project.API.Services;

public class ProjectService : IProjectService
{
    private readonly ProjectManagementDbContext _context;

    public ProjectService(ProjectManagementDbContext context)
    {
        _context = context;
    }

    // Lấy danh sách dự án
    public async Task<IEnumerable<ProjectResponseDto>> GetProjectsAsync()
    {
        return await _context.Projects
            .AsNoTracking()
            .Select(p => new ProjectResponseDto
            {
                ProjectId = p.ProjectId,
                ProjectName = p.ProjectName,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status,
                Budget = p.Budget
            })
            .ToListAsync();
    }

    // Lấy thông tin một dự án
    public async Task<ProjectResponseDto?> GetProjectAsync(Guid id)
    {
        return await _context.Projects
            .AsNoTracking()
            .Where(p => p.ProjectId == id)
            .Select(p => new ProjectResponseDto
            {
                ProjectId = p.ProjectId,
                ProjectName = p.ProjectName,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status,
                Budget = p.Budget
            })
            .FirstOrDefaultAsync();
    }

    // Tạo dự án
    public async Task<ProjectResponseDto> CreateProjectAsync(
        CreateProjectDto dto)
    {
        var project = new ProjectEntity
        {
            ProjectId = Guid.NewGuid(),
            ProjectName = dto.ProjectName,
            Description = dto.Description,
            StartDate = NormalizeUtc(dto.StartDate),
            EndDate = NormalizeUtc(dto.EndDate),
            Status = dto.Status,
            Budget = dto.Budget
        };

        _context.Projects.Add(project);

        await _context.SaveChangesAsync();

        return new ProjectResponseDto
        {
            ProjectId = project.ProjectId,
            ProjectName = project.ProjectName,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Status = project.Status,
            Budget = project.Budget
        };
    }

    // Cập nhật dự án
    public async Task<bool> UpdateProjectAsync(
        Guid id,
        UpdateProjectDto dto)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project == null)
        {
            return false;
        }
        EnsureProjectEditable(project);


        project.ProjectName = dto.ProjectName;
        project.Description = dto.Description;
        project.StartDate = NormalizeUtc(dto.StartDate);
        project.EndDate = NormalizeUtc(dto.EndDate);
        project.Status = dto.Status;
        project.Budget = dto.Budget;

        await _context.SaveChangesAsync();

        return true;
    }

    private static DateTime NormalizeUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }

    private static DateTime? NormalizeUtc(DateTime? value)
    {
        return value.HasValue ? NormalizeUtc(value.Value) : null;
    }

    // Xóa dự án
    public async Task<bool> DeleteProjectAsync(Guid id)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project == null)
        {
            return false;
        }
        EnsureProjectEditable(project);

        _context.Projects.Remove(project);

        await _context.SaveChangesAsync();

        return true;
    }
    private static void EnsureProjectEditable(ProjectEntity project)
{
    if (project.Status == "Completed" ||
        project.Status == "Cancelled")
    {
        throw new InvalidOperationException(
            "Không thể thay đổi dự án đã hoàn thành hoặc đã hủy.");
    }
}
}