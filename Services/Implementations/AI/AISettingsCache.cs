using Microsoft.Extensions.Caching.Memory;
using SchoolManagementSystem.Repositories.Interfaces.AI;
using SchoolManagementSystem.Services.Interfaces.AI;

namespace SchoolManagementSystem.Services.Implementations.AI;

public class AISettingsCache : IAISettingsCache, IDisposable
{
    private const string SettingsCacheKey = "AISettings";
    private const string ProviderCacheKey = "AIProvider";
    private readonly IMemoryCache _cache;
    private readonly IAIAdminRepository _repo;
    private readonly IAICacheInvalidator _invalidator;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    public AISettingsCache(
        IMemoryCache cache,
        IAIAdminRepository repo,
        IAICacheInvalidator invalidator)
    {
        _cache = cache;
        _repo = repo;
        _invalidator = invalidator;

        _invalidator.SettingsChanged += InvalidateCache;
    }

    public async Task<string> GetApiKeyAsync()
    {
        var provider = await GetActiveProviderAsync();
        return provider?.ApiKey ?? string.Empty;
    }

    public async Task<string> GetEndpointAsync()
    {
        var provider = await GetActiveProviderAsync();
        return provider?.BaseUrl ?? "https://api.openai.com/v1/chat/completions";
    }

    public async Task<string> GetModelAsync() => await GetSettingAsync("AI.DefaultModel", "gpt-4o-mini");
    public async Task<int> GetMaxTokensAsync() => int.TryParse(await GetSettingAsync("AI.MaxTokens", "2048"), out var v) ? v : 2048;
    public async Task<double> GetTemperatureAsync() => double.TryParse(await GetSettingAsync("AI.Temperature", "0.7"), out var v) ? v : 0.7;
    public async Task<int> GetRetryCountAsync() => int.TryParse(await GetSettingAsync("AI.RetryCount", "3"), out var v) ? v : 3;
    public async Task<int> GetTimeoutSecondsAsync() => int.TryParse(await GetSettingAsync("AI.TimeoutSeconds", "60"), out var v) ? v : 60;
    public async Task<decimal> GetCostPerPromptTokenAsync() => decimal.TryParse(await GetSettingAsync("AI.CostPerPromptToken", "0.00000015"), out var v) ? v : 0.00000015m;
    public async Task<decimal> GetCostPerCompletionTokenAsync() => decimal.TryParse(await GetSettingAsync("AI.CostPerCompletionToken", "0.00000060"), out var v) ? v : 0.00000060m;

    private async Task<Models.DTOs.AI.AIProviderDto?> GetActiveProviderAsync()
    {
        if (_cache.TryGetValue<Models.DTOs.AI.AIProviderDto>(ProviderCacheKey, out var cached))
            return cached;

        await _refreshLock.WaitAsync();
        try
        {
            if (_cache.TryGetValue<Models.DTOs.AI.AIProviderDto>(ProviderCacheKey, out cached))
                return cached;

            var providers = await _repo.GetProvidersAsync();
            var active = providers
                .Where(p => p.IsEnabled)
                .OrderBy(p => p.Priority)
                .FirstOrDefault();

            if (active != null)
                _cache.Set(ProviderCacheKey, active, TimeSpan.FromMinutes(5));

            return active;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private async Task<string> GetSettingAsync(string key, string defaultValue)
    {
        var settings = await GetCachedSettingsAsync();
        return settings.TryGetValue(key, out var value) && !string.IsNullOrEmpty(value) ? value : defaultValue;
    }

    private async Task<Dictionary<string, string>> GetCachedSettingsAsync()
    {
        if (_cache.TryGetValue<Dictionary<string, string>>(SettingsCacheKey, out var cached))
            return cached!;

        await _refreshLock.WaitAsync();
        try
        {
            if (_cache.TryGetValue<Dictionary<string, string>>(SettingsCacheKey, out cached))
                return cached!;

            var settings = (await _repo.GetSettingsAsync())
                .ToDictionary(s => s.Key, s => s.Value, StringComparer.OrdinalIgnoreCase);

            _cache.Set(SettingsCacheKey, settings, TimeSpan.FromMinutes(5));
            return settings;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private void InvalidateCache()
    {
        _cache.Remove(SettingsCacheKey);
        _cache.Remove(ProviderCacheKey);
    }

    public void Dispose()
    {
        _invalidator.SettingsChanged -= InvalidateCache;
        _refreshLock.Dispose();
    }
}
