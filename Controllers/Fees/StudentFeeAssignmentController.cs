using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.ViewModels.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class StudentFeeAssignmentController : Controller
{
    private readonly IStudentFeeAssignmentService _service;
    private readonly IFeeSecurityService _security;
    public StudentFeeAssignmentController(IStudentFeeAssignmentService service, IFeeSecurityService security) { _service = service; _security = security; }

    [RequirePermission("StudentFeeAssignments.Read")]
    public IActionResult Index() { return View(); }

    [HttpGet]
    [RequirePermission("StudentFeeAssignments.Create")]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("StudentFeeAssignments.Update")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    [RequirePermission("StudentFeeAssignments.Read")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null, int? studentId = null, int? feeStructureId = null)
    {
        if (_security.HasStudentRole(User)) studentId = _security.GetCurrentStudentId(User);
        var result = await _service.GetPagedAsync(page, size, search, studentId, feeStructureId);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (!_security.Can(User, id.HasValue && id > 0 ? "StudentFeeAssignments.Update" : "StudentFeeAssignments.Create"))
            return Forbid();
        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            if (!_security.IsStudentScope(User, dto.StudentId)) return Forbid();
            return View(new StudentFeeAssignmentViewModel { Id = dto.Id, StudentId = dto.StudentId, FeeStructureId = dto.FeeStructureId, AcademicYearId = dto.AcademicYearId, CustomAmount = dto.CustomAmount, IsActive = dto.IsActive, ValidFrom = dto.ValidFrom, ValidTo = dto.ValidTo });
        }
        return View(new StudentFeeAssignmentViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(StudentFeeAssignmentViewModel vm)
    {
        if (!_security.Can(User, vm.IsEditMode ? "StudentFeeAssignments.Update" : "StudentFeeAssignments.Create"))
            return Forbid();
        if (!ModelState.IsValid) return View(vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (vm.IsEditMode) { await _service.UpdateAsync(vm, userId); TempData["SuccessMessage"] = "Assignment updated."; }
        else { await _service.CreateAsync(vm, userId); TempData["SuccessMessage"] = "Assignment created."; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(StudentFeeAssignmentViewModel vm) => CreateEdit(vm);

    [HttpGet]
    [RequirePermission("StudentFeeAssignments.Read")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        if (!_security.IsStudentScope(User, dto.StudentId)) return Forbid();
        return View(new StudentFeeAssignmentViewModel { Id = dto.Id, StudentId = dto.StudentId, FeeStructureId = dto.FeeStructureId, AcademicYearId = dto.AcademicYearId, CustomAmount = dto.CustomAmount, IsActive = dto.IsActive, ValidFrom = dto.ValidFrom, ValidTo = dto.ValidTo });
    }

    [HttpGet]
    [RequirePermission("StudentFeeAssignments.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        if (!_security.IsStudentScope(User, dto.StudentId)) return Forbid();
        return View(new StudentFeeAssignmentViewModel { Id = dto.Id, StudentId = dto.StudentId, FeeStructureId = dto.FeeStructureId, AcademicYearId = dto.AcademicYearId, CustomAmount = dto.CustomAmount, IsActive = dto.IsActive, ValidFrom = dto.ValidFrom, ValidTo = dto.ValidTo });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("StudentFeeAssignments.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId);
        TempData["SuccessMessage"] = "Assignment deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("StudentFeeAssignments.Delete")]
    public async Task<IActionResult> Restore(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        if (!_security.IsStudentScope(User, dto.StudentId)) return Forbid();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.RestoreAsync(id, userId);
        TempData["SuccessMessage"] = "Assignment restored successfully.";
        return RedirectToAction(nameof(Index));
    }
}
