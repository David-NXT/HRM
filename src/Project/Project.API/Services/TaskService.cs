using Microsoft.EntityFrameworkCore;
using Project.API.Data;
using Project.API.DTOs;
using ProjectTask = Project.API.Entities.Task;

namespace Project.API.Services;

public class TaskService : ITaskService
{
    private readonly ProjectManagementDbContext _context;

    public TaskService(ProjectManagementDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskResponseDto>> GetTasksAsync(Guid? projectId = null)
    {
        var query = _context.Tasks.AsNoTracking();

        if (projectId.HasValue)
            query = query.Where(t => t.ProjectId == projectId.Value);

        return await query.Select(t => new TaskResponseDto
        {
            TaskId = t.TaskId,
            TaskName = t.TaskName,
            Description = t.Description,
            StartDate = t.StartDate,
            DueDate = t.DueDate,
            Status = t.Status,
            Priority = t.Priority,
            ProjectId = t.ProjectId
        }).ToListAsync();
    }

    public async Task<TaskResponseDto?> GetTaskAsync(Guid id)
    {
        return await _context.Tasks
            .AsNoTracking()
            .Where(t => t.TaskId == id)
            .Select(t => new TaskResponseDto
            {
                TaskId = t.TaskId,
                TaskName = t.TaskName,
                Description = t.Description,
                StartDate = t.StartDate,
                DueDate = t.DueDate,
                Status = t.Status,
                Priority = t.Priority,
                ProjectId = t.ProjectId
            })
            .FirstOrDefaultAsync();
    }

    public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto)
    {
        await EnsureProjectEditableAsync(dto.ProjectId);
        var task = new ProjectTask
        {
            TaskId = Guid.NewGuid(),
            TaskName = dto.TaskName,
            Description = dto.Description,
            StartDate = dto.StartDate,
            DueDate = dto.DueDate,
            Status = dto.Status,
            Priority = dto.Priority,
            ProjectId = dto.ProjectId
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return new TaskResponseDto
        {
            TaskId = task.TaskId,
            TaskName = task.TaskName,
            Description = task.Description,
            StartDate = task.StartDate,
            DueDate = task.DueDate,
            Status = task.Status,
            Priority = task.Priority,
            ProjectId = task.ProjectId
        };
    }

    public async Task<bool> UpdateTaskAsync(Guid id, CreateTaskDto dto)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
            return false;
        // Kiểm tra dự án hiện tại
        await EnsureProjectEditableAsync(task.ProjectId);

        // Nếu chuyển Task sang dự án khác, kiểm tra cả dự án mới
        if (task.ProjectId != dto.ProjectId)
        {
            await EnsureProjectEditableAsync(dto.ProjectId);
        }
        task.TaskName = dto.TaskName;
        task.Description = dto.Description;
        task.StartDate = dto.StartDate;
        task.DueDate = dto.DueDate;
        task.Status = dto.Status;
        task.Priority = dto.Priority;
        task.ProjectId = dto.ProjectId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTaskAsync(Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
            return false;
        await EnsureProjectEditableAsync(task.ProjectId);
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return true;
    }
    private async Task EnsureProjectEditableAsync(Guid projectId)
{
    var project = await _context.Projects
        .FirstOrDefaultAsync(p => p.ProjectId == projectId);

    if (project == null)
        throw new ArgumentException("Project does not exist.");

    if (project.Status == "Completed" ||
        project.Status == "Cancelled")
    {
        throw new InvalidOperationException(
            "Không thể thay đổi công việc của dự án đã hoàn thành hoặc đã hủy.");
    }
}
}
