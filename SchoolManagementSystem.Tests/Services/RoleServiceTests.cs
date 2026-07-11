using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Moq;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Services.Implementations.Admin;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using Xunit;
using static SchoolManagementSystem.Tests.Services.AsyncQueryableHelper;

namespace SchoolManagementSystem.Tests.Services;

public class RoleServiceTests
{
    private static Mock<IBaseRepository<T>> CreateRepoMock<T>(IEnumerable<T>? data = null) where T : class
    {
        var mock = new Mock<IBaseRepository<T>>(MockBehavior.Loose);
        var list = (data ?? []).ToList();
        mock.Setup(r => r.Query()).Returns(list.AsAsyncQueryable());
        mock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => list.OfType<Role>().FirstOrDefault(r => r.Id == id) as T);
        mock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<T, bool>> expr, CancellationToken _) => list.AsQueryable().Any(expr));
        mock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<T, bool>> expr, CancellationToken _) => list.AsQueryable().FirstOrDefault(expr)!);
        mock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<T, bool>>? expr, CancellationToken _) => expr == null ? list.Count : list.AsQueryable().Count(expr));
        mock.Setup(r => r.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .Callback<T, CancellationToken>((e, _) => { list.Add(e); });
        mock.Setup(r => r.ListAsync(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<T, bool>>? expr, CancellationToken _) =>
                expr == null ? list.ToList().AsReadOnly() : list.AsQueryable().Where(expr).ToList().AsReadOnly());
        mock.Setup(r => r.Remove(It.IsAny<T>()))
            .Callback<T>(e => { if (e is UserRole ur) list.RemoveAll(x => x is UserRole ur2 && ur2.UserId == ur.UserId && ur2.RoleId == ur.RoleId); list.Remove(e); });
        return mock;
    }

    // ─── GET PAGED TESTS ────────────────────────────────────────────

    [Fact(DisplayName = "1. GetPagedAsync returns paged results")]
    public async Task GetPagedAsync_ReturnsPagedResults()
    {
        var roles = Enumerable.Range(1, 15).Select(i => new Role
        {
            Id = i,
            Name = $"Role{i}",
            Description = $"Description {i}",
            IsDeleted = false,
            RolePermissions = new List<RolePermission>(),
            UserRoles = new List<UserRole>()
        }).ToList();

        var roleRepoMock = CreateRepoMock(roles);
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.Repository<Role>()).Returns(roleRepoMock.Object);
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var cacheMock = new Mock<IPermissionCacheService>(MockBehavior.Loose);
        var service = new RoleService(uowMock.Object, cacheMock.Object, new Mock<IHttpContextAccessor>(MockBehavior.Loose).Object);

        var result = await service.GetPagedAsync(1, 5, null);

        Assert.NotNull(result);
        Assert.Equal(15, result.TotalItems);
        Assert.Equal(5, result.Items.Count);
        Assert.Equal(1, result.Page);
        Assert.Equal(5, result.PageSize);
    }

    [Fact(DisplayName = "2. GetPagedAsync filters by search term")]
    public async Task GetPagedAsync_FiltersBySearchTerm()
    {
        var roles = new List<Role>
        {
            new() { Id = 1, Name = "Admin", Description = "Administrator", IsDeleted = false, RolePermissions = new List<RolePermission>(), UserRoles = new List<UserRole>() },
            new() { Id = 2, Name = "Teacher", Description = "Teaching staff", IsDeleted = false, RolePermissions = new List<RolePermission>(), UserRoles = new List<UserRole>() },
            new() { Id = 3, Name = "Student", Description = "Student role", IsDeleted = false, RolePermissions = new List<RolePermission>(), UserRoles = new List<UserRole>() }
        };

        var roleRepoMock = CreateRepoMock(roles);
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.Repository<Role>()).Returns(roleRepoMock.Object);

        var cacheMock = new Mock<IPermissionCacheService>(MockBehavior.Loose);
        var service = new RoleService(uowMock.Object, cacheMock.Object, new Mock<IHttpContextAccessor>(MockBehavior.Loose).Object);

        var result = await service.GetPagedAsync(1, 10, "Admin");

        Assert.Single(result.Items);
        var item = (object)result.Items[0];
        var itemType = item.GetType();
        Assert.Equal("Admin", itemType.GetProperty("Name")?.GetValue(item) as string);
    }

    [Fact(DisplayName = "3. GetPagedAsync filters by description")]
    public async Task GetPagedAsync_FiltersByDescription()
    {
        var roles = new List<Role>
        {
            new() { Id = 1, Name = "Admin", Description = "System Administrator", IsDeleted = false, RolePermissions = new List<RolePermission>(), UserRoles = new List<UserRole>() },
            new() { Id = 2, Name = "Teacher", Description = "Teaching staff", IsDeleted = false, RolePermissions = new List<RolePermission>(), UserRoles = new List<UserRole>() }
        };

        var roleRepoMock = CreateRepoMock(roles);
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.Repository<Role>()).Returns(roleRepoMock.Object);

        var cacheMock = new Mock<IPermissionCacheService>(MockBehavior.Loose);
        var service = new RoleService(uowMock.Object, cacheMock.Object, new Mock<IHttpContextAccessor>(MockBehavior.Loose).Object);

        var result = await service.GetPagedAsync(1, 10, "staff");

        Assert.Single(result.Items);
        var item = (object)result.Items[0];
        var itemType = item.GetType();
        Assert.Equal("Teacher", itemType.GetProperty("Name")?.GetValue(item) as string);
    }

    // ─── DELETE TESTS ──────────────────────────────────────────────

    [Fact(DisplayName = "4. DeleteAsync soft-deletes role and removes associations")]
    public async Task DeleteAsync_SoftDeletesRole()
    {
        var role = new Role { Id = 5, Name = "CustomRole", IsDeleted = false };
        var roles = new List<Role> { role };
        var roleRepoMock = CreateRepoMock(roles);

        var userRoles = new List<UserRole>
        {
            new() { UserId = 1, RoleId = 5 },
            new() { UserId = 2, RoleId = 5 }
        };
        var userRoleRepoMock = CreateRepoMock(userRoles);
        userRoleRepoMock.Setup(r => r.ListAsync(It.IsAny<Expression<Func<UserRole, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<UserRole, bool>>? expr, CancellationToken _) =>
                userRoles.AsQueryable().Where(expr ?? (u => true)).ToList().AsReadOnly());
        userRoleRepoMock.Setup(r => r.Remove(It.IsAny<UserRole>()))
            .Callback<UserRole>(ur => userRoles.RemoveAll(x => x.UserId == ur.UserId && x.RoleId == ur.RoleId));

        var rolePerms = new List<RolePermission>
        {
            new() { RoleId = 5, PermissionId = 1 },
            new() { RoleId = 5, PermissionId = 2 }
        };
        var rolePermRepoMock = CreateRepoMock(rolePerms);
        rolePermRepoMock.Setup(r => r.ListAsync(It.IsAny<Expression<Func<RolePermission, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<RolePermission, bool>>? expr, CancellationToken _) =>
                rolePerms.AsQueryable().Where(expr ?? (rp => true)).ToList().AsReadOnly());
        rolePermRepoMock.Setup(r => r.Remove(It.IsAny<RolePermission>()))
            .Callback<RolePermission>(rp => rolePerms.RemoveAll(x => x.RoleId == rp.RoleId && x.PermissionId == rp.PermissionId));

        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.Repository<Role>()).Returns(roleRepoMock.Object);
        uowMock.Setup(u => u.Repository<UserRole>()).Returns(userRoleRepoMock.Object);
        uowMock.Setup(u => u.Repository<RolePermission>()).Returns(rolePermRepoMock.Object);
        uowMock.Setup(u => u.Repository<AuditLog>()).Returns(CreateRepoMock<AuditLog>().Object);
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var cacheMock = new Mock<IPermissionCacheService>(MockBehavior.Loose);
        var service = new RoleService(uowMock.Object, cacheMock.Object, new Mock<IHttpContextAccessor>(MockBehavior.Loose).Object);

        await service.DeleteAsync(5);

        Assert.True(role.IsDeleted);
        Assert.Empty(userRoles);
        Assert.Empty(rolePerms);
        cacheMock.Verify(c => c.InvalidateRolePermissions(5), Times.Once);
    }

    [Fact(DisplayName = "5. DeleteAsync prevents deleting system roles")]
    public async Task DeleteAsync_PreventsDeletingSystemRoles()
    {
        var role = new Role { Id = 1, Name = "Super Admin", IsDeleted = false };
        var roles = new List<Role> { role };
        var roleRepoMock = CreateRepoMock(roles);

        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.Repository<Role>()).Returns(roleRepoMock.Object);
        uowMock.Setup(u => u.Repository<UserRole>()).Returns(CreateRepoMock<UserRole>().Object);
        uowMock.Setup(u => u.Repository<RolePermission>()).Returns(CreateRepoMock<RolePermission>().Object);
        uowMock.Setup(u => u.Repository<AuditLog>()).Returns(CreateRepoMock<AuditLog>().Object);

        var cacheMock = new Mock<IPermissionCacheService>(MockBehavior.Loose);
        var service = new RoleService(uowMock.Object, cacheMock.Object, new Mock<IHttpContextAccessor>(MockBehavior.Loose).Object);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteAsync(1));
        Assert.Contains("system role", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "6. DeleteAsync prevents deleting Admin system role")]
    public async Task DeleteAsync_PreventsDeletingAdminRole()
    {
        var role = new Role { Id = 2, Name = "Admin", IsDeleted = false };
        var roles = new List<Role> { role };
        var roleRepoMock = CreateRepoMock(roles);

        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.Repository<Role>()).Returns(roleRepoMock.Object);
        uowMock.Setup(u => u.Repository<UserRole>()).Returns(CreateRepoMock<UserRole>().Object);
        uowMock.Setup(u => u.Repository<RolePermission>()).Returns(CreateRepoMock<RolePermission>().Object);

        var cacheMock = new Mock<IPermissionCacheService>(MockBehavior.Loose);
        var service = new RoleService(uowMock.Object, cacheMock.Object, new Mock<IHttpContextAccessor>(MockBehavior.Loose).Object);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteAsync(2));
        Assert.Contains("system role", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ─── PERMISSION ASSIGNMENT TESTS ───────────────────────────────

    [Fact(DisplayName = "7. AssignPermissionsToRoleAsync replaces permissions")]
    public async Task AssignPermissionsToRoleAsync_ReplacesPermissions()
    {
        var existingPerms = new List<RolePermission>
        {
            new() { RoleId = 3, PermissionId = 1 },
            new() { RoleId = 3, PermissionId = 2 },
            new() { RoleId = 3, PermissionId = 3 }
        };
        var rolePermRepoMock = CreateRepoMock(existingPerms);
        rolePermRepoMock.Setup(r => r.Remove(It.IsAny<RolePermission>()))
            .Callback<RolePermission>(rp => existingPerms.RemoveAll(x => x.RoleId == rp.RoleId && x.PermissionId == rp.PermissionId));

        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.Repository<RolePermission>()).Returns(rolePermRepoMock.Object);
        uowMock.Setup(u => u.Repository<AuditLog>()).Returns(CreateRepoMock<AuditLog>().Object);
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var cacheMock = new Mock<IPermissionCacheService>(MockBehavior.Loose);
        var service = new RoleService(uowMock.Object, cacheMock.Object, new Mock<IHttpContextAccessor>(MockBehavior.Loose).Object);

        var result = await service.AssignPermissionsToRoleAsync(3, [4, 5]);

        Assert.True(result);
        Assert.Empty(existingPerms.Where(rp => rp.PermissionId == 1 || rp.PermissionId == 2));
        Assert.Contains(existingPerms, rp => rp.RoleId == 3 && rp.PermissionId == 4);
        Assert.Contains(existingPerms, rp => rp.RoleId == 3 && rp.PermissionId == 5);
    }

    [Fact(DisplayName = "8. AssignPermissionsToRoleAsync invalidates cache")]
    public async Task AssignPermissionsToRoleAsync_InvalidatesCache()
    {
        var rolePermRepoMock = CreateRepoMock<RolePermission>();
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.Repository<RolePermission>()).Returns(rolePermRepoMock.Object);
        uowMock.Setup(u => u.Repository<AuditLog>()).Returns(CreateRepoMock<AuditLog>().Object);
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var cacheMock = new Mock<IPermissionCacheService>(MockBehavior.Loose);
        var service = new RoleService(uowMock.Object, cacheMock.Object, new Mock<IHttpContextAccessor>(MockBehavior.Loose).Object);

        await service.AssignPermissionsToRoleAsync(1, [10, 20]);

        cacheMock.Verify(c => c.InvalidateRolePermissions(1), Times.Once);
    }

    [Fact(DisplayName = "9. AssignPermissionsToRoleAsync adds new permissions correctly")]
    public async Task AssignPermissionsToRoleAsync_AddsNewPermissions()
    {
        var existingPerms = new List<RolePermission>();
        var rolePermRepoMock = CreateRepoMock(existingPerms);

        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.Repository<RolePermission>()).Returns(rolePermRepoMock.Object);
        uowMock.Setup(u => u.Repository<AuditLog>()).Returns(CreateRepoMock<AuditLog>().Object);
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var cacheMock = new Mock<IPermissionCacheService>(MockBehavior.Loose);
        var service = new RoleService(uowMock.Object, cacheMock.Object, new Mock<IHttpContextAccessor>(MockBehavior.Loose).Object);

        var result = await service.AssignPermissionsToRoleAsync(1, [100, 200]);

        Assert.True(result);
        Assert.Contains(existingPerms, rp => rp.PermissionId == 100);
        Assert.Contains(existingPerms, rp => rp.PermissionId == 200);
    }

    // ─── GET PERMISSIONS TESTS ────────────────────────────────────

    [Fact(DisplayName = "10. GetPermissionsByRoleIdAsync returns permission IDs")]
    public async Task GetPermissionsByRoleIdAsync_ReturnsPermissionIds()
    {
        var rolePerms = new List<RolePermission>
        {
            new() { RoleId = 2, PermissionId = 5 },
            new() { RoleId = 2, PermissionId = 10 },
            new() { RoleId = 2, PermissionId = 15 }
        };
        var rolePermRepoMock = CreateRepoMock(rolePerms);

        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.Repository<RolePermission>()).Returns(rolePermRepoMock.Object);

        var cacheMock = new Mock<IPermissionCacheService>(MockBehavior.Loose);
        var service = new RoleService(uowMock.Object, cacheMock.Object, new Mock<IHttpContextAccessor>(MockBehavior.Loose).Object);

        var ids = await service.GetPermissionsByRoleIdAsync(2);

        Assert.Equal(3, ids.Count);
        Assert.Contains(5, ids);
        Assert.Contains(10, ids);
        Assert.Contains(15, ids);
    }
}
