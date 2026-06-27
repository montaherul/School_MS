using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Services.Interfaces.Employee;

namespace SchoolManagementSystem.Controllers.Employee;

[AllowAnonymous]
public class OnboardingController : Controller
{
    private readonly IEmployeeInvitationService _invitationService;
    private readonly IDepartmentService _departmentService;
    private readonly IDesignationService _designationService;
    private readonly ILogger<OnboardingController> _logger;

    public OnboardingController(
        IEmployeeInvitationService invitationService,
        IDepartmentService departmentService,
        IDesignationService designationService,
        ILogger<OnboardingController> logger)
    {
        _invitationService = invitationService;
        _departmentService = departmentService;
        _designationService = designationService;
        _logger = logger;
    }

    private async Task LoadViewBagAsync(CancellationToken ct)
    {
        ViewBag.Departments = await _departmentService.GetAllAsync(ct);
        ViewBag.Designations = await _designationService.GetAllAsync(ct);
    }

    // GET: /Onboarding/Welcome?token=xxx
    public async Task<IActionResult> Welcome(string token, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        var invitation = await _invitationService.GetInvitationByTokenAsync(token, ct);
        if (invitation == null)
            return View("~/Views/EmployeeInvitation/InvalidToken.cshtml");

        if (invitation.IsUsed)
            return View("~/Views/EmployeeInvitation/AlreadyUsed.cshtml");

        if (invitation.ExpiresAt < DateTime.UtcNow)
            return View("~/Views/EmployeeInvitation/Expired.cshtml");

        return RedirectToAction(nameof(Start), new { token });
    }

    // GET: /Onboarding/Start?token=xxx
    public async Task<IActionResult> Start(string token, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        var invitation = await _invitationService.GetInvitationByTokenAsync(token, ct);
        if (invitation == null)
            return View("~/Views/EmployeeInvitation/InvalidToken.cshtml");

        if (invitation.IsUsed)
            return View("~/Views/EmployeeInvitation/AlreadyUsed.cshtml");

        if (invitation.ExpiresAt < DateTime.UtcNow)
            return View("~/Views/EmployeeInvitation/Expired.cshtml");

        await LoadViewBagAsync(ct);
        ViewData["InvitationToken"] = token;
        ViewData["IsOnboarding"] = true;

        await _invitationService.MarkInvitationOpenedAsync(token, ct);

        var model = new EmployeeUpsertDto
        {
            FullName = invitation.FullName,
            Email = invitation.Email,
            Phone = invitation.Mobile,
            DepartmentId = invitation.DepartmentId,
            DesignationId = invitation.DesignationId,
            JoiningDate = invitation.JoiningDate,
            EmployeeType = invitation.EmploymentType,
            Status = invitation.Status,
            IsTeachingStaff = invitation.IsTeachingStaff
        };

        return View("~/Views/EmployeeInvitation/Onboarding.cshtml", model);
    }

    // POST: /Onboarding/Submit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(
        EmployeeUpsertDto model,
        string token,
        string password,
        string confirmPassword,
        CancellationToken ct)
    {
        _logger.LogInformation("Onboarding Submit started for token={Token}", token);

        if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
        {
            ModelState.AddModelError(string.Empty, "Password and confirmation password do not match.");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Onboarding Submit ModelState invalid for token={Token}. Errors: {Errors}",
                token, string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)));

            await LoadViewBagAsync(ct);
            ViewData["InvitationToken"] = token;
            ViewData["IsOnboarding"] = true;
            return View("~/Views/EmployeeInvitation/Onboarding.cshtml", model);
        }

        try
        {
            var (success, message) = await _invitationService.CompleteOnboardingAsync(model, token, password, ct);
            if (success)
            {
                _logger.LogInformation("Onboarding Submit succeeded for token={Token}", token);
                return View("~/Views/Onboarding/Success.cshtml");
            }

            _logger.LogWarning("Onboarding Submit service returned failure for token={Token}: {Message}", token, message);
            ModelState.AddModelError("", message);
            await LoadViewBagAsync(ct);
            ViewData["InvitationToken"] = token;
            ViewData["IsOnboarding"] = true;
            return View("~/Views/EmployeeInvitation/Onboarding.cshtml", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Onboarding Submit unexpected error for token={Token}", token);
            ModelState.AddModelError("", "An unexpected error occurred. Please try again or contact support.");
            await LoadViewBagAsync(ct);
            ViewData["InvitationToken"] = token;
            ViewData["IsOnboarding"] = true;
            return View("~/Views/EmployeeInvitation/Onboarding.cshtml", model);
        }
    }
}
