using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Students;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
public class TranscriptController : Controller
{
    private readonly ITranscriptService _transcriptService;
    private readonly IStudentService _studentService;

    public TranscriptController(ITranscriptService transcriptService, IStudentService studentService)
    {
        _transcriptService = transcriptService;
        _studentService = studentService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller,Teacher,Senior Lecturer,Lecturer")]
    public async Task<IActionResult> Index(int studentId, int academicYearId)
    {
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
