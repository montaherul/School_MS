using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class AcademicCalendarEventService : IAcademicCalendarEventService
{
    private readonly IAcademicCalendarEventRepository _eventRepo;
    private readonly IUnitOfWork _uow;
    private readonly ICalendarAuditService _auditService;

    public AcademicCalendarEventService(
        IAcademicCalendarEventRepository eventRepo,
        IUnitOfWork uow,
        ICalendarAuditService auditService)
    {
        _eventRepo = eventRepo;
        _uow = uow;
        _auditService = auditService;
    }

    public async Task<AcademicCalendarDto?> GetCalendarByIdAsync(int calendarId, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<AcademicCalendar>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == calendarId && !x.IsDeleted, ct);
        return entity is null ? null : new AcademicCalendarDto
        {
            Id = entity.Id,
            AcademicYearId = entity.AcademicYearId,
            Date = entity.Date,
            Title = entity.Title,
            Description = entity.Description,
            IsHoliday = entity.IsHoliday,
            IsWorkingDay = entity.IsWorkingDay,
            IsExamDay = entity.IsExamDay,
            IsEventDay = entity.IsEventDay,
            Remarks = entity.Remarks,
            HolidayType = entity.HolidayType,
            IsActive = entity.IsActive
        };
    }

    private static AcademicCalendarEventDto MapToDto(AcademicCalendarEvent e) => new()
    {
        Id = e.Id,
        AcademicCalendarId = e.AcademicCalendarId,
        Title = e.Title,
        Description = e.Description,
        EventType = e.EventType.ToString(),
        StartDate = e.StartDate,
        EndDate = e.EndDate,
        IsRecurringWeekly = e.IsRecurringWeekly,
        IsActive = e.IsActive
    };

    public async Task<List<AcademicCalendarEventDto>> GetEventsByCalendarAsync(int calendarId, CancellationToken ct = default)
    {
        return await _eventRepo.Query().AsNoTracking()
            .Where(x => x.AcademicCalendarId == calendarId && !x.IsDeleted)
            .OrderBy(x => x.StartDate)
            .Select(x => MapToDto(x))
            .ToListAsync(ct);
    }

    public async Task<AcademicCalendarEventDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _eventRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<int> CreateAsync(AcademicCalendarEventDto dto, string createdBy, CancellationToken ct = default)
    {
        var entity = new AcademicCalendarEvent
        {
            AcademicCalendarId = dto.AcademicCalendarId,
            Title = dto.Title,
            Description = dto.Description,
            EventType = Enum.TryParse<AcademicEventType>(dto.EventType, out var et) ? et : AcademicEventType.Event,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsRecurringWeekly = dto.IsRecurringWeekly,
            IsActive = dto.IsActive,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await _eventRepo.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        await _auditService.LogAsync("Created", "AcademicCalendarEvent", entity.Id, null, dto.Title, ct);

        return entity.Id;
    }

    public async Task UpdateAsync(AcademicCalendarEventDto dto, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _eventRepo.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Calendar event not found.");

        entity.Title = dto.Title;
        entity.Description = dto.Description;
        entity.EventType = Enum.TryParse<AcademicEventType>(dto.EventType, out var et) ? et : AcademicEventType.Event;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.IsRecurringWeekly = dto.IsRecurringWeekly;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _uow.SaveChangesAsync(ct);

        await _auditService.LogAsync("Updated", "AcademicCalendarEvent", dto.Id, null, dto.Title, ct);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _eventRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Calendar event not found.");

        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);

        await _auditService.LogAsync("Deleted", "AcademicCalendarEvent", id, null, null, ct);
    }
}
