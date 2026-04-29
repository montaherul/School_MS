using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Services.Interfaces.Students;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers;

[Authorize]
public class StudentController : Controller
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [RequirePermission("Student.View")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [RequirePermission("Student.Create")]
    public IActionResult Create()
    {
        return RedirectToAction(nameof(CreateEdit));
    }

    [HttpGet]
    [RequirePermission("Student.Edit")]
    public IActionResult Edit(int id)
    {
        return RedirectToAction(nameof(CreateEdit), new { id });
    }

    [HttpGet]
    [RequirePermission("Student.View")]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var dto = await _studentService.GetForEditAsync(id, cancellationToken);
        return dto is null ? NotFound() : View(dto);
    }

    [HttpGet]
    [RequirePermission("Student.View")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null, CancellationToken cancellationToken = default)
    {
        var result = await _studentService.GetPagedAsync(page, size, search, cancellationToken);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id, CancellationToken cancellationToken)
    {
        if (id.HasValue && id > 0)
        {
            if (!User.HasClaim("Permission", "Student.Edit") && !User.IsInRole("Super Admin")) return Forbid();
            var dto = await _studentService.GetForEditAsync(id.Value, cancellationToken);
            if (dto == null) return NotFound();
            return View(dto);
        }

        if (!User.HasClaim("Permission", "Student.Create") && !User.IsInRole("Super Admin")) return Forbid();
        return View(new StudentUpsertDto { DateOfBirth = DateTime.Today.AddYears(-10) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(StudentUpsertDto model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        if (model.Id == 0)
        {
            if (!User.HasClaim("Permission", "Student.Create") && !User.IsInRole("Super Admin")) return Forbid();
            await _studentService.CreateAsync(model, userId, cancellationToken);
            TempData["SuccessMessage"] = "Student created successfully.";
        }
        else
        {
            if (!User.HasClaim("Permission", "Student.Edit") && !User.IsInRole("Super Admin")) return Forbid();
            await _studentService.UpdateAsync(model, userId, cancellationToken);
            TempData["SuccessMessage"] = "Student updated successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(StudentUpsertDto model, CancellationToken cancellationToken)
    {
        return CreateEdit(model, cancellationToken);
    }

    [HttpGet]
    [RequirePermission("Student.Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var dto = await _studentService.GetForEditAsync(id, cancellationToken);
        return dto is null ? NotFound() : View("Delete", dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Student.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _studentService.DeleteAsync(id, userId, cancellationToken);
        TempData["SuccessMessage"] = "Student deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
