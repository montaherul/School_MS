using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.ViewModels.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using System.Security.Claims;
using System.Data;
using Microsoft.Data.SqlClient;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
public class SectionController : GenericCrudController<Section>
{
    private readonly ISectionService _service;
    private readonly SchoolDbContext _db;

    public SectionController(ISectionService service, SchoolDbContext db) : base(db, "Section")
    {
        _service = service;
        _db = db;
    }

    public override async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);

        if (Request.Headers["Accept"].ToString().Contains("application/json") || Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Query.ContainsKey("page"))
        {
            // Load from EF with hierarchy info (no stored proc change needed)
            var allSections = await _db.Sections
                .Where(s => !s.IsDeleted)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.SchoolClassId,
                    ClassName = _db.Classes.Where(c => c.Id == s.SchoolClassId).Select(c => c.Name).FirstOrDefault() ?? "",
                    s.ParentSectionId,
                    GroupName = s.ParentSectionId != null
                        ? _db.Sections.Where(p => p.Id == s.ParentSectionId).Select(p => p.Name).FirstOrDefault()
                        : null,
                    StudentCount = _db.Students.Count(st => st.SectionId == s.Id && !st.IsDeleted)
                })
                .ToListAsync(cancellationToken);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lower = search.ToLower();
                allSections = allSections
                    .Where(s => s.Name.ToLower().Contains(lower) || s.ClassName.ToLower().Contains(lower))
                    .ToList();
            }

            int totalCount = allSections.Count;
            var paged = allSections
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new
                {
                    id = s.Id,
                    name = s.Name,
                    schoolClassId = s.SchoolClassId,
                    className = s.ClassName,
                    parentSectionId = s.ParentSectionId,
                    groupName = s.GroupName,
                    studentCount = s.StudentCount
                })
                .ToList<object>();

            return Json(new
            {
                data = paged,
                last_page = Math.Ceiling((double)totalCount / pageSize),
                total_records = totalCount
            });
        }

        return View();
    }

    /// <summary>Returns top-level group sections (ParentSectionId IS NULL) for a class. Used by CreateEdit UI.</summary>
    [HttpGet]
    public async Task<IActionResult> GetGroupsForClass(int classId, CancellationToken cancellationToken = default)
    {
        var groups = await _db.Sections
            .Where(s => s.SchoolClassId == classId && s.ParentSectionId == null && !s.IsDeleted)
            .Select(s => new { id = s.Id, name = s.Name })
            .ToListAsync(cancellationToken);
        return Json(groups);
    }

    [HttpGet]
    public async Task<IActionResult> GetSectionsByClass(int classId, CancellationToken cancellationToken = default)
    {
        var sections = await _db.Sections
            .Where(s => s.SchoolClassId == classId && !s.IsDeleted)
            .Select(s => new { id = s.Id, name = s.Name })
            .ToListAsync(cancellationToken);
        return Json(sections);
    }

    [HttpGet]
    public override async Task<IActionResult> CreateEdit(int? id = null, CancellationToken cancellationToken = default)
    {
        ViewBag.Classes = await _db.Classes.Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
        
        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value, cancellationToken);
            if (dto == null) return NotFound();
            var vm = new SectionViewModel
            {
                Id = dto.Id,
                SchoolClassId = dto.SchoolClassId,
                Name = dto.Name,
                ParentSectionId = dto.ParentSectionId
            };
            return View(vm);
        }
        return View(new SectionViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(SectionViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Classes = await _db.Classes.Where(x => !x.IsDeleted).ToListAsync();
            return View(vm);
        }
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (vm.IsEditMode) 
        { 
            await _service.UpdateAsync(new SectionUpsertDto
            {
                Id = vm.Id,
                SchoolClassId = vm.SchoolClassId,
                Name = vm.Name,
                ParentSectionId = vm.ParentSectionId
            }, userId); 
            TempData["SuccessMessage"] = "Section updated successfully."; 
        }
        else 
        { 
            await _service.CreateAsync(new SectionUpsertDto
            {
                SchoolClassId = vm.SchoolClassId,
                Name = vm.Name,
                ParentSectionId = vm.ParentSectionId
            }, userId); 
            TempData["SuccessMessage"] = "Section created successfully."; 
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(SectionViewModel vm) => CreateEdit(vm);

    [HttpGet]
    public new async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetForEditAsync(id, cancellationToken);
        return dto is null ? NotFound() : View(dto);
    }

    [HttpGet]
    public new async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetForEditAsync(id, cancellationToken);
        return dto is null ? NotFound() : View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId, cancellationToken);
        TempData["SuccessMessage"] = "Section deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

}

