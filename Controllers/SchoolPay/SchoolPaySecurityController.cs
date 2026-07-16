using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Controllers.SchoolPay;

[Authorize]
[Route("SchoolPay/Security")]
public class SchoolPaySecurityController : Controller
{
    private readonly ISecurityAuditService _auditService;
    private readonly IMerchantSecretService _secretService;
    private readonly IProviderManagementService _providerService;

    public SchoolPaySecurityController(
        ISecurityAuditService auditService,
        IMerchantSecretService secretService,
        IProviderManagementService providerService)
    {
        _auditService = auditService;
        _secretService = secretService;
        _providerService = providerService;
    }

    [RequirePermission("SchoolPay.Manage")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var providers = await _providerService.GetAllProvidersAsync(ct);
        ViewBag.Providers = providers;
        var auditLog = await _auditService.GetAuditLogAsync(null, 7, ct);
        return View(auditLog);
    }

    [RequirePermission("SchoolPay.Manage")]
    public async Task<IActionResult> Secrets(int providerId, CancellationToken ct)
    {
        var provider = await _providerService.GetProviderByIdAsync(providerId, ct);
        if (provider == null) return NotFound();
        ViewBag.Provider = provider;
        var secrets = await _secretService.GetSecretsAsync(providerId, ct);
        return View(secrets);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("SchoolPay.Manage")]
    public async Task<IActionResult> RotateSecret(int providerId, string keyName, string newValue, CancellationToken ct)
    {
        var user = User.Identity?.Name ?? "System";
        await _secretService.RotateSecretAsync(providerId, keyName, newValue, user, ct);
        TempData["Success"] = $"Secret '{keyName}' rotated successfully";
        return RedirectToAction(nameof(Secrets), new { providerId });
    }

    [RequirePermission("SchoolPay.Manage")]
    public async Task<IActionResult> AuditLog(int? providerId, int days = 30, CancellationToken ct = default)
    {
        ViewBag.Providers = await _providerService.GetAllProvidersAsync(ct);
        ViewBag.Days = days;
        ViewBag.SelectedProviderId = providerId;
        var log = await _auditService.GetAuditLogAsync(providerId, days, ct);
        return View(log);
    }
}
