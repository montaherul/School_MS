using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace SchoolManagementSystem.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var errorId = Activity.Current?.Id ?? context.TraceIdentifier;
            var userId = context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
            var url = context.Request.GetDisplayUrl();
            var controller = context.Request.RouteValues["controller"]?.ToString() ?? "Unknown";
            var action = context.Request.RouteValues["action"]?.ToString() ?? "Unknown";
            var method = context.Request.Method;

            _logger.LogError(ex,
                "[GlobalException] ErrorId={ErrorId}, UserId={UserId}, Url={Url}, Controller={Controller}, Action={Action}, Method={Method}",
                errorId, userId, url, controller, action, method);

            if (context.Response.HasStarted)
            {
                return;
            }

            context.Response.Clear();
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/html; charset=utf-8";

            var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

            var html = new System.Text.StringBuilder();
            html.Append("<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            html.Append("<title>Error - School Management System</title><style>");
            html.Append("*{margin:0;padding:0;box-sizing:border-box;}");
            html.Append("body{font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,sans-serif;background:#f5f5f5;color:#333;display:flex;align-items:center;justify-content:center;min-height:100vh;padding:2rem;}");
            html.Append(".error-card{background:#fff;border-radius:12px;box-shadow:0 4px 24px rgba(0,0,0,0.1);padding:3rem;max-width:560px;width:100%;text-align:center;}");
            html.Append(".error-icon{width:72px;height:72px;background:#fee2e2;border-radius:50%;display:flex;align-items:center;justify-content:center;margin:0 auto 1.5rem;font-size:2rem;color:#dc2626;}");
            html.Append("h1{font-size:1.5rem;margin-bottom:0.75rem;color:#111;}");
            html.Append("p{color:#666;line-height:1.6;margin-bottom:0.5rem;}");
            html.Append(".error-id{display:inline-block;background:#f0f0f0;padding:0.4rem 1rem;border-radius:6px;font-family:Consolas,monospace;font-size:0.85rem;color:#555;margin:1rem 0;}");
            html.Append(".btn{display:inline-block;margin-top:1rem;padding:0.65rem 1.5rem;background:#2563eb;color:#fff;text-decoration:none;border-radius:8px;font-weight:500;font-size:0.95rem;}");
            html.Append(".btn:hover{background:#1d4ed8;}");
            html.Append(".details{margin-top:1.5rem;text-align:left;font-size:0.85rem;color:#888;}");
            html.Append("</style></head><body><div class=\"error-card\">");
            html.Append("<div class=\"error-icon\">&#9888;</div>");
            html.Append("<h1>Something went wrong</h1>");
            html.Append("<p>An unexpected error occurred. Our team has been notified.</p>");
            html.AppendFormat("<div class=\"error-id\">Ref: {0}</div>", errorId);

            if (isDevelopment)
            {
                html.Append("<div class=\"details\">");
                html.AppendFormat("<strong>Exception:</strong> {0}<br>", ex.GetType().Name);
                html.AppendFormat("<strong>Message:</strong> {0}<br>", ex.Message);
                html.AppendFormat("<strong>Controller:</strong> {0}.{1}<br>", controller, action);
                html.AppendFormat("<strong>User:</strong> {0}<br>", userId);
                html.AppendFormat("<strong>Url:</strong> {0}<br>", url);
                html.AppendFormat("<pre style=\"margin-top:0.5rem;overflow:auto;max-height:200px;\">{0}</pre>", ex.StackTrace);
                html.Append("</div>");
            }

            html.Append("<a href=\"/\" class=\"btn\">Return to Home</a>");
            html.Append("</div></body></html>");

            await context.Response.WriteAsync(html.ToString());
        }
    }
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder builder)
        => builder.UseMiddleware<GlobalExceptionMiddleware>();
}
