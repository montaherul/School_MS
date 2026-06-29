using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.DTOs.Fees;
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

    public AdmissionFinanceService(
        IUnitOfWork unitOfWork,
        IAdmissionRepository admissionRepository,
        ILogger<AdmissionFinanceService> logger,
        IFeeInvoiceService feeInvoiceService,
        IFeeInvoiceItemService feeInvoiceItemService,
        IFeeDiscountService feeDiscountService,
        IFeeWaiverService feeWaiverService,
        IFeeRefundService feeRefundService)
    {
        _unitOfWork = unitOfWork;
        _admissionRepository = admissionRepository;
        _logger = logger;
        _feeInvoiceService = feeInvoiceService;
        _feeInvoiceItemService = feeInvoiceItemService;
        _feeDiscountService = feeDiscountService;
        _feeWaiverService = feeWaiverService;
        _feeRefundService = feeRefundService;
    }

    public async Task<AdmissionFeeSummaryDto> GetFeeSummaryAsync(int applicationId, CancellationToken ct = default)
    {
        var app = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Application not found.");

        var payments = await _unitOfWork.Repository<Payment>().Query().AsNoTracking()
            .Where(p => p.Remarks != null && p.Remarks.Contains($"ADM-{applicationId}") && !p.IsDeleted)
            .ToListAsync(ct);

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
        var app = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == request.ApplicationId && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Application not found.");

        await _unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var payment = new Payment
            {
                FeeInvoiceId = 0,
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
                .Where(p => p.Remarks != null && p.Remarks.Contains($"ADM-{request.ApplicationId}") && !p.IsDeleted)
                .SumAsync(p => (decimal?)p.Amount, ct) ?? 0;

            if (totalPaid >= app.AdmissionFee)
            {
                app.AdmissionFeePaid = true;
                app.UpdatedBy = receivedBy;
                app.UpdatedAt = DateTime.UtcNow;
                _admissionRepository.Update(app);
                await _unitOfWork.SaveChangesAsync(ct);
            }

            await _unitOfWork.CommitTransactionAsync(ct);

            return new AdmissionPaymentHistoryDto
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
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            throw;
        }
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

        await _unitOfWork.BeginTransactionAsync(ct);
        try
        {
            await _unitOfWork.Repository<FeeRefund>().AddAsync(new FeeRefund
            {
                FeePaymentId = 0,
                RefundAmount = amount,
                RefundMethod = PaymentMethod.Cash,
                Reason = reason,
                IsApproved = true,
                ApprovedBy = processedBy,
                ApprovedAt = DateTime.UtcNow,
                CreatedBy = processedBy,
                CreatedAt = DateTime.UtcNow
            }, ct);

            await _unitOfWork.SaveChangesAsync(ct);

            app.AdmissionFeePaid = false;
            app.UpdatedBy = processedBy;
            app.UpdatedAt = DateTime.UtcNow;
            _admissionRepository.Update(app);
            await _unitOfWork.SaveChangesAsync(ct);

            await _unitOfWork.CommitTransactionAsync(ct);
            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            throw;
        }
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
            Status = isPaid ? PaymentStatus.Paid : PaymentStatus.Unpaid,
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

            var paymentDto = new FeePaymentUpsertDto
            {
                FeeInvoiceId = invoiceId,
                Amount = admissionFee,
                Method = (int)paymentMethodParsed,
                ReferenceNo = transactionDetails,
                PaidAt = DateTime.UtcNow,
                Remarks = $"Admission payment for {invoiceNo}"
            };

            await _unitOfWork.Repository<Payment>().AddAsync(new Payment
            {
                FeeInvoiceId = invoiceId,
                Amount = admissionFee,
                Method = paymentMethodParsed,
                ReferenceNo = transactionDetails,
                PaidAt = DateTime.UtcNow,
                Remarks = $"Admission payment for {invoiceNo}",
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            }, ct);

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
        }

        return invoiceId;
    }
}
