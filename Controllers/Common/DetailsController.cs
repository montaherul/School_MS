using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SchoolManagementSystem.Constants;

namespace SchoolManagementSystem.Controllers.Common;

[Authorize]
public class DetailsController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        if (User.IsInRole(Roles.Student))
        {
            return RedirectToAction("Details", "Student");
        }
        
        if (User.IsInRole(Roles.Teacher) || User.IsInRole(Roles.Principal))
        {
            return RedirectToAction("Details", "Teacher");
        }

        // If Admin or other roles, send to Dashboard or Student List
        return RedirectToAction("Index", "Dashboard");
    }
}

