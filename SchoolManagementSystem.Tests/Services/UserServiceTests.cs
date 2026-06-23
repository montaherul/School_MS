using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Moq;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Helpers.Security;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.Entities.Guardian;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.ViewModels.User;
using SchoolManagementSystem.Services.Implementations.Admin;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using Xunit;
using static SchoolManagementSystem.Tests.Services.AsyncQueryableHelper;

namespace SchoolManagementSystem.Tests.Services;

public class UserServiceTests
{
    private static Mock<IBaseRepository<T>> CreateRepoMock<T>(IEnumerable<T>? data = null) where T : class
    {
        var mock = new Mock<IBaseRepository<T>>(MockBehavior.Loose);
        var list = (data ?? []).ToList();
        mock.Setup(r => r.Query()).Returns(list.AsAsyncQueryable());
        mock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<T, bool>> expr, CancellationToken _) => list.AsQueryable().Any(expr));
        mock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<T, bool>> expr, CancellationToken _) => list.AsQueryable().FirstOrDefault(expr)!);
        mock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<T, bool>>? expr, CancellationToken _) => expr == null ? list.Count : list.AsQueryable().Count(expr));
        mock.Setup(r => r.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .Callback<T, CancellationToken>((e, _) => { if (e is ApplicationUser u && u.Id == 0) u.Id = list.Count + 1; list.Add(e); });
        mock.Setup(r => r.ListAsync(It.IsAny<Expression<Func<T, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<T, bool>>? expr, CancellationToken _) =>
                expr == null ? list.ToList().AsReadOnly() : list.AsQueryable().Where(expr).ToList().AsReadOnly());
        return mock;
    }

    // ─── CREATE TESTS ───────────────────────────────────────────────

    [Fact(DisplayName = "1. CreateAsync creates user when valid data provided")]
    public async Task CreateAsync_CreatesUser_WhenValidDataProvided()
    {
        var users = new List<ApplicationUser>();
        var userRepoMock = CreateRepoMock(users);
        userRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.Repository<ApplicationUser>()).Returns(userRepoMock.Object);
        uowMock.Setup(u => u.Repository<UserRole>()).Returns(CreateRepoMock<UserRole>().Object);
        uowMock.Setup(u => u.Repository<Role>()).Returns(CreateRepoMock<Role>().Object);
        uowMock.Setup(u => u.Repository<AuditLog>()).Returns(CreateRepoMock<AuditLog>().Object);
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var pwdMock = new Mock<IPasswordHashService>(MockBehavior.Loose);
        pwdMock.Setup(p => p.HashPassword(It.IsAny<string>())).Returns("hashed-pwd");

        var httpMock = new Mock<IHttpContextAccessor>(MockBehavior.Loose);
        var service = new UserService(uowMock.Object, pwdMock.Object, httpMock.Object);
        var model = new UserUpsertViewModel
        {
            UserName = "newuser",
            Email = "new@test.com",
            Password = "Password1",
            Status = AccountStatus.Active
        };

        var id = await service.CreateAsync(model, "admin");

        Assert.Equal(1, id);
        userRepoMock.Verify(r => r.AddAsync(It.Is<ApplicationUser>(u => u.UserName == "newuser"), It.IsAny<CancellationToken>()), Times.Once);
        uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact(DisplayName = "2. CreateAsync throws when username exists")]
    public async Task CreateAsync_ThrowsException_WhenUserNameExists()
    {
        var users = new List<ApplicationUser>
        {
            new() { Id = 1, UserName = "existing", Email = "e@t.com", IsDeleted = false }
        };
        var userRepoMock = CreateRepoMock(users);
        userRepoMock.Setup(r => r.AnyAsync(It.Is<Expression<Func<ApplicationUser, bool>>>(e => e.Compile()(users[0])), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.Repository<ApplicationUser>()).Returns(userRepoMock.Object);

        var pwdMock = new Mock<IPasswordHashService>(MockBehavior.Loose);
        var httpMock = new Mock<IHttpContextAccessor>(MockBehavior.Loose);
        var service = new UserService(uowMock.Object, pwdMock.Object, httpMock.Object);
        var model = new UserUpsertViewModel
        {
            UserName = "existing",
            Email = "new@test.com",
            Password = "Password1"
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(model, "admin"));
        Assert.Contains("already taken", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "3. CreateAsync assigns roles with performerId null")]
    public async Task CreateAsync_AssignsRolesWithPerformerId()
    {
        var users = new List<ApplicationUser>();
        var userRepoMock = CreateRepoMock(users);
        userRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var userRoles = new List<UserRole>();
        var userRoleRepoMock = new Mock<IBaseRepository<UserRole>>(MockBehavior.Loose);
        userRoleRepoMock.Setup(r => r.Query()).Returns(userRoles.AsAsyncQueryable());
        userRoleRepoMock.Setup(r => r.ListAsync(It.IsAny<Expression<Func<UserRole, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<UserRole, bool>>? expr, CancellationToken _) =>
                userRoles.AsQueryable().Where(expr ?? (ur => true)).ToList().AsReadOnly());
        userRoleRepoMock.Setup(r => r.AddAsync(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()))
            .Callback<UserRole, CancellationToken>((ur, _) => userRoles.Add(ur));

        var roles = new List<Role>
        {
            new() { Id = 1, Name = "Teacher" }
        };
        var roleRepoMock = CreateRepoMock(roles);

        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.Repository<ApplicationUser>()).Returns(userRepoMock.Object);
        uowMock.Setup(u => u.Repository<UserRole>()).Returns(userRoleRepoMock.Object);
        uowMock.Setup(u => u.Repository<Role>()).Returns(roleRepoMock.Object);
        uowMock.Setup(u => u.Repository<AuditLog>()).Returns(CreateRepoMock<AuditLog>().Object);
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var pwdMock = new Mock<IPasswordHashService>(MockBehavior.Loose);
        pwdMock.Setup(p => p.HashPassword(It.IsAny<string>())).Returns("hashed");

        var httpMock = new Mock<IHttpContextAccessor>(MockBehavior.Loose);
        var service = new UserService(uowMock.Object, pwdMock.Object, httpMock.Object);
        var model = new UserUpsertViewModel
        {
            UserName = "teacher_user",
            Email = "teacher@test.com",
            Password = "Password1",
            Status = AccountStatus.Active,
            SelectedRoleIds = [1]
        };

        var id = await service.CreateAsync(model, "admin");

        Assert.Equal(1, id);
        Assert.Contains(userRoles, ur => ur.UserId == id && ur.RoleId == 1);
    }

    // ─── UPDATE TESTS ───────────────────────────────────────────────

    [Fact(DisplayName = "4. UpdateAsync updates user when valid data provided")]
    public async Task UpdateAsync_UpdatesUser_WhenValidDataProvided()
    {
        var user = new ApplicationUser
        {
            Id = 1,
            UserName = "oldname",
            Email = "old@test.com",
            Status = AccountStatus.Active,
            IsDeleted = false,
            PasswordHash = "oldhash"
        };
        var users = new List<ApplicationUser> { user };
        var userRepoMock = CreateRepoMock(users);
        userRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.Repository<ApplicationUser>()).Returns(userRepoMock.Object);
        uowMock.Setup(u => u.Repository<UserRole>()).Returns(CreateRepoMock<UserRole>().Object);
        uowMock.Setup(u => u.Repository<Role>()).Returns(CreateRepoMock<Role>().Object);
        uowMock.Setup(u => u.Repository<AuditLog>()).Returns(CreateRepoMock<AuditLog>().Object);
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var pwdMock = new Mock<IPasswordHashService>(MockBehavior.Loose);
        pwdMock.Setup(p => p.HashPassword(It.IsAny<string>())).Returns("newhash");

        var httpMock = new Mock<IHttpContextAccessor>(MockBehavior.Loose);
        var service = new UserService(uowMock.Object, pwdMock.Object, httpMock.Object);
        var model = new UserUpsertViewModel
        {
            Id = 1,
            UserName = "newname",
            Email = "new@test.com",
            Status = AccountStatus.Inactive
        };

        await service.UpdateAsync(model, "editor");

        Assert.Equal("newname", user.UserName);
        Assert.Equal("new@test.com", user.Email);
        Assert.Equal(AccountStatus.Inactive, user.Status);
        Assert.Equal("editor", user.UpdatedBy);
        Assert.NotNull(user.UpdatedAt);
    }

    [Fact(DisplayName = "5. UpdateAsync calls GetCurrentUserId for performer")]
    public async Task UpdateAsync_PassesPerformerIdToAssignRoles()
    {
        var user = new ApplicationUser
        {
            Id = 1,
            UserName = "user",
            Email = "u@test.com",
            Status = AccountStatus.Active,
            IsDeleted = false,
            PasswordHash = "hash"
        };
        var users = new List<ApplicationUser> { user };
        var userRepoMock = CreateRepoMock(users);
        userRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var userRoles = new List<UserRole>();
        var userRoleRepoMock = new Mock<IBaseRepository<UserRole>>(MockBehavior.Loose);
        userRoleRepoMock.Setup(r => r.Query()).Returns(userRoles.AsAsyncQueryable());
        userRoleRepoMock.Setup(r => r.ListAsync(It.IsAny<Expression<Func<UserRole, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<UserRole, bool>>? expr, CancellationToken _) =>
                userRoles.AsQueryable().Where(expr ?? (ur => true)).ToList().AsReadOnly());
        userRoleRepoMock.Setup(r => r.AddAsync(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()))
            .Callback<UserRole, CancellationToken>((ur, _) => userRoles.Add(ur));
        // Performer (user 42) has Super Admin role, so they can assign any role
        var performerRoles = new List<UserRole>
        {
            new() { UserId = 42, RoleId = 99, Role = new Role { Id = 99, Name = "Super Admin" } }
        };
        userRoleRepoMock.Setup(r => r.Query()).Returns(performerRoles.Concat(userRoles).AsAsyncQueryable());

        var roles = new List<Role> { new() { Id = 2, Name = "Teacher" } };
        var roleRepoMock = CreateRepoMock(roles);

        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.Repository<ApplicationUser>()).Returns(userRepoMock.Object);
        uowMock.Setup(u => u.Repository<UserRole>()).Returns(userRoleRepoMock.Object);
        uowMock.Setup(u => u.Repository<Role>()).Returns(roleRepoMock.Object);
        uowMock.Setup(u => u.Repository<AuditLog>()).Returns(CreateRepoMock<AuditLog>().Object);
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var pwdMock = new Mock<IPasswordHashService>(MockBehavior.Loose);

        var httpContext = new DefaultHttpContext();
        var claimsPrincipal = new System.Security.Claims.ClaimsPrincipal(
            new System.Security.Claims.ClaimsIdentity([
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "42")
            ]));
        httpContext.User = claimsPrincipal;
        var httpMock = new Mock<IHttpContextAccessor>(MockBehavior.Loose);
        httpMock.Setup(h => h.HttpContext).Returns(httpContext);

        var service = new UserService(uowMock.Object, pwdMock.Object, httpMock.Object);
        var model = new UserUpsertViewModel
        {
            Id = 1,
            UserName = "user",
            Email = "u@test.com",
            Status = AccountStatus.Active,
            SelectedRoleIds = [2]
        };

        await service.UpdateAsync(model, "editor");

        // Verify role was assigned; performerId=42 (Super Admin) allowed assignment
        Assert.Contains(userRoles, ur => ur.UserId == 1 && ur.RoleId == 2);
    }

    // ─── DELETE TESTS ────────────────────────────────────────────────

    [Fact(DisplayName = "6. DeleteAsync soft-deletes user")]
    public async Task DeleteAsync_SoftDeletesUser()
    {
        var user = new ApplicationUser
        {
            Id = 1,
            UserName = "todelete",
            Email = "del@test.com",
            IsDeleted = false,
            Status = AccountStatus.Active,
            PasswordHash = "hash"
        };
        var users = new List<ApplicationUser> { user };
        var userRepoMock = CreateRepoMock(users);

        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.Repository<ApplicationUser>()).Returns(userRepoMock.Object);
        uowMock.Setup(u => u.Repository<AuditLog>()).Returns(CreateRepoMock<AuditLog>().Object);
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var pwdMock = new Mock<IPasswordHashService>(MockBehavior.Loose);
        var httpMock = new Mock<IHttpContextAccessor>(MockBehavior.Loose);
        var service = new UserService(uowMock.Object, pwdMock.Object, httpMock.Object);

        await service.DeleteAsync(1, "deleter");

        Assert.True(user.IsDeleted);
        Assert.Equal(AccountStatus.Inactive, user.Status);
        Assert.Equal("deleter", user.UpdatedBy);
        Assert.NotNull(user.UpdatedAt);
    }

    // ─── GET PAGED TESTS ────────────────────────────────────────────

    [Fact(DisplayName = "7. GetPagedAsync returns paged results")]
    public async Task GetPagedAsync_ReturnsPagedResults()
    {
        var users = Enumerable.Range(1, 25).Select(i => new ApplicationUser
        {
            Id = i,
            UserName = $"user{i}",
            Email = $"user{i}@test.com",
            IsDeleted = false,
            Status = AccountStatus.Active,
            PasswordHash = "hash",
            UserRoles = new List<UserRole>
            {
                new() { UserId = i, RoleId = 1, Role = new Role { Id = 1, Name = "Teacher" } }
            }
        }).ToList();

        var userRepoMock = CreateRepoMock(users);
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.Repository<ApplicationUser>()).Returns(userRepoMock.Object);
        uowMock.Setup(u => u.Repository<Employee>()).Returns(CreateRepoMock<Employee>().Object);
        uowMock.Setup(u => u.Repository<Guardian>()).Returns(CreateRepoMock<Guardian>().Object);
        uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var pwdMock = new Mock<IPasswordHashService>(MockBehavior.Loose);
        var httpMock = new Mock<IHttpContextAccessor>(MockBehavior.Loose);
        var service = new UserService(uowMock.Object, pwdMock.Object, httpMock.Object);

        var result = await service.GetPagedAsync(1, 10, null);

        Assert.NotNull(result);
        Assert.Equal(25, result.TotalItems);
        Assert.Equal(10, result.Items.Count);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
    }

    [Fact(DisplayName = "8. GetPagedAsync filters by search term")]
    public async Task GetPagedAsync_FiltersBySearchTerm()
    {
        var users = new List<ApplicationUser>
        {
            new() { Id = 1, UserName = "john", Email = "john@test.com", IsDeleted = false, Status = AccountStatus.Active, PasswordHash = "hash", UserRoles = [] },
            new() { Id = 2, UserName = "jane", Email = "jane@test.com", IsDeleted = false, Status = AccountStatus.Active, PasswordHash = "hash", UserRoles = [] },
            new() { Id = 3, UserName = "bob", Email = "bob@test.com", IsDeleted = false, Status = AccountStatus.Active, PasswordHash = "hash", UserRoles = [] }
        };

        var userRepoMock = CreateRepoMock(users);
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.Repository<ApplicationUser>()).Returns(userRepoMock.Object);
        uowMock.Setup(u => u.Repository<Employee>()).Returns(CreateRepoMock<Employee>().Object);
        uowMock.Setup(u => u.Repository<Guardian>()).Returns(CreateRepoMock<Guardian>().Object);

        var pwdMock = new Mock<IPasswordHashService>(MockBehavior.Loose);
        var httpMock = new Mock<IHttpContextAccessor>(MockBehavior.Loose);
        var service = new UserService(uowMock.Object, pwdMock.Object, httpMock.Object);

        var result = await service.GetPagedAsync(1, 10, search: "john");

        Assert.Single(result.Items);
        Assert.Contains(result.Items, i => i.UserName == "john");
    }

    [Fact(DisplayName = "9. GetPagedAsync filters by status")]
    public async Task GetPagedAsync_FiltersByStatus()
    {
        var users = new List<ApplicationUser>
        {
            new() { Id = 1, UserName = "active1", Email = "a1@test.com", IsDeleted = false, Status = AccountStatus.Active, PasswordHash = "hash", UserRoles = [] },
            new() { Id = 2, UserName = "active2", Email = "a2@test.com", IsDeleted = false, Status = AccountStatus.Active, PasswordHash = "hash", UserRoles = [] },
            new() { Id = 3, UserName = "inactive1", Email = "i1@test.com", IsDeleted = false, Status = AccountStatus.Inactive, PasswordHash = "hash", UserRoles = [] }
        };

        var userRepoMock = CreateRepoMock(users);
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.Repository<ApplicationUser>()).Returns(userRepoMock.Object);
        uowMock.Setup(u => u.Repository<Employee>()).Returns(CreateRepoMock<Employee>().Object);
        uowMock.Setup(u => u.Repository<Guardian>()).Returns(CreateRepoMock<Guardian>().Object);

        var pwdMock = new Mock<IPasswordHashService>(MockBehavior.Loose);
        var httpMock = new Mock<IHttpContextAccessor>(MockBehavior.Loose);
        var service = new UserService(uowMock.Object, pwdMock.Object, httpMock.Object);

        var result = await service.GetPagedAsync(1, 10, null, status: (int)AccountStatus.Active);

        Assert.Equal(2, result.TotalItems);
        Assert.All(result.Items, i => Assert.Equal(AccountStatus.Active, i.Status));
    }

    [Fact(DisplayName = "10. GetPagedAsync populates UserListItemVm projections")]
    public async Task GetPagedAsync_UsesDtoProjection()
    {
        var users = new List<ApplicationUser>
        {
            new()
            {
                Id = 1, UserName = "emp_user", Email = "emp@test.com",
                IsDeleted = false, Status = AccountStatus.Active,
                PasswordHash = "hash", EmployeeId = 10,
                UserRoles = new List<UserRole>
                {
                    new() { UserId = 1, RoleId = 1, Role = new Role { Id = 1, Name = "Employee" } }
                }
            }
        };

        var userRepoMock = CreateRepoMock(users);
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.Repository<ApplicationUser>()).Returns(userRepoMock.Object);

        var employees = new List<Employee>
        {
            new() { Id = 10, UserId = 1, FullName = "John Employee", IsTeachingStaff = true, ProfilePicturePath = "/photos/john.jpg" }
        };
        uowMock.Setup(u => u.Repository<Employee>()).Returns(CreateRepoMock(employees).Object);
        uowMock.Setup(u => u.Repository<Guardian>()).Returns(CreateRepoMock<Guardian>().Object);

        var pwdMock = new Mock<IPasswordHashService>(MockBehavior.Loose);
        var httpMock = new Mock<IHttpContextAccessor>(MockBehavior.Loose);
        var service = new UserService(uowMock.Object, pwdMock.Object, httpMock.Object);

        var result = await service.GetPagedAsync(1, 10, null);

        var item = result.Items.Single();
        Assert.Equal("Employee", item.UserType);
        Assert.Equal("John Employee", item.LinkedEntityName);
        Assert.True(item.IsTeachingStaff);
        Assert.Equal("/photos/john.jpg", item.ProfilePicturePath);
        Assert.Contains("Employee", item.RolesText);
    }

    [Fact(DisplayName = "11. GetPagedAsync guardian filter uses correlated subquery")]
    public async Task GetPagedAsync_GuardianFilter_DoesNotUseInClause()
    {
        var users = new List<ApplicationUser>
        {
            new() { Id = 1, UserName = "guardian_user", Email = "g@test.com", IsDeleted = false, Status = AccountStatus.Active, PasswordHash = "hash", UserRoles = [] },
            new() { Id = 2, UserName = "other", Email = "o@test.com", IsDeleted = false, Status = AccountStatus.Active, PasswordHash = "hash", UserRoles = [] }
        };
        var userRepoMock = CreateRepoMock(users);

        var guardians = new List<Guardian>
        {
            new() { Id = 1, UserId = 1, FullName = "Guardian One", IsDeleted = false }
        };
        var guardianRepoMock = CreateRepoMock(guardians);

        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.Repository<ApplicationUser>()).Returns(userRepoMock.Object);
        uowMock.Setup(u => u.Repository<Guardian>()).Returns(guardianRepoMock.Object);
        uowMock.Setup(u => u.Repository<Employee>()).Returns(CreateRepoMock<Employee>().Object);

        var pwdMock = new Mock<IPasswordHashService>(MockBehavior.Loose);
        var httpMock = new Mock<IHttpContextAccessor>(MockBehavior.Loose);
        var service = new UserService(uowMock.Object, pwdMock.Object, httpMock.Object);

        var result = await service.GetPagedAsync(1, 10, null, userType: "Guardian");

        Assert.Single(result.Items);
        Assert.Equal("guardian_user", result.Items[0].UserName);
    }

    // ─── GET DETAILS TESTS ──────────────────────────────────────────

    [Fact(DisplayName = "12. GetDetailsAsync returns user details with roles")]
    public async Task GetDetailsAsync_ReturnsUserDetails()
    {
        var users = new List<ApplicationUser>
        {
            new()
            {
                Id = 1, UserName = "emp_user", Email = "emp@test.com",
                IsDeleted = false, Status = AccountStatus.Active,
                PasswordHash = "hash", CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow.AddDays(-1),
                UserRoles = new List<UserRole>
                {
                    new() { UserId = 1, RoleId = 1, Role = new Role { Id = 1, Name = "Employee" } }
                }
            }
        };

        var userRepoMock = CreateRepoMock(users);
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Loose);
        uowMock.Setup(u => u.Repository<ApplicationUser>()).Returns(userRepoMock.Object);

        var employees = new List<Employee>
        {
            new() { Id = 10, UserId = 1, FullName = "John Employee" }
        };
        uowMock.Setup(u => u.Repository<Employee>()).Returns(CreateRepoMock(employees).Object);
        uowMock.Setup(u => u.Repository<Guardian>()).Returns(CreateRepoMock<Guardian>().Object);

        var pwdMock = new Mock<IPasswordHashService>(MockBehavior.Loose);
        var httpMock = new Mock<IHttpContextAccessor>(MockBehavior.Loose);
        var service = new UserService(uowMock.Object, pwdMock.Object, httpMock.Object);

        var details = await service.GetDetailsAsync(1);

        Assert.NotNull(details);
        Assert.Equal("emp_user", details.UserName);
        Assert.Equal("emp@test.com", details.Email);
        Assert.Equal("Employee", details.UserType);
        Assert.Equal("John Employee", details.LinkedEntityName);
        Assert.Contains("Employee", details.Roles);
        Assert.NotNull(details.LastLoginAt);
        Assert.NotNull(details.CreatedAt);
    }
}
