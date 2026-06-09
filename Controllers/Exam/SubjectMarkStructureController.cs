using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.ViewModels.Exam;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Exam;

[Authorize]
public class SubjectMarkStructureController : Controller
{
    private readonly ISubjectMarkStructureService _service;
    private readonly IUnitOfWork _uow;

    public SubjectMarkStructureController(ISubjectMarkStructureService service, IUnitOfWork uow)
    {
        _service = service;
        _uow = uow;
    }

    [Permission("Exam", "View")]
    public async Task<IActionResult> Index(int? subjectId)
    {
        ViewBag.Subjects = await _uow.Repository<Subject>().Query()
            .Where(s => !s.IsDeleted)
            .OrderBy(s => s.Name)
            .ToListAsync();

        if (subjectId.HasValue && subjectId > 0)
        {
            var structures = await _service.GetBySubjectAsync(subjectId.Value);
            return View(structures);
        }

        return View(new List<SubjectMarkStructureDto>());
    }

    [Permission("Exam", "View")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        if (dto == null) return NotFound();
        return View(dto);
    }

    [Permission("Exam", "Create")]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View(new SubjectMarkStructureUpsertDto { IsActive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("Exam", "Create")]
    public async Task<IActionResult> Create(SubjectMarkStructureUpsertDto dto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync();
            return View(dto);
        }

        try
        {
            var createdBy = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
            await _service.CreateAsync(dto, createdBy);
            TempData["SuccessMessage"] = "Subject mark structure created.";
            return RedirectToAction(nameof(Index), new { subjectId = dto.SubjectId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            await PopulateDropdownsAsync();
            return View(dto);
        }
    }

    [Permission("Exam", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        if (dto == null) return NotFound();

        var upsertDto = new SubjectMarkStructureUpsertDto
        {
            Id = dto.Id,
            ComponentId = dto.ComponentId,
            ClassId = dto.ClassId,
            SubjectId = dto.SubjectId,
            StudentGroupId = dto.StudentGroupId,
            FullMarks = dto.FullMarks,
            PassMarks = dto.PassMarks,
            DisplayOrder = dto.DisplayOrder,
            IsActive = dto.IsActive
        };

        await PopulateDropdownsAsync(upsertDto.SubjectId, upsertDto.ComponentId);
        return View(upsertDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("Exam", "Edit")]
    public async Task<IActionResult> Edit(int id, SubjectMarkStructureUpsertDto dto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync();
            return View(dto);
        }

        try
        {
            var updatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
            var result = await _service.UpdateAsync(id, dto, updatedBy);
            if (result == null) return NotFound();

            TempData["SuccessMessage"] = "Subject mark structure updated.";
            return RedirectToAction(nameof(Index), new { subjectId = dto.SubjectId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            await PopulateDropdownsAsync();
            return View(dto);
        }
    }

    [Permission("Exam", "Edit")]
    public async Task<IActionResult> BulkEdit(int subjectId)
    {
        var existing = await _service.GetBySubjectAsync(subjectId);
        var allComponents = await _uow.Repository<ExamComponent>().Query()
            .Where(c => !c.IsDeleted && c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        var model = new SubjectMarkStructureBulkViewModel
        {
            SubjectId = subjectId,
            SubjectName = (await _uow.Repository<Subject>().GetByIdAsync(subjectId))?.Name ?? "",
            Components = allComponents.Select(c => new SubjectMarkStructureItemViewModel
            {
                ComponentId = c.Id,
                ComponentName = c.Name,
                ComponentCode = c.Code,
                FullMarks = existing.FirstOrDefault(e => e.ComponentId == c.Id)?.FullMarks ?? c.DefaultFullMarks,
                PassMarks = existing.FirstOrDefault(e => e.ComponentId == c.Id)?.PassMarks ?? c.DefaultPassMarks,
                DisplayOrder = existing.FirstOrDefault(e => e.ComponentId == c.Id)?.DisplayOrder ?? c.DisplayOrder,
                IsEnabled = existing.Any(e => e.ComponentId == c.Id)
            }).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("Exam", "Edit")]
    public async Task<IActionResult> BulkEdit(int subjectId, SubjectMarkStructureBulkViewModel model)
    {
        var updatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
        var items = model.Components
            .Where(c => c.IsEnabled && c.FullMarks > 0)
            .Select((c, i) => new SubjectMarkStructureUpsertDto
            {
                ComponentId = c.ComponentId,
                SubjectId = subjectId,
                FullMarks = c.FullMarks,
                PassMarks = c.PassMarks,
                DisplayOrder = i + 1,
                IsActive = true
            }).ToList();

        if (items.Count == 0)
        {
            ModelState.AddModelError("", "At least one component must be enabled.");
            var allComponents = await _uow.Repository<ExamComponent>().Query()
                .Where(c => !c.IsDeleted && c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
            model.Components = allComponents.Select(c => new SubjectMarkStructureItemViewModel
            {
                ComponentId = c.Id,
                ComponentName = c.Name,
                ComponentCode = c.Code,
                FullMarks = c.DefaultFullMarks,
                PassMarks = c.DefaultPassMarks,
                DisplayOrder = c.DisplayOrder,
                IsEnabled = items.Any(i => i.ComponentId == c.Id)
            }).ToList();
            return View(model);
        }

        await _service.SaveBulkAsync(subjectId, items, updatedBy);
        TempData["SuccessMessage"] = "Subject mark structure saved.";
        return RedirectToAction(nameof(Index), new { subjectId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission("Exam", "Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result) return NotFound();

        TempData["SuccessMessage"] = "Subject mark structure deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync(int? selectedSubjectId = null, int? selectedComponentId = null)
    {
        ViewBag.Subjects = await _uow.Repository<Subject>().Query()
            .Where(s => !s.IsDeleted)
            .OrderBy(s => s.Name)
            .ToListAsync();
        ViewBag.ExamComponents = await _uow.Repository<ExamComponent>().Query()
            .Where(c => !c.IsDeleted && c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
        ViewBag.Classes = await _uow.Repository<SchoolClass>().Query()
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.SortOrder)
            .ToListAsync();
        ViewBag.StudentGroups = await _uow.Repository<StudentGroup>().Query()
            .Where(g => !g.IsDeleted && g.IsActive)
            .OrderBy(g => g.DisplayOrder)
            .ToListAsync();
    }
}
