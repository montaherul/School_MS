using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Services.Implementations.Admin;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class PermissionCacheServiceTests
{
    /// <summary>
    /// Creates a fresh InMemory SchoolDbContext for each test.
    /// Uses EnsureDeleted+EnsureCreated to avoid seed data conflicts from DbInitializer.Seed.
    /// </summary>
    private static SchoolDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SchoolDbContext>()
            .UseInMemoryDatabase($"PermCacheTest_{Guid.NewGuid()}")
            .Options;
        var ctx = new SchoolDbContext(options);
        ctx.Database.EnsureDeleted();
        ctx.Database.EnsureCreated();
        return ctx;
    }

    private static (PermissionCacheService Service, SchoolDbContext Db) CreateService()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var db = CreateDbContext();

        var scopeMock = new Mock<IServiceScope>(MockBehavior.Loose);
        scopeMock.Setup(s => s.ServiceProvider.GetService(typeof(SchoolDbContext))).Returns(db);

        var spMock = new Mock<IServiceProvider>(MockBehavior.Loose);
        spMock.Setup(s => s.CreateScope()).Returns(scopeMock.Object);

        return (new PermissionCacheService(spMock.Object, cache), db);
    }

    [Fact(DisplayName = "1. HasPermissionAsync returns false when role has no matching permissions")]
    public async Task HasPermissionAsync_ReturnsFalse_WhenRoleLacksPermission()
    {
        var (service, db) = CreateService();
        db.RolePermissions.Add(new RolePermission
        {
            RoleId = 1001,
            PermissionId = 1001,
            Role = new Role { Id = 1001, Name = "Teacher" },
            Permission = new Permission { Id = 1001, Code = "Students.View" }
        });
        db.SaveChanges();

        var result = await service.HasPermissionAsync(["Teacher"], "Students.Edit");

        Assert.False(result);
        db.Dispose();
    }

    [Fact(DisplayName = "2. HasPermissionAsync returns true when role has permission")]
    public async Task HasPermissionAsync_ReturnsTrue_WhenRoleHasPermission()
    {
        var (service, db) = CreateService();
        db.RolePermissions.Add(new RolePermission
        {
            RoleId = 1002,
            PermissionId = 1002,
            Role = new Role { Id = 1002, Name = "Teacher" },
            Permission = new Permission { Id = 1002, Code = "Students.View" }
        });
        db.SaveChanges();

        var result = await service.HasPermissionAsync(["Teacher"], "Students.View");

        Assert.True(result);
        db.Dispose();
    }

    [Fact(DisplayName = "3. HasPermissionAsync uses cache on second call")]
    public async Task HasPermissionAsync_UsesCache_OnSecondCall()
    {
        var (service, db) = CreateService();
        db.RolePermissions.Add(new RolePermission
        {
            RoleId = 1003,
            PermissionId = 1003,
            Role = new Role { Id = 1003, Name = "Teacher" },
            Permission = new Permission { Id = 1003, Code = "Students.View" }
        });
        db.SaveChanges();

        var result1 = await service.HasPermissionAsync(["Teacher"], "Students.View");
        Assert.True(result1);

        // Remove from db to prove caching works
        db.RolePermissions.RemoveRange(db.RolePermissions);
        db.SaveChanges();

        var result2 = await service.HasPermissionAsync(["Teacher"], "Students.View");
        Assert.True(result2);

        db.Dispose();
    }

    [Fact(DisplayName = "4. InvalidateRolePermissions clears cache for subsequent calls")]
    public async Task InvalidateRolePermissions_ClearsCache()
    {
        var (service, db) = CreateService();
        db.RolePermissions.Add(new RolePermission
        {
            RoleId = 1004,
            PermissionId = 1004,
            Role = new Role { Id = 1004, Name = "Teacher" },
            Permission = new Permission { Id = 1004, Code = "Students.View" }
        });
        db.SaveChanges();

        var result1 = await service.HasPermissionAsync(["Teacher"], "Students.View");
        Assert.True(result1);

        // Clear the data
        db.RolePermissions.RemoveRange(db.RolePermissions);
        db.SaveChanges();

        // Invalidate cache
        service.InvalidateRolePermissions(1004);

        // Should re-query and return false
        var result2 = await service.HasPermissionAsync(["Teacher"], "Students.View");
        Assert.False(result2);

        db.Dispose();
    }

    [Fact(DisplayName = "5. HasPermissionAsync returns false for empty role names")]
    public async Task HasPermissionAsync_EmptyRoleNames_ReturnsFalse()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var spMock = new Mock<IServiceProvider>(MockBehavior.Loose);
        var service = new PermissionCacheService(spMock.Object, cache);

        var result = await service.HasPermissionAsync([], "Any.Code");

        Assert.False(result);
    }

    [Fact(DisplayName = "6. HasPermissionAsync matches multiple roles")]
    public async Task HasPermissionAsync_MultipleRoles_MatchesCorrectly()
    {
        var (service, db) = CreateService();
        db.RolePermissions.Add(new RolePermission
        {
            RoleId = 1005,
            PermissionId = 1005,
            Role = new Role { Id = 1005, Name = "Admin" },
            Permission = new Permission { Id = 1005, Code = "Users.Manage" }
        });
        db.RolePermissions.Add(new RolePermission
        {
            RoleId = 1006,
            PermissionId = 1006,
            Role = new Role { Id = 1006, Name = "Teacher" },
            Permission = new Permission { Id = 1006, Code = "Students.View" }
        });
        db.SaveChanges();

        var result1 = await service.HasPermissionAsync(["Admin", "Teacher"], "Users.Manage");
        Assert.True(result1);

        var result2 = await service.HasPermissionAsync(["Teacher"], "Users.Manage");
        Assert.False(result2);

        db.Dispose();
    }
}
