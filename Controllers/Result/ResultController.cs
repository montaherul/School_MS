using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
[RequirePermission("Result.View")]
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
    [ValidateAntiForgeryToken]
    public IActionResult Save()
        => RedirectToAction("Index", "Marks");

    [HttpGet]
    public IActionResult Delete(int id)
        => RedirectToAction("Index", "Marks");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
        => RedirectToAction("Index", "Marks");
}
