using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SchoolManagementSystem.Controllers.Result;

/// <summary>
/// Redirects to MarksController - the sole mark entry controller.
/// Generic CRUD on MarkEntry bypasses validation; use MarksController instead.
/// </summary>
[Authorize]
public class ResultController : Controller
{
    [HttpGet]
    public IActionResult Index()
        => RedirectToAction("Index", "Marks");

    [HttpGet]
    public IActionResult Details(int id)
        => RedirectToAction("Index", "Marks");

    [HttpGet]
    public IActionResult CreateEdit(int? id = null)
        => RedirectToAction("Index", "Marks");

    [HttpPost]
    public IActionResult Save()
        => RedirectToAction("Index", "Marks");

    [HttpGet]
    public IActionResult Delete(int id)
        => RedirectToAction("Index", "Marks");

    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
        => RedirectToAction("Index", "Marks");
}
