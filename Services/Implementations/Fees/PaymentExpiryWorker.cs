using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class PaymentExpiryWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PaymentExpiryWorker> _logger;

    public PaymentExpiryWorker(IServiceProvider serviceProvider, ILogger<PaymentExpiryWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PaymentExpiryWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExpirePendingPaymentsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error expiring payments");
            }

            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }

    private async Task ExpirePendingPaymentsAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();

        var now = DateTime.UtcNow;
        var expired = await db.OnlinePaymentRequests
            .Where(r => r.Status == OnlinePaymentRequestStatus.GatewayPending
                     && r.PaymentExpiryAt != null
                     && r.PaymentExpiryAt <= now
                     && !r.IsDeleted)
            .ToListAsync(ct);

        foreach (var request in expired)
        {
            request.Status = OnlinePaymentRequestStatus.Rejected;
            request.RejectedBy = "system~Expiry";
            request.RejectedAt = now;
            request.AdminNotes = "Auto-expired after 24h";
            request.UpdatedAt = now;
            _logger.LogInformation("Expired payment request {RequestId}", request.Id);
        }

        if (expired.Count > 0)
        {
            await db.SaveChangesAsync(ct);
            _logger.LogInformation("Expired {Count} pending payments", expired.Count);
        }
    }
}
