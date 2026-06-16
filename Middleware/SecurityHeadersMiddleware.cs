using Microsoft.AspNetCore.Http;

namespace SchoolManagementSystem.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var response = context.Response;

        // Prevent MIME type sniffing
        response.Headers["X-Content-Type-Options"] = "nosniff";

        // Enable XSS filter in older browsers
        response.Headers["X-XSS-Protection"] = "1; mode=block";

        // Prevent clickjacking
        response.Headers["X-Frame-Options"] = "DENY";

        // Referrer policy
        response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        // HSTS (if HTTPS)
        if (context.Request.IsHttps)
        {
            response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
        }

        // Content-Security-Policy
        response.Headers["Content-Security-Policy"] =
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' 'unsafe-eval' " +
                "https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://unpkg.com https://cdn.quilljs.com; " +
            "style-src 'self' 'unsafe-inline' " +
                "https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://unpkg.com https://cdn.quilljs.com https://fonts.googleapis.com; " +
            "img-src 'self' data: https://www.transparenttextures.com; " +
            "font-src 'self' data: https://cdn.jsdelivr.net https://fonts.gstatic.com; " +
            "connect-src 'self' https://cdn.jsdelivr.net; " +
            "frame-ancestors 'none';";

        // Permissions-Policy
        response.Headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";

        await _next(context);
    }
}

public static class SecurityHeadersMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
        => builder.UseMiddleware<SecurityHeadersMiddleware>();
}
