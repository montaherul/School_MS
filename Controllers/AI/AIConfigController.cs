using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.AI;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.ViewModels.AI;
using SchoolManagementSystem.Services.Interfaces.AI;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.AI;

[Authorize(Roles = "Admin,Principal")]
[Route("AI/[controller]")]
public class AIConfigController : Controller
{
    private readonly IAIAdminService _adminService;
    private readonly ILogger<AIConfigController> _logger;

    public AIConfigController(IAIAdminService adminService, ILogger<AIConfigController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

    [HttpGet("")]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var settingsResult = await _adminService.GetSettingsAsync(ct);
        var providersResult = await _adminService.GetProvidersAsync(ct);
        var modelsResult = await _adminService.GetModelsAsync(ct);
        var promptsResult = await _adminService.GetPromptsAsync(ct);
        var flagsResult = await _adminService.GetFeatureFlagsAsync(ct);
        var quotasResult = await _adminService.GetQuotasAsync(ct);
        var securityResult = await _adminService.GetSecurityPoliciesAsync(ct);

        var vm = new AISettingsIndexViewModel
        {
            Settings = settingsResult.ValueOrDefault([]),
            Providers = providersResult.ValueOrDefault([]),
            Models = modelsResult.ValueOrDefault([]),
            Prompts = promptsResult.ValueOrDefault([]),
            FeatureFlags = flagsResult.ValueOrDefault([]),
            Quotas = quotasResult.ValueOrDefault([]),
            SecurityPolicies = securityResult.ValueOrDefault([])
        };

        return View("~/Views/AI/Config/Index.cshtml", vm);
    }

    // Settings
    [HttpPost("UpsertSetting")]
    [ValidateAntiForgeryToken]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> UpsertSetting([FromBody] AISettingUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, error = "Invalid data." });

        var result = await _adminService.UpsertSettingAsync(dto, GetUserId(), ct);
        if (result.IsFailure)
            return Json(new { success = false, error = result.ErrorMessage });

        return Json(new { success = true, id = result.Data });
    }

    [HttpPost("DeleteSetting")]
    [ValidateAntiForgeryToken]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> DeleteSetting(int id, CancellationToken ct)
    {
        var result = await _adminService.DeleteSettingAsync(id, ct);
        if (result.IsFailure)
            return Json(new { success = false, error = result.ErrorMessage });

        return Json(new { success = true });
    }

    // Providers
    [HttpPost("UpsertProvider")]
    [ValidateAntiForgeryToken]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> UpsertProvider([FromBody] AIProviderUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, error = "Invalid data." });

        var result = await _adminService.UpsertProviderAsync(dto, GetUserId(), ct);
        if (result.IsFailure)
            return Json(new { success = false, error = result.ErrorMessage });

        return Json(new { success = true, id = result.Data });
    }

    [HttpPost("DeleteProvider")]
    [ValidateAntiForgeryToken]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> DeleteProvider(int id, CancellationToken ct)
    {
        var result = await _adminService.DeleteProviderAsync(id, ct);
        if (result.IsFailure)
            return Json(new { success = false, error = result.ErrorMessage });

        return Json(new { success = true });
    }

    // GET helpers for edit modal
    [HttpGet("GetSetting/{id}")]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> GetSetting(int id, CancellationToken ct)
    {
        var list = await _adminService.GetSettingsAsync(ct);
        var item = list.Data?.FirstOrDefault(x => x.Id == id);
        return Json(item);
    }

    [HttpGet("GetProvider/{id}")]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> GetProvider(int id, CancellationToken ct)
    {
        var list = await _adminService.GetProvidersAsync(ct);
        var item = list.Data?.FirstOrDefault(x => x.Id == id);
        return Json(item);
    }

    [HttpGet("GetModel/{id}")]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> GetModel(int id, CancellationToken ct)
    {
        var list = await _adminService.GetModelsAsync(ct);
        var item = list.Data?.FirstOrDefault(x => x.Id == id);
        return Json(item);
    }

    [HttpGet("GetPrompt/{id}")]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> GetPrompt(int id, CancellationToken ct)
    {
        var list = await _adminService.GetPromptsAsync(ct);
        var item = list.Data?.FirstOrDefault(x => x.Id == id);
        return Json(item);
    }

    [HttpGet("GetFeature/{id}")]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> GetFeatureFlag(int id, CancellationToken ct)
    {
        var list = await _adminService.GetFeatureFlagsAsync(ct);
        var item = list.Data?.FirstOrDefault(x => x.Id == id);
        return Json(item);
    }

    [HttpGet("GetQuota/{id}")]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> GetQuota(int id, CancellationToken ct)
    {
        var list = await _adminService.GetQuotasAsync(ct);
        var item = list.Data?.FirstOrDefault(x => x.Id == id);
        return Json(item);
    }

    [HttpGet("GetSecurity/{id}")]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> GetSecurityPolicy(int id, CancellationToken ct)
    {
        var list = await _adminService.GetSecurityPoliciesAsync(ct);
        var item = list.Data?.FirstOrDefault(x => x.Id == id);
        return Json(item);
    }

    // Toggle
    [HttpPost("ToggleProvider/{id}")]
    [ValidateAntiForgeryToken]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> ToggleProvider(int id, CancellationToken ct)
    {
        var list = await _adminService.GetProvidersAsync(ct);
        var item = list.Data?.FirstOrDefault(x => x.Id == id);
        if (item is null) return Json(new { success = false });
        await _adminService.UpsertProviderAsync(new AIProviderUpsertDto
        {
            Name = item.Name,
            ProviderType = Enum.TryParse<AIProviderType>(item.ProviderType, out var pt) ? (int)pt : 1,
            BaseUrl = item.BaseUrl,
            ApiKey = item.ApiKey,
            IsEnabled = !item.IsEnabled,
            Priority = item.Priority,
            RetryCount = item.RetryCount,
            TimeoutSeconds = item.TimeoutSeconds
        }, GetUserId(), ct);
        return Json(new { success = true });
    }

    [HttpPost("ToggleModel/{id}")]
    [ValidateAntiForgeryToken]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> ToggleModel(int id, CancellationToken ct)
    {
        var list = await _adminService.GetModelsAsync(ct);
        var item = list.Data?.FirstOrDefault(x => x.Id == id);
        if (item is null) return Json(new { success = false });
        await _adminService.UpsertModelAsync(new AIModelUpsertDto
        {
            Name = item.Name,
            ProviderId = item.ProviderId,
            Role = Enum.TryParse<AIModelRole>(item.Role, out var r) ? (int)r : 1,
            IsDefault = item.IsDefault,
            MaxTokens = item.MaxTokens,
            Temperature = item.Temperature,
            IsEnabled = !item.IsEnabled
        }, GetUserId(), ct);
        return Json(new { success = true });
    }

    [HttpPost("ToggleFeature/{id}")]
    [ValidateAntiForgeryToken]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> ToggleFeature(int id, CancellationToken ct)
    {
        var list = await _adminService.GetFeatureFlagsAsync(ct);
        var item = list.Data?.FirstOrDefault(x => x.Id == id);
        if (item is null) return Json(new { success = false });
        await _adminService.UpsertFeatureFlagAsync(new AIFeatureFlagUpsertDto
        {
            Key = item.Key,
            DisplayName = item.DisplayName,
            IsEnabled = !item.IsEnabled,
            Category = item.Category,
            Description = item.Description
        }, GetUserId(), ct);
        return Json(new { success = true });
    }

    // ── Delete endpoints ────────────────────────────────────────────────

    [HttpPost("DeleteModel/{id}")]
    [ValidateAntiForgeryToken]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> DeleteModel(int id, CancellationToken ct)
    {
        var result = await _adminService.DeleteModelAsync(id, ct);
        if (result.IsFailure)
            return Json(new { success = false, error = result.ErrorMessage });
        return Json(new { success = true });
    }

    [HttpPost("DeletePrompt/{id}")]
    [ValidateAntiForgeryToken]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> DeletePrompt(int id, CancellationToken ct)
    {
        var result = await _adminService.DeletePromptAsync(id, ct);
        if (result.IsFailure)
            return Json(new { success = false, error = result.ErrorMessage });
        return Json(new { success = true });
    }

    [HttpPost("DeleteFeature/{id}")]
    [ValidateAntiForgeryToken]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> DeleteFeatureFlag(int id, CancellationToken ct)
    {
        var result = await _adminService.DeleteFeatureFlagAsync(id, ct);
        if (result.IsFailure)
            return Json(new { success = false, error = result.ErrorMessage });
        return Json(new { success = true });
    }

    [HttpPost("DeleteQuota/{id}")]
    [ValidateAntiForgeryToken]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> DeleteQuota(int id, CancellationToken ct)
    {
        var result = await _adminService.DeleteQuotaAsync(id, ct);
        if (result.IsFailure)
            return Json(new { success = false, error = result.ErrorMessage });
        return Json(new { success = true });
    }

    [HttpPost("DeleteSecurity/{id}")]
    [ValidateAntiForgeryToken]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> DeleteSecurityPolicy(int id, CancellationToken ct)
    {
        var result = await _adminService.DeleteSecurityPolicyAsync(id, ct);
        if (result.IsFailure)
            return Json(new { success = false, error = result.ErrorMessage });
        return Json(new { success = true });
    }

    // Models
    [HttpPost("UpsertModel")]
    [ValidateAntiForgeryToken]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> UpsertModel([FromBody] AIModelUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, error = "Invalid data." });

        var result = await _adminService.UpsertModelAsync(dto, GetUserId(), ct);
        if (result.IsFailure)
            return Json(new { success = false, error = result.ErrorMessage });

        return Json(new { success = true, id = result.Data });
    }

    // Prompts
    [HttpPost("UpsertPrompt")]
    [ValidateAntiForgeryToken]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> UpsertPrompt([FromBody] AIPromptUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, error = "Invalid data." });

        var result = await _adminService.UpsertPromptAsync(dto, GetUserId(), ct);
        if (result.IsFailure)
            return Json(new { success = false, error = result.ErrorMessage });

        return Json(new { success = true, id = result.Data });
    }

    // Feature Flags
    [HttpPost("UpsertFeature")]
    [ValidateAntiForgeryToken]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> UpsertFeatureFlag([FromBody] AIFeatureFlagUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, error = "Invalid data." });

        var result = await _adminService.UpsertFeatureFlagAsync(dto, GetUserId(), ct);
        if (result.IsFailure)
            return Json(new { success = false, error = result.ErrorMessage });

        return Json(new { success = true, id = result.Data });
    }

    // Quotas
    [HttpPost("UpsertQuota")]
    [ValidateAntiForgeryToken]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> UpsertQuota([FromBody] AIQuotaUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, error = "Invalid data." });

        var result = await _adminService.UpsertQuotaAsync(dto, GetUserId(), ct);
        if (result.IsFailure)
            return Json(new { success = false, error = result.ErrorMessage });

        return Json(new { success = true, id = result.Data });
    }

    // Security Policies
    [HttpPost("UpsertSecurity")]
    [ValidateAntiForgeryToken]
    [RequirePermission("AI.Manage")]
    public async Task<IActionResult> UpsertSecurityPolicy([FromBody] AISecurityPolicyUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, error = "Invalid data." });

        var result = await _adminService.UpsertSecurityPolicyAsync(dto, GetUserId(), ct);
        if (result.IsFailure)
            return Json(new { success = false, error = result.ErrorMessage });

        return Json(new { success = true, id = result.Data });
    }
}
