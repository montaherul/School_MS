using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Employee;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Constants;

namespace SchoolManagementSystem.Controllers.Employee;

[Authorize]
public class HolidayController : Controller
{
    private readonly IHolidayRepository _holidayRepo;
    private readonly IUnitOfWork _uow;

    public HolidayController(IHolidayRepository holidayRepo, IUnitOfWork uow)
    {
        _holidayRepo = holidayRepo;
        _uow = uow;
    }

    [RequirePermission(Permissions.Settings.View)]
    public async Task<IActionResult> Index()
    {
        var holidays = await _holidayRepo.Query().OrderByDescending(h => h.StartDate).ToListAsync();
        return View(holidays);
    }

    [HttpPost]
    [RequirePermission(Permissions.Settings.Update)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Holiday model)
    {
        if (ModelState.IsValid)
        {
            await _holidayRepo.AddAsync(model);
            await _uow.SaveChangesAsync();
            TempData["SuccessMessage"] = "Holiday added successfully.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [RequirePermission(Permissions.Settings.Update)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id)
    {
        var h = await _holidayRepo.FirstOrDefaultAsync(x => x.Id == id);
        if (h != null)
        {
            _holidayRepo.Remove(h);
            await _uow.SaveChangesAsync();
            TempData["SuccessMessage"] = "Holiday deleted.";
        }
        return RedirectToAction(nameof(Index));
    }
}
