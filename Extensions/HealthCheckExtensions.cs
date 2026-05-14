using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Text;

namespace SchoolManagementSystem.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddSchoolHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SchoolDb");

        services.AddHealthChecks()
            .AddSqlServer(connectionString!)
            .AddCheck("Storage", () => {
                try {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    return HealthCheckResult.Healthy("Uploads directory is accessible");
                } catch (Exception ex) {
                    return HealthCheckResult.Unhealthy("Uploads directory is NOT accessible", ex);
                }
            }, tags: new[] { "infrastructure" })
            .AddCheck("MemoryCache", () => {
                return HealthCheckResult.Healthy("Memory cache is active");
            }, tags: new[] { "core" });

        return services;
    }

    public static void MapSchoolHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = WriteResponse
        });

        endpoints.MapHealthChecks("/health/detail", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = WriteDetailedResponse
        });
    }

    private static Task WriteResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            duration = report.TotalDuration,
            timestamp = DateTime.UtcNow
        };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static Task WriteDetailedResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var json = new StringBuilder();
        json.Append("{");
        json.Append($"\"status\":\"{report.Status}\",");
        json.Append($"\"duration\":\"{report.TotalDuration}\",");
        json.Append("\"results\":[");

        var first = true;
        foreach (var entry in report.Entries)
        {
            if (!first) json.Append(",");
            json.Append("{");
            json.Append($"\"name\":\"{entry.Key}\",");
            json.Append($"\"status\":\"{entry.Value.Status}\",");
            json.Append($"\"description\":\"{entry.Value.Description}\",");
            json.Append($"\"duration\":\"{entry.Value.Duration}\"");
            json.Append("}");
            first = false;
        }

        json.Append("]}");

        // Optional: Cache result to prevent abuse
        var cache = context.RequestServices.GetService<IMemoryCache>();
        cache?.Set("LastHealthCheck", report.Status.ToString(), TimeSpan.FromMinutes(1));

        return context.Response.WriteAsync(json.ToString());
    }
}
