using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.Fees;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class FeeDashboardController : Controller
{
    private readonly IFeeDashboardService _service;
    private readonly IFeeSecurityService _security;
    public FeeDashboardController(IFeeDashboardService service, IFeeSecurityService security) { _service = service; _security = security; }

    [RequirePermission("FeeDashboard.Read")]
    public async Task<IActionResult> Index(int? academicYearId = null)
    {
        var data = await _service.GetDashboardDataAsync(academicYearId);
        return View(data);
    }

}
