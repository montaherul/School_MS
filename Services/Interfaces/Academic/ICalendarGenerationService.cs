using SchoolManagementSystem.Models.Entities.Academic;

namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface ICalendarGenerationService
{
    Task<List<AcademicCalendar>> GenerateYearAsync(int academicYearId, int year, CancellationToken ct = default);
    Task<List<AcademicCalendar>> RegenerateYearAsync(int academicYearId, int year, CancellationToken ct = default);
    Task RepairMissingDatesAsync(int academicYearId, int year, CancellationToken ct = default);
    Task<int> SyncHolidaysAsync(int academicYearId, int year, CancellationToken ct = default);
    Task<int> SyncExamDaysAsync(int academicYearId, int year, CancellationToken ct = default);
    Task ValidateCalendarAsync(int academicYearId, CancellationToken ct = default);
}
