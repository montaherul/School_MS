using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.ViewModels.Auth;
using SchoolManagementSystem.Repositories.Interfaces.Website;
using SchoolManagementSystem.Services.Interfaces.Auth;

namespace SchoolManagementSystem.Controllers.Guardian;

[AllowAnonymous]
[Route("Guardian")]
public class GuardianActivationController : Controller
{
    private readonly IAuthService _authService;
    private readonly ISchoolSettingRepository _settingRepo;
    private readonly ILogger<GuardianActivationController> _logger;

    public GuardianActivationController(IAuthService authService, ISchoolSettingRepository settingRepo, ILogger<GuardianActivationController> logger)
    {
        _authService = authService;
        _settingRepo = settingRepo;
        _logger = logger;
    }

    private async Task<bool> IsGuardianPortalEnabledAsync()
    {
        var settings = await _settingRepo.GetCurrentSettingsAsync();
        return settings?.EnableGuardianPortal == true;
    }

    [HttpGet("Activate")]
    public async Task<IActionResult> Activate(string? token, CancellationToken ct)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return NotFound();
        if (string.IsNullOrWhiteSpace(token)) return NotFound();
        if (!await _authService.IsActivationTokenValidAsync(token, ct))
        {
            ViewBag.ErrorMessage = "This activation link is invalid or has expired.";
            return View("Expired");
        }
        return View(new SetPasswordViewModel { Token = token });
    }

    [HttpPost("Activate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(SetPasswordViewModel model, CancellationToken ct)
    {
        if (!await IsGuardianPortalEnabledAsync())
            return NotFound();
        if (!ModelState.IsValid) return View(model);
        var (success, message) = await _authService.ActivateAccountAsync(model, ct);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, message);
            return View(model);
        }
        TempData["SuccessMessage"] = "Your guardian account has been activated. Please log in.";
        return RedirectToAction("Login", "Auth");
    }
}
