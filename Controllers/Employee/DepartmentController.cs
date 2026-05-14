using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.ViewModels.Employee;
using SchoolManagementSystem.Services.Interfaces.Employee;

namespace SchoolManagementSystem.Controllers.Employee;

public class DepartmentController : Controller
{
    private readonly IDepartmentService _departmentService;

    public DepartmentController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _departmentService.GetAllAsync();
        return View(list);
    }

    public IActionResult Create() => View("CreateEdit", new DepartmentViewModel());


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DepartmentViewModel model)
    {
        if (ModelState.IsValid)
        {
            await _departmentService.CreateAsync(model, User.Identity?.Name ?? "system");
            return RedirectToAction(nameof(Index));
        }
        return View("CreateEdit", model);
    }

    public async Task<IActionResult> Edit(long id)
    {
        var model = await _departmentService.GetByIdAsync(id);
        if (model == null) return NotFound();
        return View("CreateEdit", model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(DepartmentViewModel model)
    {
        if (ModelState.IsValid)
        {
            await _departmentService.UpdateAsync(model, User.Identity?.Name ?? "system");
            return RedirectToAction(nameof(Index));
        }
        return View("CreateEdit", model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            await _departmentService.DeleteAsync(id, User.Identity?.Name ?? "system");
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}
