using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Service.Interfaces.Dashboard;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Dashboard;

[Authorize]
public class DashboardController : Controller
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        if (User.IsInRole("Student"))
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out var userId))
            {
                var studentData = await _service.GetStudentDashboardAsync(userId);
                return View("StudentIndex", studentData);
            }
        }

        if (User.IsInRole("Guardian"))
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out var userId))
            {
                var guardianData = await _service.GetGuardianDashboardAsync(userId);
                return View("GuardianIndex", guardianData);
            }
        }

        if (User.IsInRole("Teacher") || User.IsInRole("Senior Lecturer") || User.IsInRole("Lecturer") || 
            User.IsInRole("Assistant Head") || User.IsInRole("Principal") || User.IsInRole("Office Staff"))
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out var userId))
            {
                var teacherData = await _service.GetTeacherDashboardAsync(userId);
                return View("TeacherIndex", teacherData);
            }
        }

        if (User.IsInRole("Exam Controller"))
        {
            var examControllerData = await _service.GetExamControllerDashboardAsync();
            return View("ExamControllerIndex", examControllerData);
        }

        if (User.IsInRole("Accountant"))
        {
            var accountantData = await _service.GetAccountantDashboardAsync();
            return View("AccountantIndex", accountantData);
        }

        var data = await _service.GetDashboardAsync();
        return View(data);
    }
}
