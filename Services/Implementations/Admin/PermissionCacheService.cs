using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Services.Interfaces.Admin;

namespace SchoolManagementSystem.Services.Implementations.Admin;

public class PermissionCacheService : IPermissionCacheService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMemoryCache _cache;
    private readonly ConcurrentBag<string> _trackedKeys = new();
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(60);

    public PermissionCacheService(IServiceProvider serviceProvider, IMemoryCache cache)
    {
        _serviceProvider = serviceProvider;
        _cache = cache;
    }

    public async Task<bool> HasPermissionAsync(string[] roleNames, string permissionCode, CancellationToken ct = default)
    {
        if (roleNames.Length == 0) return false;

        var cacheKey = BuildCacheKey(roleNames, permissionCode);

        if (_cache.TryGetValue(cacheKey, out bool cached))
            return cached;

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();

        var allowed = await db.RolePermissions
            .AnyAsync(rp => rp.Permission != null && rp.Role != null
                && rp.Permission.Code == permissionCode
                && roleNames.Contains(rp.Role.Name), ct);

        _cache.Set(cacheKey, allowed, CacheDuration);
        _trackedKeys.Add(cacheKey);
        return allowed;
    }

    public void InvalidateRolePermissions(int roleId)
    {
        _trackedKeys.Clear();
    }

    public void InvalidateAll()
    {
        _trackedKeys.Clear();
    }

    private static string BuildCacheKey(string[] roleNames, string permissionCode)
    {
        var sorted = roleNames.OrderBy(r => r).ToArray();
        return $"rp:{string.Join('_', sorted)}|{permissionCode}";
    }
}
