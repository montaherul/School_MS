using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Models.ViewModels.Result;
using System.Security.Claims;
using SchoolManagementSystem.Helpers.Common;

namespace SchoolManagementSystem.Controllers;

[Authorize]
public class ResultManagementController : Controller
{
    private readonly SchoolDbContext _db;
    private readonly IResultService _resultService;

    public ResultManagementController(SchoolDbContext db, IResultService resultService)
    {
        _db = db;
        _resultService = resultService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal")]
    public async Task<IActionResult> AdminIndex()
    {
        var academicYear = await _db.AcademicYears.FirstOrDefaultAsync(x => x.IsActive);
        if (academicYear == null) return View(new List<ExamUpsertDto>());
        
        var exams = await _resultService.GetExamsAsync(academicYear.Id);
        return View(exams);
    }

    [HttpGet]
    [Authorize(Roles = "Teacher,Senior Lecturer,Lecturer")]
    public async Task<IActionResult> TeacherEntry()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return RedirectToAction("Index", "Home");

        var teacher = await _db.Teachers.FirstOrDefaultAsync(t => t.UserId == userId && !t.IsDeleted);
        if (teacher == null) return NotFound("Teacher profile not found.");

        // Only show classes the teacher is assigned to
        var assignments = await _db.TeacherClassAssignments
            .Include(a => a.Class)
            .Include(a => a.Section)
            .Where(a => a.TeacherId == teacher.Id && !a.IsDeleted)
            .ToListAsync();

        ViewBag.Assignments = assignments;
        
        var academicYear = await _db.AcademicYears.FirstOrDefaultAsync(x => x.IsActive);
        var exams = await _db.Exams
            .Where(x => !x.IsDeleted && x.AcademicYearId == (academicYear != null ? academicYear.Id : 0))
            .Select(x => new { x.Id, x.Name })
            .ToListAsync();
            
        ViewBag.Exams = exams;
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Teacher,Senior Lecturer,Lecturer")]
    public async Task<IActionResult> GetSubjectsForTeacher(int classId, int sectionId)
    {
        var teacherId = await GetTeacherIdAsync();
        if (teacherId == null) return Json(new List<object>());

        var subjects = await _db.TeacherSubjectAssignments
            .Include(a => a.Subject)
            .Where(a => a.TeacherId == teacherId && a.ClassId == classId && a.SectionId == sectionId && !a.IsDeleted)
            .Select(a => new 
            { 
                SubjectId = a.SubjectId, 
                SubjectName = a.Subject.Name 
            })
            .Distinct()
            .ToListAsync();

        return Json(subjects);
    }

    [HttpGet]
    public async Task<IActionResult> GetSectionsForClass(int classId)
    {
        var teacherId = await GetTeacherIdAsync();
        if (teacherId == null) return Json(new List<object>());

        var sections = await _db.TeacherClassAssignments
            .Include(a => a.Section)
            .Where(a => a.TeacherId == teacherId && a.ClassId == classId && !a.IsDeleted)
            .Select(a => new { id = a.SectionId, name = a.Section.Name })
            .Distinct()
            .ToListAsync();

        return Json(sections);
    }

    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> StudentIndex()
    {
        // Get logged in user id
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userIdStr))
        {
            TempData["Error"] = "User not found.";
            return RedirectToAction("Index", "Home");
        }

        // Convert to int
        if (!int.TryParse(userIdStr, out var userId))
        {
            TempData["Error"] = "Invalid user.";
            return RedirectToAction("Index", "Home");
        }

        // Find student
        var student = await _db.Students
            .Include(x => x.Class)
            .Include(x => x.Section)
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (student == null)
        {
            TempData["Error"] = "Student profile not found.";
            return RedirectToAction("Index", "Home");
        }

        // Get all subject results
        var model = await _resultService.GetStudentResultsAsync(student.Id);

        // Safety check
        if (model == null)
        {
            TempData["Error"] = "Result data not found.";
            return RedirectToAction("Index", "Home");
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> GetExams()
    {
        var academicYear = await _db.AcademicYears.FirstOrDefaultAsync(x => x.IsActive);
        var exams = await _resultService.GetExamsAsync(academicYear?.Id ?? 0);
        return Json(exams);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetSubjectsForClass(int classId)
    {
        var subjects = await _db.ClassSubjects
            .Include(cs => cs.Subject)
            .Where(cs => cs.SchoolClassId == classId
                         && !cs.IsDeleted)
            .Select(cs => new
            {
                subjectId = cs.SubjectId,
                subjectName = cs.IsReligionSubject &&
                              !string.IsNullOrEmpty(cs.ReligionType)
                    ? ReligionHelper.GetReligionSubjectName(cs.ReligionType)
                    : cs.Subject!.Name
            })
            .Distinct()
            .ToListAsync();

        return Json(subjects);
    }

    [HttpGet]
    [Authorize(Roles = "Principal,Teacher,Senior Lecturer,Lecturer")]
    public async Task<IActionResult> MarkEntry(
    int examId,
    int classId,
    int sectionId,
    int subjectId)
    {
        if (examId <= 0 || classId <= 0 || sectionId <= 0 || subjectId <= 0)
        {
            return BadRequest();
        }

        var vm = await _resultService.GetMarkEntryDataAsync(
    examId: examId,
    subjectId: subjectId,
    classId: classId,
    sectionId: sectionId);

        return View(vm);
    }

    [HttpPost]
    [Authorize(Roles = "Principal,Teacher,Senior Lecturer,Lecturer")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> SaveMarks([FromBody] MarkBatchDto dto)
    {
        var teacherId = await GetTeacherIdAsync();
        if (teacherId == null) return Json(new { success = false, message = "Teacher not found" });
        
        dto.TeacherId = teacherId.Value;
        await _resultService.SubmitMarksBatchAsync(dto);
        return Json(new { success = true });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Super Admin,Principal")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> PublishResults([FromBody] ResultPublishDto dto)
    {
        try 
        {
            await _resultService.PublishResultsAsync(dto);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal")]
    public async Task<IActionResult> ReEvaluationDashboard()
    {
        var model = await _resultService.GetReEvaluationDashboardAsync();
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Super Admin,Principal")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> ProcessReEvaluation([FromBody] ReEvaluationProcessDto dto)
    {
        var adminIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(adminIdStr, out var adminId))
        {
            await _resultService.ProcessReEvaluationAsync(dto, adminId);
            return Json(new { success = true });
        }
        return Json(new { success = false, message = "Admin not found" });
    }

    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> RequestReEvaluation(int examId, int subjectId)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdStr, out var userId))
        {
            var student = await _db.Students.FirstOrDefaultAsync(x => x.UserId == userId);
            if (student != null)
            {
                var dto = new ReEvaluationRequestDto
                {
                    ExamId = examId,
                    SubjectId = subjectId,
                    StudentId = student.Id
                };
                return View(dto);
            }
        }
        return RedirectToAction("StudentIndex");
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> RequestReEvaluation(ReEvaluationRequestDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdStr, out var userId))
        {
            await _resultService.RequestReEvaluationAsync(dto, userId);
            return RedirectToAction("StudentIndex");
        }
        return RedirectToAction("StudentIndex");
    }

    [HttpGet]
    public async Task<IActionResult> DownloadReportCard(int examId, int studentId)
    {
        var result = await _db.StudentExamResults
            .Include(x => x.Student)
            .Include(x => x.Exam)
            .FirstOrDefaultAsync(x =>
                x.ExamId == examId &&
                x.StudentId == studentId);

        if (result == null)
            return NotFound();

        var marks = await _db.Marks
            .Include(x => x.Subject)
            .Where(x =>
                x.ExamId == examId &&
                x.StudentId == studentId)
            .OrderBy(x => x.Subject.DisplayOrder)
            .ToListAsync();

        var pdfGenerator = HttpContext.RequestServices
            .GetService<SchoolManagementSystem.Helpers.Pdf.IPdfGenerator>();

        if (pdfGenerator == null)
            return NotFound();

        var pdf = pdfGenerator.GenerateSchoolReportCard(result, marks);

        return File(
            pdf,
            "application/pdf",
            $"ReportCard_{result.Student.StudentNo}_{result.Exam.Name}.pdf"
        );
    }

    private async Task<int?> GetTeacherIdAsync()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdStr, out var userId))
        {
            var teacher = await _db.Teachers.FirstOrDefaultAsync(x => x.UserId == userId);
            return teacher?.Id;
        }
        return null;
    }
}
