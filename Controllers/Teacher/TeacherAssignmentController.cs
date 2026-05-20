using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.Teachers;

namespace SchoolManagementSystem.Controllers.Teacher;

[RequirePermission("Teachers.Assign")]
[Route("TeacherAssignment")]
public class TeacherAssignmentController : Controller
{
    private readonly ITeacherAssignmentService _service;

    public TeacherAssignmentController(ITeacherAssignmentService service)
    {
        _service = service;
    }

    [HttpGet("{teacherId}")]
    public async Task<IActionResult> Index(int teacherId, CancellationToken ct)
    {
        var teacher = await _service.GetTeacherWithAssignmentsAsync(teacherId, ct);
        if (teacher == null) return NotFound();

        ViewBag.Classes = await _service.GetClassesAsync(ct);
        ViewBag.AcademicYears = await _service.GetAcademicYearsAsync(ct);
        ViewBag.Subjects = await _service.GetSubjectsAsync(ct);

        return View(teacher);
    }



    /// <summary>JSON endpoint used by Teacher/Details.cshtml assignment tab.</summary>
    [HttpGet("GetByTeacher/{teacherId}")]
    public async Task<IActionResult> GetByTeacher(int teacherId, CancellationToken ct)
    {
        var classAssignments  = await _service.GetTeacherClassAssignmentsAsync(teacherId, ct);
        var subjectAssignments = await _service.GetTeacherSubjectAssignmentsAsync(teacherId, 0, 0, ct);

        // Merge into a unified flat list for the Details tab
        var merged = subjectAssignments.Select(s => {
            var classAssign = classAssignments.FirstOrDefault(ca => ca.ClassId == s.ClassId && ca.SectionId == s.SectionId);
            return new
            {
                className      = classAssign?.ClassName ?? "Unknown",
                sectionName    = classAssign?.SectionName ?? "Unknown",
                subjectName    = s.SubjectName,
                isClassTeacher = false
            };
        }).ToList();

        // Append class-only rows that have no subject assignments
        foreach (var ca in classAssignments)
        {
            bool hasSubjects = subjectAssignments.Any(s =>
                s.ClassId == ca.ClassId && s.SectionId == ca.SectionId);

            if (!hasSubjects)
            {
                merged.Add(new
                {
                    className      = ca.ClassName,
                    sectionName    = ca.SectionName,
                    subjectName    = (string?)"—",
                    isClassTeacher = true // Since it's a class assignment without specific subjects, assume class teacher role or general assignment
                });
            }
        }

        return Json(merged);
    }


    [HttpPost("AssignClass")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignClass(int teacherId, int classId, int sectionId, int academicYearId)
    {
        try
        {
            var success = await _service.AssignClassAsync(teacherId, classId, sectionId, academicYearId, User.Identity?.Name ?? "System");
            TempData[success ? "SuccessMessage" : "ErrorMessage"] = success ? "Class assigned successfully." : "This assignment already exists.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Index), new { teacherId });
    }

    [HttpPost("AssignSubject")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignSubject(int teacherId, int subjectId, int classId, int sectionId, int academicYearId)
    {
        try
        {
            var success = await _service.AssignSubjectAsync(teacherId, subjectId, classId, sectionId, academicYearId, User.Identity?.Name ?? "System");
            TempData[success ? "SuccessMessage" : "ErrorMessage"] = success ? "Subject assigned successfully." : "This assignment already exists.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Index), new { teacherId });
    }

    [HttpPost("RemoveClassAssignment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveClassAssignment(int id)
    {
        await _service.RemoveClassAssignmentAsync(id);
        return Ok();
    }

    [HttpPost("RemoveSubjectAssignment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveSubjectAssignment(int id)
    {
        await _service.RemoveSubjectAssignmentAsync(id);
        return Ok();
    }
}
