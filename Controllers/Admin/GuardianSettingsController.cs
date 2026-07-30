using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;

namespace SchoolManagementSystem.Controllers.Admin;

[Authorize]
[Route("Admin/GuardianSettings")]
public class GuardianSettingsController : Controller
{
    [HttpGet("")]
    [RequirePermission("Settings.Manage")]
    public IActionResult Index()
    {
        return RedirectToAction("Index", "PortalSettings");
    }
}
