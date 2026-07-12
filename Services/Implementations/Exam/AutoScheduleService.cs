using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Routine;
using SchoolManagementSystem.Services.Interfaces.Exam;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;
using ExamScheduleEntity = SchoolManagementSystem.Models.Entities.Exam.ExamSchedule;
using ExamSubjectEntity = SchoolManagementSystem.Models.Entities.Exam.ExamSubject;

namespace SchoolManagementSystem.Services.Implementations.Exam;

public class AutoScheduleService : IAutoScheduleService
{
    private readonly IUnitOfWork _uow;
    private const int DefaultDuration = 180;
    private const int MinGap = 30;
    private const int WorkStart = 8;
    private const int WorkEnd = 16;

    public AutoScheduleService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<AutoScheduleResultDto> GenerateScheduleAsync(int examId, CancellationToken ct = default)
    {
        var exam = await _uow.Repository<ExamEntity>().Query()
            .Include(e => e.ExamSubjects.Where(es => !es.IsDeleted))
                .ThenInclude(es => es.Subject)
            .FirstOrDefaultAsync(e => e.Id == examId && !e.IsDeleted, ct)
            ?? throw new KeyNotFoundException($"Exam with ID {examId} not found.");

        var result = new AutoScheduleResultDto
        {
            ExamId = exam.Id,
            ExamName = exam.Name,
            TotalSubjects = exam.ExamSubjects.Count
        };

        if (exam.ExamSubjects.Count == 0)
        {
            result.Warnings.Add("No subjects found for this exam.");
            return result;
        }

        var blockedDates = await GetBlockedDatesAsync(exam.StartsOn, exam.EndsOn, ct);

        var rooms = await _uow.Repository<Room>().Query()
            .AsNoTracking()
            .Where(r => r.IsActive && !r.IsDeleted)
            .Select(r => r.RoomNo)
            .ToListAsync(ct);

        if (rooms.Count == 0)
        {
            var existingRooms = exam.ExamSubjects
                .Where(es => !string.IsNullOrWhiteSpace(es.RoomNumber))
                .Select(es => es.RoomNumber!)
                .Distinct()
                .ToList();

            if (existingRooms.Count > 0)
                rooms.AddRange(existingRooms);
            else
                rooms.AddRange(Enumerable.Range(1, 5).Select(i => $"Room {100 + i}"));
        }

        var existingSchedules = await _uow.Repository<ExamScheduleEntity>().Query()
            .Where(s => s.ExamId == examId && !s.IsDeleted)
            .ToListAsync(ct);

        var teacherAssignment = await _uow.Repository<ExamSubjectEntity>().Query()
            .AsNoTracking()
            .Where(es => es.ExamId == examId && !es.IsDeleted)
            .ToDictionaryAsync(es => es.SubjectId, es => es.TeacherId, ct);

        var sortedSubjects = exam.ExamSubjects
            .OrderBy(es => es.IsOptional)
            .ThenBy(es => es.ExamDate ?? DateOnly.MaxValue)
            .ThenBy(es => es.Subject?.Name ?? "")
            .ToList();

        var roomIndex = 0;

        foreach (var examSubject in sortedSubjects)
        {
            var duration = examSubject.ExamDuration ?? DefaultDuration;
            var item = new AutoScheduleItemDto
            {
                SubjectId = examSubject.SubjectId,
                SubjectName = examSubject.Subject?.Name ?? $"Subject #{examSubject.SubjectId}",
                IsScheduled = false
            };

            var existing = existingSchedules.FirstOrDefault(s => s.SubjectId == examSubject.SubjectId);
            if (existing != null)
            {
                item.IsScheduled = true;
                item.ExamDate = existing.ExamDate;
                item.StartTime = existing.StartsAt;
                item.EndTime = existing.EndsAt;
                item.RoomNo = existing.RoomNo;
                result.Items.Add(item);
                result.Scheduled++;
                continue;
            }

            teacherAssignment.TryGetValue(examSubject.SubjectId, out var teacherId);

            var slot = FindSlot(
                exam.StartsOn, exam.EndsOn,
                duration, blockedDates,
                existingSchedules, teacherId,
                rooms,
                examSubject.ExamDate, examSubject.ExamStartTime,
                teacherAssignment);

            if (slot != null)
            {
                var (date, start, end, room) = slot.Value;

                var schedule = new ExamScheduleEntity
                {
                    ExamId = examId,
                    SubjectId = examSubject.SubjectId,
                    ClassId = examSubject.ClassId,
                    StudentGroupId = examSubject.StudentGroupId,
                    SectionId = exam.SectionId,
                    ExamDate = date,
                    StartsAt = start,
                    EndsAt = end,
                    RoomNo = room,
                    CreatedAt = DateTime.UtcNow
                };

                await _uow.Repository<ExamScheduleEntity>().AddAsync(schedule, ct);
                existingSchedules.Add(schedule);

                item.IsScheduled = true;
                item.ExamDate = date;
                item.StartTime = start;
                item.EndTime = end;
                item.RoomNo = room;
                result.Scheduled++;

                roomIndex = (roomIndex + 1) % rooms.Count;
            }
            else
            {
                item.Reason = "No available time slot within the exam date range.";
                result.Skipped++;
            }

            result.Items.Add(item);
        }

        if (result.Scheduled > 0)
            await _uow.SaveChangesAsync(ct);

        return result;
    }

    private async Task<HashSet<DateOnly>> GetBlockedDatesAsync(DateOnly start, DateOnly end, CancellationToken ct)
    {
        var blocked = new HashSet<DateOnly>();

        for (var d = start; d <= end; d = d.AddDays(1))
        {
            if (d.DayOfWeek == DayOfWeek.Friday)
                blocked.Add(d);
        }

        var holidays = await _uow.Repository<HolidayMaster>().Query()
            .AsNoTracking()
            .Where(h => h.IsActive && !h.IsDeleted && h.CountryCode == "BD")
            .ToListAsync(ct);

        foreach (var h in holidays)
        {
            if (h.IsRecurring)
            {
                for (var d = start; d <= end; d = d.AddDays(1))
                {
                    if (d.Month == h.HolidayDate.Month && d.Day == h.HolidayDate.Day)
                        blocked.Add(d);
                }
            }
            else
            {
                if (h.HolidayDate >= start && h.HolidayDate <= end)
                    blocked.Add(h.HolidayDate);
            }
        }

        return blocked;
    }

    private static (DateOnly Date, TimeOnly Start, TimeOnly End, string Room)? FindSlot(
        DateOnly examStart, DateOnly examEnd,
        int duration,
        HashSet<DateOnly> blocked,
        List<ExamScheduleEntity> schedules,
        int? teacherId,
        List<string> rooms,
        DateOnly? preferredDate,
        TimeOnly? preferredTime,
        Dictionary<int, int?> teacherAssignment)
    {
        var workStart = new TimeOnly(WorkStart, 0);
        var workEnd = new TimeOnly(WorkEnd, 0);

        var dates = new List<DateOnly>();
        if (preferredDate.HasValue && preferredDate >= examStart && preferredDate <= examEnd)
            dates.Add(preferredDate.Value);
        for (var d = examStart; d <= examEnd; d = d.AddDays(1))
            if (!dates.Contains(d)) dates.Add(d);

        foreach (var date in dates)
        {
            if (blocked.Contains(date)) continue;

            var times = new List<TimeOnly>();
            if (preferredTime.HasValue && date == preferredDate)
                times.Add(preferredTime.Value);
            for (var t = workStart; t.AddMinutes(duration) <= workEnd; t = t.AddMinutes(MinGap))
                if (!times.Contains(t)) times.Add(t);

            foreach (var start in times)
            {
                var end = start.AddMinutes(duration);
                if (end > workEnd) continue;

                foreach (var room in rooms)
                {
                    var conflict = schedules.Any(s =>
                        s.ExamDate == date &&
                        string.Equals(s.RoomNo, room, StringComparison.OrdinalIgnoreCase) &&
                        s.StartsAt < end &&
                        s.EndsAt > start);

                    if (conflict) continue;

                    if (teacherId.HasValue)
                    {
                        var teacherConflict = schedules.Any(s =>
                            s.ExamDate == date &&
                            s.StartsAt < end &&
                            s.EndsAt > start &&
                            teacherAssignment.TryGetValue(s.SubjectId, out var tId) &&
                            tId == teacherId.Value);

                        if (teacherConflict) continue;
                    }

                    return (date, start, end, room);
                }
            }
        }

        return null;
    }
}
