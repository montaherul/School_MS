using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class ClassRoutineService : IClassRoutineService
{
    private readonly SchoolDbContext _db;
    private readonly IUnitOfWork _uow;

    public ClassRoutineService(SchoolDbContext db, IUnitOfWork uow)
    {
        _db = db;
        _uow = uow;
    }

    public async Task<IEnumerable<ClassRoutineDto>> GetBySectionAsync(int sectionId, CancellationToken ct = default)
    {
        return await _db.ClassRoutines
            .Include(r => r.Subject)
            .Include(r => r.Employee)
            .Include(r => r.Class)
            .Include(r => r.Section)
            .Where(r => r.SectionId == sectionId && !r.IsDeleted)
            .OrderBy(r => r.DayOfWeek).ThenBy(r => r.StartTime)
            .Select(r => MapToDto(r))
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<ClassRoutineDto>> GetByTeacherAsync(long employeeId, CancellationToken ct = default)
    {
        return await _db.ClassRoutines
            .Include(r => r.Subject)
            .Include(r => r.Class)
            .Include(r => r.Section)
            .Where(r => r.EmployeeId == employeeId && !r.IsDeleted)
            .OrderBy(r => r.DayOfWeek).ThenBy(r => r.StartTime)
            .Select(r => MapToDto(r))
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<string>> DetectConflictsAsync(ClassRoutineDto dto, CancellationToken ct = default)
    {
        var conflicts = new List<string>();

        // Overlap condition: (StartA < EndB) AND (EndA > StartB)
        var overlaps = await _db.ClassRoutines
            .Where(r => !r.IsDeleted && r.DayOfWeek == dto.DayOfWeek && r.Id != dto.Id)
            .Where(r => dto.StartTime < r.EndTime && dto.EndTime > r.StartTime)
            .ToListAsync(ct);

        if (overlaps.Any(r => r.EmployeeId == dto.EmployeeId))
            conflicts.Add("Teacher is already assigned to another class during this time.");

        if (overlaps.Any(r => r.SectionId == dto.SectionId))
            conflicts.Add("This section already has a scheduled subject during this time.");

        if (!string.IsNullOrEmpty(dto.RoomNo) && overlaps.Any(r => r.RoomNo == dto.RoomNo))
            conflicts.Add($"Room {dto.RoomNo} is already occupied during this time.");

        return conflicts;
    }

    public async Task<bool> AddRoutineAsync(ClassRoutineDto dto, string createdBy, CancellationToken ct = default)
    {
        var conflicts = await DetectConflictsAsync(dto, ct);
        if (conflicts.Any()) return false;

        var routine = new ClassRoutine
        {
            ClassId = dto.ClassId,
            SectionId = dto.SectionId,
            SubjectId = dto.SubjectId,
            EmployeeId = dto.EmployeeId,
            DayOfWeek = dto.DayOfWeek,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            RoomNo = dto.RoomNo,
            CreatedBy = createdBy
        };

        await _db.ClassRoutines.AddAsync(routine, ct);
        await _uow.SaveChangesAsync(ct);
        return true;
    }

    public async Task DeleteRoutineAsync(int id, CancellationToken ct = default)
    {
        var routine = await _db.ClassRoutines.FindAsync(new object[] { id }, ct);
        if (routine != null)
        {
            routine.IsDeleted = true;
            await _uow.SaveChangesAsync(ct);
        }
    }

    private static ClassRoutineDto MapToDto(ClassRoutine r) => new ClassRoutineDto
    {
        Id = r.Id,
        ClassId = r.ClassId,
        ClassName = r.Class?.Name ?? "",
        SectionId = r.SectionId,
        SectionName = r.Section?.Name ?? "",
        SubjectId = r.SubjectId,
        SubjectName = r.Subject?.Name ?? "",
        EmployeeId = r.EmployeeId,
        TeacherName = r.Employee?.FullName ?? "",
        DayOfWeek = r.DayOfWeek,
        StartTime = r.StartTime,
        EndTime = r.EndTime,
        RoomNo = r.RoomNo
    };
}
