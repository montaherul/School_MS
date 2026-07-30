using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class BillingService : IBillingService
{
    private readonly IUnitOfWork _uow;
    private readonly IFeeInvoiceService _invoiceService;
    private readonly IFeeInvoiceItemService _itemService;
    private readonly IAuditLogService _audit;

    public BillingService(IUnitOfWork uow, IFeeInvoiceService invoiceService, IFeeInvoiceItemService itemService, IAuditLogService audit)
    {
        _uow = uow;
        _invoiceService = invoiceService;
        _itemService = itemService;
        _audit = audit;
    }

    public async Task<BillingCategoryInfoDto> GetCategoryInfoAsync(string categoryName)
    {
        var cat = await _uow.Repository<FeeCategory>().FirstOrDefaultAsync(x => x.Name == categoryName && !x.IsDeleted);
        var types = cat != null
            ? await _uow.Repository<FeeType>().ListAsync(x => x.Name != null && !x.IsDeleted && x.IsActive)
            : [];
        return new BillingCategoryInfoDto
        {
            CategoryId = cat?.Id,
            FeeTypes = types.OrderBy(t => t.DisplayOrder).Select(t => new BillingFeeTypeDto
            {
                Id = t.Id,
                Name = t.Name,
                DisplayOrder = t.DisplayOrder
            }).ToList()
        };
    }

    public async Task<List<BillingStudentDto>> SearchStudentsAsync(string? term)
    {
        var query = _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .Where(x => !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(term))
        {
            var lower = term.ToLower();
            query = query.Where(x => x.FullName.ToLower().Contains(lower)
                || x.StudentNo.ToLower().Contains(lower));
        }
        return await query.Take(20).Select(x => new BillingStudentDto
        {
            Id = x.Id,
            Name = x.FullName,
            StudentNo = x.StudentNo
        }).ToListAsync();
    }

    public async Task<string> CreateBillingInvoiceAsync(int studentId, string categoryName, List<BillingItemDto> items, DateOnly dueDate, string? remarks, string createdBy)
    {
        if (studentId <= 0 || items == null || items.Count == 0 || !items.Any(i => i.Amount > 0))
            throw new InvalidOperationException("Select a student and at least one item with a valid amount.");

        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>()
            .FirstOrDefaultAsync(x => x.Id == studentId && !x.IsDeleted);
        if (student == null) throw new InvalidOperationException("Student not found.");

        var cat = await _uow.Repository<FeeCategory>().FirstOrDefaultAsync(x => x.Name == categoryName && !x.IsDeleted);
        var categoryPrefix = categoryName switch
        {
            "Certificate" => "CRT",
            "Inventory" => "INV",
            "Other Charges" => "OTH",
            _ => "BILL"
        };

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var invoiceNo = $"INV-{categoryPrefix}-{today:yyyyMMdd}-{studentId:D6}-{DateTime.UtcNow:HHmmss}";
        var total = items.Where(i => i.Amount > 0).Sum(i => i.Amount);

        var dto = new FeeInvoiceUpsertDto
        {
            InvoiceNo = invoiceNo,
            StudentId = studentId,
            DueDate = dueDate,
            TotalAmount = total,
            PaidAmount = 0,
            DiscountAmount = 0,
            LateFee = 0,
            Status = (int)PaymentStatus.Issued,
            Remarks = remarks ?? $"{categoryName} billing — {items.Count(i => i.Amount > 0)} item(s)"
        };

        var invoiceId = await _invoiceService.CreateAsync(dto, createdBy);

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
            await _itemService.CreateAsync(itemDto, createdBy);
        }

        await _audit.LogAsync($"{categoryName}Billing", "Create",
            $"{categoryName} invoice {invoiceNo} created for student {studentId}, total {total}", createdBy);

        return invoiceNo;
    }
}
