using System.Net;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

namespace Project.API.Services;

public interface IAttendanceWorkHoursClient
{
    Task<AttendanceWorkHours?> GetWorkHoursAsync(Guid employeeId, DateOnly date);
}

public sealed class AttendanceWorkHoursClient(HttpClient httpClient) : IAttendanceWorkHoursClient
{
    public async Task<AttendanceWorkHours?> GetWorkHoursAsync(Guid employeeId, DateOnly date)
    {
        var dateText = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var query = $"api/Attendance/work-hours?employeeId={employeeId:D}&date={dateText}";
        HttpResponseMessage response;

        try
        {
            response = await httpClient.GetAsync(query);
        }
        catch (OperationCanceledException exception)
        {
            throw new HttpRequestException("Attendance service timed out.", exception);
        }

        using (response)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    "Attendance service could not verify recorded work hours.",
                    null,
                    response.StatusCode);
            }

            try
            {
                var result = await response.Content.ReadFromJsonAsync<AttendanceWorkHours>();
                if (result is null || result.EmployeeId != employeeId || result.Date != date)
                {
                    throw new HttpRequestException("Attendance service returned an invalid work-hours response.");
                }

                return result;
            }
            catch (JsonException exception)
            {
                throw new HttpRequestException("Attendance service returned invalid work-hours data.", exception);
            }
        }
    }
}

public sealed class AttendanceWorkHours
{
    public Guid EmployeeId { get; set; }

    public DateOnly Date { get; set; }

    public decimal TotalWorkHours { get; set; }

    public int RecordCount { get; set; }
}
