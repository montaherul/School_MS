using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Controllers.SchoolPay;

[AllowAnonymous]
[Route("SchoolPay/Webhook")]
public class SchoolPayWebhookController : Controller
{
    private readonly IWebhookService _webhookService;
    private readonly ILogger<SchoolPayWebhookController> _logger;

    public SchoolPayWebhookController(IWebhookService webhookService, ILogger<SchoolPayWebhookController> logger)
    {
        _webhookService = webhookService;
        _logger = logger;
    }

    [HttpPost("Receive/{providerCode}")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Receive(string providerCode, CancellationToken ct)
    {
        using var reader = new StreamReader(Request.Body);
        var rawPayload = await reader.ReadToEndAsync(ct);

        var transactionRef = Request.Form["tran_id"].FirstOrDefault()
            ?? Request.Query["tran_id"].FirstOrDefault();
        var eventType = Request.Form["status"].FirstOrDefault()
            ?? Request.Query["status"].FirstOrDefault();

        _logger.LogInformation("SchoolPay webhook received for {Provider}: tran={TranId}, status={Status}",
            providerCode, transactionRef, eventType);

        var success = await _webhookService.ProcessWebhookAsync(providerCode, transactionRef, eventType, rawPayload, ct);
        return Ok(success ? "SUCCESS" : "FAILED");
    }

    [HttpGet("Recent")]
    public async Task<IActionResult> Recent(int count = 50, CancellationToken ct = default)
    {
        var webhooks = await _webhookService.GetRecentWebhooksAsync(count, ct);
        return Json(webhooks);
    }

    [HttpPost("Retry/{webhookId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Retry(int webhookId, CancellationToken ct)
    {
        var success = await _webhookService.RetryWebhookAsync(webhookId, ct);
        return Json(new { success });
    }
}
