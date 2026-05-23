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

    public OnboardingController(
        IEmployeeInvitationService invitationService,
        IDepartmentService departmentService,
        IDesignationService designationService)
    {
        _invitationService = invitationService;
        _departmentService = departmentService;
        _designationService = designationService;
    }

    public async Task<IActionResult> Welcome(string token, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

        var invitation = await _invitationService.GetInvitationByTokenAsync(token, ct);
        if (invitation == null || invitation.IsUsed || invitation.ExpiresAt < DateTime.UtcNow)
        {
            TempData["Error"] = "This invitation link is invalid or has expired.";
            return RedirectToAction("Login", "Auth");
        }

        return RedirectToAction(nameof(Start), new { token });
    }

    public async Task<IActionResult> Start(string token, CancellationToken ct)
    {
        var invitation = await _invitationService.GetInvitationByTokenAsync(token, ct);
        if (invitation == null || invitation.IsUsed || invitation.ExpiresAt < DateTime.UtcNow)
        {
            return RedirectToAction(nameof(Welcome), new { token });
        }

        ViewBag.Departments = await _departmentService.GetAllAsync(ct);
        ViewBag.Designations = await _designationService.GetAllAsync(ct);
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(EmployeeUpsertDto model, string token, string password, string confirmPassword, CancellationToken ct)
    {
        if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
        {
            ModelState.AddModelError(string.Empty, "Password and confirmation password do not match.");
        }

        if (!ModelState.IsValid)
        {
            ViewData["InvitationToken"] = token;
            ViewData["IsOnboarding"] = true;
            return View("~/Views/EmployeeInvitation/Onboarding.cshtml", model);
        }

        var (success, message) = await _invitationService.CompleteOnboardingAsync(model, token, password, ct);
        if (success)
        {
            return View("Success");
        }

        ModelState.AddModelError("", message);
        ViewData["InvitationToken"] = token;
        ViewData["IsOnboarding"] = true;
        return View("~/Views/EmployeeInvitation/Onboarding.cshtml", model);
    }
}
