using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Auth;

namespace SchoolManagementSystem.Middleware;

public class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public AuditLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, SchoolDbContext db)
    {
        await _next(context);

        if (context.User.Identity?.IsAuthenticated == true &&
            HttpMethods.IsPost(context.Request.Method))
        {
            db.AuditLogs.Add(new AuditLog
            {
                Module = context.Request.Path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "System",
                Action = context.Request.Method,
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                Details = $"{context.Response.StatusCode} {context.Request.Path}",
                CreatedBy = context.User.Identity.Name ?? "system"
            });
            await db.SaveChangesAsync();
        }
    }
}
