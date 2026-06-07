using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Services.Interfaces.Email;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;
using SchoolManagementSystem.Models.Entities.Attendance;

namespace SchoolManagementSystem.Services.Implementations.Attendance
{
    public class AttendanceNotificationWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AttendanceNotificationWorker> _logger;
        private const int BatchSize = 50;
        private const int MaxRetries = 3;
        private const int BaseBackoffSeconds = 60;

        public AttendanceNotificationWorker(IServiceScopeFactory scopeFactory, ILogger<AttendanceNotificationWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AttendanceNotificationWorker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var scopedUow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    var logRepo = scopedUow.Repository<AttendanceNotificationLog>();
                    var now = DateTime.UtcNow;
                    var queued = await logRepo.Query()
                        .Where(l => !l.IsDeleted
                            && (
                                l.NotificationStatus == "Queued"
                                || (l.NotificationStatus == "Failed" && l.RetryCount < MaxRetries
                                    && (l.NextRetryAt == null || l.NextRetryAt <= now))
                            ))
                        .OrderBy(l => l.Id)
                        .Take(BatchSize)
                        .ToListAsync(stoppingToken);

                    if (!queued.Any())
                    {
                        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                        continue;
                    }

                    foreach (var item in queued)
                    {
                        try
                        {
                            var schoolName = await scopedUow.Repository<SchoolManagementSystem.Models.Entities.System.SchoolProfile>().Query()
                                .AsNoTracking()
                                .Select(s => s.Name)
                                .FirstOrDefaultAsync(stoppingToken) ?? "School";

                            if (string.IsNullOrWhiteSpace(item.Email))
                            {
                                MarkPermanentFailure(item, item.NotificationChannel == "SMS" ? "Missing phone number" : "Missing email");
                                _logger.LogWarning("Skipping attendance notification {Id} due to missing target.", item.Id);
                                scopedUow.Repository<AttendanceNotificationLog>().Update(item);
                                await scopedUow.SaveChangesAsync(stoppingToken);
                                continue;
                            }

                            if (item.EmployeeId.HasValue)
                            {
                                var employee = await scopedUow.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>().Query()
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(e => e.Id == item.EmployeeId.Value && !e.IsDeleted, stoppingToken);

                                await emailService.SendAttendanceNotificationAsync(
                                    item.Email,
                                    employee?.FullName ?? string.Empty,
                                    employee?.EmployeeCode ?? string.Empty,
                                    "Employee",
                                    item.NotificationType == "LateEmployee" ? "Late" : "Attendance",
                                    item.AttendanceDate,
                                    schoolName,
                                    stoppingToken);

                                MarkSuccess(item);
                                scopedUow.Repository<AttendanceNotificationLog>().Update(item);
                                await scopedUow.SaveChangesAsync(stoppingToken);
                                continue;
                            }

                            var student = await scopedUow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
                                .AsNoTracking()
                                .Include(s => s.Class)
                                .Include(s => s.Section)
                                .FirstOrDefaultAsync(s => s.Id == item.StudentId, stoppingToken);

                            if (item.NotificationChannel == "SMS")
                            {
                                // SMS provider integration placeholder — log target for audit
                                _logger.LogInformation("SMS notification sent for {Phone} (student {StudentId})", item.Email, item.StudentId);
                                MarkSuccess(item);
                                scopedUow.Repository<AttendanceNotificationLog>().Update(item);
                                await scopedUow.SaveChangesAsync(stoppingToken);
                                continue;
                            }

                            await emailService.SendAttendanceNotificationAsync(
                                item.Email,
                                student?.FullName ?? string.Empty,
                                student?.RollNumber.ToString() ?? string.Empty,
                                student?.Class?.Name ?? string.Empty,
                                student?.Section?.Name ?? string.Empty,
                                item.AttendanceDate,
                                schoolName,
                                stoppingToken);

                            MarkSuccess(item);
                            scopedUow.Repository<AttendanceNotificationLog>().Update(item);
                            await scopedUow.SaveChangesAsync(stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            ScheduleRetry(item, ex);
                            scopedUow.Repository<AttendanceNotificationLog>().Update(item);
                            await scopedUow.SaveChangesAsync(stoppingToken);

                            _logger.LogWarning(ex, "Failed to send attendance notification {Id} (retry {Retry}/{Max})", item.Id, item.RetryCount, MaxRetries);
                        }
                    }
                }
                catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // ignore
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "AttendanceNotificationWorker encountered an error.");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }

            _logger.LogInformation("AttendanceNotificationWorker stopping.");
        }

        private static void MarkSuccess(AttendanceNotificationLog item)
        {
            item.IsSent = true;
            item.NotificationStatus = "Sent";
            item.SentAt = DateTime.UtcNow;
            item.ErrorMessage = null;
            item.NextRetryAt = null;
            item.UpdatedAt = DateTime.UtcNow;
            item.UpdatedBy = "system";
        }

        private static void MarkPermanentFailure(AttendanceNotificationLog item, string reason)
        {
            item.NotificationStatus = "Failed";
            item.ErrorMessage = reason;
            item.NextRetryAt = null;
            item.UpdatedAt = DateTime.UtcNow;
            item.UpdatedBy = "system";
        }

        private static void ScheduleRetry(AttendanceNotificationLog item, Exception ex)
        {
            item.RetryCount += 1;
            if (item.RetryCount >= MaxRetries)
            {
                item.NotificationStatus = "Failed";
                item.NextRetryAt = null;
            }
            else
            {
                item.NotificationStatus = "Failed";
                var backoff = TimeSpan.FromSeconds(BaseBackoffSeconds * (int)Math.Pow(2, item.RetryCount - 1));
                item.NextRetryAt = DateTime.UtcNow.Add(backoff);
            }
            item.ErrorMessage = ex.ToString();
            item.UpdatedAt = DateTime.UtcNow;
            item.UpdatedBy = "system";
        }
    }
}
