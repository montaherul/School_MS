using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class InventoryBillingController : Controller
{
    private const string ViewPath = "~/Views/Fee/InventoryBilling";
    private readonly IBillingService _billing;
    private readonly IFeeInvoiceService _invoiceService;
    private readonly IFeeSecurityService _security;
    private const string CategoryName = "Inventory";

    public InventoryBillingController(IBillingService billing, IFeeInvoiceService invoiceService, IFeeSecurityService security)
    {
        _billing = billing;
        _invoiceService = invoiceService;
        _security = security;
    }

    [RequirePermission("InventoryBilling.Read")]
    public IActionResult Index() => View($"{ViewPath}/Index.cshtml");

    [HttpGet]
    [RequirePermission("InventoryBilling.Read")]
    public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? search = null)
    {
        var result = await _invoiceService.GetPagedAsync(page, pageSize, search);
        var filtered = result.Items.Where(x => x.Remarks?.StartsWith("Inventory billing") == true).ToList();
        return Json(new { data = filtered, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    [RequirePermission("InventoryBilling.Create")]
    public async Task<IActionResult> Create()
    {
        var catInfo = await _billing.GetCategoryInfoAsync(CategoryName);
        ViewBag.CategoryId = catInfo.CategoryId;
        ViewBag.FeeTypes = catInfo.FeeTypes;
        return View($"{ViewPath}/Create.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("InventoryBilling.Create")]
    public async Task<IActionResult> Create(int studentId, List<BillingItemDto> items, DateOnly dueDate, string? remarks)
    {
        if (!_security.Can(User, "InventoryBilling.Create"))
            return Forbid();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        try
        {
            var invoiceNo = await _billing.CreateBillingInvoiceAsync(studentId, CategoryName, items, dueDate, remarks, userId);
            TempData["SuccessMessage"] = $"Inventory invoice {invoiceNo} created successfully.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Create));
    }

    [HttpGet]
    [RequirePermission("InventoryBilling.Read")]
    public async Task<IActionResult> GetStudents(string? term)
    {
        var students = await _billing.SearchStudentsAsync(term);
        return Json(students);
    }
}
