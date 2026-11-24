using System.Net;
using System.Net.Http.Json;

namespace Attendance.Services;

public interface IHrEmployeeDirectoryClient
{
    Task<EmployeeDirectoryPage> GetActiveEmployeesAsync(string? keyword, int page, int pageSize, CancellationToken cancellationToken);
    Task<List<EmployeeDirectoryEntry>> GetAllActiveEmployeesAsync(CancellationToken cancellationToken = default);
    Task<EmployeeDirectoryEntry?> GetActiveEmployeeAsync(Guid employeeId, CancellationToken cancellationToken);
    Task<IReadOnlyList<HrDepartmentDto>> GetDepartmentsAsync(CancellationToken cancellationToken);
}

public sealed class HrEmployeeDirectoryClient(HttpClient httpClient) : IHrEmployeeDirectoryClient
{
    public async Task<EmployeeDirectoryPage> GetActiveEmployeesAsync(
        string? keyword,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        int validPageSize = Math.Clamp(pageSize, 1, 100);
        string query = $"?page={page}&pageSize={validPageSize}";
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query += "&keyword=" + Uri.EscapeDataString(keyword.Trim());
        }

        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "internal/employees" + query);
        request.Headers.TryAddWithoutValidation("X-Role", "HR");
        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EmployeeDirectoryPage>(cancellationToken)
            ?? throw new InvalidDataException("HR returned an empty employee directory response.");
    }

    public async Task<List<EmployeeDirectoryEntry>> GetAllActiveEmployeesAsync(CancellationToken cancellationToken = default)
    {
        List<EmployeeDirectoryEntry> allEmployees = new List<EmployeeDirectoryEntry>();
        int page = 1;
        while (true)
        {
            EmployeeDirectoryPage? p = await GetActiveEmployeesAsync(null, page, 100, cancellationToken);
            if (p?.Items == null || p.Items.Count == 0)
            {
                break;
            }
            allEmployees.AddRange(p.Items);
            if (allEmployees.Count >= p.Total || p.Items.Count < 100)
            {
                break;
            }
            page++;
        }
        return allEmployees;
    }

    public async Task<EmployeeDirectoryEntry?> GetActiveEmployeeAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"internal/employees/{employeeId}");
        request.Headers.TryAddWithoutValidation("X-Role", "HR");
        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EmployeeDirectoryEntry>(cancellationToken);
    }

    public async Task<IReadOnlyList<HrDepartmentDto>> GetDepartmentsAsync(CancellationToken cancellationToken)
    {
        try
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "departments");
            request.Headers.TryAddWithoutValidation("X-Role", "HR");
            using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return Array.Empty<HrDepartmentDto>();
            }

            List<HrDepartmentDto>? list = await response.Content.ReadFromJsonAsync<List<HrDepartmentDto>>(cancellationToken);
            return list ?? (IReadOnlyList<HrDepartmentDto>)Array.Empty<HrDepartmentDto>();
        }
        catch
        {
            return Array.Empty<HrDepartmentDto>();
        }
    }
}

public sealed class HrDepartmentDto
{
    [System.Text.Json.Serialization.JsonPropertyName("departmentId")]
    public Guid DepartmentId { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public Guid Id
    {
        get => DepartmentId;
        set => DepartmentId = value;
    }

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public HrDepartmentDto() { }

    public HrDepartmentDto(Guid id, string code, string name, string status)
    {
        Id = id;
        DepartmentId = id;
        Code = code;
        Name = name;
        Status = status;
    }
}

public sealed record EmployeeDirectoryEntry(
    Guid EmployeeId,
    string EmployeeCode,
    string FullName,
    string Status,
    string? DepartmentName = null,
    string? PositionName = null);

public sealed record EmployeeDirectoryPage(
    IReadOnlyList<EmployeeDirectoryEntry> Items,
    int Page,
    int PageSize,
    int Total);

public sealed class EmployeeNotAvailableException(Guid employeeId)
    : ArgumentException($"Employee {employeeId} does not exist or is not active in HR.", nameof(employeeId));
