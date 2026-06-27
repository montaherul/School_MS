using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SchoolManagementSystem.Repositories.Interfaces.Website;

namespace SchoolManagementSystem.Helpers.Email;

public class EmailUrlResolver
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    private readonly EmailOptions _options;
    private readonly ISchoolSettingRepository _settingRepo;
    private readonly ILogger<EmailUrlResolver> _logger;

    public EmailUrlResolver(
        IHttpContextAccessor httpContextAccessor,
        IOptions<EmailOptions> options,
        ISchoolSettingRepository settingRepo,
        ILogger<EmailUrlResolver> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _options = options.Value;
        _settingRepo = settingRepo;
        _logger = logger;
    }

    public async Task<string> ResolveAsync()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        // Priority 1: Current HTTP Request (works for localhost, IIS, Azure, Render, etc.)
        var httpContext = _httpContextAccessor?.HttpContext;
        if (httpContext?.Request != null)
        {
            var request = httpContext.Request;

            var forwardedProto = request.Headers["X-Forwarded-Proto"].FirstOrDefault();
            var forwardedHost = request.Headers["X-Forwarded-Host"].FirstOrDefault();
            var forwardedPrefix = request.Headers["X-Forwarded-Prefix"].FirstOrDefault();

            if (!string.IsNullOrEmpty(forwardedProto) && !string.IsNullOrEmpty(forwardedHost))
            {
                var resolved = $"{forwardedProto}://{forwardedHost}{forwardedPrefix ?? ""}";
                _logger.LogDebug("Resolved URL from reverse proxy headers: {Url} (env={Env})", resolved, env);
                return resolved.TrimEnd('/');
            }

            var scheme = request.Scheme;
            var host = request.Host.Value;
            var resolvedRequest = $"{scheme}://{host}{request.PathBase}";
            _logger.LogDebug("Resolved URL from HTTP request: {Url} (env={Env})", resolvedRequest, env);
            return resolvedRequest.TrimEnd('/');
        }

        // Priority 2: DB SchoolSetting.BaseUrl (admin-configured)
        try
        {
            var settings = await _settingRepo.GetCurrentSettingsAsync();
            if (!string.IsNullOrWhiteSpace(settings?.BaseUrl))
            {
                _logger.LogDebug("Resolved URL from DB SchoolSetting.BaseUrl: {Url} (env={Env})", settings.BaseUrl, env);
                return settings.BaseUrl.TrimEnd('/');
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read SchoolSetting.BaseUrl from DB (env={Env})", env);
        }

        // Priority 3: EmailOptions.BaseUrl (environment-aware via appsettings.{env}.json)
        if (!string.IsNullOrWhiteSpace(_options.BaseUrl))
        {
            _logger.LogDebug("Resolved URL from EmailOptions.BaseUrl: {Url} (env={Env})", _options.BaseUrl, env);
            return _options.BaseUrl.TrimEnd('/');
        }

        // Priority 4: EmailOptions.LocalUrl (development fallback)
        if (!string.IsNullOrWhiteSpace(_options.LocalUrl))
        {
            _logger.LogDebug("Resolved URL from EmailOptions.LocalUrl: {Url} (env={Env})", _options.LocalUrl, env);
            return _options.LocalUrl.TrimEnd('/');
        }

        // Priority 5: Production fallback (last resort)
        const string fallback = "https://school-ms-7l3e.onrender.com";
        _logger.LogWarning("No URL source found; using production fallback: {Url} (env={Env})", fallback, env);
        return fallback;
    }
}
