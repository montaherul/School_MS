using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.ViewModels.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using System.Security.Claims;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Helpers.Reports;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class FeeStructureController : Controller
{
    private const string ViewPath = "~/Views/Fee/FeeStructure";
    private readonly IFeeStructureService _service;
    private readonly IFeeSecurityService _security;
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IFeeStructureWizardService _wizardService;
    private static readonly JsonSerializerOptions _jsonOpts = new() { PropertyNameCaseInsensitive = true };
    public FeeStructureController(IFeeStructureService service, IFeeSecurityService security, IPdfGenerator pdfGenerator, IFeeStructureWizardService wizardService) { _service = service; _security = security; _pdfGenerator = pdfGenerator; _wizardService = wizardService; }

    private FeeStructureWizardDto? GetWizardState()
    {
        var raw = TempData["WizardState"] as string;
        if (string.IsNullOrEmpty(raw)) return null;
        try { return JsonSerializer.Deserialize<FeeStructureWizardDto>(raw, _jsonOpts); }
        catch { return null; }
    }

    private void SetWizardState(FeeStructureWizardDto state)
    {
        TempData["WizardState"] = JsonSerializer.Serialize(state);
    }

    private void ClearWizardState()
    {
        TempData.Remove("WizardState");
    }

    [RequirePermission("FeeStructures.Read")]
    public IActionResult Index() { return View($"{ViewPath}/Index.cshtml"); }

    [HttpGet]
    [RequirePermission("FeeStructures.Create")]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("FeeStructures.Update")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    [RequirePermission("FeeStructures.Read")]
    public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? search = null, int? schoolClassId = null, int? feeCategoryId = null)
    {
        var result = await _service.GetPagedAsync(page, pageSize, search, schoolClassId, feeCategoryId);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (!_security.Can(User, id.HasValue && id > 0 ? "FeeStructures.Update" : "FeeStructures.Create"))
        {
            return Forbid();
        }

        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            var vm = new FeeStructureViewModel
            {
                Id = dto.Id, SchoolClassId = dto.SchoolClassId, FeeCategoryId = dto.FeeCategoryId,
                AcademicYearId = dto.AcademicYearId, FeeName = dto.FeeName, Description = dto.Description,
                Amount = dto.Amount, IsRecurring = dto.IsRecurring, Frequency = dto.Frequency,
                DueDay = dto.DueDay, IsActive = dto.IsActive
            };
            return View($"{ViewPath}/CreateEdit.cshtml", vm);
        }
        return View($"{ViewPath}/CreateEdit.cshtml", new FeeStructureViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(FeeStructureViewModel vm)
    {
        if (!_security.Can(User, vm.IsEditMode ? "FeeStructures.Update" : "FeeStructures.Create"))
        {
            return Forbid();
        }

        if (!ModelState.IsValid) return View($"{ViewPath}/CreateEdit.cshtml", vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (vm.IsEditMode) { await _service.UpdateAsync(vm, userId); TempData["SuccessMessage"] = "FeeStructure updated successfully."; }
        else { await _service.CreateAsync(vm, userId); TempData["SuccessMessage"] = "FeeStructure created successfully."; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(FeeStructureViewModel vm) => CreateEdit(vm);

    [HttpGet]
    [RequirePermission("FeeStructures.Read")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View($"{ViewPath}/Details.cshtml", new FeeStructureViewModel
        {
            Id = dto.Id, SchoolClassId = dto.SchoolClassId, FeeCategoryId = dto.FeeCategoryId,
            AcademicYearId = dto.AcademicYearId, FeeName = dto.FeeName, Description = dto.Description,
            Amount = dto.Amount, IsRecurring = dto.IsRecurring, Frequency = dto.Frequency,
            DueDay = dto.DueDay, IsActive = dto.IsActive
        });
    }

    [HttpGet]
    [RequirePermission("FeeStructures.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View($"{ViewPath}/Delete.cshtml", new FeeStructureViewModel
        {
            Id = dto.Id, SchoolClassId = dto.SchoolClassId, FeeCategoryId = dto.FeeCategoryId,
            AcademicYearId = dto.AcademicYearId, FeeName = dto.FeeName, Description = dto.Description,
            Amount = dto.Amount, IsRecurring = dto.IsRecurring, Frequency = dto.Frequency,
            DueDay = dto.DueDay, IsActive = dto.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeStructures.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId);
        TempData["SuccessMessage"] = "FeeStructure deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeStructures.Delete")]
    public async Task<IActionResult> Restore(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.RestoreAsync(id, userId);
        TempData["SuccessMessage"] = "Fee structure restored successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("FeeStructures.Read")]
    public async Task<IActionResult> ExportExcel(string? search = null, int? schoolClassId = null, int? feeCategoryId = null)
    {
        var result = await _service.GetPagedAsync(1, 100000, search, schoolClassId, feeCategoryId);
        var bytes = FeeListExporter.ExportToExcel(result.Items.ToList(), "Fee Structures");
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "fee-structures.xlsx");
    }

    [HttpGet]
    [RequirePermission("FeeStructures.Read")]
    public async Task<IActionResult> ExportPdf(string? search = null, int? schoolClassId = null, int? feeCategoryId = null)
    {
        var result = await _service.GetPagedAsync(1, 100000, search, schoolClassId, feeCategoryId);
        var html = FeeListExporter.BuildExportHtml(result.Items.ToList(), "Fee Structures");
        var bytes = _pdfGenerator.GenerateFromHtml(html);
        return File(bytes, "application/pdf", "fee-structures.pdf");
    }

    [HttpGet]
    [RequirePermission("FeeStructures.Create")]
    public async Task<IActionResult> Wizard()
    {
        ClearWizardState();
        var vm = await _wizardService.GetWizardDataAsync(new FeeStructureWizardDto { Step = 1 });
        return View($"{ViewPath}/Wizard.cshtml", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeStructures.Create")]
    public async Task<IActionResult> Wizard([FromForm] FeeStructureWizardDto wizard)
    {
        var state = GetWizardState() ?? new FeeStructureWizardDto();
        state.Step = wizard.Step;
        state.AcademicYearId = wizard.AcademicYearId;
        state.SchoolClassId = wizard.SchoolClassId;
        state.SectionId = wizard.SectionId;
        state.StudentGroupId = wizard.StudentGroupId;
        state.FeeHeads = wizard.FeeHeads ?? [];
        state.Discounts = wizard.Discounts ?? [];
        state.FineRules = wizard.FineRules ?? [];
        state.IsActive = wizard.IsActive;
        SetWizardState(state);

        if (wizard.Step >= 5)
        {
            return Json(new { step = 5 });
        }

        var vm = await _wizardService.GetWizardDataAsync(state);
        return Json(new { step = state.Step, vm });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeStructures.Create")]
    public async Task<IActionResult> SaveWizard()
    {
        var state = GetWizardState();
        if (state == null)
        {
            TempData["ErrorMessage"] = "Wizard state not found. Please start again.";
            return RedirectToAction(nameof(Wizard));
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        var result = await _wizardService.SaveWizardAsync(state, userId);
        ClearWizardState();

        if (result.Success)
        {
            TempData["SuccessMessage"] = $"Fee structure wizard completed. {result.InvoicesGenerated} fee heads, {result.StudentsBilled} discounts/rules created.";
        }
        else
        {
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Failed to save wizard data.";
        }

        return RedirectToAction(nameof(Index));
    }

}
