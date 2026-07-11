using SchoolManagementSystem.Models.DTOs.Academic;

namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface IAcademicCalendarEventService
{
    Task<AcademicCalendarDto?> GetCalendarByIdAsync(int calendarId, CancellationToken ct = default);
    Task<List<AcademicCalendarEventDto>> GetEventsByCalendarAsync(int calendarId, CancellationToken ct = default);
    Task<AcademicCalendarEventDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(AcademicCalendarEventDto dto, string createdBy, CancellationToken ct = default);
    Task UpdateAsync(AcademicCalendarEventDto dto, string updatedBy, CancellationToken ct = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default);
}
