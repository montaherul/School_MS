using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using StudentEntity = SchoolManagementSystem.Models.Entities.Student.Student;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class CashierCollectionService : ICashierCollectionService
{
    private readonly IUnitOfWork _uow;
    private readonly IFeeReceiptService _receiptService;

    public CashierCollectionService(IUnitOfWork uow, IFeeReceiptService receiptService)
    {
        _uow = uow;
        _receiptService = receiptService;
    }

    public async Task<List<StudentSearchResultDto>> SearchStudentsAsync(string searchTerm, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return [];

        var query = _uow.Repository<StudentEntity>().Query()
            .Where(s => !s.IsDeleted && s.Status == StudentStatus.Active);

        if (int.TryParse(searchTerm, out _))
            query = query.Where(s => s.StudentNo.Contains(searchTerm) || s.RollNumber.ToString().Contains(searchTerm));
        else
            query = query.Where(s => s.FullName.Contains(searchTerm));

        return await query
            .Take(20)
            .Select(s => new StudentSearchResultDto
            {
                StudentId = s.Id,
                StudentName = s.FullName,
                StudentCode = s.StudentNo,
                ClassName = s.Class.Name
            })
            .ToListAsync(ct);
    }

    public async Task<CashierCollectionDto> GetStudentCollectionDataAsync(int studentId, CancellationToken ct = default)
    {
        var student = await _uow.Repository<StudentEntity>().Query()
            .Where(s => s.Id == studentId && !s.IsDeleted)
            .Select(s => new { s.Id, s.FullName, s.StudentNo, ClassName = s.Class.Name })
            .FirstOrDefaultAsync(ct);

        if (student is null) return null!;

        var invoices = await _uow.Repository<FeeInvoice>()
            .ListAsync(x => x.StudentId == studentId && !x.IsDeleted
                && x.Status != PaymentStatus.Paid
                && x.Status != PaymentStatus.Waived);

        return new CashierCollectionDto
        {
            StudentId = student.Id,
            StudentName = student.FullName,
            StudentCode = student.StudentNo,
            ClassName = student.ClassName,
            Invoices = invoices.Select(i => new CashierInvoiceItemDto
            {
                InvoiceId = i.Id,
                InvoiceNo = i.InvoiceNo,
                DueDate = i.DueDate,
                TotalAmount = i.TotalAmount,
                PaidAmount = i.PaidAmount,
                DueAmount = i.TotalAmount - i.PaidAmount,
                DiscountAmount = i.DiscountAmount,
                LateFee = i.LateFee,
                Status = (int)i.Status,
                StatusName = i.Status switch
                {
                    PaymentStatus.Draft => "Draft",
                    PaymentStatus.Issued => "Issued",
                    PaymentStatus.Partial => "Partial",
                    PaymentStatus.Paid => "Paid",
                    PaymentStatus.Cancelled => "Cancelled",
                    PaymentStatus.Waived => "Waived",
                    PaymentStatus.Refunded => "Refunded",
                    _ => ""
                },
                IsSelected = false
            }).ToList()
        };
    }

    public async Task<CashierPaymentResultDto> ProcessPaymentAsync(int studentId, List<int> invoiceIds, CashierPaymentDto payment, string createdBy, CancellationToken ct = default)
    {
        if (invoiceIds == null || invoiceIds.Count == 0)
            return new CashierPaymentResultDto { Success = false, ErrorMessage = "No invoices selected." };

        if (payment.Amount <= 0)
            return new CashierPaymentResultDto { Success = false, ErrorMessage = "Payment amount must be greater than zero." };

        int paymentId = 0;

        await _uow.ExecuteInTransactionAsync(async () =>
        {
            foreach (var invoiceId in invoiceIds)
            {
                var invoice = await _uow.Repository<FeeInvoice>()
                    .FirstOrDefaultAsync(x => x.Id == invoiceId && !x.IsDeleted, ct)
                    ?? throw new InvalidOperationException($"Invoice #{invoiceId} not found.");

                if (invoice.StudentId != studentId)
                    throw new InvalidOperationException($"Invoice #{invoiceId} does not belong to this student.");

                var remainingBalance = invoice.TotalAmount - invoice.PaidAmount;
                if (remainingBalance <= 0) continue;

                var allocAmount = Math.Min(payment.Amount, remainingBalance);

                var pay = new Payment
                {
                    CreatedBy = createdBy,
                    FeeInvoiceId = invoiceId,
                    Amount = allocAmount,
                    LateFee = 0,
                    DiscountAmount = 0,
                    Method = (PaymentMethod)payment.Method,
                    ReferenceNo = payment.ReferenceNo,
                    PaidAt = DateTime.UtcNow,
                    Remarks = payment.Remarks
                };
                await _uow.Repository<Payment>().AddAsync(pay, ct);
                await _uow.SaveChangesAsync(ct);

                invoice.PaidAmount += allocAmount;
                invoice.UpdatedAt = DateTime.UtcNow;

                var dueAmount = invoice.TotalAmount - invoice.PaidAmount;
                if (dueAmount <= 0)
                    invoice.Status = PaymentStatus.Paid;
                else
                    invoice.Status = PaymentStatus.Partial;

                _uow.Repository<FeeInvoice>().Update(invoice);
                await _uow.SaveChangesAsync(ct);

                var ledger = new FeeLedger
                {
                    StudentId = studentId,
                    FeeInvoiceId = invoiceId,
                    FeePaymentId = pay.Id,
                    TransactionType = FeeLedgerType.Payment,
                    Debit = 0,
                    Credit = allocAmount,
                    Balance = -allocAmount,
                    Description = $"Cashier collection: {pay.ReferenceNo ?? "N/A"}",
                    TransactionDate = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                };
                await _uow.Repository<FeeLedger>().AddAsync(ledger, ct);
                await _uow.SaveChangesAsync(ct);

                payment.Amount -= allocAmount;
                if (paymentId == 0) paymentId = pay.Id;
            }
        }, ct);

        return new CashierPaymentResultDto
        {
            Success = true,
            PaymentId = paymentId,
            ReceiptUrl = $"/FeePayment/Receipt/{paymentId}"
        };
    }
}
