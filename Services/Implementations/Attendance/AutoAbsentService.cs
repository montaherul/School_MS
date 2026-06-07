using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Attendance
{
    public class AutoAbsentService : IAutoAbsentService
    {
        private readonly IUnitOfWork _uow;
        private readonly IAttendanceSettingService _settingService;
        private readonly IAttendanceValidationService _validationService;
        private readonly ILogger<AutoAbsentService> _logger;

        public AutoAbsentService(
            IUnitOfWork uow,
            IAttendanceSettingService settingService,
            IAttendanceValidationService validationService,
            ILogger<AutoAbsentService> logger)
        {
            _uow = uow;
            _settingService = settingService;
            _validationService = validationService;
            _logger = logger;
        }

        public async Task<AutoAbsentExecutionLog?> RunForTodayAsync(string executedBy = "system", CancellationToken ct = default)
        {
            var setting = await _settingService.GetOrCreateDefaultAsync(ct);
            if (!setting.AutoAbsentEnabled)
            {
                _logger.LogInformation("Auto-Absent is disabled in settings; skipping run.");
                return null;
            }
            return await RunForDateAsync(DateTime.Today, executedBy, ct);
        }

        public async Task<AutoAbsentExecutionLog> RunForDateAsync(DateTime targetDate, string executedBy = "system", CancellationToken ct = default)
        {
            var stopwatch = Stopwatch.StartNew();
            var log = new AutoAbsentExecutionLog
            {
                ExecutionDate = DateTime.UtcNow,
                TargetDate = targetDate.Date,
                Status = "Success",
                CreatedBy = executedBy,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                if (await _validationService.IsHolidayAsync(DateOnly.FromDateTime(targetDate), ct))
                {
                    log.HolidaysSkipped = 1;
                    log.WorkingDaysEvaluated = 0;
                    log.Message = $"Target date {targetDate:yyyy-MM-dd} is a holiday. Auto-absent skipped.";
                    log.Status = "Skipped";
                    await PersistLogAsync(log, stopwatch, ct);
                    _logger.LogInformation("Auto-Absent skipped for {Date}: holiday.", targetDate);
                    return log;
                }

                if (!await _validationService.IsWorkingDayAsync(DateOnly.FromDateTime(targetDate), ct))
                {
                    log.WeeklyOffsSkipped = 1;
                    log.WorkingDaysEvaluated = 0;
                    log.Message = $"Target date {targetDate:yyyy-MM-dd} is not a working day. Auto-absent skipped.";
                    log.Status = "Skipped";
                    await PersistLogAsync(log, stopwatch, ct);
                    _logger.LogInformation("Auto-Absent skipped for {Date}: weekly off.", targetDate);
                    return log;
                }

                var setting = await _settingService.GetOrCreateDefaultAsync(ct);
                if (!setting.AutoAbsentEnabled)
                {
                    log.Status = "Skipped";
                    log.Message = "Auto-Absent is disabled in attendance settings.";
                    await PersistLogAsync(log, stopwatch, ct);
                    return log;
                }

                log.WorkingDaysEvaluated = 1;

                var (studentsProcessed, studentsMarked) = await AutoMarkStudentsAsync(targetDate, executedBy, ct);
                var (employeesProcessed, employeesMarked) = await AutoMarkEmployeesAsync(targetDate, executedBy, ct);

                log.StudentsProcessed = studentsProcessed;
                log.StudentsMarkedAbsent = studentsMarked;
                log.EmployeesProcessed = employeesProcessed;
                log.EmployeesMarkedAbsent = employeesMarked;
                log.Message = $"Auto-Absent completed. Students: {studentsMarked}/{studentsProcessed} marked. Employees: {employeesMarked}/{employeesProcessed} marked.";

                _logger.LogInformation("Auto-Absent run for {Date}: students {S}/{SP}, employees {E}/{EP}.",
                    targetDate, studentsMarked, studentsProcessed, employeesMarked, employeesProcessed);

                await PersistLogAsync(log, stopwatch, ct);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                log.Status = "Failed";
                log.Message = ex.Message;
                log.DurationMs = (int)stopwatch.ElapsedMilliseconds;
                log.UpdatedAt = DateTime.UtcNow;
                _logger.LogError(ex, "Auto-Absent failed for {Date}", targetDate);
                try
                {
                    await PersistLogAsync(log, stopwatch, ct);
                }
                catch
                {
                    /* swallow */
                }
            }

            return log;
        }

        public async Task<List<AutoAbsentExecutionLog>> GetRecentExecutionsAsync(int count, CancellationToken ct = default)
        {
            return await _uow.Repository<AutoAbsentExecutionLog>().Query()
                .AsNoTracking()
                .OrderByDescending(x => x.ExecutionDate)
                .Take(Math.Clamp(count, 1, 100))
                .ToListAsync(ct);
        }

        public async Task<AutoAbsentExecutionLog?> GetLastExecutionAsync(CancellationToken ct = default)
        {
            return await _uow.Repository<AutoAbsentExecutionLog>().Query()
                .AsNoTracking()
                .OrderByDescending(x => x.ExecutionDate)
                .FirstOrDefaultAsync(ct);
        }

        private async Task<(int Processed, int Marked)> AutoMarkStudentsAsync(DateTime targetDate, string executedBy, CancellationToken ct)
        {
            var dateOnly = DateOnly.FromDateTime(targetDate.Date);
            var students = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
                .AsNoTracking()
                .Where(s => s.Status == StudentStatus.Active && !s.IsDeleted)
                .Select(s => new { s.Id, s.ClassId, s.SectionId, s.StudentGroupId })
                .ToListAsync(ct);

            if (students.Count == 0) return (0, 0);

            var existing = await _uow.Repository<AttendanceRecord>().Query()
                .Where(a => a.AttendanceDate == dateOnly && !a.IsDeleted)
                .Select(a => a.StudentId)
                .ToListAsync(ct);
            var existingSet = new HashSet<int>(existing);

            var toMark = students.Where(s => !existingSet.Contains(s.Id)).ToList();
            if (toMark.Count == 0) return (students.Count, 0);

            var records = toMark.Select(s => new AttendanceRecord
            {
                StudentId = s.Id,
                SchoolClassId = s.ClassId,
                SectionId = s.SectionId,
                AttendanceDate = dateOnly,
                Status = AttendanceStatus.Absent,
                Remarks = "Auto-marked absent by system",
                CreatedBy = executedBy,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _uow.Repository<AttendanceRecord>().AddRangeAsync(records, ct);
            await _uow.SaveChangesAsync(ct);

            await _uow.Repository<AttendanceLog>().AddAsync(new AttendanceLog
            {
                UserId = executedBy,
                Action = $"Auto-marked {records.Count} students as absent for {dateOnly:yyyy-MM-dd}",
                EntityName = "AttendanceRecord",
                EntityId = 0
            }, ct);
            await _uow.SaveChangesAsync(ct);

            return (students.Count, toMark.Count);
        }

        private async Task<(int Processed, int Marked)> AutoMarkEmployeesAsync(DateTime targetDate, string executedBy, CancellationToken ct)
        {
            var dateOnly = targetDate.Date;
            var employees = await _uow.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>().Query()
                .AsNoTracking()
                .Where(e => e.Status == "Active" && !e.IsDeleted)
                .Select(e => e.Id)
                .ToListAsync(ct);

            if (employees.Count == 0) return (0, 0);

            var existing = await _uow.Repository<EmployeeAttendance>().Query()
                .Where(a => a.AttendanceDate.Date == dateOnly && !a.IsDeleted)
                .Select(a => a.EmployeeId)
                .ToListAsync(ct);
            var existingSet = new HashSet<int>(existing);

            var toMark = employees.Where(id => !existingSet.Contains(id)).ToList();
            if (toMark.Count == 0) return (employees.Count, 0);

            var records = toMark.Select(id => new EmployeeAttendance
            {
                EmployeeId = id,
                AttendanceDate = dateOnly,
                Status = AttendanceStatus.Absent,
                Remarks = "Auto-marked absent by system",
                CreatedBy = executedBy,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _uow.Repository<EmployeeAttendance>().AddRangeAsync(records, ct);
            await _uow.SaveChangesAsync(ct);

            await _uow.Repository<AttendanceLog>().AddAsync(new AttendanceLog
            {
                UserId = executedBy,
                Action = $"Auto-marked {records.Count} employees as absent for {dateOnly:yyyy-MM-dd}",
                EntityName = "EmployeeAttendance",
                EntityId = 0
            }, ct);
            await _uow.SaveChangesAsync(ct);

            return (employees.Count, toMark.Count);
        }

        private async Task PersistLogAsync(AutoAbsentExecutionLog log, Stopwatch stopwatch, CancellationToken ct)
        {
            stopwatch.Stop();
            log.DurationMs = (int)stopwatch.ElapsedMilliseconds;
            log.UpdatedAt = DateTime.UtcNow;
            await _uow.Repository<AutoAbsentExecutionLog>().AddAsync(log, ct);
            await _uow.SaveChangesAsync(ct);
        }
    }
}
