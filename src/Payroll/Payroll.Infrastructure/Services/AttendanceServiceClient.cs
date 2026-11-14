using System.Net;
using System.Net.Http.Json;
using Payroll.Application.DTOs.PayrollAttendance;
using Payroll.Application.Interfaces;

namespace Payroll.Infrastructure.Services;

public class AttendanceServiceClient : IAttendanceServiceClient //để định dạng dữ liệu cần trả về
{
    private readonly HttpClient _httpClient;

    public AttendanceServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AttendanceEmployeeSummaryResponse?>
        GetEmployeeMonthlySummaryAsync(
            Guid employeeID,
            int month,
            int year,
            CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
            $"api/timesheets/employee-summary?employeeId={employeeID}&month={month}&year={year}",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<AttendanceEmployeeSummaryResponse>(
                cancellationToken: cancellationToken);
    }

    public async Task<decimal> GetOvertimeMultiplierAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
            "api/OvertimeRules/multiplier",
            cancellationToken);
    
        response.EnsureSuccessStatusCode();
    
        var result = await response.Content
            .ReadFromJsonAsync<OvertimeMultiplierResponse>(
                cancellationToken: cancellationToken);
    
        if (result == null || result.Multiplier <= 0)
        {
            throw new InvalidOperationException(
                "Invalid overtime multiplier returned by Attendance Service.");
        }
    
        return result.Multiplier;
    }
}