using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.ViewModels.Employee;
using SchoolManagementSystem.Services.Interfaces.Employee;

namespace SchoolManagementSystem.Controllers.Employee;

public class DesignationController : Controller
{
    private readonly IDesignationService _designationService;

    public DesignationController(IDesignationService designationService)
    {
        _designationService = designationService;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _designationService.GetAllAsync();
        return View(list);
    }

    public IActionResult Create() => View("CreateEdit", new DesignationViewModel());


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DesignationViewModel model)
    {
        if (ModelState.IsValid)
        {
            await _designationService.CreateAsync(model, User.Identity?.Name ?? "system");
            return RedirectToAction(nameof(Index));
        }
        return View("CreateEdit", model);
    }


    public async Task<IActionResult> Edit(long id)
    {
        var model = await _designationService.GetByIdAsync(id);
        if (model == null) return NotFound();
        return View("CreateEdit", model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(DesignationViewModel model)
    {
        if (ModelState.IsValid)
        {
            await _designationService.UpdateAsync(model, User.Identity?.Name ?? "system");
            return RedirectToAction(nameof(Index));
        }
        return View("CreateEdit", model);
    }


    [HttpPost]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            await _designationService.DeleteAsync(id, User.Identity?.Name ?? "system");
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}
