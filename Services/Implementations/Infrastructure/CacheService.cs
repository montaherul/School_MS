using Microsoft.Extensions.Caching.Memory;
using SchoolManagementSystem.Constants;

namespace SchoolManagementSystem.Services.Implementations.Infrastructure;

/// <summary>
/// Shared cache helper wrapping IMemoryCache with consistent key strategy and expiry policy.
/// Use this instead of injecting IMemoryCache directly in services.
/// </summary>
public interface ICacheService
{
    T? Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan? duration = null);
    void Remove(string key);
    Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? duration = null);
}

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
    {
        _cache  = cache;
        _logger = logger;
    }

    public T? Get<T>(string key)
    {
        _cache.TryGetValue(key, out T? value);
        return value;
    }

    public void Set<T>(string key, T value, TimeSpan? duration = null)
    {
        var expiry = duration ?? AppConstants.CacheDuration.Medium;
        _cache.Set(key, value, expiry);
        _logger.LogDebug("Cache SET: {Key} (expires in {Minutes}min)", key, expiry.TotalMinutes);
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
        _logger.LogDebug("Cache REMOVE: {Key}", key);
    }

    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? duration = null)
    {
        if (_cache.TryGetValue(key, out T? cached) && cached is not null)
            return cached;

        var value = await factory();
        Set(key, value, duration);
        return value;
    }
}
