using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.Calendar;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class CalendarDashboardService : ICalendarDashboardService
{
    private readonly IUnitOfWork _uow;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CalendarDashboardService> _logger;
    private const string CachePrefix = "CalWidget_";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);
    private int _cacheHits;
    private int _cacheMisses;

    public CalendarDashboardService(IUnitOfWork uow, IMemoryCache cache, ILogger<CalendarDashboardService> logger)
    {
        _uow = uow;
        _cache = cache;
        _logger = logger;
    }

    public double CacheHitRatio => (_cacheHits + _cacheMisses) > 0
        ? Math.Round((double)_cacheHits / (_cacheHits + _cacheMisses) * 100, 1)
        : 0;

    public async Task<List<UpcomingHolidayDto>> GetUpcomingHolidaysAsync(int count = 5, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}Holidays_{count}";
        if (_cache.TryGetValue(cacheKey, out List<UpcomingHolidayDto>? cached))
        {
            Interlocked.Increment(ref _cacheHits);
            return cached!;
        }
        Interlocked.Increment(ref _cacheMisses);

        var today = DateOnly.FromDateTime(DateTime.Today);
        var result = await _uow.Repository<AcademicCalendar>().Query().AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsHoliday && x.Date >= today && !x.IsExamDay)
            .OrderBy(x => x.Date)
            .Take(count)
            .Select(x => new UpcomingHolidayDto
            {
                Date = x.Date,
                Name = x.Title ?? "Holiday",
                HolidayType = x.HolidayType ?? "General"
            })
            .ToListAsync(ct);

        _cache.Set(cacheKey, result, CacheDuration);
        return result;
    }

    public async Task<List<UpcomingExamDto>> GetUpcomingExamsAsync(int count = 5, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}Exams_{count}";
        if (_cache.TryGetValue(cacheKey, out List<UpcomingExamDto>? cached))
        {
            Interlocked.Increment(ref _cacheHits);
            return cached!;
        }
        Interlocked.Increment(ref _cacheMisses);

        var today = DateOnly.FromDateTime(DateTime.Today);
        var result = await _uow.Repository<AcademicCalendar>().Query().AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsExamDay && x.Date >= today)
            .OrderBy(x => x.Date)
            .Take(count)
            .Select(x => new UpcomingExamDto
            {
                Date = x.Date,
                ExamName = x.Title ?? "Exam",
                Subject = x.Description ?? ""
            })
            .ToListAsync(ct);

        _cache.Set(cacheKey, result, CacheDuration);
        return result;
    }

    public async Task<List<UpcomingEventDto>> GetUpcomingEventsAsync(int count = 5, CancellationToken ct = default)
    {
        var cacheKey = $"{CachePrefix}Events_{count}";
        if (_cache.TryGetValue(cacheKey, out List<UpcomingEventDto>? cached))
        {
            Interlocked.Increment(ref _cacheHits);
            return cached!;
        }
        Interlocked.Increment(ref _cacheMisses);

        var today = DateOnly.FromDateTime(DateTime.Today);
        var result = await _uow.Repository<AcademicCalendar>().Query().AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsEventDay && x.Date >= today)
            .OrderBy(x => x.Date)
            .Take(count)
            .Select(x => new UpcomingEventDto
            {
                Date = x.Date,
                Title = x.Title ?? "Event",
                Description = x.Description ?? ""
            })
            .ToListAsync(ct);

        _cache.Set(cacheKey, result, CacheDuration);
        return result;
    }

    public async Task<MonthSummaryDto> GetCurrentMonthSummaryAsync(CancellationToken ct = default)
    {
        var today = DateTime.Today;
        var cacheKey = $"{CachePrefix}MonthSummary_{today.Year}_{today.Month}";
        if (_cache.TryGetValue(cacheKey, out MonthSummaryDto? cached))
        {
            Interlocked.Increment(ref _cacheHits);
            return cached!;
        }
        Interlocked.Increment(ref _cacheMisses);

        var currentMonth = today.Month;
        var currentYear = today.Year;
        var startDate = new DateOnly(currentYear, currentMonth, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var entries = await _uow.Repository<AcademicCalendar>().Query().AsNoTracking()
            .Where(x => !x.IsDeleted && x.Date >= startDate && x.Date <= endDate)
            .ToListAsync(ct);

        var totalDays = endDate.Day;
        var result = new MonthSummaryDto
        {
            TotalDays = totalDays,
            WorkingDays = entries.Count(x => x.IsWorkingDay),
            HolidayCount = entries.Count(x => x.IsHoliday),
            ExamDayCount = entries.Count(x => x.IsExamDay),
            MonthName = today.ToString("MMMM"),
            Year = currentYear
        };

        _cache.Set(cacheKey, result, CacheDuration);
        return result;
    }

    public async Task<CalendarWidgetDto> GetAllWidgetsAsync(CancellationToken ct = default)
    {
        var holidays = await GetUpcomingHolidaysAsync(5, ct);
        var exams = await GetUpcomingExamsAsync(5, ct);
        var events = await GetUpcomingEventsAsync(5, ct);
        var summary = await GetCurrentMonthSummaryAsync(ct);

        return new CalendarWidgetDto
        {
            UpcomingHolidays = holidays,
            UpcomingExams = exams,
            UpcomingEvents = events,
            MonthSummary = summary
        };
    }
}
