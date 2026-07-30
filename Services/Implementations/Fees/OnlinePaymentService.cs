using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Accounting;
using SchoolManagementSystem.Services.Interfaces.Audit;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Repositories.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class OnlinePaymentService : IOnlinePaymentService
{
    private readonly IOnlinePaymentRepository _onlinePaymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFinancePostingService _financePostingService;
    private readonly IAuditService _auditService;
    private readonly ILogger<OnlinePaymentService> _logger;

    public OnlinePaymentService(
        IOnlinePaymentRepository onlinePaymentRepository,
        IUnitOfWork unitOfWork,
        IFinancePostingService financePostingService,
        IAuditService auditService,
        ILogger<OnlinePaymentService> logger)
    {
        _onlinePaymentRepository = onlinePaymentRepository;
        _unitOfWork = unitOfWork;
        _financePostingService = financePostingService;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<PagedResult<OnlinePaymentRequestListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? statusFilter = null, CancellationToken ct = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        var (items, totalCount) = await _onlinePaymentRepository.GetPagedAsync(page, pageSize, search, statusFilter, ct);
        return new PagedResult<OnlinePaymentRequestListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }

    public async Task<OnlinePaymentRequest> CreateAsync(int studentId, OnlinePaymentSubmitDto dto, string createdBy, CancellationToken ct = default)
    {
        var entity = new OnlinePaymentRequest
        {
            StudentId = studentId,
            FeeInvoiceId = dto.FeeInvoiceId,
            Amount = dto.Amount,
            PaymentMethod = dto.PaymentMethod,
            ReferenceNo = dto.ReferenceNo,
            Remarks = dto.Remarks,
            Status = OnlinePaymentRequestStatus.Pending,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.Repository<OnlinePaymentRequest>().AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<List<OnlinePaymentRequestDto>> GetPendingAsync(CancellationToken ct = default)
    {
        var requests = await _unitOfWork.Repository<OnlinePaymentRequest>()
            .QueryNoTracking()
            .Where(r => r.Status == OnlinePaymentRequestStatus.Pending && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

        return requests.Select(r => new OnlinePaymentRequestDto
        {
            Id = r.Id,
            StudentId = r.StudentId,
            StudentName = _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Student.Student>()
                .QueryNoTracking().Where(s => s.Id == r.StudentId).Select(s => s.FullName).FirstOrDefault(),
            FeeInvoiceId = r.FeeInvoiceId,
            InvoiceNo = r.FeeInvoice?.InvoiceNo ?? "",
            Amount = r.Amount,
            PaymentMethod = r.PaymentMethod.ToString(),
            ReferenceNo = r.ReferenceNo,
            Status = r.Status.ToString(),
            Remarks = r.Remarks,
            CreatedAt = r.CreatedAt,
            VerifiedAt = r.VerifiedAt
        }).ToList();
    }

    public async Task<List<OnlinePaymentRequestDto>> GetAllAsync(CancellationToken ct = default)
    {
        var requests = await _unitOfWork.Repository<OnlinePaymentRequest>()
            .QueryNoTracking()
            .Where(r => !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

        return requests.Select(r => new OnlinePaymentRequestDto
        {
            Id = r.Id,
            StudentId = r.StudentId,
            StudentName = _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Student.Student>()
                .QueryNoTracking().Where(s => s.Id == r.StudentId).Select(s => s.FullName).FirstOrDefault(),
            FeeInvoiceId = r.FeeInvoiceId,
            InvoiceNo = r.FeeInvoice?.InvoiceNo ?? "",
            Amount = r.Amount,
            PaymentMethod = r.PaymentMethod.ToString(),
            ReferenceNo = r.ReferenceNo,
            Status = r.Status.ToString(),
            Remarks = r.Remarks,
            AdminNotes = r.AdminNotes,
            CreatedAt = r.CreatedAt,
            VerifiedAt = r.VerifiedAt
        }).ToList();
    }

    public async Task<List<OnlinePaymentRequestDto>> GetByStudentAsync(int studentId, CancellationToken ct = default)
    {
        var requests = await _unitOfWork.Repository<OnlinePaymentRequest>()
            .QueryNoTracking()
            .Where(r => r.StudentId == studentId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

        return requests.Select(r => new OnlinePaymentRequestDto
        {
            Id = r.Id,
            StudentId = r.StudentId,
            StudentName = _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Student.Student>()
                .QueryNoTracking().Where(s => s.Id == r.StudentId).Select(s => s.FullName).FirstOrDefault(),
            FeeInvoiceId = r.FeeInvoiceId,
            InvoiceNo = r.FeeInvoice?.InvoiceNo ?? "",
            Amount = r.Amount,
            PaymentMethod = r.PaymentMethod.ToString(),
            ReferenceNo = r.ReferenceNo,
            Status = r.Status.ToString(),
            Remarks = r.Remarks,
            CreatedAt = r.CreatedAt,
            VerifiedAt = r.VerifiedAt
        }).ToList();
    }

    public async Task<bool> VerifyAsync(int id, string verifiedBy, string? adminNotes, CancellationToken ct = default)
    {
        var entity = await _unitOfWork.Repository<OnlinePaymentRequest>()
            .Query()
            .Include(r => r.FeeInvoice)
            .FirstOrDefaultAsync(r => r.Id == id && r.Status == OnlinePaymentRequestStatus.Pending && !r.IsDeleted, ct);
        if (entity == null) return false;

        var invoice = entity.FeeInvoice;
        if (invoice == null)
        {
            invoice = await _unitOfWork.Repository<FeeInvoice>()
                .FirstOrDefaultAsync(i => i.Id == entity.FeeInvoiceId && !i.IsDeleted, ct);
            if (invoice == null) return false;
        }

        var dueAmount = invoice.TotalAmount - invoice.PaidAmount;
        if (dueAmount <= 0)
        {
            entity.Status = OnlinePaymentRequestStatus.Rejected;
            entity.AdminNotes = "Invoice already paid.";
            entity.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(ct);
            return false;
        }

        var allocAmount = Math.Min(entity.Amount, dueAmount);

        var payment = new Payment
        {
            FeeInvoiceId = entity.FeeInvoiceId,
            Amount = allocAmount,
            Method = entity.PaymentMethod,
            ReferenceNo = entity.ReferenceNo ?? $"VRF-{entity.Id:D6}",
            PaidAt = DateTime.UtcNow,
            Remarks = $"Verified online payment request #{entity.Id}. {entity.Remarks}".Trim(),
            CreatedBy = verifiedBy,
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.Repository<Payment>().AddAsync(payment, ct);

        invoice.PaidAmount += allocAmount;
        invoice.UpdatedAt = DateTime.UtcNow;
        var remaining = invoice.TotalAmount - invoice.PaidAmount;
        invoice.Status = remaining <= 0 ? PaymentStatus.Paid : PaymentStatus.Partial;
        _unitOfWork.Repository<FeeInvoice>().Update(invoice);

        var ledger = new FeeLedger
        {
            StudentId = entity.StudentId,
            FeeInvoiceId = entity.FeeInvoiceId,
            FeePaymentId = payment.Id,
            TransactionType = FeeLedgerType.Payment,
            Debit = 0,
            Credit = allocAmount,
            Balance = -allocAmount,
            Description = $"Online payment verified: {entity.ReferenceNo ?? "N/A"}, request #{entity.Id}",
            TransactionDate = DateTime.UtcNow,
            CreatedBy = verifiedBy,
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.Repository<FeeLedger>().AddAsync(ledger, ct);

        entity.Status = OnlinePaymentRequestStatus.Verified;
        entity.VerifiedBy = verifiedBy;
        entity.VerifiedAt = DateTime.UtcNow;
        entity.AdminNotes = adminNotes;
        entity.UpdatedBy = verifiedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(ct);

        try
        {
            await _financePostingService.PostFeeCollectionAsync(
                entity.StudentId, allocAmount, entity.FeeInvoiceId, verifiedBy, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Finance posting failed for verified online payment request {RequestId} — payment recorded but accounting pending", id);
        }

        await _auditService.LogAsync(null, "Payment", "OnlinePaymentVerified",
            $"RequestId={entity.Id}, Invoice={invoice.InvoiceNo}, Amount={allocAmount}, VerifiedBy={verifiedBy}", ct);

        return true;
    }

    public async Task<bool> RejectAsync(int id, string rejectedBy, string? adminNotes, CancellationToken ct = default)
    {
        var entity = await _unitOfWork.Repository<OnlinePaymentRequest>()
            .FirstOrDefaultAsync(r => r.Id == id && r.Status == OnlinePaymentRequestStatus.Pending && !r.IsDeleted, ct);
        if (entity == null) return false;

        entity.Status = OnlinePaymentRequestStatus.Rejected;
        entity.RejectedBy = rejectedBy;
        entity.RejectedAt = DateTime.UtcNow;
        entity.AdminNotes = adminNotes;
        entity.UpdatedBy = rejectedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    public async Task<OnlinePaymentRequest> CreateGatewayPendingAsync(int studentId, int invoiceId, string createdBy, CancellationToken ct = default)
    {
        var invoice = await _unitOfWork.Repository<FeeInvoice>()
            .QueryNoTracking()
            .FirstOrDefaultAsync(i => i.Id == invoiceId && !i.IsDeleted, ct);

        var dueAmount = invoice != null ? invoice.TotalAmount - invoice.PaidAmount : 0;
        if (dueAmount <= 0)
            throw new InvalidOperationException("Invoice is already paid.");

        var entity = new OnlinePaymentRequest
        {
            StudentId = studentId,
            FeeInvoiceId = invoiceId,
            Amount = dueAmount,
            PaymentMethod = PaymentMethod.Online,
            Status = OnlinePaymentRequestStatus.GatewayPending,
            ReferenceNo = $"SSL-{DateTime.UtcNow:yyyyMMddHHmmss}",
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.Repository<OnlinePaymentRequest>().AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<OnlinePaymentRequest?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<OnlinePaymentRequest>()
            .Query()
            .Include(r => r.FeeInvoice)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, ct);
    }

    public async Task<OnlinePaymentRequest?> GetByGatewayTransactionIdAsync(string tranId, CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<OnlinePaymentRequest>()
            .Query()
            .Include(r => r.FeeInvoice)
            .FirstOrDefaultAsync(r => r.GatewayTransactionId == tranId && !r.IsDeleted, ct);
    }

    public async Task<int> GetPaymentIdByInvoiceIdAsync(int feeInvoiceId, CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<Payment>()
            .QueryNoTracking()
            .Where(p => p.FeeInvoiceId == feeInvoiceId)
            .OrderByDescending(p => p.Id)
            .Select(p => p.Id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Payment?> GetPaymentByInvoiceIdAsync(int feeInvoiceId, CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<Payment>()
            .QueryNoTracking()
            .Where(p => p.FeeInvoiceId == feeInvoiceId)
            .OrderByDescending(p => p.Id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task ExpireStaleRequestsAsync(CancellationToken ct = default)
    {
        var staleRequests = await _unitOfWork.Repository<OnlinePaymentRequest>()
            .QueryNoTracking()
            .Where(r => r.Status == OnlinePaymentRequestStatus.GatewayPending
                       && r.PaymentExpiryAt != null
                       && r.PaymentExpiryAt < DateTime.UtcNow
                       && !r.IsDeleted)
            .ToListAsync(ct);

        foreach (var request in staleRequests)
        {
            request.Status = OnlinePaymentRequestStatus.Rejected;
            request.RejectedBy = "system~expiry";
            request.RejectedAt = DateTime.UtcNow;
            request.AdminNotes = "Payment expired after 24 hours.";
            request.GatewayResponse = "Expired: payment window closed.";
            request.UpdatedAt = DateTime.UtcNow;
        }

        if (staleRequests.Count > 0)
        {
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
