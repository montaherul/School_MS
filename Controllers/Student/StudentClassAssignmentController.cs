using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Assignment;
using SchoolManagementSystem.Services.Interfaces.Assignment;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Student;

using SchoolManagementSystem.Filters;

[RequirePermission("Assignment.View")]
public class StudentClassAssignmentController : Controller
{
    private readonly IAssignmentService _service;

    public StudentClassAssignmentController(IAssignmentService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, CancellationToken ct = default)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return Forbid();

        var query = _service.Query();
        query = await _service.ApplySecurityFiltersAsync(query, userId, User.IsInRole("Student"), 
            User.IsInRole("Teacher") || User.IsInRole("Senior Lecturer") || User.IsInRole("Lecturer"),
            User.IsInRole("Super Admin") || User.IsInRole("Principal") || User.IsInRole("Assistant Head"), ct);

        // Basic pagination for now, or I could use a PagedResult pattern
        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        ViewBag.TotalItems = total;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;

        return View(items);
    }

    [HttpGet]
    public IActionResult Create() => View("CreateEdit", new AssignmentTask());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AssignmentTask model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View("CreateEdit", model);
        await _service.CreateAsync(model, User.Identity?.Name ?? "System", ct);
        TempData["SuccessMessage"] = "Assignment created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // Add Edit, Details, Delete as needed...
}

