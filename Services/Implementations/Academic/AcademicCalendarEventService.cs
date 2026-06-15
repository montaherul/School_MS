using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class AcademicCalendarEventService : IAcademicCalendarEventService
{
    private readonly IUnitOfWork _uow;

    public AcademicCalendarEventService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    private static AcademicCalendarEventDto MapToDto(AcademicCalendarEvent e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        EventType = e.EventType.ToString(),
        StartDate = e.StartDate,
        EndDate = e.EndDate,
        IsActive = e.IsActive
    };

    public async Task<List<AcademicCalendarEventDto>> GetEventsByCalendarAsync(int calendarId, CancellationToken ct = default)
    {
        return await _uow.Repository<AcademicCalendarEvent>().Query()
            .Where(x => x.AcademicCalendarId == calendarId && !x.IsDeleted)
            .OrderBy(x => x.StartDate)
            .Select(x => MapToDto(x))
            .ToListAsync(ct);
    }

    public async Task<AcademicCalendarEventDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<AcademicCalendarEvent>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<int> CreateAsync(AcademicCalendarEventDto dto, string createdBy, CancellationToken ct = default)
    {
        var entity = new AcademicCalendarEvent
        {
            AcademicCalendarId = dto.Id,
            Title = dto.Title,
            Description = null,
            EventType = Enum.TryParse<AcademicEventType>(dto.EventType, out var et) ? et : AcademicEventType.Event,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsRecurringWeekly = false,
            IsActive = dto.IsActive,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<AcademicCalendarEvent>().AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(AcademicCalendarEventDto dto, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<AcademicCalendarEvent>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Calendar event not found.");

        entity.Title = dto.Title;
        entity.EventType = Enum.TryParse<AcademicEventType>(dto.EventType, out var et) ? et : AcademicEventType.Event;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<AcademicCalendarEvent>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Calendar event not found.");

        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }
}
