using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Repositories.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class AcademicCalendarService : IAcademicCalendarService
{
        private readonly IAcademicCalendarRepository _calendarRepo;
        private readonly IUnitOfWork _uow;
        private readonly ICalendarAuditService _auditService;

        public AcademicCalendarService(
            IAcademicCalendarRepository calendarRepo,
            IUnitOfWork uow,
            ICalendarAuditService auditService)
        {
            _calendarRepo = calendarRepo;
            _uow = uow;
            _auditService = auditService;
        }

        public async Task<List<AcademicCalendarDto>> GetCalendarDaysAsync(DateTime start, DateTime end, CancellationToken ct = default)
        {
            var startOnly = DateOnly.FromDateTime(start.Date);
            var endOnly = DateOnly.FromDateTime(end.Date);

            return await _calendarRepo.Query()
                .AsNoTracking()
                .Include(x => x.AcademicYear)
                .Where(x => x.Date >= startOnly && x.Date <= endOnly && !x.IsDeleted)
                .OrderBy(x => x.Date)
                .Select(x => new AcademicCalendarDto
                {
                    Id = x.Id,
                    AcademicYearId = x.AcademicYearId,
                    AcademicYearName = x.AcademicYear != null ? x.AcademicYear.Name : "",
                    Date = x.Date,
                    Title = x.Title,
                    Description = x.Description,
                    IsHoliday = x.IsHoliday,
                    IsWorkingDay = x.IsWorkingDay,
                    IsExamDay = x.IsExamDay,
                    IsEventDay = x.IsEventDay,
                    Remarks = x.Remarks,
                    HolidayType = x.HolidayType,
                    IsActive = x.IsActive,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync(ct);
        }

        public async Task<List<CalendarPublishedEventDto>> GetPublishedEventsAsync(DateTime start, DateTime end, CancellationToken ct = default)
        {
            return await _uow.Repository<Event>().Query()
                .AsNoTracking()
                .Where(e => e.IsPublished && e.EventDate >= start && e.EventDate <= end)
                .Select(e => new CalendarPublishedEventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    EventDate = e.EventDate,
                    EventLocation = e.EventLocation
                })
                .ToListAsync(ct);
        }

        public async Task<List<CalendarExamScheduleDto>> GetExamSchedulesAsync(DateOnly start, DateOnly end, CancellationToken ct = default)
        {
            return await _uow.Repository<ExamSchedule>().Query()
                .AsNoTracking()
                .Include(es => es.Exam)
                .Include(es => es.Subject)
                .Include(es => es.Class)
                .Include(es => es.StudentGroup)
                .Where(es => es.ExamDate >= start && es.ExamDate <= end)
                .Select(es => new CalendarExamScheduleDto
                {
                    Id = es.Id,
                    ExamId = es.ExamId,
                    ExamName = es.Exam != null ? es.Exam.Name : "",
                    SubjectName = es.Subject != null ? es.Subject.Name : "",
                    ClassName = es.Class != null ? es.Class.Name : "",
                    StudentGroupName = es.StudentGroup != null ? es.StudentGroup.Name : null,
                    ExamDate = es.ExamDate,
                    StartsAt = es.StartsAt,
                    EndsAt = es.EndsAt,
                    RoomNo = es.RoomNo,
                    Instructions = es.Instructions
                })
                .ToListAsync(ct);
        }

        public async Task<AcademicCalendarDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _calendarRepo.Query()
                .AsNoTracking()
                .Include(x => x.AcademicYear)
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new AcademicCalendarDto
                {
                    Id = x.Id,
                    AcademicYearId = x.AcademicYearId,
                    AcademicYearName = x.AcademicYear != null ? x.AcademicYear.Name : "",
                    Date = x.Date,
                    Title = x.Title,
                    Description = x.Description,
                    IsHoliday = x.IsHoliday,
                    IsWorkingDay = x.IsWorkingDay,
                    IsExamDay = x.IsExamDay,
                    IsEventDay = x.IsEventDay,
                    Remarks = x.Remarks,
                    HolidayType = x.HolidayType,
                    IsActive = x.IsActive,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedAt = x.UpdatedAt
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<AcademicCalendarUpsertDto?> GetForEditAsync(int id, CancellationToken ct = default)
        {
            return await _calendarRepo.Query()
                .AsNoTracking()
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new AcademicCalendarUpsertDto
                {
                    Id = x.Id,
                    AcademicYearId = x.AcademicYearId,
                    Date = x.Date,
                    Title = x.Title,
                    Description = x.Description,
                    IsHoliday = x.IsHoliday,
                    IsWorkingDay = x.IsWorkingDay,
                    IsExamDay = x.IsExamDay,
                    IsEventDay = x.IsEventDay,
                    Remarks = x.Remarks,
                    HolidayType = x.HolidayType,
                    IsActive = x.IsActive
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<int> CreateAsync(AcademicCalendarUpsertDto dto, string createdBy, CancellationToken ct = default)
        {
            var entity = new AcademicCalendar
            {
                AcademicYearId = dto.AcademicYearId,
                Date = dto.Date,
                Title = dto.Title,
                Description = dto.Description,
                IsHoliday = dto.IsHoliday,
                IsWorkingDay = dto.IsWorkingDay,
                IsExamDay = dto.IsExamDay,
                IsEventDay = dto.IsEventDay,
                Remarks = dto.Remarks,
                HolidayType = dto.HolidayType,
                IsActive = dto.IsActive
            };

            if (entity.AcademicYearId == 0)
            {
                var activeYear = await _uow.Repository<AcademicYear>().Query()
                    .FirstOrDefaultAsync(y => y.IsActive && !y.IsDeleted, ct);
                if (activeYear != null)
                {
                    entity.AcademicYearId = activeYear.Id;
                }
            }

            entity.CreatedBy = createdBy;
            entity.CreatedAt = DateTime.UtcNow;

            await _calendarRepo.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);

            await _auditService.LogAsync("Created", "AcademicCalendar", entity.Id, null, dto.Date.ToString(), ct);

            return entity.Id;
        }

        public async Task UpdateAsync(AcademicCalendarUpsertDto dto, string updatedBy, CancellationToken ct = default)
        {
            var entity = await _calendarRepo.Query()
                .FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, ct);
            if (entity == null) return;

            entity.AcademicYearId = dto.AcademicYearId;
            entity.Date = dto.Date;
            entity.Title = dto.Title;
            entity.Description = dto.Description;
            entity.IsHoliday = dto.IsHoliday;
            entity.IsWorkingDay = dto.IsWorkingDay;
            entity.IsExamDay = dto.IsExamDay;
            entity.IsEventDay = dto.IsEventDay;
            entity.Remarks = dto.Remarks;
            entity.HolidayType = dto.HolidayType;
            entity.IsActive = dto.IsActive;
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;

            _calendarRepo.Update(entity);
            await _uow.SaveChangesAsync(ct);

            await _auditService.LogAsync("Updated", "AcademicCalendar", dto.Id, entity.Date.ToString(), dto.Date.ToString(), ct);
        }

        public async Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default)
        {
            var entity = await _calendarRepo.GetByIdAsync(id, ct);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.UpdatedBy = updatedBy;
                entity.UpdatedAt = DateTime.UtcNow;
                _calendarRepo.Update(entity);
                await _uow.SaveChangesAsync(ct);

                await _auditService.LogAsync("Deleted", "AcademicCalendar", id, null, null, ct);
            }
        }
    }
