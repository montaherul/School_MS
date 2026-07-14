using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Helpers.Email;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Guardian;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Accounting;
using SchoolManagementSystem.Services.Interfaces.Audit;
using SchoolManagementSystem.Services.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class SslCommerzGatewayService : IPaymentGatewayService
{
    private readonly SslCommerzConfig _config;
    private readonly SchoolDbContext _db;
    private readonly IOnlinePaymentService _onlinePaymentService;
    private readonly IFinancePostingService _financePostingService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAuditService _auditService;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<SslCommerzGatewayService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SslCommerzGatewayService(
        IOptions<SslCommerzConfig> config,
        SchoolDbContext db,
        IOnlinePaymentService onlinePaymentService,
        IFinancePostingService financePostingService,
        IHttpClientFactory httpClientFactory,
        IAuditService auditService,
        IEmailSender emailSender,
        ILogger<SslCommerzGatewayService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _config = config.Value;
        _db = db;
        _onlinePaymentService = onlinePaymentService;
        _financePostingService = financePostingService;
        _httpClientFactory = httpClientFactory;
        _auditService = auditService;
        _emailSender = emailSender;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<SslCommerzInitResponse?> InitiatePaymentAsync(int onlinePaymentRequestId, CancellationToken ct = default)
    {
        var request = await _db.OnlinePaymentRequests
            .Include(r => r.FeeInvoice)
            .FirstOrDefaultAsync(r => r.Id == onlinePaymentRequestId && !r.IsDeleted, ct);
        if (request == null || request.FeeInvoice == null)
        {
            _logger.LogWarning("OnlinePaymentRequest {Id} not found", onlinePaymentRequestId);
            return null;
        }

        if (request.FeeInvoice.Status == PaymentStatus.Paid)
        {
            _logger.LogWarning("Invoice {InvoiceId} already paid", request.FeeInvoiceId);
            return null;
        }

        if (request.Amount <= 0)
        {
            _logger.LogWarning("Invalid amount {Amount} for request {Id}", request.Amount, onlinePaymentRequestId);
            return null;
        }

        var httpCtx = _httpContextAccessor.HttpContext;
        var baseUrl = $"{httpCtx.Request.Scheme}://{httpCtx.Request.Host}";
        var successUrl = $"{baseUrl}/Fees/PaymentGateway/Success";
        var failUrl = $"{baseUrl}/Fees/PaymentGateway/Fail";
        var cancelUrl = $"{baseUrl}/Fees/PaymentGateway/Cancel";
        var ipnUrl = $"{baseUrl}/Fees/PaymentGateway/Ipn";

        PaymentGatewayTransaction? existingTx = null;
        if (!string.IsNullOrEmpty(request.GatewaySessionKey))
        {
            existingTx = await _db.PaymentGatewayTransactions
                .Where(t => t.OnlinePaymentRequestId == request.Id && t.GatewayName == "SSLCommerz")
                .OrderByDescending(t => t.Id)
                .FirstOrDefaultAsync(ct);
        }

        var student = await _db.Students.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.StudentId, ct);

        var isAdmissionPayment = request.StudentId == 0 || student == null;
        string cusName, cusEmail, cusPhone, cusAddr;
        if (isAdmissionPayment)
        {
            cusName = "Admission Applicant";
            cusEmail = "applicant@school.com";
            cusPhone = "N/A";
            cusAddr = "N/A";
            var remarks = request.FeeInvoice?.Remarks ?? "";
            if (remarks.StartsWith("AdmissionApp_") && int.TryParse(remarks["AdmissionApp_".Length..], out var appId))
            {
                var app = await _db.Set<SchoolManagementSystem.Models.Entities.Admission.AdmissionApplication>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id == appId, ct);
                if (app != null)
                {
                    cusName = app.ApplicantName;
                    cusEmail = app.ApplicantEmail ?? cusEmail;
                    cusPhone = app.ApplicantMobileNumber ?? cusPhone;
                    cusAddr = $"{app.PresentVillage ?? ""} {app.PresentPostOffice ?? ""} {app.PresentThana ?? ""} {app.PresentDistrict ?? ""}".Trim();
                    if (string.IsNullOrWhiteSpace(cusAddr)) cusAddr = "N/A";
                }
            }
        }
        else
        {
            cusName = student?.FullName ?? "Student";
            cusEmail = student?.EmailAddress ?? "student@school.com";
            cusPhone = student?.MobileNumber ?? "N/A";
            cusAddr = $"{(student?.PresentVillage ?? "")} {(student?.PresentPostOffice ?? "")} {(student?.PresentThana ?? "")} {(student?.PresentDistrict ?? "")}".Trim();
        }

        var tranId = $"SCH{request.Id:D6}{DateTime.UtcNow:yyyyMMddHHmmss}";

        var initData = new SslCommerzInitRequest
        {
            store_id = _config.StoreId,
            store_passwd = _config.StorePassword,
            total_amount = request.Amount,
            currency = _config.Currency,
            tran_id = tranId,
            success_url = successUrl,
            fail_url = failUrl,
            cancel_url = cancelUrl,
            ipn_url = ipnUrl,
            cus_name = cusName,
            cus_email = cusEmail,
            cus_phone = cusPhone,
            cus_add1 = cusAddr,
            cus_city = "Dhaka",
            cus_country = "Bangladesh",
            product_name = $"Fee Invoice #{request.FeeInvoice.InvoiceNo}",
            product_category = "Fee Payment",
            product_profile = "general",
            value_a = request.Id.ToString(),
            value_b = request.FeeInvoiceId.ToString(),
            value_c = request.StudentId.ToString()
        };

        var json = JsonSerializer.Serialize(initData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var client = _httpClientFactory.CreateClient("SslCommerz");
        HttpResponseMessage response;
        try
        {
            response = await client.PostAsync(_config.InitUrl, content, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SSLCommerz gateway timeout for request {RequestId}", onlinePaymentRequestId);
            await _auditService.LogAsync(null, "Payment", "GatewayTimeout",
                $"RequestId={request.Id}, Invoice={request.FeeInvoice.InvoiceNo}, Amount={request.Amount}", ct);
            return null;
        }

        var responseBody = await response.Content.ReadAsStringAsync(ct);

        SslCommerzInitResponse? result;
        try
        {
            result = JsonSerializer.Deserialize<SslCommerzInitResponse>(responseBody);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse SSLCommerz init response");
            return null;
        }

        var gatewayTx = new PaymentGatewayTransaction
        {
            OnlinePaymentRequestId = request.Id,
            GatewayName = "SSLCommerz",
            GatewayTransactionId = tranId,
            Currency = _config.Currency,
            GatewayAmount = request.Amount,
            GatewayStatus = result?.status,
            AttemptCount = (existingTx?.AttemptCount ?? 0) + 1,
            InitRequestPayload = json,
            InitResponsePayload = responseBody,
            InitiatedAt = DateTime.UtcNow
        };
        _db.PaymentGatewayTransactions.Add(gatewayTx);

        if (result != null && result.status == "SUCCESS")
        {
            request.Status = OnlinePaymentRequestStatus.GatewayPending;
            request.GatewayTransactionId = tranId;
            request.GatewaySessionKey = result.sessionkey;
            request.GatewayResponse = responseBody;
            request.PaymentExpiryAt = DateTime.UtcNow.AddHours(24);
            request.UpdatedAt = DateTime.UtcNow;

            await _auditService.LogAsync(null, "Payment", "GatewayInit",
                $"RequestId={request.Id}, Invoice={request.FeeInvoice.InvoiceNo}, TranId={tranId}, Amount={request.Amount}", ct);
        }

        await _db.SaveChangesAsync(ct);
        return result;
    }

    public async Task<SslCommerzValidationResponse?> ValidateTransactionAsync(string valId, CancellationToken ct = default)
    {
        var client = _httpClientFactory.CreateClient("SslCommerz");
        var url = $"{_config.ValidationUrl}?val_id={valId}&store_id={_config.StoreId}&store_passwd={_config.StorePassword}&format=json";
        var response = await client.GetAsync(url, ct);
        var body = await response.Content.ReadAsStringAsync(ct);

        try
        {
            return JsonSerializer.Deserialize<SslCommerzValidationResponse>(body);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse SSLCommerz validation response");
            return null;
        }
    }

    public async Task<bool> ProcessIpnAsync(string? bankTranId, string? tranId, string? valId, string status, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(tranId))
        {
            _logger.LogWarning("IPN received without tran_id");
            return false;
        }

        var request = await _db.OnlinePaymentRequests
            .Include(r => r.FeeInvoice)
            .FirstOrDefaultAsync(r => r.GatewayTransactionId == tranId && !r.IsDeleted, ct);
        if (request == null)
        {
            _logger.LogWarning("No OnlinePaymentRequest found for transaction {TranId}", tranId);
            return false;
        }

        if (request.Status == OnlinePaymentRequestStatus.Verified)
        {
            _logger.LogInformation("Transaction {TranId} already verified — skipping duplicate IPN", tranId);
            return true;
        }

        if (request.FeeInvoice != null && request.FeeInvoice.Status == PaymentStatus.Paid)
        {
            _logger.LogWarning("Invoice {InvoiceId} already paid — rejecting duplicate payment for tran {TranId}", request.FeeInvoiceId, tranId);
            request.Status = OnlinePaymentRequestStatus.Rejected;
            request.GatewayResponse = $"Rejected: Invoice already paid. IPN tran_id={tranId}";
            request.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            return false;
        }

        if (status != "VALID" && status != "VALIDATED")
        {
            _logger.LogWarning("IPN received with non-valid status: {Status}", status);
            request.Status = OnlinePaymentRequestStatus.Rejected;
            request.GatewayResponse = $"IPN Status: {status}";
            request.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            return false;
        }

        SslCommerzValidationResponse? validation = null;
        if (!string.IsNullOrEmpty(valId))
        {
            validation = await ValidateTransactionAsync(valId, ct);
        }

        var gatewayTx = await _db.PaymentGatewayTransactions
            .Where(t => t.GatewayTransactionId == tranId && t.GatewayName == "SSLCommerz")
            .FirstOrDefaultAsync(ct);
        if (gatewayTx != null)
        {
            var ipnPayload = JsonSerializer.Serialize(new { bankTranId, tranId, valId, status });
            gatewayTx.IpnPayload = ipnPayload;
            gatewayTx.BankTransactionId = bankTranId;
            gatewayTx.ValidationId = valId;
        }

        if (validation == null || (validation.status != "VALID" && validation.status != "VALIDATED"))
        {
            _logger.LogWarning("Transaction validation failed for {TranId}", tranId);
            request.Status = OnlinePaymentRequestStatus.Rejected;
            request.GatewayResponse = JsonSerializer.Serialize(new { validation, bankTranId, ipnStatus = status });
            request.UpdatedAt = DateTime.UtcNow;

            if (gatewayTx != null)
            {
                gatewayTx.GatewayStatus = "VALIDATION_FAILED";
                gatewayTx.ValidationPayload = JsonSerializer.Serialize(validation);
            }

            await _db.SaveChangesAsync(ct);
            return false;
        }

        if (validation.amount.HasValue && validation.amount.Value != request.Amount)
        {
            _logger.LogError("Amount mismatch for tran {TranId}: invoice={InvoiceAmount}, gateway={GatewayAmount}",
                tranId, request.Amount, validation.amount.Value);
            request.Status = OnlinePaymentRequestStatus.Rejected;
            request.GatewayResponse = $"Amount mismatch: expected={request.Amount}, got={validation.amount}";
            request.UpdatedAt = DateTime.UtcNow;

            if (gatewayTx != null)
            {
                gatewayTx.GatewayStatus = "AMOUNT_MISMATCH";
                gatewayTx.ValidationPayload = JsonSerializer.Serialize(validation);
            }

            await _db.SaveChangesAsync(ct);
            await _auditService.LogAsync(null, "Payment", "AmountMismatch",
                $"TranId={tranId}, Expected={request.Amount}, Gateway={validation.amount}, Invoice={request.FeeInvoice?.InvoiceNo}", ct);
            return false;
        }

        if (!string.IsNullOrEmpty(validation.currency) && validation.currency != "BDT")
        {
            _logger.LogError("Currency mismatch for tran {TranId}: expected=BDT, got={Currency}", tranId, validation.currency);
            request.Status = OnlinePaymentRequestStatus.Rejected;
            request.GatewayResponse = $"Currency mismatch: expected=BDT, got={validation.currency}";
            request.UpdatedAt = DateTime.UtcNow;

            if (gatewayTx != null)
            {
                gatewayTx.GatewayStatus = "CURRENCY_MISMATCH";
                gatewayTx.ValidationPayload = JsonSerializer.Serialize(validation);
            }

            await _db.SaveChangesAsync(ct);
            return false;
        }

        if (validation.risk_level == "1" || validation.risk_title == "Suspicious")
        {
            _logger.LogWarning("High-risk transaction detected: tran={TranId}, risk={RiskLevel}", tranId, validation.risk_level);
        }

        request.Status = OnlinePaymentRequestStatus.Verified;
        request.GatewayResponse = JsonSerializer.Serialize(validation);
        request.VerifiedAt = DateTime.UtcNow;
        request.VerifiedBy = "sslcOM~Auto";
        request.PaymentExpiryAt = null;
        request.UpdatedAt = DateTime.UtcNow;

        if (gatewayTx != null)
        {
            gatewayTx.GatewayStatus = "VERIFIED";
            gatewayTx.BankTransactionId = bankTranId ?? validation.bank_tran_id;
            gatewayTx.CardType = validation.card_type;
            gatewayTx.RiskLevel = validation.risk_level;
            gatewayTx.ValidationPayload = JsonSerializer.Serialize(validation);
            gatewayTx.CompletedAt = DateTime.UtcNow;
        }

        var payment = new Payment
        {
            FeeInvoiceId = request.FeeInvoiceId,
            Amount = request.Amount,
            Method = PaymentMethod.Online,
            ReferenceNo = bankTranId ?? tranId,
            PaidAt = DateTime.UtcNow,
            Remarks = $"SSLCommerz auto-verified. RequestId: {request.Id}, ValId: {valId}",
            CreatedBy = "sslcOM~Auto",
            CreatedAt = DateTime.UtcNow
        };
        _db.Payments.Add(payment);

        if (request.FeeInvoice != null)
        {
            request.FeeInvoice.PaidAmount += request.Amount;
            request.FeeInvoice.UpdatedAt = DateTime.UtcNow;
            var remaining = request.FeeInvoice.TotalAmount - request.FeeInvoice.PaidAmount;
            request.FeeInvoice.Status = remaining <= 0 ? PaymentStatus.Paid : PaymentStatus.Partial;
            _db.FeeInvoices.Update(request.FeeInvoice);
        }

        var ledger = new FeeLedger
        {
            StudentId = request.StudentId,
            FeeInvoiceId = request.FeeInvoiceId,
            FeePaymentId = payment.Id,
            TransactionType = FeeLedgerType.Payment,
            Debit = 0,
            Credit = request.Amount,
            Balance = -request.Amount,
            Description = $"SSLCommerz payment: {bankTranId ?? tranId}, request #{request.Id}",
            TransactionDate = DateTime.UtcNow,
            CreatedBy = "sslcOM~Auto",
            CreatedAt = DateTime.UtcNow
        };
        _db.FeeLedgers.Add(ledger);

        await _db.SaveChangesAsync(ct);

        try
        {
            if (request.PaymentPurpose == PaymentPurpose.AdmissionFee && request.AdmissionApplicationId.HasValue)
            {
                var app = await _db.Set<SchoolManagementSystem.Models.Entities.Admission.AdmissionApplication>()
                    .FirstOrDefaultAsync(a => a.Id == request.AdmissionApplicationId.Value, ct);
                if (app != null)
                {
                    app.AdmissionFeePaid = true;
                    app.UpdatedBy = "sslcOM~Auto";
                    app.UpdatedAt = DateTime.UtcNow;
                    _db.Admissions.Update(app);
                    await _db.SaveChangesAsync(ct);
                }

                await _financePostingService.PostAdmissionFeeAsync(
                    request.AdmissionApplicationId.Value,
                    request.Amount,
                    "SSLCommerz",
                    bankTranId ?? tranId,
                    "sslcOM~Auto",
                    ct);
                _logger.LogInformation("Admission finance posting completed for transaction {TranId}, App #{AdmissionId}", tranId, request.AdmissionApplicationId);
            }
            else
            {
                await _financePostingService.PostFeeCollectionAsync(
                    request.StudentId,
                    request.Amount,
                    request.FeeInvoiceId,
                    "sslcOM~Auto");
                _logger.LogInformation("Finance posting completed for transaction {TranId}", tranId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Finance posting failed for transaction {TranId} — payment recorded but accounting pending manual fix", tranId);
        }

        await _auditService.LogAsync(null, "Payment", "GatewaySuccess",
            $"TranId={tranId}, BankTranId={bankTranId}, Invoice={request.FeeInvoice?.InvoiceNo}, Amount={request.Amount}, StudentId={request.StudentId}", ct);

        await SendPaymentNotificationsAsync(request.StudentId, request.FeeInvoice?.InvoiceNo, request.Amount, tranId, ct);

        return true;
    }

    private async Task SendPaymentNotificationsAsync(int studentId, string? invoiceNo, decimal amount, string tranId, CancellationToken ct)
    {
        try
        {
            var guardians = await _db.StudentGuardians
                .AsNoTracking()
                .Include(sg => sg.Guardian)
                .Where(sg => sg.StudentId == studentId && !sg.IsDeleted)
                .ToListAsync(ct);

            foreach (var sg in guardians)
            {
                if (sg.Guardian?.UserId == null) continue;

                var notification = new GuardianNotification
                {
                    GuardianId = sg.GuardianId,
                    Title = "Payment Successful",
                    Message = $"BDT {amount:N2} paid for Invoice #{invoiceNo}. Ref: {tranId}",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };
                _db.GuardianNotifications.Add(notification);
            }

            var studentUser = await _db.Students.AsNoTracking()
                .Where(s => s.Id == studentId)
                .Select(s => s.UserId)
                .FirstOrDefaultAsync(ct);

            if (studentUser.HasValue)
            {
                var studentNotif = new SchoolManagementSystem.Models.Entities.Notification.NotificationMessage
                {
                    UserId = studentUser.Value,
                    Title = "Payment Successful",
                    Body = $"BDT {amount:N2} paid for Invoice #{invoiceNo}. Ref: {tranId}",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };
                _db.Notifications.Add(studentNotif);
            }

            await _db.SaveChangesAsync(ct);

            var student = await _db.Students.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == studentId, ct);

            if (student != null && !string.IsNullOrEmpty(student.EmailAddress))
            {
                var subject = "Payment Successful — School Management System";
                var body = $"Dear {student.FullName},<br><br>" +
                           $"Your payment of BDT {amount:N2} for Invoice #{invoiceNo} has been received and verified successfully.<br>" +
                           $"Transaction Reference: {tranId}<br><br>" +
                           "Thank you.<br>School Management System";

                await _emailSender.SendAsync(student.EmailAddress, subject, body, ct);
            }

            _logger.LogInformation("Payment notifications sent for tran {TranId}", tranId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send payment notifications for tran {TranId}", tranId);
        }
    }
}
