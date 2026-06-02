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
                    var queued = await logRepo.Query()
                        .Where(l => l.NotificationStatus == "Queued" && !l.IsDeleted)
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
                            // Attempt to send
                            var student = await scopedUow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
                                .AsNoTracking()
                                .Include(s => s.Class)
                                .Include(s => s.Section)
                                .FirstOrDefaultAsync(s => s.Id == item.StudentId, stoppingToken);

                            var schoolName = await scopedUow.Repository<SchoolManagementSystem.Models.Entities.System.SchoolProfile>().Query()
                                .AsNoTracking()
                                .Select(s => s.Name)
                                .FirstOrDefaultAsync(stoppingToken) ?? "School";

                            if (string.IsNullOrWhiteSpace(item.Email))
                            {
                                item.NotificationStatus = "Failed";
                                item.ErrorMessage = "Missing email";
                                item.UpdatedAt = DateTime.UtcNow;
                                item.UpdatedBy = "system";
                                _logger.LogWarning("Skipping attendance notification {Id} due to missing email.", item.Id);
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

                            item.IsSent = true;
                            item.NotificationStatus = "Sent";
                            item.SentAt = DateTime.UtcNow;
                            item.ErrorMessage = null;
                            item.UpdatedAt = DateTime.UtcNow;
                            item.UpdatedBy = "system";

                            scopedUow.Repository<AttendanceNotificationLog>().Update(item);
                            await scopedUow.SaveChangesAsync(stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            item.NotificationStatus = "Failed";
                            item.ErrorMessage = ex.ToString();
                            item.UpdatedAt = DateTime.UtcNow;
                            item.UpdatedBy = "system";
                            scopedUow.Repository<AttendanceNotificationLog>().Update(item);
                            await scopedUow.SaveChangesAsync(stoppingToken);

                            _logger.LogError(ex, "Failed to send attendance notification {Id}", item.Id);
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
    }
}
