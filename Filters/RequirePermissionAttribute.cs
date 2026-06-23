using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SchoolManagementSystem.Services.Interfaces.Admin;

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

        var roles = context.HttpContext.User.Claims
            .Where(x => x.Type == System.Security.Claims.ClaimTypes.Role)
            .Select(x => x.Value)
            .Distinct()
            .ToArray();

        if (roles.Length == 0)
        {
            context.Result = new ForbidResult();
            return;
        }

        var cacheService = context.HttpContext.RequestServices.GetRequiredService<IPermissionCacheService>();
        var allowed = await cacheService.HasPermissionAsync(roles, _permissionCode, context.HttpContext.RequestAborted);

        if (!allowed)
        {
            context.Result = new ForbidResult();
        }
    }
}
