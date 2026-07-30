using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.ViewModels.Accounting;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.Accounting;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Accounting;

[Authorize]
[Route("Accounting/[controller]")]
public class JournalEntryController : Controller
{
    private readonly IJournalEntryService _service;
    private readonly IChartOfAccountService _accountService;
    private readonly IFinancialPeriodService _periodService;

    public JournalEntryController(IJournalEntryService service, IChartOfAccountService accountService,
        IFinancialPeriodService periodService)
    {
        _service = service;
        _accountService = accountService;
        _periodService = periodService;
    }

    [HttpGet("")]
    [RequirePermission("Accounting.View")]
    public IActionResult Index() => View("~/Views/Accounting/JournalEntry/Index.cshtml");

    [HttpGet("GetList")]
    [RequirePermission("Accounting.View")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null, int? entryType = null)
    {
        var result = await _service.GetPagedAsync(page, size, search, entryType);
        return Json(new { data = result.Items, last_page = (int)Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet("Details/{id:int}")]
    [RequirePermission("Accounting.View")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _service.GetDetailAsync(id);
        if (dto == null) return NotFound();
        return View("~/Views/Accounting/JournalEntry/Details.cshtml", dto);
    }

    [HttpGet("CreateEdit/{id?}")]
    [RequirePermission("Accounting.View")]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        ViewBag.EntryTypes = Enum.GetValues<JournalEntryType>()
            .Select(e => new SelectListItem { Value = ((int)e).ToString(), Text = e.ToString() });
        ViewBag.Accounts = await _accountService.GetActiveAccountSelectListAsync();
        ViewBag.Periods = await _periodService.GetPeriodSelectListAsync();

        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            return View("~/Views/Accounting/JournalEntry/CreateEdit.cshtml", new JournalEntryViewModel
            {
                Id = dto.Id, JournalNo = dto.JournalNo, EntryDate = dto.EntryDate,
                EntryType = dto.EntryType, Description = dto.Description,
                FinancialPeriodId = dto.FinancialPeriodId, Lines = dto.Lines
            });
        }

        var journalNo = await _service.GenerateJournalNoAsync(DateTime.Today, default);
        return View("~/Views/Accounting/JournalEntry/CreateEdit.cshtml", new JournalEntryViewModel { JournalNo = journalNo, EntryDate = DateTime.Today, EntryType = JournalEntryType.Manual });
    }

    [HttpPost("CreateEdit")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Accounting.Post")]
    public async Task<IActionResult> CreateEdit(JournalEntryViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.EntryTypes = Enum.GetValues<JournalEntryType>()
                .Select(e => new SelectListItem { Value = ((int)e).ToString(), Text = e.ToString() });
            ViewBag.Accounts = await _accountService.GetActiveAccountSelectListAsync();
            ViewBag.Periods = await _periodService.GetPeriodSelectListAsync();
            return View("~/Views/Accounting/JournalEntry/CreateEdit.cshtml", vm);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (vm.IsEditMode)
        { await _service.UpdateAsync(vm, userId); TempData["SuccessMessage"] = "Journal entry updated."; }
        else
        { await _service.CreateAsync(vm, userId); TempData["SuccessMessage"] = "Journal entry created."; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Post")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Accounting.Post")]
    public async Task<IActionResult> Post(int id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _service.PostAsync(id, userId);
            TempData["SuccessMessage"] = "Journal entry posted to General Ledger.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Accounting.Post")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _service.DeleteAsync(id, userId);
            TempData["SuccessMessage"] = "Journal entry deleted.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Cannot delete journal entry: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}
