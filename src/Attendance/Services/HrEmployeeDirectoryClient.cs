using System.Net;
using System.Net.Http.Json;

namespace Attendance.Services;

public interface IHrEmployeeDirectoryClient
{
    Task<EmployeeDirectoryPage> GetActiveEmployeesAsync(string? keyword, int page, int pageSize, CancellationToken cancellationToken);
    Task<EmployeeDirectoryEntry?> GetActiveEmployeeAsync(Guid employeeId, CancellationToken cancellationToken);
}

public sealed class HrEmployeeDirectoryClient(HttpClient httpClient) : IHrEmployeeDirectoryClient
{
    public async Task<EmployeeDirectoryPage> GetActiveEmployeesAsync(
        string? keyword,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = $"?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query += "&keyword=" + Uri.EscapeDataString(keyword.Trim());
        }

        using var response = await httpClient.GetAsync("internal/employees" + query, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EmployeeDirectoryPage>(cancellationToken)
            ?? throw new InvalidDataException("HR returned an empty employee directory response.");
    }

    public async Task<EmployeeDirectoryEntry?> GetActiveEmployeeAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync($"internal/employees/{employeeId}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EmployeeDirectoryEntry>(cancellationToken);
    }
}

public sealed record EmployeeDirectoryEntry(Guid EmployeeId, string EmployeeCode, string FullName, string Status);

public sealed record EmployeeDirectoryPage(
    IReadOnlyList<EmployeeDirectoryEntry> Items,
    int Page,
    int PageSize,
    int Total);

public sealed class EmployeeNotAvailableException(Guid employeeId)
    : ArgumentException($"Employee {employeeId} does not exist or is not active in HR.", nameof(employeeId));
