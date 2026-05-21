using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Academic;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
public class ResultManagementController : Controller
{
    private readonly IExamService _examService;
    private readonly IResultPublicationService _publicationService;
    private readonly IMarkEntryService _markEntryService;
    private readonly IReEvaluationService _reEvaluationService;
    private readonly IReportCardService _reportCardService;
    private readonly ITeacherService _teacherService;
    private readonly IStudentService _studentService;
    private readonly IAcademicYearService _academicYearService;
    private readonly ISectionService _sectionService;
    private readonly ITeacherAssignmentService _assignmentService;

    public ResultManagementController(
        IExamService examService,
        IResultPublicationService publicationService,
        IMarkEntryService markEntryService,
        IReEvaluationService reEvaluationService,
        IReportCardService reportCardService,
        ITeacherService teacherService,
        IStudentService studentService,
        IAcademicYearService academicYearService,
        ISectionService sectionService,
        ITeacherAssignmentService assignmentService)
    {
        _examService = examService;
        _publicationService = publicationService;
        _markEntryService = markEntryService;
        _reEvaluationService = reEvaluationService;
        _reportCardService = reportCardService;
        _teacherService = teacherService;
        _studentService = studentService;
        _academicYearService = academicYearService;
        _sectionService = sectionService;
        _assignmentService = assignmentService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal")]
    public async Task<IActionResult> AdminIndex(CancellationToken ct)
    {
        var exams = await _examService.GetExamsAsync(0); 
        return View(exams);
    }

    [HttpGet]
    [Authorize(Roles = "Teacher,Senior Lecturer,Lecturer")]
    public async Task<IActionResult> TeacherEntry(CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        if (teacher == null) return NotFound("Teacher profile not found.");

        // Get active academic year
        var academicYears = await _academicYearService.GetPagedAsync(1, 100, string.Empty, ct);
        var activeYear = academicYears.Items.FirstOrDefault(ay => ay.IsActive);
        var academicYearId = activeYear?.Id ?? 1; // Default to 1 if no active year found

        ViewBag.Assignments = await _assignmentService.GetTeacherClassAssignmentsAsync(teacher.Id, ct);
        ViewBag.Exams = await _examService.GetExamsAsync(academicYearId);
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Teacher,Senior Lecturer,Lecturer")]
    public async Task<IActionResult> GetSubjectsForTeacher(int classId, int sectionId, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        if (teacher == null) return Json(new List<object>());

        var subjects = await _assignmentService.GetTeacherSubjectAssignmentsAsync(teacher.Id, classId, sectionId, ct);
        return Json(subjects.Select(s => new { subjectId = s.SubjectId, subjectName = s.SubjectName }));
    }

    [HttpGet]
    public async Task<IActionResult> GetSectionsForClass(int classId, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        if (teacher == null) return Json(new List<object>());

        var assignments = await _assignmentService.GetTeacherClassAssignmentsAsync(teacher.Id, ct);
        var sections = assignments.Where(a => a.ClassId == classId).Select(a => new { id = a.SectionId, name = a.SectionName }).Distinct();
        return Json(sections);
    }

    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> StudentIndex(CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var student = await _studentService.GetByUserIdAsync(currentUserId, ct);
        if (student == null)
        {
            TempData["Error"] = "Student profile not found.";
            return RedirectToAction("Index", "Home");
        }

        var model = await _publicationService.GetStudentResultsAsync(student.Id);
        if (model == null)
        {
            TempData["Error"] = "Result data not found.";
            return RedirectToAction("Index", "Home");
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> GetExams(CancellationToken ct)
    {
        var exams = await _examService.GetExamsAsync(0);
        return Json(exams);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetSubjectsForClass(int classId, CancellationToken ct)
    {
        var subjects = await _examService.GetSubjectsByClassIdAsync(classId, ct);
        return Json(subjects);
    }

    [HttpGet]
    [Authorize(Roles = "Principal,Teacher,Senior Lecturer,Lecturer")]
    public async Task<IActionResult> MarkEntry(int examId, int classId, int sectionId, int subjectId)
    {
        if (examId <= 0 || classId <= 0 || sectionId <= 0 || subjectId <= 0) return BadRequest();
        var vm = await _markEntryService.GetMarkEntryDataAsync(examId, subjectId, classId, sectionId);
        return View(vm);
    }

    [HttpPost]
    [Authorize(Roles = "Principal,Teacher,Senior Lecturer,Lecturer")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> SaveMarks([FromBody] MarkBatchDto dto, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        if (teacher == null) return Json(new { success = false, message = "Teacher not found" });
        
        dto.TeacherId = teacher.Id;
        await _markEntryService.SubmitMarksBatchAsync(dto);
        return Json(new { success = true });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Super Admin,Principal")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> PublishResults([FromBody] ResultPublishDto dto)
    {
        try 
        {
            await _publicationService.PublishResultsAsync(dto);
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
        var model = await _reEvaluationService.GetReEvaluationDashboardAsync();
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Super Admin,Principal")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> ProcessReEvaluation([FromBody] ReEvaluationProcessDto dto)
    {
        var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        await _reEvaluationService.ProcessReEvaluationAsync(dto, adminId);
        return Json(new { success = true });
    }

    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> RequestReEvaluation(int examId, int subjectId, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var student = await _studentService.GetByUserIdAsync(currentUserId, ct);
        if (student == null) return RedirectToAction("StudentIndex");

        var dto = new ReEvaluationRequestDto { ExamId = examId, SubjectId = subjectId, StudentId = student.Id };
        return View(dto);
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> RequestReEvaluation(ReEvaluationRequestDto dto)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        await _reEvaluationService.RequestReEvaluationAsync(dto, currentUserId);
        return RedirectToAction("StudentIndex");
    }

    [HttpGet]
    public async Task<IActionResult> DownloadReportCard(int examId, int studentId)
    {
        var pdf = await _reportCardService.GenerateReportCardPdfAsync(examId, studentId);
        if (pdf == null) return NotFound();
        return File(pdf, "application/pdf", $"ReportCard_{studentId}_{examId}.pdf");
    }
}

