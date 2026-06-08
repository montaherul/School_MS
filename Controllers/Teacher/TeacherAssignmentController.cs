using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.Teachers;

namespace SchoolManagementSystem.Controllers.Teacher;

[Authorize]
[Route("TeacherAssignment")]
public class TeacherAssignmentController : Controller
{
    private readonly ITeacherAssignmentService _service;

    public TeacherAssignmentController(ITeacherAssignmentService service)
    {
        _service = service;
    }

    [HttpGet("{teacherId}")]
    [RequirePermission("Teachers.Assign")]
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
            // New JSON endpoints for Bangladesh curriculum filtering
        // Get classes assigned to teacher
        [HttpGet("GetAssignedClasses/{teacherId}")]
        public async Task<IActionResult> GetAssignedClasses(int teacherId, CancellationToken ct)
        {
            var classes = await _service.GetClassesByTeacherIdAsync(teacherId, ct);
            return Json(classes.Select(c => new { id = c.Id, name = c.Name, isGroupBased = c.IsGroupBased }));
        }

        // Get groups assigned to teacher for a class
        [HttpGet("GetAssignedGroups/{teacherId}/{classId}")]
        public async Task<IActionResult> GetAssignedGroups(int teacherId, int classId, CancellationToken ct)
        {
            var groups = await _service.GetTeacherAssignedGroupsAsync(teacherId, classId, ct);
            return Json(groups.Select(g => new { id = g.Id, name = g.Name }));
        }

        // Get sections assigned to teacher for a class, optionally filtered by group
        [HttpGet("GetAssignedSections/{teacherId}/{classId}")]
        public async Task<IActionResult> GetAssignedSections(int teacherId, int classId, int? groupId, CancellationToken ct)
        {
            var sections = await _service.GetTeacherAssignedSectionsAsync(teacherId, classId, groupId, ct);
            return Json(sections.Select(s => new { id = s.Id, name = s.Name }));
        }

        // Get subjects assigned to teacher for a class, optionally filtered by group and section
        [HttpGet("GetAssignedSubjects/{teacherId}/{classId}")]
        public async Task<IActionResult> GetAssignedSubjects(int teacherId, int classId, int? groupId, int? sectionId, CancellationToken ct)
        {
            var subjects = await _service.GetTeacherAssignedSubjectsAsync(teacherId, classId, groupId, sectionId, ct);
            return Json(subjects.Select(s => new { subjectId = s.Id, subjectName = s.Name }));
        }
    [HttpGet("GetByTeacher/{teacherId}")]
    public async Task<IActionResult> GetByTeacher(int teacherId, int? classId, int? groupId, int? sectionId, int? subjectId, CancellationToken ct)
    {
        var classAssignments  = await _service.GetTeacherClassAssignmentsAsync(teacherId, ct);
        var subjectAssignments = await _service.GetTeacherSubjectAssignmentsAsync(teacherId, classId ?? 0, sectionId ?? 0, ct);

        // If classId is specified, filter class assignments too
        if (classId.HasValue && classId.Value > 0)
        {
            classAssignments = classAssignments.Where(ca => ca.ClassId == classId.Value).ToList();
        }

        // If groupId is specified, filter subject and class assignments
        if (groupId.HasValue && groupId.Value > 0)
        {
            subjectAssignments = subjectAssignments.Where(s => s.GroupId == groupId.Value).ToList();
            classAssignments = classAssignments.Where(ca => ca.GroupId == groupId.Value).ToList();
        }

        // If subjectId is specified, filter subject assignments
        if (subjectId.HasValue && subjectId.Value > 0)
        {
            subjectAssignments = subjectAssignments.Where(s => s.SubjectId == subjectId.Value).ToList();
        }

        // Merge into a unified flat list for the Details tab
        var merged = subjectAssignments.Select(s => {
            var classAssign = classAssignments.FirstOrDefault(ca => ca.ClassId == s.ClassId && ca.SectionId == s.SectionId);
            return new
            {
                className      = classAssign?.ClassName ?? "Unknown",
                sectionName    = classAssign?.SectionName ?? "Unknown",
                subjectName    = s.SubjectName,
                isClassTeacher = false,
                groupName      = s.GroupName
            };
        }).ToList();

        // Append class-only rows that have no subject assignments
        // Only if we are not filtering by a specific subject
        if (!subjectId.HasValue || subjectId.Value <= 0)
        {
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
                        isClassTeacher = true,
                        groupName      = ca.GroupName
                    });
                }
            }
        }

        return Json(merged);
    }

    [HttpPost("AssignClass")]
    [RequirePermission("Teachers.Assign")]
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
    [RequirePermission("Teachers.Assign")]
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
    [RequirePermission("Teachers.Assign")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveClassAssignment(int id)
    {
        await _service.RemoveClassAssignmentAsync(id);
        return Ok();
    }

    [HttpPost("RemoveSubjectAssignment")]
    [RequirePermission("Teachers.Assign")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveSubjectAssignment(int id)
    {
        await _service.RemoveSubjectAssignmentAsync(id);
        return Ok();
    }
}
