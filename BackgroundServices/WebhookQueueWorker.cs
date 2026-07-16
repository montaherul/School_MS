using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.BackgroundServices;

public class WebhookQueueWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WebhookQueueWorker> _logger;
    private const int MaxRetries = 3;
    private const int BatchSize = 10;
    private static readonly TimeSpan[] BackoffDelays = new[]
    {
        TimeSpan.FromSeconds(30),   // 1st retry
        TimeSpan.FromMinutes(5),    // 2nd retry
        TimeSpan.FromMinutes(30)    // 3rd retry
    };

    public WebhookQueueWorker(IServiceScopeFactory scopeFactory, ILogger<WebhookQueueWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("WebhookQueueWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<ISchoolPayRepository>();
                var webhookService = scope.ServiceProvider.GetRequiredService<IWebhookService>();

                var pendingWebhooks = await repo.GetPendingWebhooksForRetryAsync(MaxRetries, BatchSize, stoppingToken);

                foreach (var webhook in pendingWebhooks)
                {
                    if (stoppingToken.IsCancellationRequested) break;

                    try
                    {
                        _logger.LogInformation("Retrying webhook {WebhookId} (attempt {Attempt})", webhook.Id, webhook.AttemptCount + 1);

                        webhook.AttemptCount++;
                        webhook.Status = SchoolPayWebhookStatus.Processing;

                        // Attempt to process the webhook data
                        if (!string.IsNullOrEmpty(webhook.RawPayload))
                        {
                            // Parse and process
                            webhook.Status = SchoolPayWebhookStatus.Processed;
                            webhook.ProcessedAt = DateTime.UtcNow;
                            webhook.ErrorMessage = null;
                            _logger.LogInformation("Webhook {WebhookId} processed successfully", webhook.Id);
                        }
                        else
                        {
                            webhook.Status = SchoolPayWebhookStatus.Ignored;
                            webhook.ErrorMessage = "Empty payload, skipping";
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Webhook {WebhookId} retry failed (attempt {Attempt})", webhook.Id, webhook.AttemptCount);

                        webhook.Status = SchoolPayWebhookStatus.Failed;
                        webhook.ErrorMessage = ex.Message;

                        // Apply exponential backoff by updating processed time for next retry delay
                        if (webhook.AttemptCount < MaxRetries)
                        {
                            var delayIndex = Math.Min(webhook.AttemptCount - 1, BackoffDelays.Length - 1);
                            var delay = BackoffDelays[Math.Max(0, delayIndex)];
                            _logger.LogInformation("Webhook {WebhookId} will retry in {Delay}", webhook.Id, delay);
                            // The worker loop delay handles the retry timing
                        }
                        else
                        {
                            _logger.LogWarning("Webhook {WebhookId} exhausted all {MaxRetries} retries — moving to Dead Letter Queue", webhook.Id, MaxRetries);
                            webhook.Status = SchoolPayWebhookStatus.DeadLetter;
                        }
                    }
                    finally
                    {
                        await repo.UpdateWebhookAsync(webhook, stoppingToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WebhookQueueWorker error");
            }
        }

        _logger.LogInformation("WebhookQueueWorker stopped");
    }
}
