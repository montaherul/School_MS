using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
public class TranscriptController : Controller
{
    private readonly ITranscriptService _transcriptService;
    private readonly IStudentService _studentService;
    private readonly ITeacherScopeService _teacherScopeService;

    public TranscriptController(ITranscriptService transcriptService, IStudentService studentService, ITeacherScopeService teacherScopeService)
    {
        _transcriptService = transcriptService;
        _studentService = studentService;
        _teacherScopeService = teacherScopeService;
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
    [Authorize]
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
}
