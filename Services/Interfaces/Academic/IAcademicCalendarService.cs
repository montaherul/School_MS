using SchoolManagementSystem.Models.Entities.Academic;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Services.Interfaces.Academic
{
    public interface IAcademicCalendarService
    {
        Task<List<AcademicCalendar>> GetCalendarDaysAsync(DateTime start, DateTime end, CancellationToken ct = default);
        Task<AcademicCalendar?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<int> CreateAsync(AcademicCalendar entity, CancellationToken ct = default);
        Task UpdateAsync(AcademicCalendar entity, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}