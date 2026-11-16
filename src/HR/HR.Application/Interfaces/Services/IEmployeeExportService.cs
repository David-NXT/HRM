using HR.Application.DTOs.Employees;

namespace HR.Application.Interfaces.Services;

public interface IEmployeeExportService
{
    Task<EmployeeExportFile> ExportAsync(string? keyword, CancellationToken cancellationToken);
}
