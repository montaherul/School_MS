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
        var data = await _service.GetDashboardAsync();
        return View(data);
    }
}
