using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Service.Interfaces.Dashboard;
using SchoolManagementSystem.Repositories.Interfaces.Website;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Dashboard;

[Authorize]
public class DashboardController : Controller
{
    private readonly IDashboardService _service;
    private readonly ISchoolSettingRepository _settingRepo;

    public DashboardController(IDashboardService service, ISchoolSettingRepository settingRepo)
    {
        _service = service;
        _settingRepo = settingRepo;
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

        if (User.IsInRole("Guardian") || User.IsInRole("Parent"))
        {
            var settings = await _settingRepo.GetCurrentSettingsAsync();
            if (settings?.EnableGuardianPortal == true)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdStr, out var userId))
                {
                    try
                    {
                        var viewName = User.IsInRole("Parent") ? "ParentIndex" : "GuardianIndex";
                        var parentData = await _service.GetGuardianDashboardAsync(userId);
                        return View(viewName, parentData);
                    }
                    catch (InvalidOperationException)
                    {
                        // Guardian profile not found — fall through to default dashboard
                    }
                }
            }
            // Guardian portal disabled or profile not found — fall through to default dashboard
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

        if (User.IsInRole("Librarian"))
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out var userId))
            {
                var librarianData = await _service.GetLibrarianDashboardAsync();
                return View("LibrarianIndex", librarianData);
            }
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
