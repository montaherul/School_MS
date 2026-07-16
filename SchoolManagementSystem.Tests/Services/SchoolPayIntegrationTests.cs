using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Entities.SchoolPay;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.SchoolPay;
using SchoolManagementSystem.Services.Implementations.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class SchoolPayIntegrationTests
{
    private readonly Mock<ISchoolPayRepository> _repoMock;
    private readonly Mock<ILogger<CheckoutService>> _checkoutLoggerMock;
    private readonly Mock<ILogger<DeadLetterQueueService>> _dlqLoggerMock;
    private readonly Mock<ILogger<ReconciliationService>> _reconLoggerMock;
    private readonly Mock<ILogger<FailoverService>> _failoverLoggerMock;
    private readonly Mock<ILogger<EventBus>> _eventBusLoggerMock;
    private readonly Mock<IProviderManagementService> _providerMgmtMock;
    private readonly Mock<IPaymentRoutingService> _routingMock;
    private readonly GatewayFactory _factory;
    private readonly IEventBus _eventBus;

    public SchoolPayIntegrationTests()
    {
        _repoMock = new Mock<ISchoolPayRepository>();
        _checkoutLoggerMock = new Mock<ILogger<CheckoutService>>();
        _dlqLoggerMock = new Mock<ILogger<DeadLetterQueueService>>();
        _reconLoggerMock = new Mock<ILogger<ReconciliationService>>();
        _failoverLoggerMock = new Mock<ILogger<FailoverService>>();
        _eventBusLoggerMock = new Mock<ILogger<EventBus>>();
        _providerMgmtMock = new Mock<IProviderManagementService>();
        _routingMock = new Mock<IPaymentRoutingService>();
        var svcProvider = new Mock<IServiceProvider>();
        var scope = new Mock<IServiceScope>();
        var scopeFactory = new Mock<IServiceScopeFactory>();
        scope.Setup(s => s.ServiceProvider).Returns(svcProvider.Object);
        scopeFactory.Setup(f => f.CreateScope()).Returns(scope.Object);
        _factory = new GatewayFactory(scopeFactory.Object, Mock.Of<ILogger<GatewayFactory>>());
        _eventBus = new EventBus(_eventBusLoggerMock.Object);
    }

    [Fact]
    public async Task DeadLetterQueue_Reprocess_MovesToReceived()
    {
        var dlqService = new DeadLetterQueueService(_repoMock.Object, _dlqLoggerMock.Object);
        _repoMock.Setup(r => r.ReprocessDeadLetterAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await dlqService.ReprocessAsync(1);

        _repoMock.Verify(r => r.ReprocessDeadLetterAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeadLetterQueue_Ignore_MovesToIgnored()
    {
        var dlqService = new DeadLetterQueueService(_repoMock.Object, _dlqLoggerMock.Object);
        _repoMock.Setup(r => r.IgnoreDeadLetterAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await dlqService.IgnoreAsync(1);

        _repoMock.Verify(r => r.IgnoreDeadLetterAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Reconciliation_RunReconciliation_UpdatesSettlementStatus()
    {
        var reconService = new ReconciliationService(_repoMock.Object, _reconLoggerMock.Object);
        var settlementId = 1;

        var result = new SchoolPayReconciliationResultDto
        {
            SettlementId = settlementId,
            SettlementReference = "STL-001",
            SettlementAmount = 1000m,
            MatchedAmount = 1000m,
            Difference = 0m,
            MatchedTransactionCount = 1,
            UnmatchedTransactionCount = 0
        };

        _repoMock.Setup(r => r.GetReconciliationDataAsync(settlementId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SchoolPayReconciliationResultDto> { result });

        _repoMock.Setup(r => r.GetSettlementEntityByIdAsync(settlementId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentGatewaySettlement
            {
                Id = settlementId,
                SettlementReference = "STL-001",
                Amount = 1000m,
                PaymentProviderId = 1,
                Status = SettlementStatus.Pending
            });

        var actual = await reconService.RunReconciliationAsync(settlementId);

        Assert.NotNull(actual);
        Assert.Equal(0m, actual!.Difference);
        _repoMock.Verify(r => r.UpdateSettlementAsync(It.IsAny<PaymentGatewaySettlement>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Reconciliation_RunReconciliation_MismatchMarksDisputed()
    {
        var reconService = new ReconciliationService(_repoMock.Object, _reconLoggerMock.Object);
        var settlementId = 2;

        var result = new SchoolPayReconciliationResultDto
        {
            SettlementId = settlementId,
            SettlementReference = "STL-002",
            SettlementAmount = 1000m,
            MatchedAmount = 950m,
            Difference = 50m,
            MatchedTransactionCount = 1,
            UnmatchedTransactionCount = 1
        };

        _repoMock.Setup(r => r.GetReconciliationDataAsync(settlementId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SchoolPayReconciliationResultDto> { result });

        _repoMock.Setup(r => r.GetSettlementEntityByIdAsync(settlementId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentGatewaySettlement
            {
                Id = settlementId,
                SettlementReference = "STL-002",
                Amount = 1000m,
                PaymentProviderId = 1,
                Status = SettlementStatus.Pending
            });

        var actual = await reconService.RunReconciliationAsync(settlementId);

        Assert.NotNull(actual);
        Assert.NotEqual(0m, actual!.Difference);
        _repoMock.Verify(r => r.UpdateSettlementAsync(It.Is<PaymentGatewaySettlement>(s => s.Status == SettlementStatus.Disputed), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FailoverService_ResolveWithFailover_ReturnsHealthyProvider()
    {
        var repoMock = new Mock<ISchoolPayRepository>();
        var routingMock = new Mock<IPaymentRoutingService>();
        var loggerMock = new Mock<ILogger<FailoverService>>();

        var failoverService = new FailoverService(repoMock.Object, routingMock.Object, loggerMock.Object);

        repoMock.Setup(r => r.GetActiveRouteRulesForAmountAsync(1000m, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SchoolPayRouteRuleDto>
            {
                new() { PaymentProviderId = 1, RuleName = "Primary", Priority = PaymentRoutePriority.Primary },
                new() { PaymentProviderId = 2, RuleName = "Backup", Priority = PaymentRoutePriority.Secondary }
            });

        repoMock.Setup(r => r.GetLatestHealthStatusAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SchoolPayHealthStatusDto>
            {
                new() { ProviderId = 1, Status = ProviderHealthStatus.Unhealthy },
                new() { ProviderId = 2, Status = ProviderHealthStatus.Healthy }
            });

        var providerId = await failoverService.ResolveWithFailoverAsync(1000m);

        // Should skip provider 1 (unhealthy) and return provider 2 (healthy)
        Assert.Equal(2, providerId);
    }

    [Fact]
    public async Task FailoverService_ResolveWithFailover_AllUnhealthy_ReturnsNull()
    {
        var repoMock = new Mock<ISchoolPayRepository>();
        var routingMock = new Mock<IPaymentRoutingService>();
        var loggerMock = new Mock<ILogger<FailoverService>>();

        var failoverService = new FailoverService(repoMock.Object, routingMock.Object, loggerMock.Object);

        repoMock.Setup(r => r.GetActiveRouteRulesForAmountAsync(1000m, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SchoolPayRouteRuleDto>
            {
                new() { PaymentProviderId = 1, RuleName = "Primary", Priority = PaymentRoutePriority.Primary }
            });

        repoMock.Setup(r => r.GetLatestHealthStatusAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SchoolPayHealthStatusDto>
            {
                new() { ProviderId = 1, Status = ProviderHealthStatus.Unhealthy }
            });

        var providerId = await failoverService.ResolveWithFailoverAsync(1000m);

        Assert.Null(providerId);
    }

    [Fact]
    public async Task EventBus_Publish_CallsRegisteredHandlers()
    {
        var handlerCalled = false;

        _eventBus.Subscribe<SchoolPayPaymentEvent>(async (evt, ct) =>
        {
            handlerCalled = true;
            await Task.CompletedTask;
        });

        await _eventBus.PublishAsync(new SchoolPayPaymentEvent
        {
            EventType = "Test",
            TransactionReference = "TXN-001",
            Amount = 100
        });

        Assert.True(handlerCalled);
    }

    [Fact]
    public async Task EventBus_Publish_MultipleHandlers_AllCalled()
    {
        var callCount = 0;

        _eventBus.Subscribe<SchoolPayPaymentEvent>(async (evt, ct) =>
        {
            Interlocked.Increment(ref callCount);
            await Task.CompletedTask;
        });

        _eventBus.Subscribe<SchoolPayPaymentEvent>(async (evt, ct) =>
        {
            Interlocked.Increment(ref callCount);
            await Task.CompletedTask;
        });

        await _eventBus.PublishAsync(new SchoolPayPaymentEvent
        {
            EventType = "Test",
            TransactionReference = "TXN-002"
        });

        Assert.Equal(2, callCount);
    }

    [Fact]
    public void WebhookSignatureValidator_ValidatesCorrectly()
    {
        var validator = new WebhookSignatureValidator(Mock.Of<ILogger<WebhookSignatureValidator>>());
        var payload = "{\"event\":\"payment.success\",\"txn\":\"TXN-001\"}";
        var secret = "test_secret_key_12345";

        var signature = validator.GenerateSignature(payload, secret);
        var isValid = validator.ValidateSignature(payload, signature, secret);

        Assert.True(isValid);
    }

    [Fact]
    public void WebhookSignatureValidator_WrongSignature_Fails()
    {
        var validator = new WebhookSignatureValidator(Mock.Of<ILogger<WebhookSignatureValidator>>());
        var payload = "{\"event\":\"payment.success\"}";
        var secret = "test_secret";
        var wrongSignature = "invalid_signature";

        var isValid = validator.ValidateSignature(payload, wrongSignature, secret);

        Assert.False(isValid);
    }

    [Fact]
    public void WebhookSignatureValidator_EmptyPayload_Fails()
    {
        var validator = new WebhookSignatureValidator(Mock.Of<ILogger<WebhookSignatureValidator>>());
        var isValid = validator.ValidateSignature("", "sig", "key");
        Assert.False(isValid);
    }

    [Fact]
    public async Task OperationsCenter_ReturnsData()
    {
        var operationsService = new OperationsCenterService(_repoMock.Object, Mock.Of<ILogger<OperationsCenterService>>());

        _repoMock.Setup(r => r.GetOperationsDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SchoolPayOperationsDto
            {
                PendingPayments = 5,
                GatewayPending = 2,
                WebhookQueueSize = 3,
                FailedPayments = 1,
                HealthyProviders = 2
            });

        var data = await operationsService.GetOperationsDataAsync();

        Assert.Equal(5, data.PendingPayments);
        Assert.Equal(2, data.GatewayPending);
        Assert.Equal(3, data.WebhookQueueSize);
        Assert.Equal(1, data.FailedPayments);
        Assert.Equal(2, data.HealthyProviders);
    }

    [Fact]
    public async Task MonitoringService_ReturnsMonitoringData()
    {
        var monitoringService = new MonitoringService(_repoMock.Object, Mock.Of<ILogger<MonitoringService>>());

        _repoMock.Setup(r => r.GetMonitoringDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SchoolPayMonitoringDto
            {
                QueueMetrics = new SchoolPayQueueMetricsDto
                {
                    WebhookQueueDepth = 5,
                    DlqDepth = 1
                },
                Trends = new SchoolPayTrendDto
                {
                    SuccessRate24h = 98.5,
                    TotalTransactions24h = 150
                }
            });

        var data = await monitoringService.GetMonitoringDataAsync();

        Assert.Equal(5, data.QueueMetrics.WebhookQueueDepth);
        Assert.Equal(1, data.QueueMetrics.DlqDepth);
        Assert.Equal(98.5, data.Trends.SuccessRate24h);
        Assert.Equal(150, data.Trends.TotalTransactions24h);
    }

    [Fact]
    public async Task SecurityAuditService_LogsEvent()
    {
        var auditService = new SecurityAuditService(_repoMock.Object, Mock.Of<ILogger<SecurityAuditService>>());

        _repoMock.Setup(r => r.LogSecurityEventAsync(
            It.IsAny<PaymentSecurityEventType>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await auditService.LogSecurityEventAsync(PaymentSecurityEventType.SignatureVerified, "Test details", "tester", "127.0.0.1");

        _repoMock.Verify(r => r.LogSecurityEventAsync(PaymentSecurityEventType.SignatureVerified, "Test details", "tester", "127.0.0.1", It.IsAny<CancellationToken>()), Times.Once);
    }
}
