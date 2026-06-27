using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SchoolManagementSystem.Helpers.Email;
using SchoolManagementSystem.Repositories.Interfaces.Website;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class EmailUrlResolverTests
{
    private readonly Mock<IHttpContextAccessor> _httpMock = new(MockBehavior.Loose);
    private readonly Mock<ISchoolSettingRepository> _settingRepoMock = new(MockBehavior.Loose);
    private readonly Mock<ILogger<EmailUrlResolver>> _loggerMock = new(MockBehavior.Loose);

    private EmailUrlResolver CreateResolver(string baseUrl, string localUrl, string? publicUrl = null)
    {
        var options = Options.Create(new EmailOptions
        {
            BaseUrl = baseUrl,
            LocalUrl = localUrl,
            PublicUrl = publicUrl
        });
        return new EmailUrlResolver(_httpMock.Object, options, _settingRepoMock.Object, _loggerMock.Object);
    }

    [Fact(DisplayName = "1. ResolveAsync returns HTTP request URL when HttpContext is available")]
    public async Task ResolveAsync_ReturnsHttpRequestUrl_WhenContextAvailable()
    {
        var ctx = new DefaultHttpContext();
        ctx.Request.Scheme = "https";
        ctx.Request.Host = new HostString("localhost:7192");
        _httpMock.Setup(a => a.HttpContext).Returns(ctx);

        var resolver = CreateResolver("https://school-ms-7l3e.onrender.com", "https://localhost:7192");
        var url = await resolver.ResolveAsync();

        Assert.Equal("https://localhost:7192", url);
    }

    [Fact(DisplayName = "2. ResolveAsync uses X-Forwarded-Proto and X-Forwarded-Host when present")]
    public async Task ResolveAsync_UsesForwardedHeaders_WhenPresent()
    {
        var ctx = new DefaultHttpContext();
        ctx.Request.Scheme = "http";
        ctx.Request.Host = new HostString("internal:80");
        ctx.Request.Headers["X-Forwarded-Proto"] = "https";
        ctx.Request.Headers["X-Forwarded-Host"] = "school.example.com";
        _httpMock.Setup(a => a.HttpContext).Returns(ctx);

        var resolver = CreateResolver("https://school-ms-7l3e.onrender.com", "https://localhost:7192");
        var url = await resolver.ResolveAsync();

        Assert.Equal("https://school.example.com", url);
    }

    [Fact(DisplayName = "3. ResolveAsync uses DB setting when no HTTP context")]
    public async Task ResolveAsync_UsesDbSetting_WhenNoHttpContext()
    {
        _httpMock.Setup(a => a.HttpContext).Returns((HttpContext?)null);
        _settingRepoMock.Setup(r => r.GetCurrentSettingsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SchoolManagementSystem.Models.Entities.Website.SchoolSetting
            {
                BaseUrl = "https://db-configured.example.com"
            });

        var resolver = CreateResolver("https://school-ms-7l3e.onrender.com", "https://localhost:7192");
        var url = await resolver.ResolveAsync();

        Assert.Equal("https://db-configured.example.com", url);
    }

    [Fact(DisplayName = "4. ResolveAsync falls back to EmailOptions.BaseUrl when DB is empty")]
    public async Task ResolveAsync_FallsBackToConfigBaseUrl()
    {
        _httpMock.Setup(a => a.HttpContext).Returns((HttpContext?)null);
        _settingRepoMock.Setup(r => r.GetCurrentSettingsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SchoolManagementSystem.Models.Entities.Website.SchoolSetting { BaseUrl = null });

        var resolver = CreateResolver("https://school-ms-7l3e.onrender.com", "https://localhost:7192");
        var url = await resolver.ResolveAsync();

        Assert.Equal("https://school-ms-7l3e.onrender.com", url);
    }

    [Fact(DisplayName = "5. ResolveAsync falls back to LocalUrl when BaseUrl is empty")]
    public async Task ResolveAsync_FallsBackToLocalUrl()
    {
        _httpMock.Setup(a => a.HttpContext).Returns((HttpContext?)null);
        _settingRepoMock.Setup(r => r.GetCurrentSettingsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SchoolManagementSystem.Models.Entities.Website.SchoolSetting { BaseUrl = null });

        var resolver = CreateResolver("", "https://localhost:7192");
        var url = await resolver.ResolveAsync();

        Assert.Equal("https://localhost:7192", url);
    }

    [Fact(DisplayName = "6. ResolveAsync uses production fallback when all sources are empty")]
    public async Task ResolveAsync_UsesProductionFallback()
    {
        _httpMock.Setup(a => a.HttpContext).Returns((HttpContext?)null);
        _settingRepoMock.Setup(r => r.GetCurrentSettingsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SchoolManagementSystem.Models.Entities.Website.SchoolSetting { BaseUrl = null });

        var resolver = CreateResolver("", "");
        var url = await resolver.ResolveAsync();

        Assert.StartsWith("https://", url);
        Assert.DoesNotContain("localhost", url);
    }

    [Fact(DisplayName = "7. Development appsettings overrides production BaseUrl")]
    public async Task DevelopmentConfigOverridesProductionBaseUrl()
    {
        // Simulate appsettings.Development.json override via ASP.NET Core config hierarchy
        _httpMock.Setup(a => a.HttpContext).Returns((HttpContext?)null);
        _settingRepoMock.Setup(r => r.GetCurrentSettingsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SchoolManagementSystem.Models.Entities.Website.SchoolSetting { BaseUrl = null });

        // When running in Development, the config system merges appsettings.Development.json
        // which sets Email:BaseUrl to https://localhost:7192/
        var resolver = CreateResolver("https://localhost:7192/", "https://localhost:7192");
        var url = await resolver.ResolveAsync();

        Assert.Equal("https://localhost:7192", url);
    }

    [Fact(DisplayName = "8. HTTP request URL always wins even when DB has different value")]
    public async Task HttpRequestUrlWinsOverDbSetting()
    {
        var ctx = new DefaultHttpContext();
        ctx.Request.Scheme = "http";
        ctx.Request.Host = new HostString("localhost:3000");
        _httpMock.Setup(a => a.HttpContext).Returns(ctx);

        _settingRepoMock.Setup(r => r.GetCurrentSettingsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SchoolManagementSystem.Models.Entities.Website.SchoolSetting
            {
                BaseUrl = "https://db.example.com"
            });

        var resolver = CreateResolver("https://config.example.com", "https://localhost:7192");
        var url = await resolver.ResolveAsync();

        Assert.Equal("http://localhost:3000", url);
    }
}

public class TemplatePlaceholderValidatorTests
{
    [Fact(DisplayName = "1. FindUnresolved returns empty for fully resolved text")]
    public void FindUnresolved_ReturnsEmpty_ForResolvedText()
    {
        var text = "<p>Hello John, welcome to School Name!</p>";
        var unresolved = TemplatePlaceholderValidator.FindUnresolved(text);
        Assert.Empty(unresolved);
    }

    [Fact(DisplayName = "2. FindUnresolved detects single unresolved placeholder")]
    public void FindUnresolved_DetectsSinglePlaceholder()
    {
        var text = "<p>Dear {GuardianName},</p>";
        var unresolved = TemplatePlaceholderValidator.FindUnresolved(text);
        Assert.Contains("{GuardianName}", unresolved);
    }

    [Fact(DisplayName = "3. FindUnresolved detects multiple unresolved placeholders")]
    public void FindUnresolved_DetectsMultiplePlaceholders()
    {
        var text = "<p>Dear {GuardianName}, your child {StudentName} was absent.</p>";
        var unresolved = TemplatePlaceholderValidator.FindUnresolved(text);
        Assert.Contains("{GuardianName}", unresolved);
        Assert.Contains("{StudentName}", unresolved);
        Assert.Equal(2, unresolved.Count);
    }

    [Fact(DisplayName = "4. HasUnresolved returns false for clean text")]
    public void HasUnresolved_ReturnsFalse_ForCleanText()
    {
        Assert.False(TemplatePlaceholderValidator.HasUnresolved("No placeholders here"));
    }

    [Fact(DisplayName = "5. HasUnresolved returns true for text with placeholders")]
    public void HasUnresolved_ReturnsTrue_ForTextWithPlaceholders()
    {
        Assert.True(TemplatePlaceholderValidator.HasUnresolved("Hello {Name}"));
    }

    [Fact(DisplayName = "6. HasUnresolved returns false for null or empty")]
    public void HasUnresolved_ReturnsFalse_ForNullOrEmpty()
    {
        Assert.False(TemplatePlaceholderValidator.HasUnresolved(null));
        Assert.False(TemplatePlaceholderValidator.HasUnresolved(""));
    }

    [Fact(DisplayName = "7. SanitizeForLog truncates long text")]
    public void SanitizeForLog_TruncatesLongText()
    {
        var longText = new string('x', 500);
        var result = TemplatePlaceholderValidator.SanitizeForLog(longText);
        Assert.Equal(203, result.Length);
        Assert.EndsWith("...", result);
    }

    [Fact(DisplayName = "8. SanitizeForLog returns null as null")]
    public void SanitizeForLog_ReturnsNullForNull()
    {
        Assert.Null(TemplatePlaceholderValidator.SanitizeForLog(null));
    }
}
