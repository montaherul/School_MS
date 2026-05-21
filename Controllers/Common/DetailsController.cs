using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Common;

[Authorize]
public class DetailsController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        if (User.IsInRole("Student"))
        {
            return RedirectToAction("Details", "Student");
        }
        
        if (User.IsInRole("Teacher") || 
            User.IsInRole("Senior Lecturer") || 
            User.IsInRole("Lecturer") || 
            User.IsInRole("Assistant Head") || 
            User.IsInRole("Principal"))
        {
            return RedirectToAction("Details", "Teacher");
        }

        // If Admin or other roles, send to Dashboard or Student List
        return RedirectToAction("Index", "Dashboard");
    }
}

