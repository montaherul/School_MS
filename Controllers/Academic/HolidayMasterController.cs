using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Services.Implementations.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
public class HolidayMasterController : Controller
{
    private readonly IHolidayMasterService _service;
    private readonly ICalendarGenerationService _calendarGenService;

    public HolidayMasterController(IHolidayMasterService service, ICalendarGenerationService calendarGenService)
    {
        _service = service;
        _calendarGenService = calendarGenService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetList(int page = 1, int pageSize = 20, string? search = null, string? type = null, string? religion = null, CancellationToken ct = default)
    {
        var result = await _service.GetPagedAsync(page, pageSize, search, type, religion, ct);
        return Json(new { data = result.Items, total = result.TotalItems, page, pageSize });
    }

    [HttpGet]
    [RequirePermission("Calendar.Create")]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var types = new[] { "National", "Religious", "Cultural", "Government", "Weekly Off", "Other" };
        ViewBag.HolidayTypes = types;
        return View(new HolidayMasterUpsertDto { CountryCode = "BD", IsRecurring = true, IsActive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Calendar.Create")]
    public async Task<IActionResult> Create(HolidayMasterUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.HolidayTypes = new[] { "National", "Religious", "Cultural", "Government", "Weekly Off", "Other" };
            return View(dto);
        }

        var id = await _service.CreateAsync(dto, User.Identity?.Name ?? "system", ct);
        TempData["Success"] = "Holiday created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("Calendar.Edit")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        if (dto == null) return NotFound();

        ViewBag.HolidayTypes = new[] { "National", "Religious", "Cultural", "Government", "Weekly Off", "Other" };
        return View(new HolidayMasterUpsertDto
        {
            Id = dto.Id,
            Name = dto.Name,
            NameBn = dto.NameBn,
            HolidayType = dto.HolidayType,
            HolidayDate = dto.HolidayDate,
            IsRecurring = dto.IsRecurring,
            Religion = dto.Religion,
            CountryCode = dto.CountryCode,
            Description = dto.Description,
            DisplayOrder = dto.DisplayOrder,
            IsActive = dto.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Calendar.Edit")]
    public async Task<IActionResult> Edit(HolidayMasterUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.HolidayTypes = new[] { "National", "Religious", "Cultural", "Government", "Weekly Off", "Other" };
            return View(dto);
        }

        await _service.UpdateAsync(dto, User.Identity?.Name ?? "system", ct);
        TempData["Success"] = "Holiday updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Calendar.Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        try
        {
            await _service.DeleteAsync(id, User.Identity?.Name ?? "system", ct);
            return Json(new { success = true, message = "Holiday deleted." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Calendar.Edit")]
    public async Task<IActionResult> Activate(int id, CancellationToken ct)
    {
        await _service.ActivateAsync(id, User.Identity?.Name ?? "system", ct);
        return Json(new { success = true, message = "Holiday activated." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Calendar.Edit")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken ct)
    {
        await _service.DeactivateAsync(id, User.Identity?.Name ?? "system", ct);
        return Json(new { success = true, message = "Holiday deactivated." });
    }

    [HttpGet]
    [RequirePermission("Calendar.Export")]
    public async Task<IActionResult> Export(CancellationToken ct)
    {
        var bytes = await _service.ExportAsync(ct);
        return File(bytes, "text/csv", $"holidays_{DateTime.Today:yyyyMMdd}.csv");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Calendar.Generate")]
    public async Task<IActionResult> GenerateBangladeshHolidays(int year, CancellationToken ct)
    {
        var holidays = HolidayProvider.GetAllAnnualHolidays(year);
        var dtoList = holidays.Select(h => new HolidayMasterUpsertDto
        {
            Name = h.Name,
            NameBn = h.NameBn,
            HolidayType = h.HolidayType,
            HolidayDate = h.HolidayDate,
            IsRecurring = h.IsRecurring,
            Religion = h.Religion,
            CountryCode = "BD",
            Description = h.Description,
            DisplayOrder = h.DisplayOrder,
            IsActive = true
        }).ToList();

        var imported = await _service.ImportAsync(dtoList, User.Identity?.Name ?? "system", ct);
        TempData["Success"] = $"Generated {imported} Bangladesh holidays for {year}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetTypes(CancellationToken ct)
    {
        var types = new[] { "National", "Religious", "Cultural", "Government", "Weekly Off", "Other" };
        return Json(types);
    }
}
