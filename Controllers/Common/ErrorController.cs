using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SchoolManagementSystem.Controllers.Common;

/// <summary>
/// Handles friendly error pages for 404, 403, and 500 scenarios.
/// </summary>
[AllowAnonymous]
[Route("Error")]
public class ErrorController : Controller
{
    [Route("NotFound")]
    public IActionResult NotFound404()
    {
        ViewData["Title"]   = "Page Not Found";
        ViewData["Code"]    = 404;
        ViewData["Message"] = "The page or resource you are looking for does not exist.";
        return View("Error");
    }

    [Route("Forbidden")]
    public IActionResult Forbidden403()
    {
        ViewData["Title"]   = "Access Denied";
        ViewData["Code"]    = 403;
        ViewData["Message"] = "You do not have permission to access this resource.";
        return View("Error");
    }

    [Route("Server")]
    public IActionResult ServerError500()
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        ViewData["Title"]   = "Server Error";
        ViewData["Code"]    = 500;
        ViewData["Message"] = "An unexpected error occurred. Our team has been notified.";
        return View("Error");
    }
}
