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

}
