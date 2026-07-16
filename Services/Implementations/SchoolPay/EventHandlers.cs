using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public static class EventBusRegistrations
{
    public static void RegisterPaymentHandlers(this IEventBus eventBus, IServiceScopeFactory scopeFactory)
    {
        var loggerFactory = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("EventBusRegistrations");

        eventBus.Subscribe<SchoolPayPaymentEvent>(async (evt, ct) =>
        {
            logger.LogInformation("EventBus: Processing {EventType} for transaction {TxnRef}", evt.EventType, evt.TransactionReference);
            
            using var scope = scopeFactory.CreateScope();
            var audit = scope.ServiceProvider.GetRequiredService<ISecurityAuditService>();

            await audit.LogSecurityEventAsync(
                evt.EventType switch
                {
                    "Initiated" => PaymentSecurityEventType.PaymentInitiated,
                    "Completed" => PaymentSecurityEventType.PaymentCompleted,
                    "Failed" => PaymentSecurityEventType.PaymentFailed,
                    _ => PaymentSecurityEventType.PaymentInitiated
                },
                $"Transaction {evt.TransactionReference}, Amount: {evt.Amount:C}",
                "EventBus", null, ct);
        });
    }
}
