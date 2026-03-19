namespace LMS.Blazor.Client.Services;

public interface IApiService
{
    Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default);
    Task PostAsync<T>(string endpoint, T data, CancellationToken ct = default);
}