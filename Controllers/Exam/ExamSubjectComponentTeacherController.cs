using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Models.Entities.Teachers;

namespace SchoolManagementSystem.Controllers.Exam;

using SchoolManagementSystem.Filters;

[RequirePermission("ExamSubjectComponent.Manage")]
public class ExamSubjectComponentTeacherController : Controller
{
    private readonly IExamSubjectComponentTeacherService _componentService;
    private readonly ITeacherAssignmentService _teacherAssignmentService;
    private readonly IUnitOfWork _uow;

    public ExamSubjectComponentTeacherController(
        IExamSubjectComponentTeacherService componentService,
        ITeacherAssignmentService teacherAssignmentService,
        IUnitOfWork uow)
    {
        _componentService = componentService;
        _teacherAssignmentService = teacherAssignmentService;
        _uow = uow;
    }

    private int GetTeacherId()
    {
        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId)) return 0;

        var teacher = _uow.Repository<Teacher>().QueryNoTracking()
            .FirstOrDefault(t => t.UserId == userId && !t.IsDeleted);

        return teacher?.Id ?? 0;
    }

    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var teacherId = GetTeacherId();
        if (teacherId == 0)
            return RedirectToAction("Login", "Auth");

        var isEnabled = await _componentService.IsCustomizationEnabledAsync(ct);
        ViewBag.IsCustomizationEnabled = isEnabled;

        var subjects = await _componentService.GetTeacherExamSubjectsAsync(teacherId, ct);
        return View(subjects);
    }

    [HttpGet]
    public async Task<IActionResult> Components(int examSubjectId, CancellationToken ct = default)
    {
        var teacherId = GetTeacherId();
        if (teacherId == 0)
            return RedirectToAction("Login", "Auth");

        var canCustomize = await _componentService.CanCustomizeAsync(teacherId, examSubjectId, ct);
        var components = await _componentService.GetExamSubjectComponentsAsync(teacherId, examSubjectId, ct);

        ViewBag.ExamSubjectId = examSubjectId;
        ViewBag.CanCustomize = canCustomize;

        return View(components);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateComponents(int examSubjectId, List<TeacherExamSubjectComponentUpsertDto> components, CancellationToken ct = default)
    {
        var teacherId = GetTeacherId();
        if (teacherId == 0)
            return Json(new { success = false, message = "Teacher not found." });

        try
        {
            var updatedBy = User.Identity?.Name ?? "System";
            await _componentService.UpdateComponentsBulkAsync(teacherId, examSubjectId, components, updatedBy, ct);
            return Json(new { success = true, message = "Components updated successfully." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "An error occurred while updating components." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> MarksEntryConfig(int examSubjectId, CancellationToken ct = default)
    {
        var teacherId = GetTeacherId();
        if (teacherId == 0)
            return Json(new { success = false, message = "Teacher not found." });

        var config = await _componentService.GetMarksEntryGridConfigAsync(teacherId, examSubjectId, ct);
        if (config == null)
            return Json(new { success = false, message = "Exam subject not found." });

        return Json(new { success = true, data = config });
    }
}