using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.DTOs.Guardian;
using SchoolManagementSystem.Services.Interfaces.Guardian;
using SchoolManagementSystem.Filters;
using Microsoft.AspNetCore.Authorization;
using SchoolManagementSystem.Models.Entities.Guardian;

namespace SchoolManagementSystem.Controllers.Admin;

[Authorize]
[Route("admin/guardians")]
public class GuardianController : Controller
{
    private readonly IGuardianService _guardianService;

    public GuardianController(IGuardianService guardianService)
    {
        _guardianService = guardianService;
    }

    [Permission("Students", "View")]
    public async Task<IActionResult> Index(string? searchTerm, string? status, int page = 1, int pageSize = 10)
    {
        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
        {
            var (items, totalCount) = await _guardianService.GetGuardianListAsync(searchTerm, status, page, pageSize);
            var lastPage = (int)Math.Ceiling((double)totalCount / pageSize);
            return Json(new { data = items, last_page = lastPage });
        }
        var (allItems, total) = await _guardianService.GetGuardianListAsync(searchTerm, status, page, 10);
        ViewBag.SearchTerm = searchTerm;
        ViewBag.Status = status;
        ViewBag.TotalCount = total;
        ViewBag.CurrentPage = page;
        return View(allItems);
    }

    [HttpGet("search")]
    [Authorize]
    public async Task<IActionResult> Search(string term)
    {
        var (items, _) = await _guardianService.GetGuardianListAsync(term, null, 1, 10);
        return Json(items.Select(i => new { id = i.Id, code = i.GuardianCode, name = i.FullName, phone = i.MobileNumber, email = i.Email ?? "" }));
    }

    [HttpGet("{id}")]
    [Permission("Students", "View")]
    public async Task<IActionResult> Details(int id)
    {
        var guardian = await _guardianService.GetGuardianByIdAsync(id);
        if (guardian == null) return NotFound();
        return View(guardian);
    }

    [HttpGet("create")]
    [Permission("Students", "Create")]
    public IActionResult Create()
    {
        return View(new GuardianUpsertDto());
    }

    [HttpPost("create")]
    [Permission("Students", "Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GuardianUpsertDto dto)
    {
        if (ModelState.IsValid)
        {
            await _guardianService.CreateGuardianAsync(dto);
            return RedirectToAction(nameof(Index));
        }
        return View(dto);
    }

    [HttpGet("edit/{id}")]
    [Permission("Students", "Update")]
    public async Task<IActionResult> Edit(int id)
    {
        var guardian = await _guardianService.GetGuardianByIdAsync(id);
        if (guardian == null) return NotFound();

        var dto = new GuardianUpsertDto
        {
            Id = guardian.Id,
            FirstName = guardian.FirstName,
            LastName = guardian.LastName,
            Gender = guardian.Gender,
            RelationType = Enum.TryParse<GuardianRelationshipType>(guardian.RelationType, out var relationType)
                ? relationType
                : GuardianRelationshipType.Other,
            MobileNumber = guardian.MobileNumber,
            Email = guardian.Email,
            NationalId = guardian.NationalId,
            Occupation = guardian.Occupation,
            PresentAddress = guardian.PresentAddress,
            PermanentAddress = guardian.PermanentAddress,
            PortalAccessEnabled = guardian.PortalAccessEnabled,
            ReceiveEmailNotifications = guardian.ReceiveEmailNotifications,
            ReceiveSMSNotifications = guardian.ReceiveSMSNotifications,
            ReceiveEventNotifications = guardian.ReceiveEventNotifications,
            ReceiveFeeNotifications = guardian.ReceiveFeeNotifications,
            ReceiveExamNotifications = guardian.ReceiveExamNotifications,
            ReceiveAttendanceNotifications = guardian.ReceiveAttendanceNotifications
        };
        return View(dto);
    }

    [HttpPost("edit/{id}")]
    [Permission("Students", "Update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GuardianUpsertDto dto)
    {
        if (id != dto.Id) return BadRequest();

        if (ModelState.IsValid)
        {
            await _guardianService.UpdateGuardianAsync(dto);
            return RedirectToAction(nameof(Index));
        }
        return View(dto);
    }

    [HttpPost("activate/{id}")]
    [Permission("Students", "Update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(int id)
    {
        await _guardianService.SetGuardianStatusAsync(id, true);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("deactivate/{id}")]
    [Permission("Students", "Update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(int id)
    {
        await _guardianService.SetGuardianStatusAsync(id, false);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("delete/{id}")]
    [Permission("Students", "Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _guardianService.DeleteGuardianAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
