using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.Fees;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class FeeDashboardController : Controller
{
    private const string ViewPath = "~/Views/Fee/FeeDashboard";
    private readonly IFeeDashboardService _service;
    private readonly IEnhancedFeeDashboardService _enhancedService;
    private readonly IFeeSecurityService _security;
    public FeeDashboardController(IFeeDashboardService service, IEnhancedFeeDashboardService enhancedService, IFeeSecurityService security) { _service = service; _enhancedService = enhancedService; _security = security; }

    [RequirePermission("FeeDashboard.Read")]
    public async Task<IActionResult> Index(int? academicYearId = null)
    {
        var data = await _enhancedService.GetDashboardAsync(academicYearId);
        return View($"{ViewPath}/Index.cshtml", data);
    }

}
