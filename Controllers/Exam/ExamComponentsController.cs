using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Services.Interfaces.Result;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Exam;

[Authorize]
public class ExamComponentsController : Controller
{
    private readonly IExamComponentService _componentService;

    public ExamComponentsController(IExamComponentService componentService)
    {
        _componentService = componentService;
    }

    [Permission("Exam", "View")]
    public async Task<IActionResult> Index()
    {
        var components = await _componentService.GetAllAsync(includeInactive: true);
        return View(components);
    }

    [Permission("Exam", "Create")]
    public IActionResult Create()
    {
        return View(new ExamComponentUpsertDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("Exam", "Create")]
    public async Task<IActionResult> Create(ExamComponentUpsertDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var createdBy = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
        await _componentService.CreateAsync(dto, createdBy);
        TempData["SuccessMessage"] = "Component created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Permission("Exam", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var component = await _componentService.GetByIdAsync(id);
        if (component == null) return NotFound();

        var dto = new ExamComponentUpsertDto
        {
            Id = component.Id,
            Name = component.Name,
            Code = component.Code,
            Description = component.Description,
            DisplayOrder = component.DisplayOrder,
            DefaultFullMarks = component.DefaultFullMarks,
            DefaultPassMarks = component.DefaultPassMarks,
            IsPractical = component.IsPractical,
            IsOptional = component.IsOptional,
            IsActive = component.IsActive
        };

        return View("Create", dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("Exam", "Edit")]
    public async Task<IActionResult> Edit(int id, ExamComponentUpsertDto dto)
    {
        if (!ModelState.IsValid)
            return View("Create", dto);

        var updatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
        var result = await _componentService.UpdateAsync(id, dto, updatedBy);
        if (result == null) return NotFound();

        TempData["SuccessMessage"] = "Component updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("Exam", "Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _componentService.DeleteAsync(id);
        if (!result) return NotFound();

        TempData["SuccessMessage"] = "Component deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("Exam", "Edit")]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var result = await _componentService.ToggleActiveAsync(id);
        if (!result) return NotFound();

        TempData["SuccessMessage"] = "Component status toggled.";
        return RedirectToAction(nameof(Index));
    }
}
