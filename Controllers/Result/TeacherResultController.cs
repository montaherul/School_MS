using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.Services.Interfaces.Academic;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
[RequirePermission("Marks.View")]
public class TeacherResultController : Controller
{
    private readonly ITeacherResultService _teacherResultService;
    private readonly ITeacherService _teacherService;
    private readonly IAcademicYearService _academicYearService;

    public TeacherResultController(
        ITeacherResultService teacherResultService,
        ITeacherService teacherService,
        IAcademicYearService academicYearService)
    {
        _teacherResultService = teacherResultService;
        _teacherService = teacherService;
        _academicYearService = academicYearService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        if (currentUserId == 0)
            return Unauthorized();

        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        if (teacher == null)
            return Forbid();

        var activeYear = await _academicYearService.GetActiveYearAsync(ct);
        var activeYearId = activeYear?.Id ?? 0;

        var dashboard = await _teacherResultService.GetDashboardAsync(teacher.Id, activeYearId, ct);

        return View("Dashboard", dashboard);
    }
}
