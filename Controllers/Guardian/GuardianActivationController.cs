using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.ViewModels.Auth;
using SchoolManagementSystem.Services.Interfaces.Auth;

namespace SchoolManagementSystem.Controllers.Guardian;

[AllowAnonymous]
[Route("Guardian")]
public class GuardianActivationController : Controller
{
    private readonly IAuthService _authService;
    private readonly ILogger<GuardianActivationController> _logger;

    public GuardianActivationController(IAuthService authService, ILogger<GuardianActivationController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpGet("Activate")]
    public async Task<IActionResult> Activate(string? token, CancellationToken ct)
    {
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
