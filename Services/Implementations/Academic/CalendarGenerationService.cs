using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class CalendarGenerationService : ICalendarGenerationService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<CalendarGenerationService> _logger;

    public CalendarGenerationService(IUnitOfWork uow, ILogger<CalendarGenerationService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<List<AcademicCalendar>> GenerateYearAsync(int academicYearId, int year, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        _logger.LogInformation("Calendar generation started for AcademicYearId={AcademicYearId}, Year={Year}", academicYearId, year);

        var result = new List<AcademicCalendar>();
        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var daysCount = DateTime.IsLeapYear(year) ? 366 : 365;
            var existing = await _uow.Repository<AcademicCalendar>().Query()
                .Where(x => x.AcademicYearId == academicYearId && !x.IsDeleted)
                .ToListAsync(ct);

            var existingDates = existing.Select(x => x.Date).ToHashSet();
            var generated = new List<AcademicCalendar>();

            for (int i = 0; i < daysCount; i++)
            {
                var date = new DateOnly(year, 1, 1).AddDays(i);
                if (existingDates.Contains(date)) continue;

                var dayOfWeek = date.DayOfWeek;
                bool isFriday = dayOfWeek == DayOfWeek.Friday;
                bool isSaturday = dayOfWeek == DayOfWeek.Saturday;

                var entry = new AcademicCalendar
                {
                    AcademicYearId = academicYearId,
                    Date = date,
                    Title = isFriday ? "Friday (Weekly Off)" : isSaturday ? "Saturday (Weekly Off)" : GetDefaultTitle(date),
                    Description = string.Empty,
                    IsHoliday = isFriday || isSaturday,
                    IsWorkingDay = !isFriday && !isSaturday,
                    IsExamDay = false,
                    IsEventDay = false,
                    HolidayType = isFriday || isSaturday ? "Weekly Off" : null,
                    IsActive = true,
                    CreatedBy = "system",
                    CreatedAt = DateTime.UtcNow
                };

                generated.Add(entry);
            }

            if (generated.Any())
            {
                await _uow.Repository<AcademicCalendar>().AddRangeAsync(generated, ct);
            }

            await SyncHolidaysAsync(academicYearId, year, ct);
            await SyncExamDaysAsync(academicYearId, year, ct);

            result = generated;
        }, ct);

        sw.Stop();
        _logger.LogInformation("Calendar generation completed for AcademicYearId={AcademicYearId}, Year={Year}, Entries={Count}, Duration={Duration}ms",
            academicYearId, year, result.Count, sw.ElapsedMilliseconds);

        return result;
    }

    public async Task<List<AcademicCalendar>> RegenerateYearAsync(int academicYearId, int year, CancellationToken ct = default)
    {
        var result = new List<AcademicCalendar>();
        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var existing = await _uow.Repository<AcademicCalendar>().Query()
                .Where(x => x.AcademicYearId == academicYearId && !x.IsDeleted)
                .ToListAsync(ct);

            foreach (var entry in existing)
            {
                entry.IsDeleted = true;
                entry.UpdatedAt = DateTime.UtcNow;
                entry.UpdatedBy = "system";
            }

            result = await GenerateYearImplAsync(academicYearId, year, ct);
        }, ct);

        return result;
    }

    private async Task<List<AcademicCalendar>> GenerateYearImplAsync(int academicYearId, int year, CancellationToken ct = default)
    {
        var daysCount = DateTime.IsLeapYear(year) ? 366 : 365;
        var generated = new List<AcademicCalendar>();

        for (int i = 0; i < daysCount; i++)
        {
            var date = new DateOnly(year, 1, 1).AddDays(i);

            var dayOfWeek = date.DayOfWeek;
            bool isFriday = dayOfWeek == DayOfWeek.Friday;
            bool isSaturday = dayOfWeek == DayOfWeek.Saturday;

            var entry = new AcademicCalendar
            {
                AcademicYearId = academicYearId,
                Date = date,
                Title = isFriday ? "Friday (Weekly Off)" : isSaturday ? "Saturday (Weekly Off)" : GetDefaultTitle(date),
                Description = string.Empty,
                IsHoliday = isFriday || isSaturday,
                IsWorkingDay = !isFriday && !isSaturday,
                IsExamDay = false,
                IsEventDay = false,
                HolidayType = isFriday || isSaturday ? "Weekly Off" : null,
                IsActive = true,
                CreatedBy = "system",
                CreatedAt = DateTime.UtcNow
            };

            generated.Add(entry);
        }

        if (generated.Any())
        {
            await _uow.Repository<AcademicCalendar>().AddRangeAsync(generated, ct);
        }

        await SyncHolidaysAsync(academicYearId, year, ct);
        await SyncExamDaysAsync(academicYearId, year, ct);

        return generated;
    }

    public async Task RepairMissingDatesAsync(int academicYearId, int year, CancellationToken ct = default)
    {
        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var daysCount = DateTime.IsLeapYear(year) ? 366 : 365;
            var existing = await _uow.Repository<AcademicCalendar>().Query()
                .Where(x => x.AcademicYearId == academicYearId && !x.IsDeleted)
                .Select(x => x.Date)
                .ToListAsync(ct);

            var existingSet = existing.ToHashSet();
            var missing = new List<AcademicCalendar>();

            for (int i = 0; i < daysCount; i++)
            {
                var date = new DateOnly(year, 1, 1).AddDays(i);
                if (existingSet.Contains(date)) continue;

                var dayOfWeek = date.DayOfWeek;
                bool isHoliday = dayOfWeek == DayOfWeek.Friday || dayOfWeek == DayOfWeek.Saturday;

                missing.Add(new AcademicCalendar
                {
                    AcademicYearId = academicYearId,
                    Date = date,
                    Title = isHoliday ? $"{date.DayOfWeek} (Weekly Off)" : GetDefaultTitle(date),
                    IsHoliday = isHoliday,
                    IsWorkingDay = !isHoliday,
                    IsActive = true,
                    CreatedBy = "system",
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (missing.Any())
            {
                await _uow.Repository<AcademicCalendar>().AddRangeAsync(missing, ct);
            }
        }, ct);
    }

    public async Task<int> SyncHolidaysAsync(int academicYearId, int year, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        _logger.LogInformation("Holiday sync started for AcademicYearId={AcademicYearId}, Year={Year}", academicYearId, year);

        var holidays = await _uow.Repository<HolidayMaster>().Query()
            .Where(h => h.IsActive && !h.IsDeleted)
            .ToListAsync(ct);

        var existingEntries = await _uow.Repository<AcademicCalendar>().Query()
            .Where(x => x.AcademicYearId == academicYearId && !x.IsDeleted)
            .ToListAsync(ct);
        var calendarMap = existingEntries.ToDictionary(x => x.Date);

        var synced = 0;
        foreach (var holiday in holidays)
        {
            var targetDate = holiday.IsRecurring
                ? new DateOnly(year, holiday.HolidayDate.Month, holiday.HolidayDate.Day)
                : holiday.HolidayDate;

            if (targetDate.Year != year) continue;

            if (calendarMap.TryGetValue(targetDate, out var calendarEntry))
            {
                calendarEntry.IsHoliday = true;
                calendarEntry.IsWorkingDay = false;
                calendarEntry.Title = holiday.Name;
                calendarEntry.HolidayType = holiday.HolidayType;
                calendarEntry.Description = holiday.Description ?? holiday.Name;
                calendarEntry.UpdatedAt = DateTime.UtcNow;
                calendarEntry.UpdatedBy = "system";
                synced++;
            }
        }

        if (synced > 0)
            await _uow.SaveChangesAsync(ct);

        sw.Stop();
        _logger.LogInformation("Holiday sync completed for AcademicYearId={AcademicYearId}, Year={Year}, Synced={Count}, Duration={Duration}ms",
            academicYearId, year, synced, sw.ElapsedMilliseconds);

        return synced;
    }

    public async Task<int> SyncExamDaysAsync(int academicYearId, int year, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        _logger.LogInformation("Exam sync started for AcademicYearId={AcademicYearId}, Year={Year}", academicYearId, year);

        var examSchedules = await _uow.Repository<ExamSchedule>().QueryNoTracking()
            .Include(x => x.Exam)
            .Where(x => !x.IsDeleted && x.ExamDate.Year == year)
            .ToListAsync(ct);

        var existingEntries = await _uow.Repository<AcademicCalendar>().Query()
            .Where(x => x.AcademicYearId == academicYearId && !x.IsDeleted)
            .ToListAsync(ct);
        var calendarMap = existingEntries.ToDictionary(x => x.Date);

        var synced = 0;
        var conflicts = new List<string>();

        foreach (var schedule in examSchedules)
        {
            var date = schedule.ExamDate;

            if (!calendarMap.TryGetValue(date, out var calendarEntry)) continue;

            if (calendarEntry.IsHoliday)
            {
                conflicts.Add($"Holiday conflict: Exam '{schedule.Exam?.Name}' on {date} is a holiday ({calendarEntry.Title})");
                continue;
            }

            if (calendarEntry.IsExamDay)
            {
                conflicts.Add($"Duplicate exam: Exam '{schedule.Exam?.Name}' on {date} already marked as exam day");
                continue;
            }

            calendarEntry.IsExamDay = true;
            calendarEntry.IsEventDay = false;
            calendarEntry.Title = $"Exam: {schedule.Exam?.Name ?? "Exam"}";
            calendarEntry.Description = $"Subject scheduled on {date:dd MMM yyyy}";
            calendarEntry.UpdatedAt = DateTime.UtcNow;
            calendarEntry.UpdatedBy = "system";
            synced++;
        }

        if (conflicts.Count > 0)
        {
            _logger.LogWarning("Exam sync conflicts for AcademicYearId={AcademicYearId}: {Conflicts}",
                academicYearId, string.Join("; ", conflicts));
        }

        if (synced > 0)
            await _uow.SaveChangesAsync(ct);

        sw.Stop();
        _logger.LogInformation("Exam sync completed for AcademicYearId={AcademicYearId}, Year={Year}, Synced={Count}, Conflicts={ConflictCount}, Duration={Duration}ms",
            academicYearId, year, synced, conflicts.Count, sw.ElapsedMilliseconds);

        return synced;
    }

    public async Task ValidateCalendarAsync(int academicYearId, CancellationToken ct = default)
    {
        var entries = await _uow.Repository<AcademicCalendar>().Query()
            .Where(x => x.AcademicYearId == academicYearId && !x.IsDeleted)
            .ToListAsync(ct);

        foreach (var entry in entries)
        {
            var isValid = true;
            var remarks = new List<string>();

            if (entry.IsHoliday && entry.IsWorkingDay)
            {
                isValid = false;
                remarks.Add("Cannot be both holiday and working day");
            }

            if (entry.IsExamDay && entry.IsHoliday)
            {
                isValid = false;
                remarks.Add("Exam cannot be on a holiday");
            }

            if (!isValid)
            {
                entry.Remarks = string.Join("; ", remarks);
                entry.UpdatedAt = DateTime.UtcNow;
                entry.UpdatedBy = "system";
            }
        }

        await _uow.SaveChangesAsync(ct);
    }

    private static string GetDefaultTitle(DateOnly date)
    {
        return date.DayOfWeek switch
        {
            DayOfWeek.Friday => "Friday (Weekly Off)",
            DayOfWeek.Saturday => "Saturday (Weekly Off)",
            _ => "Working Day"
        };
    }
}
