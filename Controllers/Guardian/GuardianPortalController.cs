using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Service.Interfaces.Dashboard;
using SchoolManagementSystem.Models.ViewModels.Dashboard;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Guardian;

[Authorize(Roles = "Guardian")]
[Route("Guardian/Portal")]
public class GuardianPortalController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<GuardianPortalController> _logger;

    public GuardianPortalController(IDashboardService dashboardService, ILogger<GuardianPortalController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    [HttpGet("Dashboard")]
    public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            int userId = int.Parse(userIdClaim.Value);
            var model = await _dashboardService.GetGuardianDashboardAsync(userId, cancellationToken);
            
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading guardian dashboard");
            return View("Error");
        }
    }
}
