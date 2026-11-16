using System.Net;
using System.Globalization;
using System.Net.Http.Json;
using Project.DTOs;

namespace Project.Services;

internal sealed class AttendanceApiService(HttpClient httpClient)
{
    public async Task<AttendanceWorkHoursDto?> GetWorkHoursAsync(Guid employeeId, DateOnly date)
    {
        var dateText = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var path = $"api/Attendance/work-hours?employeeId={employeeId:D}&date={dateText}";
        using var response = await httpClient.GetAsync(path);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            var details = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(details) ? "Unable to verify attendance." : details,
                null,
                response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<AttendanceWorkHoursDto>()
            ?? throw new InvalidDataException("Attendance service returned no work-hours data.");
    }
}
