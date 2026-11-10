using Project.API.DTOs;

namespace Project.API.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskResponseDto>> GetTasksAsync(Guid? projectId = null);
    Task<TaskResponseDto?> GetTaskAsync(Guid id);
    Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto);
    Task<bool> UpdateTaskAsync(Guid id, CreateTaskDto dto);
    Task<bool> DeleteTaskAsync(Guid id);
}