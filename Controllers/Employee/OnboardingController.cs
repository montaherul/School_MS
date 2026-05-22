using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.ViewModels.Employee;
using SchoolManagementSystem.Services.Interfaces.Employee;

namespace SchoolManagementSystem.Controllers.Employee;

[AllowAnonymous]
public class OnboardingController : Controller
{
    private readonly IEmployeeInvitationService _invitationService;

    public OnboardingController(IEmployeeInvitationService invitationService)
    {
        _invitationService = invitationService;
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

        return View(invitation);
    }

    public async Task<IActionResult> Start(string token, CancellationToken ct)
    {
        var invitation = await _invitationService.GetInvitationByTokenAsync(token, ct);
        if (invitation == null || invitation.IsUsed || invitation.ExpiresAt < DateTime.UtcNow)
        {
            return RedirectToAction(nameof(Welcome), new { token });
        }

        var model = new EmployeeOnboardingViewModel
        {
            Token = token,
            FullName = invitation.FullName,
            PersonalEmail = invitation.Email,
            MobileNumber = invitation.Mobile,
            DepartmentName = invitation.DepartmentName,
            DesignationName = invitation.DesignationName,
            JoiningDate = invitation.JoiningDate,
            EmploymentType = invitation.EmploymentType,
            IsTeachingStaff = invitation.IsTeachingStaff
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(EmployeeOnboardingViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View("Start", model);
        }

        var (success, message) = await _invitationService.CompleteOnboardingAsync(model, ct);
        if (success)
        {
            return View("Success");
        }

        ModelState.AddModelError("", message);
        return View("Start", model);
    }
}
