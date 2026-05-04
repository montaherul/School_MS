using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;

namespace SchoolManagementSystem.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string _permissionCode;

    public RequirePermissionAttribute(string permissionCode)
    {
        _permissionCode = permissionCode;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated != true)
        {
            context.Result = new ChallengeResult();
            return;
        }

        if (context.HttpContext.User.IsInRole("Super Admin"))
        {
            return;
        }

        var db = context.HttpContext.RequestServices.GetRequiredService<SchoolDbContext>();
        var roles = context.HttpContext.User.Claims
            .Where(x => x.Type == System.Security.Claims.ClaimTypes.Role)
            .Select(x => x.Value)
            .ToArray();

        var allowed = await db.RolePermissions
            .AnyAsync(rp => rp.Permission != null && rp.Role != null && rp.Permission.Code == _permissionCode && roles.Contains(rp.Role.Name));

        if (!allowed)
        {
            context.Result = new ForbidResult();
        }
    }
}
