using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Employee;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
public class ClassRoutineController : Controller
{
    private readonly IClassRoutineService _routineService;
    private readonly IEmployeeService _employeeService;

    public ClassRoutineController(IClassRoutineService routineService, IEmployeeService employeeService)
    {
        _routineService = routineService;
        _employeeService = employeeService;
    }

    [RequirePermission("Academic.View")]
    public async Task<IActionResult> Index(int sectionId)
    {
        var routine = await _routineService.GetBySectionAsync(sectionId);
        ViewBag.SectionId = sectionId;
        return View(routine);
    }

    [RequirePermission("Academic.View")]
    public async Task<IActionResult> TeacherTimetable()
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var employeeId = await _employeeService.GetEmployeeIdByUserIdAsync(userId);
        if (!employeeId.HasValue) return NotFound();

        var routine = await _routineService.GetByTeacherAsync(employeeId.Value);
        return View(routine);
    }

    [HttpPost]
    [RequirePermission("Academic.Update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClassRoutineDto model)
    {
        if (ModelState.IsValid)
        {
            var success = await _routineService.AddRoutineAsync(model, User.Identity!.Name!);
            if (success)
            {
                TempData["SuccessMessage"] = "Routine added successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Conflict detected. Could not add routine.";
            }
        }
        return RedirectToAction(nameof(Index), new { sectionId = model.SectionId });
    }

    [HttpPost]
    [RequirePermission("Academic.Update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int sectionId)
    {
        await _routineService.DeleteRoutineAsync(id);
        return RedirectToAction(nameof(Index), new { sectionId });
    }
}
