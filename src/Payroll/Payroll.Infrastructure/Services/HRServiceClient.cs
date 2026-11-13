using System.Net;
using System.Net.Http.Json;
using Payroll.Application.DTOs.HR;
using Payroll.Application.Interfaces;

namespace Payroll.Infrastructure.Services;

public class HRServiceClient : IHRServiceClient
{
    private readonly HttpClient _httpClient;

    public HRServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HREmployeeResponse?> GetEmployeeAsync(
        Guid employeeID,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
            $"employees/{employeeID}",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<HREmployeeResponse>(
                cancellationToken: cancellationToken);
    }
}