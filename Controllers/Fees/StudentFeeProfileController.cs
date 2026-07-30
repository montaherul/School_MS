using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
[Route("StudentFeeProfile")]
public class StudentFeeProfileController : Controller
{
    private readonly IStudentFeeProfileService _profileService;
    private readonly IFeeSecurityService _security;

    public StudentFeeProfileController(
        IStudentFeeProfileService profileService,
        IFeeSecurityService security)
    {
        _profileService = profileService;
        _security = security;
    }

    [RequirePermission("StudentFeeProfile.View")]
    public IActionResult Index()
    {
        return View("~/Views/Fee/StudentFeeProfile/Index.cshtml");
    }

    [HttpGet("{studentId}")]
    [RequirePermission("StudentFeeProfile.View")]
    public async Task<IActionResult> Profile(int studentId, int? academicYearId = null)
    {
        if (!_security.CanAccessStudentData(User, studentId))
            return Forbid();

        var profile = await _profileService.GetProfileAsync(studentId, academicYearId);
        if (profile.StudentId == 0)
            return NotFound();

        return View("~/Views/Fee/StudentFeeProfile/Profile.cshtml", profile);
    }

    [HttpPost("Search")]
    [RequirePermission("StudentFeeProfile.View")]
    public async Task<IActionResult> Search(string? term = null)
    {
        var students = await _profileService.SearchStudentsAsync(term);
        return Json(students);
    }

    [HttpGet("Export/{studentId}")]
    [RequirePermission("StudentFeeProfile.View")]
    public async Task<IActionResult> Export(int studentId, int? academicYearId = null)
    {
        if (!_security.CanAccessStudentData(User, studentId))
            return Forbid();

        var bytes = await _profileService.GenerateProfilePdfAsync(studentId, academicYearId);
        if (bytes.Length == 0) return NotFound();
        return File(bytes, "application/pdf", $"FeeProfile_{studentId}.pdf");
    }
}
