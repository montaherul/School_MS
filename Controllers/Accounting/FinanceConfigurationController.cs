using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Services.Interfaces.Accounting;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Accounting;

[Authorize]
[RequirePermission("Accounting.View")]
public class FinanceConfigurationController : Controller
{
    private const string ViewPath = "~/Views/Accounting/FinanceConfiguration";
    private readonly IFinanceConfigurationService _service;

    public FinanceConfigurationController(IFinanceConfigurationService service)
    {
        _service = service;
    }

    public IActionResult Index() => View($"{ViewPath}/Index.cshtml");

    [HttpGet]
    public async Task<IActionResult> GetSettings(string? category = null, CancellationToken ct = default)
    {
        var settings = await _service.GetAllSettingsAsync(category, ct);
        return Json(settings);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveSetting([FromBody] FinanceSettingUpsertDto dto, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.SetSettingAsync(dto.Key, dto.Value, dto.Description, dto.Category, userId, ct);
        return Json(new { success = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSetting(int id, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteSettingAsync(id, userId, ct);
        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> GetAccountMappings(CancellationToken ct = default)
    {
        var mappings = await _service.GetAllMappingsAsync(ct);
        return Json(mappings);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveMapping([FromBody] AccountMappingUpsertDto dto, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.SaveMappingAsync(dto, userId, ct);
        return Json(new { success = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMapping(int id, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteMappingAsync(id, userId, ct);
        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> GetFiscalSettings(CancellationToken ct = default)
    {
        var settings = await _service.GetFiscalSettingsAsync(ct);
        return Json(settings);
    }
}
