using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Helpers.Email;
using SchoolManagementSystem.Helpers.Security;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.ViewModels.Auth;

namespace SchoolManagementSystem.Controllers;

public class AuthController : Controller
{
    private readonly SchoolDbContext _db;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IEmailSender _emailSender;

    public AuthController(SchoolDbContext db, IPasswordHashService passwordHashService, IEmailSender emailSender)
    {
        _db = db;
        _passwordHashService = passwordHashService;
        _emailSender = emailSender;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _db.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x =>
                x.Status == AccountStatus.Active &&
                x.IsEmailConfirmed &&
                !x.IsDeleted &&
                (x.UserName == model.UserNameOrEmail || x.Email == model.UserNameOrEmail), cancellationToken);

        if (user is null || !_passwordHashService.VerifyPassword(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        if (user.PasswordHash == "ChangeThisHash")
        {
            user.PasswordHash = _passwordHashService.HashPassword(model.Password);
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email)
        };
        claims.AddRange(user.UserRoles.Where(x => x.Role is not null).Select(x => new Claim(ClaimTypes.Role, x.Role!.Name)));
        var roleIds = user.UserRoles.Select(x => x.RoleId).ToArray();
        var permissionCodes = await _db.RolePermissions
            .Where(x => roleIds.Contains(x.RoleId) && x.Permission != null)
            .Select(x => x.Permission!.Code)
            .Distinct()
            .ToListAsync(cancellationToken);
        claims.AddRange(permissionCodes.Select(code => new Claim("Permission", code)));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(model.RememberMe ? 12 : 2)
            });

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Dashboard");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Avoid user enumeration: always show the same confirmation UI.
        var user = await _db.Users.FirstOrDefaultAsync(
            u => u.Status == AccountStatus.Active &&
                 u.IsEmailConfirmed &&
                 !u.IsDeleted &&
                 (u.UserName == model.UserNameOrEmail || u.Email == model.UserNameOrEmail),
            cancellationToken);

        const string successMsg = "If the account exists, a password reset OTP has been sent to the registered email.";
        
        if (user is null)
        {
            TempData["SuccessMessage"] = successMsg;
            return RedirectToAction(nameof(ResetPassword), new { userNameOrEmail = model.UserNameOrEmail });
        }

        // Invalidate previous unused OTPs for this user.
        var previousTokens = await _db.PasswordResetTokens
            .Where(t => t.UserId == user.Id && !t.Used)
            .ToListAsync(cancellationToken);
        foreach (var t in previousTokens)
        {
            t.Used = true;
        }

        var otp = GenerateOtp();
        _db.PasswordResetTokens.Add(new PasswordResetToken
        {
            UserId = user.Id,
            Otp = otp,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            Used = false,
            CreatedBy = user.UserName
        });

        await _db.SaveChangesAsync(cancellationToken);

        var htmlBody = $@"
<div style='font-family: sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee; border-radius: 10px;'>
    <h2 style='color: #1a56db;'>Password Reset</h2>
    <p>Hello,</p>
    <p>You requested a password reset for your <b>SchoolMS</b> account.</p>
    <div style='background: #f0f7ff; padding: 15px; text-align: center; border-radius: 8px; margin: 20px 0;'>
        <span style='font-size: 24px; font-weight: bold; letter-spacing: 5px; color: #1a56db;'>{otp}</span>
    </div>
    <p>This code will expire in 10 minutes.</p>
    <p>If you did not request this, please ignore this email.</p>
    <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
    <p style='font-size: 12px; color: #666;'>School Management System</p>
</div>";

        await _emailSender.SendAsync(
            to: user.Email,
            subject: "Password Reset Code",
            htmlBody: htmlBody,
            cancellationToken: cancellationToken);

        TempData["SuccessMessage"] = successMsg;
        return RedirectToAction(nameof(ResetPassword), new { userNameOrEmail = model.UserNameOrEmail });
    }

    [AllowAnonymous]
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpViewModel model, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(model.UserNameOrEmail) || string.IsNullOrWhiteSpace(model.Otp))
        {
            return Json(new { success = false, message = "Username/Email and OTP are required." });
        }

        var user = await _db.Users.FirstOrDefaultAsync(
            u => u.Status == AccountStatus.Active &&
                 u.IsEmailConfirmed &&
                 !u.IsDeleted &&
                 (u.UserName == model.UserNameOrEmail || u.Email == model.UserNameOrEmail),
            cancellationToken);

        if (user is null)
        {
            return Json(new { success = false, message = "Invalid account or OTP." });
        }

        var token = await _db.PasswordResetTokens
            .Where(t => t.UserId == user.Id && !t.Used && t.ExpiresAt > DateTime.UtcNow && t.Otp == model.Otp)
            .OrderByDescending(t => t.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (token is null)
        {
            return Json(new { success = false, message = "Invalid or expired OTP." });
        }

        return Json(new { success = true, message = "OTP verified successfully." });
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ResetPassword(string? userNameOrEmail = null)
    {
        return View(new ResetPasswordViewModel { UserNameOrEmail = userNameOrEmail ?? string.Empty });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _db.Users.FirstOrDefaultAsync(
            u => u.Status == AccountStatus.Active &&
                 u.IsEmailConfirmed &&
                 !u.IsDeleted &&
                 (u.UserName == model.UserNameOrEmail || u.Email == model.UserNameOrEmail),
            cancellationToken);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid OTP or account.");
            return View(model);
        }

        var token = await _db.PasswordResetTokens
            .Where(t => t.UserId == user.Id && !t.Used && t.ExpiresAt > DateTime.UtcNow && t.Otp == model.Otp)
            .OrderByDescending(t => t.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (token is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid or expired OTP.");
            return View(model);
        }

        user.PasswordHash = _passwordHashService.HashPassword(model.NewPassword);
        token.Used = true;
        token.UpdatedAt = DateTime.UtcNow;
        token.UpdatedBy = "system";

        await _db.SaveChangesAsync(cancellationToken);

        TempData["SuccessMessage"] = "Password reset successful. Please login with your new password.";
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Activate(string? token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return NotFound();
        }

        var user = await _db.Users.FirstOrDefaultAsync(
            u => !u.IsDeleted &&
                 u.IsEmailConfirmed == false &&
                 u.ActivationToken == token &&
                 u.ActivationTokenExpiry != null &&
                 u.ActivationTokenExpiry > DateTime.UtcNow,
            cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        return View(new SetPasswordViewModel { Token = token });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(SetPasswordViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.Token))
        {
            return NotFound();
        }

        var user = await _db.Users.FirstOrDefaultAsync(
            u => !u.IsDeleted &&
                 u.IsEmailConfirmed == false &&
                 u.ActivationToken == model.Token &&
                 u.ActivationTokenExpiry != null &&
                 u.ActivationTokenExpiry > DateTime.UtcNow,
            cancellationToken);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid or expired activation token.");
            return View(model);
        }

        // One-time use: clear token after accepting.
        user.PasswordHash = _passwordHashService.HashPassword(model.Password);
        user.IsEmailConfirmed = true;
        user.Status = AccountStatus.Active;
        user.ActivationToken = null;
        user.ActivationTokenExpiry = null;
        user.UpdatedBy = user.UserName;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        TempData["SuccessMessage"] = "Account activated successfully. Please login.";
        return RedirectToAction(nameof(Login));
    }

    private static string GenerateOtp()
    {
        // 6-digit numeric OTP.
        var otp = RandomNumberGenerator.GetInt32(0, 1_000_000);
        return otp.ToString("D6");
    }
}
