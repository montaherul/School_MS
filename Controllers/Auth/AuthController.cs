using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SchoolManagementSystem.Models.ViewModels.Auth;
using SchoolManagementSystem.Services.Interfaces.Auth;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Auth;

public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Dashboard");
        }
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("Login")]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);

        var (success, message, identity) = await _authService.LoginAsync(model, ct);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, message ?? "Invalid login attempt.");
            return View(model);
        }

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity!), new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(model.RememberMe ? 12 : 2)
        });

        var sessionId = HttpContext.Session.Id;
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers.UserAgent.ToString();
        var userIdClaim = identity!.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out var userId))
        {
            await _authService.RecordLoginSessionAsync(userId, sessionId, ipAddress, userAgent, ct);
        }

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)) return Redirect(model.ReturnUrl);
        return RedirectToAction("Index", "Dashboard");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var sessionId = HttpContext.Session.Id;
        await _authService.RecordLogoutSessionAsync(sessionId, HttpContext.RequestAborted);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);
        var (success, message) = await _authService.ForgotPasswordAsync(model, ct);
        TempData["SuccessMessage"] = message;
        return RedirectToAction(nameof(ResetPassword), new { userNameOrEmail = model.UserNameOrEmail });
    }

    [AllowAnonymous]
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpViewModel model, CancellationToken ct)
    {
        var (success, message) = await _authService.VerifyOtpAsync(model, ct);
        return Json(new { success, message });
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ResetPassword(string? userNameOrEmail = null) => View(new ResetPasswordViewModel { UserNameOrEmail = userNameOrEmail ?? string.Empty });

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);
        var (success, message) = await _authService.ResetPasswordAsync(model, ct);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, message);
            return View(model);
        }
        TempData["SuccessMessage"] = message + " Please login.";
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Activate(string? token, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(token) || !await _authService.IsActivationTokenValidAsync(token, ct)) return NotFound();
        return View(new SetPasswordViewModel { Token = token });
    }

    [AllowAnonymous]
    [HttpPost]
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
        TempData["SuccessMessage"] = message;
        return RedirectToAction(nameof(Login));
    }
}

