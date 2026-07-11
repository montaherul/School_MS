using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.Academic;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
[Route("Academic/[controller]")]
public class AcademicDashboardController : Controller
{
    private readonly IAcademicDashboardService _service;

    public AcademicDashboardController(IAcademicDashboardService service)
    {
        _service = service;
    }

    [RequirePermission("Dashboard.View")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var dto = await _service.GetDashboardAsync(cancellationToken);
        return View("~/Views/Academic/AcademicDashboard/Index.cshtml", dto);
    }
}
