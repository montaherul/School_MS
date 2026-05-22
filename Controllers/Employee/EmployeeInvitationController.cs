using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Services.Interfaces.Employee;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Employee;

[Authorize]
public class EmployeeInvitationController : Controller
{
    private readonly IEmployeeInvitationService _invitationService;
    private readonly IDepartmentService _departmentService;
    private readonly IDesignationService _designationService;

    public EmployeeInvitationController(
        IEmployeeInvitationService invitationService,
        IDepartmentService departmentService,
        IDesignationService designationService)
    {
        _invitationService = invitationService;
        _departmentService = departmentService;
        _designationService = designationService;
    }

    [RequirePermission("Employees.Invite")]
    public async Task<IActionResult> Index(int page = 1, int size = 10, string? search = null, CancellationToken ct = default)
    {
        var (items, totalRecords) = await _invitationService.GetPagedInvitationsAsync(page, size, search, ct);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            var totalPages = (int)Math.Ceiling((double)totalRecords / size);
            return Json(new { data = items, last_page = totalPages, total_records = totalRecords });
        }

        return View();
    }

    [RequirePermission("Employees.Invite")]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        await PopulateLookupListsAsync(ct);
        return View(new EmployeeInvitationUpsertDto { JoiningDate = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Employees.Invite")]
    public async Task<IActionResult> Create(EmployeeInvitationUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLookupListsAsync(ct);
            return View(dto);
        }

        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            await _invitationService.CreateInvitationAsync(dto, userId, ct);
            TempData["Success"] = "Invitation sent successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            await PopulateLookupListsAsync(ct);
            return View(dto);
        }
    }

    [HttpPost]
    [RequirePermission("Employees.Invite")]
    public async Task<IActionResult> Resend(int id, CancellationToken ct)
    {
        var result = await _invitationService.ResendInvitationAsync(id, ct);
        if (result)
            return Json(new { success = true, message = "Invitation resent successfully." });

        return Json(new { success = false, message = "Failed to resend invitation." });
    }

    [HttpPost]
    [RequirePermission("Employees.Invite")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        var result = await _invitationService.CancelInvitationAsync(id, ct);
        if (result)
            return Json(new { success = true, message = "Invitation cancelled successfully." });

        return Json(new { success = false, message = "Failed to cancel invitation." });
    }

    private async Task PopulateLookupListsAsync(CancellationToken ct)
    {
        ViewBag.Departments = await _departmentService.GetAllAsync(ct);
        ViewBag.Designations = await _designationService.GetAllAsync(ct);
    }
}
