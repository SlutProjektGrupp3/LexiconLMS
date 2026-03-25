using LMS.Shared.DTOs.Modules;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace LMS.Blazor.Client.Services;

public class ClientApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;
    private readonly JsonSerializerOptions _jsonOptions;

    public ClientApiService(HttpClient httpClient, NavigationManager navigationManager)
    {
        _httpClient = httpClient;

        _navigationManager = navigationManager;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"api/proxy/{endpoint}", ct);

        if (HandleAuth(response))
            return default;

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(ct), _jsonOptions, ct);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        System.Diagnostics.Debug.WriteLine($"DEBUG: PostAsync: {json}");

        var response = await _httpClient.PostAsJsonAsync($"api/proxy/{endpoint}", data, _jsonOptions, ct);

        if (HandleAuth(response))
            return default;

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<TResponse>(await response.Content.ReadAsStreamAsync(ct), _jsonOptions, ct);
    }
    public async Task PutAsync<T>(string endpoint, T data, CancellationToken ct = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/proxy/{endpoint}", data, _jsonOptions, ct);

        if (HandleAuth(response))
            return;

        response.EnsureSuccessStatusCode();
    }
    public async Task DeleteAsync(string endpoint, CancellationToken ct = default)
    {
        var response = await _httpClient.DeleteAsync($"api/proxy/{endpoint}", ct);

        if (HandleAuth(response))
            return;

        response.EnsureSuccessStatusCode();
    }
    // =========================
    // AUTH HANDLER
    // =========================
    private bool HandleAuth(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.Forbidden)
        {
            _navigationManager.NavigateTo("/Account/Login", forceLoad: true);
            return true;
        }

        return false;
    }
}
