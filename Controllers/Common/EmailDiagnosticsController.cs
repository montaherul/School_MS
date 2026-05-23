using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Helpers.Email;

namespace SchoolManagementSystem.Controllers.Common;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("diagnostics")]
public class EmailDiagnosticsController : ControllerBase
{
    private readonly EmailDiagnosticsService _diagnosticsService;

    public EmailDiagnosticsController(EmailDiagnosticsService diagnosticsService)
    {
        _diagnosticsService = diagnosticsService;
    }

    [HttpPost("email-test")]
    public async Task<IActionResult> TestEmail([FromQuery] string? to = null, CancellationToken cancellationToken = default)
    {
        if (!IsEnabled())
        {
            return NotFound();
        }

        var recipient = string.IsNullOrWhiteSpace(to) ? "yamif16014@okcpress.com" : to;
        var result = await _diagnosticsService.RunAsync(
            recipient,
            "School Management System Email Test",
            "This is a test email from the School Management System deployed on Render.",
            cancellationToken);

        return Ok(result);
    }

    private static bool IsEnabled()
    {
        var environmentValue = Environment.GetEnvironmentVariable("ENABLE_EMAIL_DIAGNOSTICS");
        return string.Equals(environmentValue, "true", StringComparison.OrdinalIgnoreCase)
            || string.Equals(environmentValue, "1", StringComparison.OrdinalIgnoreCase)
            || string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase);
    }
}
