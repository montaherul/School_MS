using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class StudentFeeProfileService : IStudentFeeProfileService
{
    private readonly IStudentFeeAssignmentService _assignmentService;
    private readonly IFeeInvoiceService _invoiceService;
    private readonly IFeeLedgerService _ledgerService;
    private readonly IFeeDiscountService _discountService;
    private readonly IFeeWaiverService _waiverService;
    private readonly IStudentFinanceService _financeService;
    private readonly IUnitOfWork _unitOfWork;

    public StudentFeeProfileService(
        IStudentFeeAssignmentService assignmentService,
        IFeeInvoiceService invoiceService,
        IFeeLedgerService ledgerService,
        IFeeDiscountService discountService,
        IFeeWaiverService waiverService,
        IStudentFinanceService financeService,
        IUnitOfWork unitOfWork)
    {
        _assignmentService = assignmentService;
        _invoiceService = invoiceService;
        _ledgerService = ledgerService;
        _discountService = discountService;
        _waiverService = waiverService;
        _financeService = financeService;
        _unitOfWork = unitOfWork;
    }

    public async Task<StudentFeeProfileDto> GetProfileAsync(int studentId, int? academicYearId = null)
    {
        var student = await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .Include(s => s.Class)
            .Include(s => s.Section)
            .Include(s => s.StudentGuardians).ThenInclude(sg => sg.Guardian)
            .FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted);

        if (student is null)
            return new StudentFeeProfileDto();

        var academicYear = academicYearId.HasValue
            ? await _unitOfWork.Repository<AcademicYear>().FirstOrDefaultAsync(y => y.Id == academicYearId.Value)
            : await _unitOfWork.Repository<AcademicYear>().FirstOrDefaultAsync(y => y.IsCurrent);

        var primaryGuardian = student.StudentGuardians
            .FirstOrDefault(sg => sg.IsPrimaryGuardian)?.Guardian
            ?? student.StudentGuardians.FirstOrDefault()?.Guardian;

        var assignments = await _assignmentService.GetPagedAsync(1, 1000, null, studentId);
        var invoices = await _invoiceService.GetPagedAsync(1, 1000, null, studentId);
        var paymentsResult = await _financeService.GetPaymentsPagedAsync(studentId, 1, 1000, null, default);
        var ledger = await _ledgerService.GetPagedAsync(1, 1000, null, studentId);
        var waivers = await _waiverService.GetPagedAsync(1, 1000, null, studentId);

        var classDiscounts = await _discountService.GetPagedAsync(1, 1000, null);

        var feeStructureIds = assignments.Items
            .Where(a => a.FeeStructureId > 0)
            .Select(a => a.FeeStructureId)
            .Distinct()
            .ToArray();

        var feeStructures = feeStructureIds.Length > 0
            ? await _unitOfWork.Repository<FeeStructure>().Query()
                .Where(fs => feeStructureIds.Contains(fs.Id))
                .ToListAsync()
            : [];

        var feeCategoryIds = feeStructures
            .Where(fs => fs.FeeCategoryId.HasValue)
            .Select(fs => fs.FeeCategoryId!.Value)
            .Distinct()
            .ToArray();

        var feeCategories = feeCategoryIds.Length > 0
            ? await _unitOfWork.Repository<FeeCategory>().Query()
                .Where(fc => feeCategoryIds.Contains(fc.Id))
                .ToDictionaryAsync(fc => fc.Id, fc => fc.Name)
            : [];

        var feeStructureInfos = assignments.Items.Select(a =>
        {
            var fs = feeStructures.FirstOrDefault(x => x.Id == a.FeeStructureId);
            var catName = fs?.FeeCategoryId.HasValue == true
                ? feeCategories.GetValueOrDefault(fs.FeeCategoryId.Value, "")
                : "";
            return new StudentFeeStructureInfoDto
            {
                Id = a.Id,
                FeeStructureName = a.FeeStructureName,
                FeeCategoryName = catName,
                Amount = fs?.Amount ?? 0,
                CustomAmount = a.CustomAmount,
                Frequency = fs?.Frequency.ToString() ?? "",
                IsActive = a.IsActive,
                ValidFrom = a.ValidFrom,
                ValidTo = a.ValidTo
            };
        }).ToList();

        var invoiceList = invoices.Items.ToList();
        var paymentList = paymentsResult.Items.Select(p => new FeePaymentListItemDto
        {
            Id = p.Id,
            InvoiceNo = p.InvoiceNo,
            Amount = p.Amount,
            LateFee = p.LateFee,
            DiscountAmount = p.DiscountAmount,
            Method = p.Method,
            ReferenceNo = p.ReferenceNo,
            PaidAt = p.PaymentDate
        }).ToList();

        var classDiscountList = classDiscounts.Items
            .Where(d => !d.SchoolClassId.HasValue || d.SchoolClassId == student.ClassId)
            .ToList();

        var overdueCount = invoiceList.Count(i => i.Status == 1
            && i.DueDate < DateOnly.FromDateTime(DateTime.UtcNow));

        var ledgerItems = ledger.Items;
        decimal LedgerDebit(int t) => ledgerItems.Where(l => l.TransactionType == t).Sum(l => l.Debit);
        decimal LedgerCredit(int t) => ledgerItems.Where(l => l.TransactionType == t).Sum(l => l.Credit);

        var totalAssigned = LedgerDebit((int)FeeLedgerType.Invoice) + LedgerDebit((int)FeeLedgerType.LateFee);
        var totalPaid = LedgerCredit((int)FeeLedgerType.Payment);
        var totalDiscount = LedgerCredit((int)FeeLedgerType.Discount);
        var totalWaiver = LedgerCredit((int)FeeLedgerType.Waiver);
        var totalLateFee = LedgerDebit((int)FeeLedgerType.LateFee);
        var totalRefund = LedgerDebit((int)FeeLedgerType.Refund);
        var totalDue = Math.Max(ledgerItems.Sum(l => l.Debit - l.Credit), 0);

        return new StudentFeeProfileDto
        {
            StudentId = student.Id,
            StudentName = student.FullName,
            StudentCode = student.StudentNo,
            ClassName = student.Class?.Name ?? "",
            SectionName = student.Section?.Name ?? "",
            AcademicYear = academicYear?.Name ?? "",
            GuardianName = primaryGuardian?.FullName ?? "",
            GuardianPhone = primaryGuardian?.MobileNumber ?? "",
            TotalAssigned = totalAssigned,
            TotalPaid = totalPaid,
            TotalDue = Math.Max(totalDue, 0),
            TotalDiscount = totalDiscount,
            TotalWaiver = totalWaiver,
            TotalLateFee = totalLateFee,
            InvoiceCount = invoiceList.Count,
            PaymentCount = paymentList.Count,
            OverdueCount = overdueCount,
            FeeStructures = feeStructureInfos,
            Invoices = invoiceList,
            Payments = paymentList,
            LedgerEntries = ledger.Items.ToList(),
            Discounts = classDiscountList,
            Waivers = waivers.Items.ToList()
        };
    }
}
