using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Controllers.Common;

public class ErrorController : Controller
{
    public IActionResult Index(int? statusCode = null)
    {
        var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        if (feature?.Error != null)
        {
            var logger = HttpContext.RequestServices.GetRequiredService<ILogger<ErrorController>>();
            logger.LogError(feature.Error, "Unhandled exception caught by ErrorController");
        }

        var code = statusCode ?? (feature?.Error != null ? 500 : 0);
        if (code > 0)
            Response.StatusCode = code;

        var model = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };

        return View("Error", model);
    }
}
