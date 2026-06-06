using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Services.Implementations.Academic
{
    public class AcademicCalendarService : IAcademicCalendarService
    {
        private readonly IAcademicCalendarRepository _calendarRepo;
        private readonly IUnitOfWork _uow;

        public AcademicCalendarService(
            IAcademicCalendarRepository calendarRepo,
            IUnitOfWork uow)
        {
            _calendarRepo = calendarRepo;
            _uow = uow;
        }

        public async Task<List<AcademicCalendar>> GetCalendarDaysAsync(DateTime start, DateTime end, CancellationToken ct = default)
        {
            var startOnly = DateOnly.FromDateTime(start.Date);
            var endOnly = DateOnly.FromDateTime(end.Date);

            return await _calendarRepo.Query()
                .Include(x => x.AcademicYear)
                .Where(x => x.Date >= startOnly && x.Date <= endOnly && !x.IsDeleted)
                .OrderBy(x => x.Date)
                .ToListAsync(ct);
        }

        public async Task<AcademicCalendar?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _calendarRepo.Query()
                .Include(x => x.AcademicYear)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        }

        public async Task<int> CreateAsync(AcademicCalendar entity, CancellationToken ct = default)
        {
            if (entity.AcademicYearId == 0)
            {
                var activeYear = await _uow.Repository<AcademicYear>().Query()
                    .FirstOrDefaultAsync(y => y.IsActive && !y.IsDeleted, ct);
                if (activeYear != null)
                {
                    entity.AcademicYearId = activeYear.Id;
                }
            }

            await _calendarRepo.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return entity.Id;
        }

        public async Task UpdateAsync(AcademicCalendar entity, CancellationToken ct = default)
        {
            _calendarRepo.Update(entity);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _calendarRepo.GetByIdAsync(id, ct);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.UpdatedAt = DateTime.UtcNow;
                _calendarRepo.Update(entity);
                await _uow.SaveChangesAsync(ct);
            }
        }
    }
}