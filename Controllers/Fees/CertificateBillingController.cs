using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class CertificateBillingController : Controller
{
    private readonly IFeeInvoiceService _invoiceService;
    private readonly IFeeInvoiceItemService _itemService;
    private readonly IFeeSecurityService _security;
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogService _audit;
    private const string CertificateCategoryName = "Certificate";

    public CertificateBillingController(
        IFeeInvoiceService invoiceService,
        IFeeInvoiceItemService itemService,
        IFeeSecurityService security,
        IUnitOfWork uow,
        IAuditLogService audit)
    {
        _invoiceService = invoiceService;
        _itemService = itemService;
        _security = security;
        _uow = uow;
        _audit = audit;
    }

    [RequirePermission("CertificateBilling.Read")]
    public IActionResult Index() => View();

    [HttpGet]
    [RequirePermission("CertificateBilling.Read")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null)
    {
        var cat = await _uow.Repository<FeeCategory>().FirstOrDefaultAsync(x => x.Name == CertificateCategoryName && !x.IsDeleted);
        var result = await _invoiceService.GetPagedAsync(page, size, search);
        var invoiceIds = result.Items.Select(x => x.Id).ToList();
        var items = await _uow.Repository<FeeInvoiceItem>().ListAsync(i =>
            invoiceIds.Contains(i.FeeInvoiceId) && !i.IsDeleted);
        var catItems = items.Where(i => i.FeeCategoryId == cat?.Id).Select(i => i.FeeInvoiceId).Distinct().ToHashSet();
        var filtered = result.Items.Where(x => catItems.Contains(x.Id)).ToList();
        return Json(new { data = filtered, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    [RequirePermission("CertificateBilling.Create")]
    public async Task<IActionResult> Create()
    {
        var cat = await _uow.Repository<FeeCategory>().FirstOrDefaultAsync(x => x.Name == CertificateCategoryName && !x.IsDeleted);
        ViewBag.CategoryId = cat?.Id;
        var types = await _uow.Repository<FeeType>().ListAsync(x =>
            cat != null && x.Name != null && !x.IsDeleted && x.IsActive);
        ViewBag.FeeTypes = types.OrderBy(t => t.DisplayOrder).ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("CertificateBilling.Create")]
    public async Task<IActionResult> Create(int studentId, List<CertificateItemDto> items, DateOnly dueDate, string? remarks)
    {
        if (!_security.Can(User, "CertificateBilling.Create"))
            return Forbid();

        if (studentId <= 0 || items == null || items.Count == 0 || !items.Any(i => i.Amount > 0))
        {
            TempData["ErrorMessage"] = "Select a student and at least one certificate with a valid amount.";
            return RedirectToAction(nameof(Create));
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>()
            .FirstOrDefaultAsync(x => x.Id == studentId && !x.IsDeleted);
        if (student == null) return NotFound();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var invoiceNo = $"INV-CRT-{today:yyyyMMdd}-{studentId:D6}-{DateTime.UtcNow:HHmmss}";
        var cat = await _uow.Repository<FeeCategory>().FirstOrDefaultAsync(x => x.Name == CertificateCategoryName && !x.IsDeleted);
        var total = items.Where(i => i.Amount > 0).Sum(i => i.Amount);

        var invoice = new FeeInvoice
        {
            InvoiceNo = invoiceNo,
            StudentId = studentId,
            DueDate = dueDate,
            TotalAmount = total,
            PaidAmount = 0,
            DiscountAmount = 0,
            LateFee = 0,
            Status = PaymentStatus.Issued,
            Remarks = remarks ?? $"Certificate billing — {items.Count(i => i.Amount > 0)} item(s)"
        };

        var invoiceId = await _invoiceService.CreateAsync(invoice, userId);

        foreach (var item in items.Where(i => i.Amount > 0))
        {
            var itemDto = new FeeInvoiceItemUpsertDto
            {
                FeeInvoiceId = invoiceId,
                FeeCategoryId = cat?.Id,
                Description = item.Description,
                Amount = item.Amount,
                DiscountAmount = 0,
                NetAmount = item.Amount
            };
            await _itemService.CreateAsync(itemDto, userId);
        }

        await _audit.LogAsync("CertificateBilling", "Create",
            $"Certificate invoice {invoiceNo} created for student {studentId}, total {total}", userId);

        TempData["SuccessMessage"] = $"Certificate invoice {invoiceNo} created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("CertificateBilling.Read")]
    public async Task<IActionResult> GetStudents(string? term)
    {
        var query = _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .Where(x => !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(term))
        {
            var lower = term.ToLower();
            query = query.Where(x => x.FullName.ToLower().Contains(lower)
                || x.StudentNo.ToLower().Contains(lower));
        }
        var list = await query.Take(20).Select(x => new { x.Id, Name = x.FullName, StudentNo = x.StudentNo }).ToListAsync();
        return Json(list);
    }
}

public class CertificateItemDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
