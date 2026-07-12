using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces;
using SchoolManagementSystem.Services.Interfaces.Exam;
using SchoolManagementSystem.Services.Interfaces.Students;

namespace SchoolManagementSystem.Controllers.Exam;

[Authorize]
public class AdmitCardController : Controller
{
    private readonly IAdmitCardService _admitCardService;
    private readonly IStudentService _studentService;

    public AdmitCardController(
        IAdmitCardService admitCardService,
        IStudentService studentService)
    {
        _admitCardService = admitCardService;
        _studentService = studentService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("AdmitCard.Generate")]
    public async Task<IActionResult> Generate(int examId)
    {
        try
        {
            await _admitCardService.GenerateAdmitCardsAsync(examId);
            TempData["SuccessMessage"] = "Admit cards generated successfully.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction("Details", "Exam", new { id = examId });
    }

    [HttpGet]
    [RequirePermission("AdmitCard.View")]
    public async Task<IActionResult> View(int examId, int studentId)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        if (User.IsInRole("Student"))
        {
            var student = await _studentService.GetByUserIdAsync(currentUserId, default);
            if (student == null || student.Id != studentId)
                return Forbid();
        }

        try
        {
            var vm = await _admitCardService.GetAdmitCardAsync(examId, studentId);
            return View(vm);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Admit card not found.");
        }
    }

    [HttpGet]
    [RequirePermission("AdmitCard.View")]
    public async Task<IActionResult> DownloadPdf(int examId, int studentId)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        if (User.IsInRole("Student"))
        {
            var student = await _studentService.GetByUserIdAsync(currentUserId, default);
            if (student == null || student.Id != studentId)
                return Forbid();
        }

        try
        {
            var bytes = await _admitCardService.GenerateAdmitCardPdfAsync(examId, studentId);
            return File(bytes, "application/pdf", $"AdmitCard_{studentId}.pdf");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Admit card not found.");
        }
    }

    [HttpGet]
    [RequirePermission("AdmitCard.View")]
    public async Task<IActionResult> BulkDownload(int examId)
    {
        try
        {
            var bytes = await _admitCardService.GenerateBulkAdmitCardsPdfAsync(examId, null);
            return File(bytes, "application/pdf", $"AdmitCards_{examId}.pdf");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Details", "Exam", new { id = examId });
        }
    }

    [HttpGet]
    [RequirePermission("AdmitCard.View")]
    public async Task<IActionResult> BulkDownloadBySection(int examId, int sectionId)
    {
        try
        {
            var bytes = await _admitCardService.GenerateBulkAdmitCardsPdfAsync(examId, sectionId);
            return File(bytes, "application/pdf", $"AdmitCards_{examId}_Section{sectionId}.pdf");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Details", "Exam", new { id = examId });
        }
    }

    [HttpGet]
    [RequirePermission("AdmitCard.View")]
    public async Task<IActionResult> MyAdmitCard(int examId)
    {
        var currentUserId = int.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier) ?? "0");
        var student = await _studentService.GetByUserIdAsync(currentUserId, default);

        if (student == null)
        {
            TempData["ErrorMessage"] = "Student profile not found.";
            return RedirectToAction("Index", "Home");
        }

        try
        {
            var vm = await _admitCardService.GetAdmitCardAsync(examId, student.Id);
            return View("MyAdmitCard", vm);
        }
        catch (KeyNotFoundException)
        {
            TempData["ErrorMessage"] = "Admit card not found for this exam.";
            return RedirectToAction("Index", "Home");
        }
    }
}
