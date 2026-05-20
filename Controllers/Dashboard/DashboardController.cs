using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Service.Interfaces.Dashboard;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Dashboard;

[Authorize]
public class DashboardController : Controller
{

    private readonly SchoolManagementSystem.Services.Interfaces.Dashboard.IDashboardResolverService _resolver;

    public DashboardController(SchoolManagementSystem.Services.Interfaces.Dashboard.IDashboardResolverService resolver)
    {
        _resolver = resolver;

    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
    {
        _service = service;

    }

    public async Task<IActionResult> Index()
    {

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdStr, out var userId)) return RedirectToAction("Login", "Auth");

        var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

        var viewName = await _resolver.GetDashboardViewNameAsync(roles);
        var model = await _resolver.GetDashboardModelAsync(userId, roles);

        return View(viewName, model);

        if (User.IsInRole("Student"))
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out var userId))
            {
                var studentData = await _service.GetStudentDashboardAsync(userId);
                return View("StudentIndex", studentData);
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

        var data = await _service.GetDashboardAsync();
        return View(data);

    }
}
