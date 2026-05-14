using System.Net;
using System.Text.Json;
using SchoolManagementSystem.Models.Common;

namespace SchoolManagementSystem.Middleware;

/// <summary>
/// Global exception handling middleware.
/// - Maps domain exceptions to structured JSON responses (for AJAX/API requests).
/// - Redirects HTML requests to friendly error pages.
/// - Logs all unexpected exceptions with full context.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next  = next;
        _logger = logger;
        _env   = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, errors) = exception switch
        {
            NotFoundException e      => (e.StatusCode, e.Message, Array.Empty<string>()),
            ForbiddenException e     => (e.StatusCode, e.Message, Array.Empty<string>()),
            BusinessRuleException e  => (e.StatusCode, e.Message, Array.Empty<string>()),
            Models.Common.ValidationException e => (e.StatusCode, e.Message, e.Errors.ToArray()),
            ConflictException e      => (e.StatusCode, e.Message, Array.Empty<string>()),
            UnauthorizedAccessException => (401, "Unauthorized. Please log in.", Array.Empty<string>()),
            _                        => (500, _env.IsDevelopment() ? exception.Message : "An unexpected error occurred.", Array.Empty<string>())
        };

        // Log appropriately
        if (statusCode >= 500)
            _logger.LogError(exception, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);
        else if (statusCode >= 400)
            _logger.LogWarning("Domain exception [{Status}] on {Method} {Path}: {Message}", statusCode, context.Request.Method, context.Request.Path, exception.Message);

        // JSON response for AJAX requests
        if (IsAjaxRequest(context))
        {
            context.Response.StatusCode  = statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                message,
                errors,
                statusCode,
                traceId = context.TraceIdentifier
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, _json));
            return;
        }

        // HTML page redirect for browser requests
        context.Response.Redirect(statusCode switch
        {
            401 => "/Auth/Login",
            403 => "/Error/Forbidden",
            404 => "/Error/NotFound",
            _   => "/Error/Server"
        });
    }

    private static bool IsAjaxRequest(HttpContext context)
        => context.Request.Headers["X-Requested-With"] == "XMLHttpRequest"
        || context.Request.Headers["Accept"].ToString().Contains("application/json");
}
