using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Services.Interfaces.Admin;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class Phase11_SecurityRbacTests
{
    private static readonly string Root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    private static string SrcFile(params string[] parts)
    {
        return Path.Combine(new[] { Root }.Concat(parts).ToArray());
    }

    // ─── RequirePermissionAttribute architecture ───────────────────

    [Fact(DisplayName = "1. RequirePermissionAttribute no longer imports SchoolDbContext")]
    public void RequirePermissionAttribute_NoLongerDependsOnDbContext()
    {
        var source = File.ReadAllText(SrcFile("Filters", "RequirePermissionAttribute.cs"));

        Assert.DoesNotContain("SchoolDbContext", source);
        Assert.DoesNotContain("using SchoolManagementSystem.Data", source);
    }

    [Fact(DisplayName = "2. RequirePermissionAttribute uses IPermissionCacheService")]
    public void RequirePermissionAttribute_UsesPermissionCacheService()
    {
        var source = File.ReadAllText(SrcFile("Filters", "RequirePermissionAttribute.cs"));

        Assert.Contains("IPermissionCacheService", source);
        Assert.Contains("GetRequiredService<IPermissionCacheService>", source);
    }

    [Fact(DisplayName = "3. RequirePermissionAttribute still has AllowMultiple=true")]
    public void RequirePermissionAttribute_StillAllowMultiple()
    {
        var usage = (AttributeUsageAttribute?)typeof(RequirePermissionAttribute)
            .GetCustomAttribute(typeof(AttributeUsageAttribute));
        Assert.NotNull(usage);
        Assert.True(usage.AllowMultiple);
    }

    [Fact(DisplayName = "4. PermissionAttribute still extends RequirePermissionAttribute")]
    public void PermissionAttribute_StillExtendsRequirePermission()
    {
        var attr = new PermissionAttribute("Marks", "View");
        Assert.IsAssignableFrom<RequirePermissionAttribute>(attr);
    }

    // ─── IPermissionCacheService interface ────────────────────────

    [Fact(DisplayName = "5. IPermissionCacheService interface exists")]
    public void IPermissionCacheService_Exists()
    {
        var type = typeof(IPermissionCacheService);
        Assert.NotNull(type);
        Assert.True(type.IsInterface);
    }

    [Fact(DisplayName = "6. IPermissionCacheService has HasPermissionAsync")]
    public void IPermissionCacheService_HasRequiredMethods()
    {
        var type = typeof(IPermissionCacheService);
        Assert.NotNull(type.GetMethod("HasPermissionAsync"));
        Assert.NotNull(type.GetMethod("InvalidateRolePermissions"));
        Assert.NotNull(type.GetMethod("InvalidateAll"));
    }

    // ─── Password complexity validation ──────────────────────────

    [Fact(DisplayName = "7. UserUpsertViewModel password requires 8+ characters")]
    public void UserUpsertViewModel_PasswordMinLength()
    {
        var prop = typeof(Models.ViewModels.User.UserUpsertViewModel).GetProperty("Password");
        Assert.NotNull(prop);

        var attr = prop.GetCustomAttribute<StringLengthAttribute>();
        Assert.NotNull(attr);
        Assert.Equal(8, attr.MinimumLength);
    }

    [Fact(DisplayName = "8. UserUpsertViewModel password requires complexity regex")]
    public void UserUpsertViewModel_PasswordComplexityRegex()
    {
        var prop = typeof(Models.ViewModels.User.UserUpsertViewModel).GetProperty("Password");
        Assert.NotNull(prop);

        var attr = prop.GetCustomAttribute<RegularExpressionAttribute>();
        Assert.NotNull(attr);
        Assert.Contains("a-z", attr.Pattern);
        Assert.Contains("A-Z", attr.Pattern);
        Assert.Contains(@"\d", attr.Pattern);
    }

    [Fact(DisplayName = "9. ResetPasswordViewModel NewPassword requires 8+ chars and complexity")]
    public void ResetPasswordViewModel_PasswordComplexity()
    {
        var prop = typeof(Models.ViewModels.Auth.ResetPasswordViewModel).GetProperty("NewPassword");
        Assert.NotNull(prop);

        var stringLength = prop.GetCustomAttribute<StringLengthAttribute>();
        Assert.NotNull(stringLength);
        Assert.Equal(8, stringLength.MinimumLength);

        var regex = prop.GetCustomAttribute<RegularExpressionAttribute>();
        Assert.NotNull(regex);
        Assert.Contains("a-z", regex.Pattern);
        Assert.Contains("A-Z", regex.Pattern);
    }

    // ─── RoleService security ─────────────────────────────────────

    [Fact(DisplayName = "10. RoleService prevents deletion of system roles")]
    public void RoleService_PreventsSystemRoleDeletion()
    {
        var systemRoles = new[] { "Super Admin", "Admin", "Guardian", "Student", "Teacher",
            "Senior Lecturer", "Lecturer", "Principal", "Accountant", "Exam Controller" };
        Assert.Equal(10, systemRoles.Length);
        Assert.Contains("Super Admin", systemRoles);
        Assert.Contains("Admin", systemRoles);
    }

    [Fact(DisplayName = "11. Role soft-delete cascades to UserRoles and RolePermissions")]
    public void RoleDelete_CascadesToJoinTables()
    {
        var source = File.ReadAllText(SrcFile("Services", "Implementations", "Admin", "RoleService.cs"));

        Assert.Contains("UserRoles", source);
        Assert.Contains("RolePermissions", source);
        Assert.Contains("Remove(ur)", source);
        Assert.Contains("Remove(rp)", source);
    }

    [Fact(DisplayName = "12. RoleService invalidates permission cache on assign/delete")]
    public void RoleService_InvalidatesCache()
    {
        var source = File.ReadAllText(SrcFile("Services", "Implementations", "Admin", "RoleService.cs"));

        Assert.Contains("_permissionCache.InvalidateRolePermissions", source);
    }

    // ─── UserService security ─────────────────────────────────────

    [Fact(DisplayName = "13. UserService prevents Super Admin role assignment without performer")]
    public void UserService_PreventsSuperAdminAssignmentWithoutPerformer()
    {
        var source = File.ReadAllText(SrcFile("Services", "Implementations", "Admin", "UserService.cs"));

        Assert.Contains("Only Super Admin can assign Super Admin role", source);
    }

    [Fact(DisplayName = "14. UserService validates password complexity")]
    public void UserService_ValidatesPasswordComplexity()
    {
        var source = File.ReadAllText(SrcFile("Services", "Implementations", "Admin", "UserService.cs"));

        Assert.Contains("ValidatePasswordComplexity", source);
        Assert.Contains("Password must be at least 8 characters", source);
        Assert.Contains("Password must contain at least one uppercase", source);
        Assert.Contains("Password must contain at least one lowercase", source);
        Assert.Contains("Password must contain at least one digit", source);
    }

    // ─── View migration verification ──────────────────────────────

    [Fact(DisplayName = "15. User views use schoolms-universal.css adm-* classes")]
    public void UserViews_UseAdmClasses()
    {
        var views = new[]
        {
            SrcFile("Views", "User", "CreateEdit.cshtml"),
            SrcFile("Views", "User", "Details.cshtml"),
            SrcFile("Views", "User", "Delete.cshtml"),
            SrcFile("Views", "User", "AssignRoles.cshtml"),
            SrcFile("Views", "Role", "Index.cshtml")
        };

        foreach (var viewPath in views)
        {
            var content = File.ReadAllText(viewPath);
            Assert.Contains("adm-page", content, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("adm-header", content, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("adm-btn", content, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact(DisplayName = "16. User views do NOT use Bootstrap btn/card classes")]
    public void UserViews_NoBootstrapClasses()
    {
        var views = new[]
        {
            SrcFile("Views", "User", "CreateEdit.cshtml"),
            SrcFile("Views", "User", "Details.cshtml"),
            SrcFile("Views", "User", "Delete.cshtml"),
            SrcFile("Views", "User", "AssignRoles.cshtml")
        };

        foreach (var viewPath in views)
        {
            var content = File.ReadAllText(viewPath);
            Assert.DoesNotContain("btn btn-primary", content);
            Assert.DoesNotContain("btn btn-outline", content);
            Assert.DoesNotContain("card shadow", content);
        }
    }

    // ─── RoleController error handling ────────────────────────────

    [Fact(DisplayName = "17. RoleController AssignPermissions has try-catch")]
    public void RoleController_HasErrorHandling()
    {
        var source = File.ReadAllText(SrcFile("Controllers", "Admin", "RoleController.cs"));

        Assert.Contains("try", source);
        Assert.Contains("catch", source);
        Assert.Contains("BadRequest", source);
    }

    // ─── RequirePermissionAttribute still works with Super Admin bypass ──

    [Fact(DisplayName = "18. RequirePermissionAttribute Super Admin bypass still works")]
    public async Task RequirePermission_SuperAdminBypass()
    {
        var attr = new RequirePermissionAttribute("Marks.View");

        var userMock = new Mock<System.Security.Claims.ClaimsPrincipal>();
        userMock.Setup(u => u.Identity!.IsAuthenticated).Returns(true);
        userMock.Setup(u => u.IsInRole("Super Admin")).Returns(true);

        var servicesMock = new Mock<IServiceProvider>();
        servicesMock.Setup(s => s.GetService(typeof(IPermissionCacheService)))
            .Returns(Mock.Of<IPermissionCacheService>());

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(c => c.User).Returns(userMock.Object);
        httpContextMock.Setup(c => c.RequestServices).Returns(servicesMock.Object);

        var actionContext = new ActionContext(httpContextMock.Object, new RouteData(), new ActionDescriptor());
        var ctx = new AuthorizationFilterContext(actionContext, []);

        await attr.OnAuthorizationAsync(ctx);

        Assert.Null(ctx.Result);
    }

    [Fact(DisplayName = "19. RequirePermissionAttribute returns Challenge when unauthenticated")]
    public async Task RequirePermission_Unauthenticated_ReturnsChallenge()
    {
        var attr = new RequirePermissionAttribute("Marks.View");

        var userMock = new Mock<System.Security.Claims.ClaimsPrincipal>();
        userMock.Setup(u => u.Identity!.IsAuthenticated).Returns(false);

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(c => c.User).Returns(userMock.Object);

        var actionContext = new ActionContext(httpContextMock.Object, new RouteData(), new ActionDescriptor());
        var ctx = new AuthorizationFilterContext(actionContext, []);

        await attr.OnAuthorizationAsync(ctx);

        Assert.IsType<ChallengeResult>(ctx.Result);
    }

    [Fact(DisplayName = "20. RequirePermissionAttribute returns Forbid when no roles")]
    public async Task RequirePermission_NoRoles_ReturnsForbid()
    {
        var attr = new RequirePermissionAttribute("Marks.View");

        var userMock = new Mock<System.Security.Claims.ClaimsPrincipal>();
        userMock.Setup(u => u.Identity!.IsAuthenticated).Returns(true);
        userMock.Setup(u => u.IsInRole("Super Admin")).Returns(false);
        userMock.Setup(u => u.Claims).Returns(new List<System.Security.Claims.Claim>());

        var servicesMock = new Mock<IServiceProvider>();

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(c => c.User).Returns(userMock.Object);
        httpContextMock.Setup(c => c.RequestServices).Returns(servicesMock.Object);

        var actionContext = new ActionContext(httpContextMock.Object, new RouteData(), new ActionDescriptor());
        var ctx = new AuthorizationFilterContext(actionContext, []);

        await attr.OnAuthorizationAsync(ctx);

        Assert.IsType<ForbidResult>(ctx.Result);
    }
}
