using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Services.Interfaces.Attendance;

namespace SchoolManagementSystem.Services.Implementations.Attendance
{
    public class AutoAbsentWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AutoAbsentWorker> _logger;
        private const int PollIntervalSeconds = 60;

        public AutoAbsentWorker(IServiceScopeFactory scopeFactory, ILogger<AutoAbsentWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AutoAbsentWorker started.");

            var lastRunDate = DateTime.MinValue.Date;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.Now;

                    using var scope = _scopeFactory.CreateScope();
                    var settingService = scope.ServiceProvider.GetRequiredService<IAttendanceSettingService>();
                    var setting = await settingService.GetOrCreateDefaultAsync(stoppingToken);

                    if (!setting.AutoAbsentEnabled)
                    {
                        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                        continue;
                    }

                    var targetDate = now.Date;
                    if (now.TimeOfDay >= setting.AutoAbsentTime.ToTimeSpan() && lastRunDate < targetDate)
                    {
                        var autoAbsent = scope.ServiceProvider.GetRequiredService<IAutoAbsentService>();
                        var log = await autoAbsent.RunForDateAsync(targetDate, "system:AutoAbsentWorker", stoppingToken);
                        if (log != null)
                        {
                            _logger.LogInformation("Auto-Absent ran for {Date}: Status={Status}, Students={SMarked}/{SProc}, Employees={EMarked}/{EProc}.",
                                targetDate, log.Status, log.StudentsMarkedAbsent, log.StudentsProcessed, log.EmployeesMarkedAbsent, log.EmployeesProcessed);
                        }
                        lastRunDate = targetDate;
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    /* shutdown */
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "AutoAbsentWorker encountered an error.");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(PollIntervalSeconds), stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }

            _logger.LogInformation("AutoAbsentWorker stopping.");
        }
    }
}
