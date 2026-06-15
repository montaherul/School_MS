using SchoolManagementSystem.Models.DTOs.Calendar;

namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface ICalendarDashboardService
{
    Task<List<UpcomingHolidayDto>> GetUpcomingHolidaysAsync(int count = 5, CancellationToken ct = default);
    Task<List<UpcomingExamDto>> GetUpcomingExamsAsync(int count = 5, CancellationToken ct = default);
    Task<List<UpcomingEventDto>> GetUpcomingEventsAsync(int count = 5, CancellationToken ct = default);
    Task<MonthSummaryDto> GetCurrentMonthSummaryAsync(CancellationToken ct = default);
    Task<CalendarWidgetDto> GetAllWidgetsAsync(CancellationToken ct = default);
}
