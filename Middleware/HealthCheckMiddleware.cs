using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;

namespace SchoolManagementSystem.Middleware;

public class HealthCheckMiddleware
{
    private readonly RequestDelegate _next;

    public HealthCheckMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, IServiceProvider sp)
    {
        var path = context.Request.Path.Value?.Trim('/').ToLowerInvariant();

        if (path == "health" || path == "live")
        {
            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            var json = "{\"status\":\"healthy\",\"timestamp\":\"" +
                       DateTime.UtcNow.ToString("O") +
                       "\",\"service\":\"schoolms\"}";
            await context.Response.WriteAsync(json);
            return;
        }

        if (path == "ready")
        {
            try
            {
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();
                await db.Database.ExecuteSqlRawAsync("SELECT 1");
                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                var json = "{\"status\":\"ready\",\"database\":\"connected\",\"timestamp\":\"" +
                           DateTime.UtcNow.ToString("O") +
                           "\"}";
                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 503;
                context.Response.ContentType = "application/json";
                var encoded = System.Text.Json.JsonEncodedText.Encode(ex.Message).ToString();
                var json = "{\"status\":\"unready\",\"database\":\"" + encoded + "\",\"timestamp\":\"" +
                           DateTime.UtcNow.ToString("O") +
                           "\"}";
                await context.Response.WriteAsync(json);
            }
            return;
        }

        await _next(context);
    }
}
