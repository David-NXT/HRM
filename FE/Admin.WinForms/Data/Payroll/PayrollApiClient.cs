using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Admin.WinForms.Data.Payroll;

public sealed class PayrollApiClient
{
    private readonly HttpClient _httpClient;

    public PayrollApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<T>> GetListAsync<T>(string path)
    {
        using var response = await _httpClient.GetAsync(path);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<List<T>>()
            ?? throw new InvalidDataException("Payroll API trả về danh sách rỗng hoặc không hợp lệ.");
    }

    public async Task<T?> GetOptionalAsync<T>(string path) where T : class
    {
        using var response = await _httpClient.GetAsync(path);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<T>()
            ?? throw new InvalidDataException("Payroll API trả về dữ liệu rỗng hoặc không hợp lệ.");
    }

    public async Task<T?> PostForValueAsync<T>(string path, string errorMessage)
    {
        using var response = await _httpClient.PostAsync(path, null);
        await EnsureSuccessAsync(response, errorMessage);
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> PostForValueAsync<T>(string path, object value, string errorMessage)
    {
        using var response = await _httpClient.PostAsJsonAsync(path, value);
        await EnsureSuccessAsync(response, errorMessage);
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task PostAsync(string path, string errorMessage)
    {
        using var response = await _httpClient.PostAsync(path, null);
        await EnsureSuccessAsync(response, errorMessage);
    }

    public async Task PostAsync(string path, object value, string errorMessage)
    {
        using var response = await _httpClient.PostAsJsonAsync(path, value);
        await EnsureSuccessAsync(response, errorMessage);
    }

    public async Task<T> SaveAsync<T>(string path, object value, HttpMethod method)
    {
        using var request = new HttpRequestMessage(method, path)
        {
            Content = JsonContent.Create(value)
        };
        using var response = await _httpClient.SendAsync(request);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<T>()
            ?? throw new InvalidDataException("Payroll API trả về dữ liệu rỗng hoặc không hợp lệ.");
    }

    public async Task DeleteAsync(string path)
    {
        using var response = await _httpClient.DeleteAsync(path);
        await EnsureSuccessAsync(response);
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        string? errorMessage = null)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var details = await response.Content.ReadAsStringAsync();
        if (errorMessage is not null)
        {
            throw new Exception(string.IsNullOrWhiteSpace(details)
                ? errorMessage
                : $"{errorMessage}: {details}");
        }

        var message = string.IsNullOrWhiteSpace(details)
            ? $"Payroll API trả về HTTP {(int)response.StatusCode}."
            : $"Payroll API trả về HTTP {(int)response.StatusCode}: {details}";
        throw new HttpRequestException(message, null, response.StatusCode);
    }
}
