using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Filters;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Teachers;


[RequirePermission("Teachers.Assign")]
[Route("TeacherAssignment")]
public class TeacherAssignmentController : Controller
{
    private readonly SchoolDbContext _db;

    public TeacherAssignmentController(SchoolDbContext db)
    {
        _db = db;
    }

    [HttpGet("{teacherId}")]
    public async Task<IActionResult> Index(int teacherId)
    {
        var teacher = await _db.Teachers
            .Include(t => t.ClassAssignments).ThenInclude(a => a.Class)
            .Include(t => t.ClassAssignments).ThenInclude(a => a.Section)
            .Include(t => t.SubjectAssignments).ThenInclude(a => a.Subject)
            .Include(t => t.SubjectAssignments).ThenInclude(a => a.Class)
            .Include(t => t.SubjectAssignments).ThenInclude(a => a.Section)
            .FirstOrDefaultAsync(t => t.Id == teacherId && !t.IsDeleted);

        if (teacher == null)
            return NotFound();

        ViewBag.Classes = await _db.Classes
            .Where(x => !x.IsDeleted)
            .ToListAsync();

        ViewBag.AcademicYears = await _db.AcademicYears
            .Where(x => !x.IsDeleted)
            .ToListAsync();

        ViewBag.Subjects = await _db.Subjects
            .Where(x => !x.IsDeleted)
            .ToListAsync();

        return View(teacher);
    }

    [HttpPost("AssignClass")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignClass(int teacherId, int classId, int sectionId, int academicYearId)
    {
        var exists = await _db.TeacherClassAssignments
            .AnyAsync(a => a.TeacherId == teacherId && a.ClassId == classId && a.SectionId == sectionId && a.AcademicYearId == academicYearId && !a.IsDeleted);

        if (!exists)
        {
            var assignment = new TeacherClassAssignment
            {
                TeacherId = teacherId,
                ClassId = classId,
                SectionId = sectionId,
                AcademicYearId = academicYearId,
                CreatedBy = User.Identity?.Name ?? "System",
                CreatedAt = DateTime.UtcNow
            };
            _db.TeacherClassAssignments.Add(assignment);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Class assigned successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "This assignment already exists.";
        }

        return RedirectToAction(nameof(Index), new { teacherId });
    }

    [HttpPost("AssignSubject")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignSubject(int teacherId, int subjectId, int classId, int sectionId, int academicYearId)
    {
        var exists = await _db.TeacherSubjectAssignments
            .AnyAsync(a => a.TeacherId == teacherId && a.SubjectId == subjectId && a.ClassId == classId && a.SectionId == sectionId && a.AcademicYearId == academicYearId && !a.IsDeleted);

        if (!exists)
        {
            var assignment = new TeacherSubjectAssignment
            {
                TeacherId = teacherId,
                SubjectId = subjectId,
                ClassId = classId,
                SectionId = sectionId,
                AcademicYearId = academicYearId,
                CreatedBy = User.Identity?.Name ?? "System",
                CreatedAt = DateTime.UtcNow
            };
            _db.TeacherSubjectAssignments.Add(assignment);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Subject assigned successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "This assignment already exists.";
        }

        return RedirectToAction(nameof(Index), new { teacherId });
    }

    [HttpPost("RemoveClassAssignment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveClassAssignment(int id)
    {
        var assignment = await _db.TeacherClassAssignments.FindAsync(id);
        if (assignment != null)
        {
            assignment.IsDeleted = true;
            await _db.SaveChangesAsync();
        }
        return Ok();
    }

    [HttpPost("RemoveSubjectAssignment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveSubjectAssignment(int id)
    {
        var assignment = await _db.TeacherSubjectAssignments.FindAsync(id);
        if (assignment != null)
        {
            assignment.IsDeleted = true;
            await _db.SaveChangesAsync();
        }
        return Ok();
    }
}
