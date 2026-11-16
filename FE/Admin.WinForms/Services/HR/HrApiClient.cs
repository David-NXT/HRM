using Admin.WinForms.Models.HR;
using System.Net.Http.Json;
using System.Text.Json;

namespace Admin.WinForms.Services.HR;

public sealed class HrApiClient : IDisposable
{
    private readonly HttpClient _http;
    public Uri BaseAddress => _http.BaseAddress!;

    public HrApiClient() : this(new HttpClient
    {
        BaseAddress = GetBaseAddress(), Timeout = TimeSpan.FromSeconds(20)
    }) { }

    public HrApiClient(HttpClient http) => _http = http;

    private static Uri GetBaseAddress()
    {
        var address = Environment.GetEnvironmentVariable("Services__HR__BaseUrl") ?? "http://localhost:5012/";
        if (!Uri.TryCreate(address, UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https"))
            throw new InvalidOperationException("Địa chỉ HR API không hợp lệ. Kiểm tra Services__HR__BaseUrl.");
        return new Uri(uri.AbsoluteUri.TrimEnd('/') + "/");
    }

    public Task<T> GetAsync<T>(string path, CancellationToken token) =>
        SendAsync<T>(HttpMethod.Get, path, null, token);

    public async Task DeleteEmployeeAsync(Guid employeeId, CancellationToken token)
    {
        using var request = Request(HttpMethod.Delete, $"employees/{employeeId}");
        using var response = await _http.SendAsync(request, token);
        await EnsureSuccessAsync(response, token);
        // DELETE succeeds with 204 No Content, so there is no JSON to deserialize.
        token.ThrowIfCancellationRequested();
    }

    public async Task<T> SendAsync<T>(HttpMethod method, string path, object? data, CancellationToken token)
    {
        using var request = Request(method, path);
        if (data is not null) request.Content = JsonContent.Create(data);
        using var response = await _http.SendAsync(request, token);
        await EnsureSuccessAsync(response, token);
        var result = await response.Content.ReadFromJsonAsync<T>(token)
            ?? throw new JsonException("HR API trả về dữ liệu rỗng.");
        token.ThrowIfCancellationRequested();
        return result;
    }

    public async Task<byte[]> DownloadAsync(string path, CancellationToken token)
    {
        using var request = Request(HttpMethod.Get, path);
        using var response = await _http.SendAsync(request, token);
        await EnsureSuccessAsync(response, token);
        return await response.Content.ReadAsByteArrayAsync(token);
    }

    private static HttpRequestMessage Request(HttpMethod method, string path)
    {
        var request = new HttpRequestMessage(method, path);
        request.Headers.Add("X-Role", "HR"); // Contract of the existing local HR API.
        return request;
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken token)
    {
        if (response.IsSuccessStatusCode) return;
        var message = $"HR API trả về HTTP {(int)response.StatusCode}.";
        try
        {
            using var error = await JsonDocument.ParseAsync(
                await response.Content.ReadAsStreamAsync(token), cancellationToken: token);
            if (error.RootElement.ValueKind == JsonValueKind.Object)
            {
                if (error.RootElement.TryGetProperty("message", out var detail) && detail.ValueKind == JsonValueKind.String)
                    message += " " + detail.GetString();
                else if (error.RootElement.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String)
                    message += " " + title.GetString();
            }
        }
        catch (JsonException) { }
        throw new HttpRequestException(message, null, response.StatusCode);
    }

    public Task<EmployeePage> EmployeesAsync(string? keyword, int page, int size, CancellationToken token) =>
        GetAsync<EmployeePage>($"employees?keyword={Uri.EscapeDataString(keyword?.Trim() ?? "")}&page={page}&pageSize={size}", token);

    public async Task<List<EmployeeRow>> AllEmployeesAsync(CancellationToken token)
    {
        var result = new List<EmployeeRow>();
        for (var page = 1; ; page++)
        {
            var batch = await EmployeesAsync(null, page, 100, token);
            result.AddRange(batch.Items);
            if (batch.Items.Count == 0 || (long)page * 100 >= batch.Total) return result;
        }
    }

    public void Dispose() => _http.Dispose();
}
