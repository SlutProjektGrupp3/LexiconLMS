using LMS.Blazor.Client.Services;

namespace LMS.Blazor.Services;

public class ServerNoOpApiService(ILogger<ServerNoOpApiService> logger) : IApiService
{
    private readonly ILogger<ServerNoOpApiService> _logger = logger;

    public Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpApiService called for: {Endpoint}", endpoint);
        return Task.FromResult<T?>(default);
    }
    public Task PostAsync<T>(string endpoint, T data, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpApiService POST called for: {Endpoint}", endpoint);
        return Task.CompletedTask;
    }


    public Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpApiService called Post for: {Endpoint}", endpoint);
        return Task.FromResult<TResponse?>(default);
    }

    public Task PutAsync<T>(string endpoint, T data, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpApiService PUT called for: {Endpoint}", endpoint);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string endpoint, CancellationToken ct = default)
    {
        _logger.LogWarning("ServerNoOpApiService called Delete for: {Endpoint}", endpoint);
        return Task.CompletedTask;
    }
}
