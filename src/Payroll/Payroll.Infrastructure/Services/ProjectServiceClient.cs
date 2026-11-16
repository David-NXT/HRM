using System.Net.Http.Json;
using Payroll.Application.DTOs.ProjectPayroll;
using Payroll.Application.Interfaces;

namespace Payroll.Infrastructure.Services;

public class ProjectServiceClient : IProjectServiceClient
{
    private readonly HttpClient _httpClient;

    public ProjectServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<ProjectAllowanceSummaryResponse>>
        GetProjectAllowanceSummaryAsync(
            DateOnly fromDate,
            DateOnly toDate,
            CancellationToken cancellationToken = default)
    {
        if (fromDate > toDate)
        {
            throw new ArgumentException(
                "The project allowance period start must not be after its end.");
        }

        var path =
            $"api/PayrollData/summary?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}";

        using var response = await _httpClient.GetAsync(
            path,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var summaries = await response.Content
            .ReadFromJsonAsync<List<ProjectAllowanceSummaryResponse>>(
                cancellationToken: cancellationToken);

        return summaries
            ?? throw new InvalidOperationException(
                "Project Service returned an empty project allowance summary.");
    }
}
