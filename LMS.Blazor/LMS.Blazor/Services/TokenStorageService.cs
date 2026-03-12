using LMS.Shared.DTOs.AuthDtos;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Concurrent;
using System.Text.Json;

namespace LMS.Blazor.Services;

// Simple file-based token storage for teaching/demo purposes.
// You dont have to logg out and logg in every time 
// Not recommended for production since tokens are stored on disk.
//A better production solution would use a distributed cache such as Redis.

public class TokenStorageService : ITokenStorage, IDisposable
{
    private readonly ConcurrentDictionary<string, TokenDto> _tokenStore = new();
    private readonly ILogger<TokenStorageService> _logger;
    private readonly string _storageFilePath;
    private readonly SemaphoreSlim _fileLock = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public TokenStorageService(ILogger<TokenStorageService> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _storageFilePath = Path.Combine(env.ContentRootPath, "App_Data", "tokens.json");
        
        Directory.CreateDirectory(Path.GetDirectoryName(_storageFilePath)!);
        LoadTokensFromFile();
    }

    private void LoadTokensFromFile()
    {
        try
        {
            if (!File.Exists(_storageFilePath))
                return;

            var json = File.ReadAllText(_storageFilePath);
            var tokens = JsonSerializer.Deserialize<Dictionary<string, TokenDto>>(json);
            
            if (tokens == null)
                return;

            foreach (var kvp in tokens)
                _tokenStore[kvp.Key] = kvp.Value;
            
            _logger.LogInformation("Loaded {Count} tokens from storage", tokens.Count);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load tokens from file. Starting with empty storage.");
        }
    }

    private async Task SaveTokensToFileAsync()
    {
        await _fileLock.WaitAsync();
        try
        {
            var snapshot = _tokenStore.ToDictionary(k => k.Key, v => v.Value);
            var json = JsonSerializer.Serialize(snapshot, _jsonOptions);
            await File.WriteAllTextAsync(_storageFilePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save tokens to file");
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public async Task StoreTokensAsync(string userId, TokenDto tokens)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentNullException.ThrowIfNull(tokens);

        _tokenStore[userId] = tokens;
        _logger.LogDebug("Stored tokens for user {UserId}", userId);

        await SaveTokensToFileAsync();
    }

    public Task<TokenDto?> GetTokensAsync(string userId)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);

        _tokenStore.TryGetValue(userId, out var tokens);
        return Task.FromResult(tokens);
    }

    public Task<string?> GetAccessTokenAsync(string userId)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);

        _tokenStore.TryGetValue(userId, out var tokens);
        return Task.FromResult(tokens?.AccessToken);
    }

    public async Task RemoveTokensAsync(string userId)
    {

        ArgumentException.ThrowIfNullOrEmpty(userId);

        if (_tokenStore.TryRemove(userId, out _))
        {
            _logger.LogDebug("Removed tokens for user {UserId}", userId);
            await SaveTokensToFileAsync();
        }
    }

    public void Dispose()
    {
        _fileLock.Dispose();
    }
}