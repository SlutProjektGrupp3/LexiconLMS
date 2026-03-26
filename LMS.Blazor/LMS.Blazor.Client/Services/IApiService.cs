namespace LMS.Blazor.Client.Services;

public interface IApiService
{
    Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default);
    Task PostAsync<T>(string endpoint, T data, CancellationToken ct = default);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken ct = default);

    Task PutAsync<T>(string endpoint, T data, CancellationToken ct = default);
    Task DeleteAsync(string endpoint, CancellationToken ct = default);
}