using SchoolManagementSystem.Models.DTOs.Academic;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface IAcademicCalendarService
{
        Task<List<AcademicCalendarDto>> GetCalendarDaysAsync(DateTime start, DateTime end, CancellationToken ct = default);
        Task<AcademicCalendarDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<AcademicCalendarUpsertDto?> GetForEditAsync(int id, CancellationToken ct = default);
        Task<int> CreateAsync(AcademicCalendarUpsertDto dto, string createdBy, CancellationToken ct = default);
        Task UpdateAsync(AcademicCalendarUpsertDto dto, string updatedBy, CancellationToken ct = default);
        Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default);
        Task<List<CalendarPublishedEventDto>> GetPublishedEventsAsync(DateTime start, DateTime end, CancellationToken ct = default);
        Task<List<CalendarExamScheduleDto>> GetExamSchedulesAsync(DateOnly start, DateOnly end, CancellationToken ct = default);
    }
