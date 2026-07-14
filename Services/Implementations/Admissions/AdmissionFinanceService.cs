using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Admission;
using SchoolManagementSystem.Services.Interfaces.Admissions;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Admissions;

public class AdmissionFinanceService : IAdmissionFinanceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAdmissionRepository _admissionRepository;
    private readonly ILogger<AdmissionFinanceService> _logger;
    private readonly IFeeInvoiceService _feeInvoiceService;
    private readonly IFeeInvoiceItemService _feeInvoiceItemService;
    private readonly IFeeDiscountService _feeDiscountService;
    private readonly IFeeWaiverService _feeWaiverService;
    private readonly IFeeRefundService _feeRefundService;
    private readonly IOnlinePaymentService _onlinePaymentService;

    public AdmissionFinanceService(
        IUnitOfWork unitOfWork,
        IAdmissionRepository admissionRepository,
        ILogger<AdmissionFinanceService> logger,
        IFeeInvoiceService feeInvoiceService,
        IFeeInvoiceItemService feeInvoiceItemService,
        IFeeDiscountService feeDiscountService,
        IFeeWaiverService feeWaiverService,
        IFeeRefundService feeRefundService,
        IOnlinePaymentService onlinePaymentService)
    {
        _unitOfWork = unitOfWork;
        _admissionRepository = admissionRepository;
        _logger = logger;
        _feeInvoiceService = feeInvoiceService;
        _feeInvoiceItemService = feeInvoiceItemService;
        _feeDiscountService = feeDiscountService;
        _feeWaiverService = feeWaiverService;
        _feeRefundService = feeRefundService;
        _onlinePaymentService = onlinePaymentService;
    }

    public async Task<List<AdmissionFeeSummaryListItemDto>> GetAllFeeSummariesAsync(CancellationToken ct = default)
    {
        var apps = await _admissionRepository.Query().AsNoTracking()
            .Where(a => !a.IsDeleted)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(ct);

        var appIds = apps.Select(a => a.Id).ToList();
        var allPayments = await _unitOfWork.Repository<Payment>().Query().AsNoTracking()
            .Where(p => appIds.Any(id => p.Remarks != null && p.Remarks.Contains($"ADM-{id}")) && !p.IsDeleted)
            .ToListAsync(ct);

        var paymentLookup = allPayments
            .GroupBy(p =>
            {
                var prefix = "ADM-";
                var idx = p.Remarks?.IndexOf(prefix, StringComparison.OrdinalIgnoreCase) ?? -1;
                if (idx < 0) return -1;
                var after = p.Remarks!.Substring(idx + prefix.Length);
                var end = after.IndexOf(':');
                if (end < 0) end = after.IndexOf(' ');
                if (end < 0) end = after.Length;
                return int.TryParse(after.AsSpan(0, end), out var id) ? id : -1;
            })
            .ToDictionary(g => g.Key, g => g.ToList());

        var classIds = apps.Where(a => a.AppliedClassId > 0).Select(a => a.AppliedClassId).Distinct().ToList();
        var classes = classIds.Count > 0
            ? await _unitOfWork.Repository<SchoolClass>().Query().AsNoTracking()
                .Where(c => classIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c.Name, ct)
            : new Dictionary<int, string>();

        return apps.Select(app =>
        {
            var payments = paymentLookup.GetValueOrDefault(app.Id, new List<Payment>());
            var totalPaid = payments.Sum(p => p.Amount);
            return new AdmissionFeeSummaryListItemDto
            {
                ApplicationId = app.Id,
                ApplicationNo = app.ApplicationNo,
                ApplicantName = app.ApplicantName,
                AppliedClass = app.AppliedClassId > 0 && classes.ContainsKey(app.AppliedClassId) ? classes[app.AppliedClassId] : null,
                AdmissionFee = app.AdmissionFee,
                PaidAmount = totalPaid,
                Status = app.Status,
                AppliedAt = app.CreatedAt,
                LastPaymentAt = payments.Count > 0 ? payments.Max(p => p.PaidAt) : null
            };
        }).ToList();
    }

    public async Task<AdmissionFeeSummaryDto> GetFeeSummaryAsync(int applicationId, CancellationToken ct = default)
    {
        var app = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Application not found.");

        var invoiceKey = $"AdmissionApp_{applicationId}";
        var invoice = await _unitOfWork.Repository<FeeInvoice>().Query().AsNoTracking()
            .FirstOrDefaultAsync(i => i.Remarks == invoiceKey && !i.IsDeleted, ct);

        List<Payment> payments;
        if (invoice != null)
        {
            payments = await _unitOfWork.Repository<Payment>().Query().AsNoTracking()
                .Where(p => p.FeeInvoiceId == invoice.Id && !p.IsDeleted)
                .ToListAsync(ct);
        }
        else
        {
            payments = await _unitOfWork.Repository<Payment>().Query().AsNoTracking()
                .Where(p => p.Remarks != null && p.Remarks.Contains($"ADM-{applicationId}") && !p.IsDeleted)
                .ToListAsync(ct);
        }

        var totalPaid = payments.Sum(p => p.Amount);

        return new AdmissionFeeSummaryDto
        {
            ApplicationId = app.Id,
            ApplicationNo = app.ApplicationNo,
            ApplicantName = app.ApplicantName,
            AdmissionFee = app.AdmissionFee,
            PaidAmount = totalPaid,
            Payments = payments.Select(p => new AdmissionPaymentHistoryDto
            {
                Id = p.Id,
                ApplicationId = applicationId,
                ApplicationNo = app.ApplicationNo,
                Amount = p.Amount,
                PaymentMethod = p.Method.ToString(),
                TransactionId = p.ReferenceNo,
                Status = "Paid",
                PaidAt = p.PaidAt,
                Remarks = p.Remarks,
                ReceivedBy = p.CreatedBy
            }).ToList()
        };
    }

    public async Task<AdmissionPaymentHistoryDto> RecordPaymentAsync(AdmissionFeePaymentRequest request, string receivedBy, CancellationToken ct = default)
    {
        if (request.Amount <= 0)
            throw new InvalidOperationException("Payment amount must be greater than zero.");

        var app = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == request.ApplicationId && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Application not found.");

        AdmissionPaymentHistoryDto? result = null;
        Payment? payment = null;
        int invoiceId;

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var invoiceKey = $"AdmissionApp_{request.ApplicationId}";
            var invoice = await _unitOfWork.Repository<FeeInvoice>().FirstOrDefaultAsync(i => i.Remarks == invoiceKey && !i.IsDeleted, ct);

            if (invoice == null)
            {
                var invoiceNo = $"INV-ADM-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999):D4}";
                invoice = new FeeInvoice
                {
                    InvoiceNo = invoiceNo,
                    StudentId = 0,
                    DueDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(30)),
                    TotalAmount = app.AdmissionFee,
                    PaidAmount = 0,
                    Status = PaymentStatus.Draft,
                    Remarks = invoiceKey,
                    CreatedBy = receivedBy,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Repository<FeeInvoice>().AddAsync(invoice, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                invoiceId = invoice.Id;

                var itemDto = new FeeInvoiceItemUpsertDto
                {
                    FeeInvoiceId = invoiceId,
                    Description = $"Admission Fee - {app.ApplicantName}",
                    Amount = app.AdmissionFee,
                    NetAmount = app.AdmissionFee
                };
                await _feeInvoiceItemService.CreateAsync(itemDto, receivedBy, ct);
            }
            else
            {
                invoiceId = invoice.Id;
            }

            payment = new Payment
            {
                FeeInvoiceId = invoiceId,
                Amount = request.Amount,
                LateFee = 0,
                DiscountAmount = 0,
                Method = request.PaymentMethod,
                ReferenceNo = request.TransactionId,
                PaidAt = DateTime.UtcNow,
                Remarks = $"ADM-{request.ApplicationId}: {request.Remarks}",
                CreatedBy = receivedBy,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Payment>().AddAsync(payment, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            var totalPaid = await _unitOfWork.Repository<Payment>().Query().AsNoTracking()
                .Where(p => p.FeeInvoiceId == invoiceId && !p.IsDeleted)
                .SumAsync(p => (decimal?)p.Amount, ct) ?? 0;

            invoice.PaidAmount = totalPaid;
            invoice.UpdatedAt = DateTime.UtcNow;
            if (totalPaid >= invoice.TotalAmount)
                invoice.Status = PaymentStatus.Paid;
            else if (totalPaid > 0)
                invoice.Status = PaymentStatus.Partial;
            _unitOfWork.Repository<FeeInvoice>().Update(invoice);
            await _unitOfWork.SaveChangesAsync(ct);

            if (totalPaid >= app.AdmissionFee)
            {
                app.AdmissionFeePaid = true;
                app.UpdatedBy = receivedBy;
                app.UpdatedAt = DateTime.UtcNow;
                _admissionRepository.Update(app);
                await _unitOfWork.SaveChangesAsync(ct);
            }

            var ledger = new FeeLedger
            {
                StudentId = invoice.StudentId,
                FeeInvoiceId = invoiceId,
                FeePaymentId = payment.Id,
                TransactionType = FeeLedgerType.Payment,
                Debit = 0,
                Credit = request.Amount,
                Balance = -(request.Amount),
                Description = $"Admission payment: {request.TransactionId ?? "N/A"}",
                TransactionDate = DateTime.UtcNow,
                CreatedBy = receivedBy,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<FeeLedger>().AddAsync(ledger, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            result = new AdmissionPaymentHistoryDto
            {
                Id = payment.Id,
                ApplicationId = request.ApplicationId,
                ApplicationNo = app.ApplicationNo,
                Amount = request.Amount,
                PaymentMethod = request.PaymentMethod.ToString(),
                TransactionId = request.TransactionId,
                Status = "Paid",
                PaidAt = DateTime.UtcNow,
                Remarks = request.Remarks,
                ReceivedBy = receivedBy
            };
        }, ct);

        return result!;
    }

    public async Task<bool> ApplyScholarshipAsync(int applicationId, decimal percentage, string? description, string appliedBy, CancellationToken ct = default)
    {
        var app = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, ct);
        if (app == null) return false;

        var dto = new FeeDiscountUpsertDto
        {
            Name = $"Admission Scholarship - {app.ApplicationNo}",
            DiscountType = (int)FeeDiscountType.Percentage,
            Value = percentage,
            Description = description ?? $"Scholarship of {percentage}% applied",
            IsActive = true
        };

        await _feeDiscountService.CreateAsync(dto, appliedBy, ct);
        return true;
    }

    public async Task<bool> ApplyWaiverAsync(int applicationId, decimal amount, string? description, string appliedBy, CancellationToken ct = default)
    {
        var app = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, ct);
        if (app == null) return false;

        var dto = new FeeWaiverUpsertDto
        {
            StudentId = 0,
            WaiverAmount = amount,
            WaiverType = (int)FeeDiscountType.Fixed,
            WaiverValue = amount,
            Reason = description ?? $"Admission fee waiver of {amount:N2}",
            IsApproved = true
        };

        await _feeWaiverService.CreateAsync(dto, appliedBy, ct);
        return true;
    }

    public async Task<List<AdmissionInstallmentPlanDto>> CreateInstallmentPlanAsync(int applicationId, int installments, string createdBy, CancellationToken ct = default)
    {
        var app = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Application not found.");

        var plan = new List<AdmissionInstallmentPlanDto>();
        var perInstallment = Math.Round(app.AdmissionFee / installments, 2);
        var dueDate = DateTime.UtcNow.Date.AddDays(30);

        for (int i = 1; i <= installments; i++)
        {
            plan.Add(new AdmissionInstallmentPlanDto
            {
                ApplicationId = applicationId,
                InstallmentNumber = i,
                Amount = i == installments ? app.AdmissionFee - (perInstallment * (installments - 1)) : perInstallment,
                DueDate = dueDate.AddMonths(i - 1),
                IsPaid = false
            });
        }

        return plan;
    }

    public async Task<bool> ProcessRefundAsync(int applicationId, decimal amount, string reason, string processedBy, CancellationToken ct = default)
    {
        var app = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, ct);
        if (app == null) return false;

        var invoiceKey = $"AdmissionApp_{applicationId}";
        var invoice = await _unitOfWork.Repository<FeeInvoice>().FirstOrDefaultAsync(i => i.Remarks == invoiceKey && !i.IsDeleted, ct);

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var refund = new FeeRefund
            {
                FeePaymentId = invoice?.Id ?? 0,
                RefundAmount = amount,
                RefundMethod = PaymentMethod.Cash,
                Reason = reason,
                IsApproved = true,
                ApprovedBy = processedBy,
                ApprovedAt = DateTime.UtcNow,
                CreatedBy = processedBy,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<FeeRefund>().AddAsync(refund, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            if (invoice != null)
            {
                var ledger = new FeeLedger
                {
                    StudentId = invoice.StudentId,
                    FeeInvoiceId = invoice.Id,
                    TransactionType = FeeLedgerType.Refund,
                    Debit = 0,
                    Credit = amount,
                    Balance = -(amount),
                    Description = $"Admission fee refund: {reason}",
                    TransactionDate = DateTime.UtcNow,
                    CreatedBy = processedBy,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Repository<FeeLedger>().AddAsync(ledger, ct);
                await _unitOfWork.SaveChangesAsync(ct);
            }

            app.AdmissionFeePaid = false;
            app.UpdatedBy = processedBy;
            app.UpdatedAt = DateTime.UtcNow;
            _admissionRepository.Update(app);
            await _unitOfWork.SaveChangesAsync(ct);
        }, ct);

        return true;
    }

    public async Task<int> CreateAdmissionInvoiceAsync(int applicationId, int studentId, decimal admissionFee, bool isPaid, string? className, string? paymentMethod, string? transactionDetails, string createdBy, CancellationToken ct = default)
    {
        var invoiceKey = $"AdmissionApp_{applicationId}";

        if (await _unitOfWork.Repository<FeeInvoice>().AnyAsync(i => i.Remarks == invoiceKey && !i.IsDeleted, ct))
            return 0;

        var invoiceNo = $"INV-ADM-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999):D4}";

        var invoice = new FeeInvoice
        {
            InvoiceNo = invoiceNo,
            StudentId = studentId,
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(30)),
            TotalAmount = admissionFee,
            PaidAmount = isPaid ? admissionFee : 0,
            Status = isPaid ? PaymentStatus.Paid : PaymentStatus.Draft,
            Remarks = invoiceKey,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        var invoiceId = await _feeInvoiceService.CreateAsync(invoice, createdBy, ct);

        var displayClass = className ?? $"Class-{studentId}";

        var itemDto = new FeeInvoiceItemUpsertDto
        {
            FeeInvoiceId = invoiceId,
            Description = $"Admission Fee - {displayClass}",
            Amount = admissionFee,
            NetAmount = admissionFee
        };

        await _feeInvoiceItemService.CreateAsync(itemDto, createdBy, ct);

        if (isPaid)
        {
            var paymentMethodParsed = !string.IsNullOrWhiteSpace(paymentMethod)
                && Enum.TryParse<PaymentMethod>(paymentMethod, true, out var parsedMethod)
                ? parsedMethod : PaymentMethod.Cash;

            var paymentEntity = new Payment
            {
                FeeInvoiceId = invoiceId,
                Amount = admissionFee,
                Method = paymentMethodParsed,
                ReferenceNo = transactionDetails,
                PaidAt = DateTime.UtcNow,
                Remarks = $"Admission payment for {invoiceNo}",
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Payment>().AddAsync(paymentEntity, ct);

            var invoiceEntity = await _unitOfWork.Repository<FeeInvoice>().FirstOrDefaultAsync(x => x.Id == invoiceId && !x.IsDeleted, ct);
            if (invoiceEntity != null)
            {
                invoiceEntity.PaidAmount = admissionFee;
                invoiceEntity.Status = PaymentStatus.Paid;
                invoiceEntity.UpdatedBy = createdBy;
                invoiceEntity.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<FeeInvoice>().Update(invoiceEntity);
            }

            await _unitOfWork.SaveChangesAsync(ct);

            var ledger = new FeeLedger
            {
                StudentId = studentId,
                FeeInvoiceId = invoiceId,
                FeePaymentId = paymentEntity.Id,
                TransactionType = FeeLedgerType.Payment,
                Debit = 0,
                Credit = admissionFee,
                Balance = -(admissionFee),
                Description = $"Admission payment for {invoiceNo}",
                TransactionDate = DateTime.UtcNow,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<FeeLedger>().AddAsync(ledger, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        return invoiceId;
    }

    public async Task<int> CreateAdmissionOnlinePaymentAsync(int applicationId, string createdBy, CancellationToken ct = default)
    {
        var app = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Application not found.");

        var invoiceKey = $"AdmissionApp_{applicationId}";
        var existingInvoice = await _unitOfWork.Repository<FeeInvoice>()
            .FirstOrDefaultAsync(i => i.Remarks == invoiceKey && !i.IsDeleted, ct);

        int invoiceId;
        if (existingInvoice != null)
        {
            invoiceId = existingInvoice.Id;
        }
        else
        {
            invoiceId = await CreateAdmissionInvoiceAsync(
                applicationId, 0, app.AdmissionFee, false,
                null, null, null, createdBy, ct);
        }

        var onlineRequest = await _onlinePaymentService.CreateGatewayPendingAsync(0, invoiceId, createdBy, ct);

        // Tag as admission fee payment
        onlineRequest.PaymentPurpose = PaymentPurpose.AdmissionFee;
        onlineRequest.AdmissionApplicationId = applicationId;
        await _unitOfWork.SaveChangesAsync(ct);

        return onlineRequest.Id;
    }
}
