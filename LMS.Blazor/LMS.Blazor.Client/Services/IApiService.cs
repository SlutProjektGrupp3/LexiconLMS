namespace LMS.Blazor.Client.Services;

public interface IApiService
{
    Task<T?> GetAsync<T>(string endpoint, CancellationToken ct = default);
}