using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
public class TranscriptController : Controller
{
    private readonly ITranscriptService _transcriptService;
    private readonly IStudentService _studentService;
    private readonly ITeacherScopeService _teacherScopeService;
    private readonly IUnitOfWork _uow;

    public TranscriptController(ITranscriptService transcriptService, IStudentService studentService, ITeacherScopeService teacherScopeService, IUnitOfWork uow)
    {
        _transcriptService = transcriptService;
        _studentService = studentService;
        _teacherScopeService = teacherScopeService;
        _uow = uow;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller,Teacher,Senior Lecturer,Lecturer")]
    public async Task<IActionResult> Index(int studentId, int academicYearId)
    {
        var isAdmin = User.IsInRole("Admin") || User.IsInRole("Super Admin") || User.IsInRole("Principal") || User.IsInRole("Exam Controller");
        if (!isAdmin)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            if (!await _teacherScopeService.HasStudentAccessAsync(currentUserId, studentId, default))
                return Forbid();
        }

        var transcript = await _transcriptService.GetStudentTranscriptAsync(studentId, academicYearId);
        if (transcript == null)
            return NotFound("Transcript not found");
        return View(transcript);
    }

    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> MyTranscript(int academicYearId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var student = await _studentService.GetByUserIdAsync(userId, default);
        if (student == null) return Forbid();

        var transcript = await _transcriptService.GetStudentTranscriptAsync(student.Id, academicYearId);
        if (transcript == null)
            return NotFound("Transcript not found");
        return View("Index", transcript);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller,Teacher,Senior Lecturer,Lecturer,Student,Guardian")]
    public async Task<IActionResult> DownloadPdf(int studentId, int academicYearId)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        bool isAdmin = User.IsInRole("Admin") || User.IsInRole("Super Admin") || User.IsInRole("Principal") || User.IsInRole("Exam Controller");

        if (!isAdmin)
        {
            if (User.IsInRole("Student"))
            {
                var student = await _studentService.GetByUserIdAsync(currentUserId, default);
                if (student == null || student.Id != studentId)
                    return Forbid();
            }
            else if (User.IsInRole("Guardian"))
            {
                var guardianRepo = _uow.Repository<SchoolManagementSystem.Models.Entities.Guardian.StudentGuardian>();
                var hasAccess = await guardianRepo.AnyAsync(sg => sg.Guardian!.UserId == currentUserId && sg.StudentId == studentId);
                if (!hasAccess) return Forbid();
            }
            else if (User.IsInRole("Teacher") || User.IsInRole("Senior Lecturer") || User.IsInRole("Lecturer"))
            {
                if (!await _teacherScopeService.HasStudentAccessAsync(currentUserId, studentId, default))
                    return Forbid();
            }
            else
            {
                return Forbid();
            }
        }

        var isBlocked = await _transcriptService.IsResultBlockedForStudentAsync(studentId, default);
        if (isBlocked)
        {
            TempData["ErrorMessage"] = "Result access is blocked due to outstanding fees. Please clear your dues to access results.";
            return RedirectToAction("Index", "Dashboard");
        }

        var pdfBytes = await _transcriptService.GenerateTranscriptPdfAsync(studentId, academicYearId);
        if (pdfBytes == null)
            return NotFound("Transcript could not be generated.");

        return File(pdfBytes, "application/pdf", $"Transcript_Student_{studentId}_Year_{academicYearId}.pdf");
    }
}
