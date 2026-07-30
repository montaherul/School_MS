using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Services.Interfaces.Website;

namespace SchoolManagementSystem.Controllers.Admin;

[Authorize]
[Route("Admin/PortalSettings")]
public class PortalSettingsController : Controller
{
    private readonly ISchoolWebsiteService _settingsService;

    public PortalSettingsController(ISchoolWebsiteService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet("")]
    [RequirePermission("Settings.Manage")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var settings = await _settingsService.GetSettingsAsync(ct);
        return View("~/Views/Admin/PortalSettings/Index.cshtml", settings);
    }

    [HttpPost("")]
    [RequirePermission("Settings.Manage")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SchoolSetting model, CancellationToken ct)
    {
        var existing = await _settingsService.GetSettingsAsync(ct);
        existing.EnableStudentPortal = model.EnableStudentPortal;
        existing.EnableGuardianPortal = model.EnableGuardianPortal;
        existing.EnableGuardianActivation = model.EnableGuardianActivation;
        existing.RequireGuardianForAdmission = model.RequireGuardianForAdmission;
        existing.EnableGuardianNotifications = model.EnableGuardianNotifications;
        existing.EnableStudentNotifications = model.EnableStudentNotifications;

        await _settingsService.UpdateSettingsAsync(existing, ct);
        TempData["SuccessMessage"] = "Portal settings updated successfully.";
        return RedirectToAction(nameof(Index));
    }
}
