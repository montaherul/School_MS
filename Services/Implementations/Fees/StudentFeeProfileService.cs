using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Helpers.Pdf;
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
    private readonly IPdfGenerator _pdfGenerator;

    public StudentFeeProfileService(
        IStudentFeeAssignmentService assignmentService,
        IFeeInvoiceService invoiceService,
        IFeeLedgerService ledgerService,
        IFeeDiscountService discountService,
        IFeeWaiverService waiverService,
        IStudentFinanceService financeService,
        IUnitOfWork unitOfWork,
        IPdfGenerator pdfGenerator)
    {
        _assignmentService = assignmentService;
        _invoiceService = invoiceService;
        _ledgerService = ledgerService;
        _discountService = discountService;
        _waiverService = waiverService;
        _financeService = financeService;
        _unitOfWork = unitOfWork;
        _pdfGenerator = pdfGenerator;
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

    public async Task<List<StudentSearchResultDto>> SearchStudentsAsync(string? term)
    {
        return await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .Where(s => !s.IsDeleted && (
                string.IsNullOrEmpty(term) ||
                s.FullName.Contains(term) ||
                s.StudentNo.Contains(term)))
            .Select(s => new StudentSearchResultDto
            {
                StudentId = s.Id,
                StudentName = s.FullName,
                StudentCode = s.StudentNo,
                ClassName = s.Class.Name
            })
            .Take(20)
            .ToListAsync();
    }

    public async Task<byte[]> GenerateProfilePdfAsync(int studentId, int? academicYearId = null)
    {
        var profile = await GetProfileAsync(studentId, academicYearId);
        if (profile.StudentId == 0) return [];

        var html = BuildExportHtml(profile);
        return _pdfGenerator.GenerateFromHtml(html);
    }

    private static string BuildExportHtml(StudentFeeProfileDto profile)
    {
        return $@"
<html><head>
<meta charset='utf-8'/>
<style>
body{{font-family:Arial,sans-serif;margin:20px;}}
h1{{font-size:18px;margin-bottom:5px;}}
table{{width:100%;border-collapse:collapse;margin-bottom:15px;}}
th,td{{border:1px solid #ccc;padding:6px 8px;text-align:left;font-size:12px;}}
th{{background:#f0f0f0;}}
.text-right{{text-align:right;}}
.badge{{display:inline-block;padding:2px 8px;border-radius:10px;font-size:11px;background:#eee;}}
.badge-success{{background:#d4edda;}}
.badge-warning{{background:#fff3cd;}}
.badge-danger{{background:#f8d7da;}}
.badge-info{{background:#d1ecf1;}}
</style></head><body>
<h1>Student Fee Profile</h1>
<p>{profile.StudentName} ({profile.StudentCode}) - {profile.ClassName} - {profile.SectionName}</p>
<table>
<tr><th>Total Assigned</th><th>Total Paid</th><th>Total Due</th><th>Total Discount</th><th>Total Waiver</th><th>Late Fee</th></tr>
<tr>
<td class='text-right'>{profile.TotalAssigned:N2}</td>
<td class='text-right'>{profile.TotalPaid:N2}</td>
<td class='text-right'>{profile.TotalDue:N2}</td>
<td class='text-right'>{profile.TotalDiscount:N2}</td>
<td class='text-right'>{profile.TotalWaiver:N2}</td>
<td class='text-right'>{profile.TotalLateFee:N2}</td>
</tr>
</table>
<h3>Invoices ({profile.InvoiceCount})</h3>
<table>
<tr><th>Invoice No</th><th>Due Date</th><th>Total</th><th>Paid</th><th>Discount</th><th>Late Fee</th><th>Status</th></tr>
{string.Join("", profile.Invoices.Select(i => $"<tr><td>{i.InvoiceNo}</td><td>{i.DueDate}</td><td class='text-right'>{i.TotalAmount:N2}</td><td class='text-right'>{i.PaidAmount:N2}</td><td class='text-right'>{i.DiscountAmount:N2}</td><td class='text-right'>{i.LateFee:N2}</td><td>{GetStatusText(i.Status)}</td></tr>"))}
</table>
<h3>Payments ({profile.PaymentCount})</h3>
<table>
<tr><th>Date</th><th>Invoice</th><th>Amount</th><th>Method</th><th>Reference</th></tr>
{string.Join("", profile.Payments.Select(p => $"<tr><td>{p.PaidAt:yyyy-MM-dd}</td><td>{p.InvoiceNo}</td><td class='text-right'>{p.Amount:N2}</td><td>{p.Method}</td><td>{p.ReferenceNo}</td></tr>"))}
</table>
</body></html>";
    }

    private static string GetStatusText(int status)
    {
        return status switch
        {
            1 => "Draft",
            2 => "Partial",
            3 => "Paid",
            4 => "Waived",
            5 => "Issued",
            6 => "Cancelled",
            7 => "Refunded",
            _ => "Draft"
        };
    }
}
