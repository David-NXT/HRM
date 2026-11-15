using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Project.API.Services;

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

        using var response = await GetResponseAsync("internal/employees" + query, cancellationToken);
        response.EnsureSuccessStatusCode();
        var pageResult = await ReadRequiredAsync<EmployeeDirectoryPage>(response, cancellationToken);
        if (pageResult.Items is null || pageResult.Items.Any(item =>
                item is null || !string.Equals(item.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase)))
            throw new HttpRequestException("HR returned an invalid employee directory response.");
        return pageResult;
    }

    public async Task<EmployeeDirectoryEntry?> GetActiveEmployeeAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        using var response = await GetResponseAsync($"internal/employees/{employeeId}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        var employee = await ReadRequiredAsync<EmployeeDirectoryEntry>(response, cancellationToken);
        if (employee.EmployeeId != employeeId
            || !string.Equals(employee.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
            throw new HttpRequestException("HR returned an employee who is not assignable.");
        return employee;
    }

    private async Task<HttpResponseMessage> GetResponseAsync(string path, CancellationToken cancellationToken)
    {
        try { return await httpClient.GetAsync(path, cancellationToken); }
        catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        { throw new HttpRequestException("HR employee directory timed out.", exception); }
    }

    private static async Task<T> ReadRequiredAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            return await response.Content.ReadFromJsonAsync<T>(cancellationToken)
                ?? throw new HttpRequestException("HR returned an empty employee directory response.");
        }
        catch (JsonException exception)
        { throw new HttpRequestException("HR returned invalid employee directory JSON.", exception); }
    }
}

public sealed record EmployeeDirectoryEntry(Guid EmployeeId, string EmployeeCode, string FullName, string Status,
    string? DepartmentName = null, string? PositionName = null);

public sealed record EmployeeDirectoryPage(
    IReadOnlyList<EmployeeDirectoryEntry> Items,
    int Page,
    int PageSize,
    int Total);
