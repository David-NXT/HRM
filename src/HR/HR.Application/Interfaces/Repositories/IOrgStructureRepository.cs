using HR.Domain.Entities;

namespace HR.Application.Interfaces.Repositories;

public interface IOrgStructureRepository
{
    Task<IReadOnlyList<Department>> ListDepartmentsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Position>> ListPositionsAsync(CancellationToken cancellationToken);
    Task<Department?> GetDepartmentAsync(Guid id, CancellationToken cancellationToken);
    Task<Position?> GetPositionAsync(Guid id, CancellationToken cancellationToken);
    Task AddDepartmentAsync(Department department, CancellationToken cancellationToken);
    Task AddPositionAsync(Position position, CancellationToken cancellationToken);
    Task<bool> DepartmentCodeExistsAsync(string code, Guid? excludingId, CancellationToken cancellationToken);
    Task<bool> PositionCodeExistsAsync(string code, Guid? excludingId, CancellationToken cancellationToken);
    Task<bool> DepartmentHasActiveChildrenAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> HasActiveEmployeesInDepartmentsAsync(IReadOnlyCollection<Guid> departmentIds, CancellationToken cancellationToken);
    Task<bool> HasActiveEmployeesInPositionAsync(Guid positionId, CancellationToken cancellationToken);
    Task<bool> HasOpenWorkHistoryInDepartmentsAsync(IReadOnlyCollection<Guid> departmentIds, CancellationToken cancellationToken);
    Task<bool> HasOpenWorkHistoryInPositionAsync(Guid positionId, CancellationToken cancellationToken);
    Task<bool> HasCurrentWorkHistoryInDepartmentsAsync(IReadOnlyCollection<Guid> departmentIds, DateOnly effectiveOn, CancellationToken cancellationToken);
    Task<bool> HasCurrentWorkHistoryInPositionAsync(Guid positionId, DateOnly effectiveOn, CancellationToken cancellationToken);
    Task<bool> HasPartTimeAssignmentsInPositionAsync(Guid positionId, CancellationToken cancellationToken) =>
        Task.FromResult(false);
    Task<bool> HasMultipleOpenDirectorAssignmentsAsync(Guid positionId, CancellationToken cancellationToken) =>
        Task.FromResult(false);
    Task<bool> HasDirectorAssignmentInDepartmentAsync(Guid departmentId, DateOnly effectiveDate, Guid? excludingEmployeeId, CancellationToken cancellationToken) =>
        Task.FromResult(false);
    Task UpdateOpenWorkHistoryDirectorFlagsAsync(Guid positionId, bool isDirector, CancellationToken cancellationToken) =>
        Task.CompletedTask;
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
