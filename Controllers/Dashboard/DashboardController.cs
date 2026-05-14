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
    }

    public async Task<IActionResult> Index()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdStr, out var userId)) return RedirectToAction("Login", "Auth");

        var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray();

        var viewName = await _resolver.GetDashboardViewNameAsync(roles);
        var model = await _resolver.GetDashboardModelAsync(userId, roles);

        return View(viewName, model);
    }
}
