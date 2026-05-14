using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SchoolManagementSystem.Constants;

namespace SchoolManagementSystem.Filters;

/// <summary>
/// Authorizes access based on granular permission codes.
/// Supports Super Admin bypass and merged permission claims.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string _permissionCode;

    public RequirePermissionAttribute(string permissionCode)
    {
        _permissionCode = permissionCode;
    }

    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (user.Identity?.IsAuthenticated != true)
        {
            context.Result = new ChallengeResult();
            return Task.CompletedTask;
        }

        // 1. Super Admin Bypass (Centralized)
        if (user.IsInRole(Roles.SuperAdmin))
        {
            return Task.CompletedTask;
        }

        // 2. Check for merged permission claim
        // This is populated during login by AuthService and reflects the UNION of all active roles.
        var hasPermission = user.HasClaim(c => c.Type == "Permission" && c.Value == _permissionCode);

        if (!hasPermission)
        {
            context.Result = new ForbidResult();
        }

        return Task.CompletedTask;
    }
}
