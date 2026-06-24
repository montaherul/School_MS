using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Enums;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Exam;
using SchoolManagementSystem.Services.Interfaces.Guardian;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Exam;

[Authorize]
public class ExamRoutineController : Controller
{
    private readonly IExamRoutineService _examRoutineService;
    private readonly IStudentService _studentService;
    private readonly IGuardianService _guardianService;
    private readonly ITeacherService _teacherService;
    private readonly ISchoolClassService _classService;
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public ExamRoutineController(
        IExamRoutineService examRoutineService,
        IStudentService studentService,
        IGuardianService guardianService,
        ITeacherService teacherService,
        ISchoolClassService classService,
        IPdfGenerator pdfGenerator,
        IUnitOfWork unitOfWork)
    {
        _examRoutineService = examRoutineService;
        _studentService = studentService;
        _guardianService = guardianService;
        _teacherService = teacherService;
        _classService = classService;
        _pdfGenerator = pdfGenerator;
        _unitOfWork = unitOfWork;
    }

    // ── Student Portal ──────────────────────────────────────────

    [HttpGet("Student/ExamRoutine")]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> Student(CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var studentId = await _studentService.GetStudentIdByUserIdAsync(userId, ct);
        if (studentId == null)
            return NotFound("Student record not found.");

        var viewModel = await _examRoutineService.GetStudentRoutineViewAsync(studentId.Value, ct);
        return View("~/Views/ExamRoutine/Student.cshtml", viewModel);
    }

    [HttpGet("Student/ExamRoutine/DownloadPdf")]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> StudentDownloadPdf(CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var studentId = await _studentService.GetStudentIdByUserIdAsync(userId, ct);
        if (studentId == null) return NotFound();

        var viewModel = await _examRoutineService.GetStudentRoutineViewAsync(studentId.Value, ct);
        var html = await _examRoutineService.RenderRoutineHtmlAsync(
            viewModel.Schedules, viewModel.ExamName, viewModel.ClassName ?? "", viewModel.GroupName, ct);
        var pdf = _pdfGenerator.GenerateFromHtml(html);
        return File(pdf, "application/pdf", $"ExamRoutine_{viewModel.StudentNo}.pdf");
    }

    // ── Guardian Portal ─────────────────────────────────────────

    [HttpGet("Guardian/ExamRoutine")]
    [Authorize(Roles = "Guardian")]
    public async Task<IActionResult> Guardian(int? studentId, CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var children = await _guardianService.GetChildrenByUserIdAsync(userId, ct);
        if (!children.Any())
            return View("~/Views/GuardianPortal/Empty.cshtml", "No child linked to your account.");

        var sid = studentId.HasValue && children.Any(c => c.StudentId == studentId.Value)
            ? studentId.Value
            : children.First().StudentId;

        if (!await _guardianService.UserHasAccessToStudentAsync(userId, sid, ct))
            return Forbid();

        ViewBag.Children = children;
        ViewBag.SelectedStudentId = sid;

        var viewModel = await _examRoutineService.GetGuardianRoutineViewAsync(sid, ct);
        return View("~/Views/ExamRoutine/Guardian.cshtml", viewModel);
    }

    [HttpGet("Guardian/ExamRoutine/DownloadPdf")]
    [Authorize(Roles = "Guardian")]
    public async Task<IActionResult> GuardianDownloadPdf(int studentId, CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        if (!await _guardianService.UserHasAccessToStudentAsync(userId, studentId, ct))
            return Forbid();

        var viewModel = await _examRoutineService.GetGuardianRoutineViewAsync(studentId, ct);
        var html = await _examRoutineService.RenderRoutineHtmlAsync(
            viewModel.Schedules, viewModel.ExamName, viewModel.ClassName ?? "", viewModel.GroupName, ct);
        var pdf = _pdfGenerator.GenerateFromHtml(html);
        return File(pdf, "application/pdf", $"ExamRoutine_Student{studentId}.pdf");
    }

    // ── Teacher Portal ──────────────────────────────────────────

    [HttpGet("Teacher/ExamRoutine")]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> Teacher(CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(userId, ct);
        if (teacher == null)
            return NotFound("Teacher record not found.");

        var viewModel = await _examRoutineService.GetTeacherRoutineViewAsync(teacher.Id, ct);
        return View("~/Views/ExamRoutine/Teacher.cshtml", viewModel);
    }

    [HttpGet("Teacher/ExamRoutine/DownloadPdf")]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> TeacherDownloadPdf(CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(userId, ct);
        if (teacher == null) return NotFound();

        var viewModel = await _examRoutineService.GetTeacherRoutineViewAsync(teacher.Id, ct);
        var html = await _examRoutineService.RenderRoutineHtmlAsync(
            viewModel.Schedules, viewModel.ExamName, viewModel.ClassName ?? "", viewModel.GroupName, ct);
        var pdf = _pdfGenerator.GenerateFromHtml(html);
        return File(pdf, "application/pdf", $"ExamRoutine_Teacher{teacher.Id}.pdf");
    }

    // ── Admin Portal ────────────────────────────────────────────

    [HttpGet("Admin/ExamRoutine")]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> Admin(int? examId, int? classId, int? groupId, CancellationToken ct)
    {
        var exams = await _unitOfWork.Repository<ExamEntity>().Query()
            .AsNoTracking()
            .Where(e => !e.IsDeleted)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync(ct);
        ViewBag.Exams = exams;

        var classes = await _classService.GetAllAsync(ct);
        ViewBag.Classes = classes;

        if (examId.HasValue && classId.HasValue)
        {
            var schedules = await _examRoutineService.GetClassRoutineAsync(examId.Value, classId.Value, groupId, ct);
            var className = classes.FirstOrDefault(c => c.Id == classId.Value)?.Name ?? "";
            var exam = exams.FirstOrDefault(e => e.Id == examId.Value);

            var viewModel = new ExamRoutineViewModel
            {
                Schedules = schedules,
                ExamName = exam?.Name ?? "",
                ClassName = className
            };
            return View("~/Views/ExamRoutine/Admin.cshtml", viewModel);
        }

        return View("~/Views/ExamRoutine/Admin.cshtml", new ExamRoutineViewModel());
    }

    [HttpGet("Admin/ExamRoutine/Schedules")]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetSchedules(int examId, int classId, int? groupId, CancellationToken ct)
    {
        var schedules = await _examRoutineService.GetClassRoutineAsync(examId, classId, groupId, ct);
        return Json(schedules);
    }

    [HttpPost("Admin/ExamRoutine/Publish/{examId}")]
    [RequirePermission("Routine.Publish")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publish(int examId, CancellationToken ct)
    {
        var exam = await _unitOfWork.Repository<ExamEntity>().GetByIdAsync(examId, ct);
        if (exam == null) return Json(new { success = false, message = "Exam not found." });
        exam.Status = ResultWorkflowStatus.Published;
        exam.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
        return Json(new { success = true, message = "Exam routine published." });
    }

    [HttpPost("Admin/ExamRoutine/Unpublish/{examId}")]
    [RequirePermission("Routine.Publish")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unpublish(int examId, CancellationToken ct)
    {
        var exam = await _unitOfWork.Repository<ExamEntity>().GetByIdAsync(examId, ct);
        if (exam == null) return Json(new { success = false, message = "Exam not found." });
        exam.Status = ResultWorkflowStatus.Draft;
        exam.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
        return Json(new { success = true, message = "Exam routine unpublished." });
    }

    [HttpGet("Admin/ExamRoutine/BulkPdf")]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> BulkPdf(int examId, int classId, int? groupId, CancellationToken ct)
    {
        var schedules = await _examRoutineService.GetClassRoutineAsync(examId, classId, groupId, ct);
        var className = (await _classService.GetAllAsync(ct)).FirstOrDefault(c => c.Id == classId)?.Name ?? "";
        var exams = await _unitOfWork.Repository<ExamEntity>().Query()
            .AsNoTracking()
            .Where(e => !e.IsDeleted)
            .ToListAsync(ct);
        var examName = exams.FirstOrDefault(e => e.Id == examId)?.Name ?? "Exam Routine";
        var html = await _examRoutineService.RenderRoutineHtmlAsync(schedules, examName, className, null, ct);
        var pdf = _pdfGenerator.GenerateFromHtml(html);
        return File(pdf, "application/pdf", $"ExamRoutine_Class{classId}.pdf");
    }
}
