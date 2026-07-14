using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Routine;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.Teachers;
using StudentModel = SchoolManagementSystem.Models.Entities.Student.Student;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.Entities.Routine;
using SchoolManagementSystem.Repositories.Interfaces.Routine;
using SchoolManagementSystem.Services.Interfaces.Routine;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using RoutineEnt = SchoolManagementSystem.Models.Entities.Routine;

namespace SchoolManagementSystem.Services.Implementations.Routine;

public class RoutinePeriodService : IRoutinePeriodService
{
    private readonly IRoutinePeriodRepository _routinePeriodRepo;
    private readonly IUnitOfWork _unitOfWork;

    public RoutinePeriodService(IRoutinePeriodRepository routinePeriodRepo, IUnitOfWork unitOfWork)
    {
        _routinePeriodRepo = routinePeriodRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<RoutinePeriodListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
        => await _routinePeriodRepo.GetPagedAsync(page, pageSize, search, cancellationToken);

    public async Task<RoutinePeriodUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
        => await _routinePeriodRepo.GetForEditAsync(id, cancellationToken);

    public async Task<int> CreateAsync(RoutinePeriodUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new RoutineEnt.RoutinePeriod
        {
            Name = dto.Name.Trim(),
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            PeriodNumber = dto.PeriodNumber,
            IsBreak = dto.IsBreak,
            IsActive = dto.IsActive,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await _routinePeriodRepo.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(RoutinePeriodUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _routinePeriodRepo.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Routine period not found.");

        entity.Name = dto.Name.Trim();
        entity.StartTime = dto.StartTime;
        entity.EndTime = dto.EndTime;
        entity.PeriodNumber = dto.PeriodNumber;
        entity.IsBreak = dto.IsBreak;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _routinePeriodRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Routine period not found.");

        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<RoutinePeriodListItemDto>> GetActivePeriodsAsync(CancellationToken cancellationToken = default)
        => await _routinePeriodRepo.GetActivePeriodsAsync(cancellationToken);
}

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepo;
    private readonly IUnitOfWork _unitOfWork;

    public RoomService(IRoomRepository roomRepo, IUnitOfWork unitOfWork)
    {
        _roomRepo = roomRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<RoomListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
        => await _roomRepo.GetPagedAsync(page, pageSize, search, cancellationToken);

    public async Task<RoomUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
        => await _roomRepo.GetForEditAsync(id, cancellationToken);

    public async Task<int> CreateAsync(RoomUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new RoutineEnt.Room
        {
            RoomNo = dto.RoomNo.Trim(),
            Name = dto.Name?.Trim(),
            Capacity = dto.Capacity,
            Building = dto.Building?.Trim(),
            Floor = dto.Floor,
            RoomType = dto.RoomType,
            IsLab = dto.IsLab,
            RequiresDoublePeriod = dto.RequiresDoublePeriod,
            IsActive = dto.IsActive,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await _roomRepo.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(RoomUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _roomRepo.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Room not found.");

        entity.RoomNo = dto.RoomNo.Trim();
        entity.Name = dto.Name?.Trim();
        entity.Capacity = dto.Capacity;
        entity.Building = dto.Building?.Trim();
        entity.Floor = dto.Floor;
        entity.RoomType = dto.RoomType;
        entity.IsLab = dto.IsLab;
        entity.RequiresDoublePeriod = dto.RequiresDoublePeriod;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _roomRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Room not found.");

        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<RoomListItemDto>> GetActiveRoomsAsync(CancellationToken cancellationToken = default)
        => await _roomRepo.GetActiveRoomsAsync(cancellationToken);

    public Task<List<string>> GetRoomTypesAsync()
        => _roomRepo.GetRoomTypesAsync();
}

public class SubjectRequirementService : ISubjectRequirementService
{
    private readonly ISubjectRequirementRepository _subjectRequirementRepo;
    private readonly IUnitOfWork _unitOfWork;

    public SubjectRequirementService(ISubjectRequirementRepository subjectRequirementRepo, IUnitOfWork unitOfWork)
    {
        _subjectRequirementRepo = subjectRequirementRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<SubjectRequirementListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
        => await _subjectRequirementRepo.GetPagedAsync(page, pageSize, search, cancellationToken);

    public async Task<SubjectRequirementUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
        => await _subjectRequirementRepo.GetForEditAsync(id, cancellationToken);

    public async Task<int> CreateAsync(SubjectRequirementUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        // Validate subject is offered for this class+group via ClassSubjectGroup junction
        if (dto.GroupId.HasValue)
        {
            var valid = await _unitOfWork.Repository<ClassSubjectGroup>().AnyAsync(csg =>
                csg.StudentGroupId == dto.GroupId.Value &&
                csg.ClassSubject!.SchoolClassId == dto.ClassId &&
                csg.ClassSubject.SubjectId == dto.SubjectId &&
                !csg.IsDeleted && !csg.ClassSubject.IsDeleted, cancellationToken);
            if (!valid)
                throw new InvalidOperationException("The selected subject is not offered for this class and group.");
        }

        var entity = new RoutineEnt.SubjectRequirement
        {
            AcademicYearId = dto.AcademicYearId,
            ClassId = dto.ClassId,
            SectionId = dto.SectionId,
            GroupId = dto.GroupId,
            SubjectId = dto.SubjectId,
            TeacherId = dto.TeacherId,
            PeriodsPerWeek = dto.PeriodsPerWeek,
            RequiresLab = dto.RequiresLab,
            RequiresDoublePeriod = dto.RequiresDoublePeriod,
            Priority = dto.Priority,
            MaxConsecutive = dto.MaxConsecutive,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await _subjectRequirementRepo.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(SubjectRequirementUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _subjectRequirementRepo.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Subject requirement not found.");

        entity.AcademicYearId = dto.AcademicYearId;
        entity.ClassId = dto.ClassId;
        entity.SectionId = dto.SectionId;
        entity.GroupId = dto.GroupId;
        entity.SubjectId = dto.SubjectId;
        entity.TeacherId = dto.TeacherId;
        entity.PeriodsPerWeek = dto.PeriodsPerWeek;
        entity.RequiresLab = dto.RequiresLab;
        entity.RequiresDoublePeriod = dto.RequiresDoublePeriod;
        entity.Priority = dto.Priority;
        entity.MaxConsecutive = dto.MaxConsecutive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _subjectRequirementRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Subject requirement not found.");

        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<SubjectRequirementListItemDto>> GetByClassAsync(int classId, int? sectionId = null, int? groupId = null, CancellationToken cancellationToken = default)
        => await _subjectRequirementRepo.GetByClassAsync(classId, sectionId, groupId, cancellationToken);
}

public class WorkingDayService : IWorkingDayService
{
    private readonly IWorkingDayRepository _workingDayRepo;
    private readonly IUnitOfWork _unitOfWork;

    public WorkingDayService(IWorkingDayRepository workingDayRepo, IUnitOfWork unitOfWork)
    {
        _workingDayRepo = workingDayRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<WorkingDayListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
        => await _workingDayRepo.GetPagedAsync(page, pageSize, search, cancellationToken);

    public async Task<WorkingDayUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
        => await _workingDayRepo.GetForEditAsync(id, cancellationToken);

    public async Task<int> CreateAsync(WorkingDayUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new RoutineEnt.WorkingDay
        {
            AcademicYearId = dto.AcademicYearId,
            DayName = dto.DayName.Trim(),
            DayNumber = dto.DayNumber,
            IsWorkingDay = dto.IsWorkingDay,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await _workingDayRepo.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(WorkingDayUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _workingDayRepo.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Working day not found.");

        entity.AcademicYearId = dto.AcademicYearId;
        entity.DayName = dto.DayName.Trim();
        entity.DayNumber = dto.DayNumber;
        entity.IsWorkingDay = dto.IsWorkingDay;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _workingDayRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Working day not found.");

        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<WorkingDayListItemDto>> GetByAcademicYearAsync(int academicYearId, CancellationToken cancellationToken = default)
        => await _workingDayRepo.GetByAcademicYearAsync(academicYearId, cancellationToken);
}

public class TeacherAvailabilityService : ITeacherAvailabilityService
{
    private readonly ITeacherAvailabilityRepository _teacherAvailabilityRepo;
    private readonly IUnitOfWork _unitOfWork;

    public TeacherAvailabilityService(ITeacherAvailabilityRepository teacherAvailabilityRepo, IUnitOfWork unitOfWork)
    {
        _teacherAvailabilityRepo = teacherAvailabilityRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<TeacherAvailabilityListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
        => await _teacherAvailabilityRepo.GetPagedAsync(page, pageSize, search, cancellationToken);

    public async Task<TeacherAvailabilityUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
        => await _teacherAvailabilityRepo.GetForEditAsync(id, cancellationToken);

    public async Task<int> CreateAsync(TeacherAvailabilityUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new RoutineEnt.TeacherAvailability
        {
            TeacherId = dto.TeacherId,
            RoutinePeriodId = dto.RoutinePeriodId,
            DayNumber = dto.DayNumber,
            IsAvailable = dto.IsAvailable,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await _teacherAvailabilityRepo.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(TeacherAvailabilityUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _teacherAvailabilityRepo.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Teacher availability not found.");

        entity.TeacherId = dto.TeacherId;
        entity.RoutinePeriodId = dto.RoutinePeriodId;
        entity.DayNumber = dto.DayNumber;
        entity.IsAvailable = dto.IsAvailable;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _teacherAvailabilityRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Teacher availability not found.");

        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<TeacherAvailabilityListItemDto>> GetByTeacherAsync(int teacherId, CancellationToken cancellationToken = default)
        => await _teacherAvailabilityRepo.GetByTeacherAsync(teacherId, cancellationToken);

    private static string GetDayName(int dayNumber) => dayNumber switch
    {
        1 => "Saturday",
        2 => "Sunday",
        3 => "Monday",
        4 => "Tuesday",
        5 => "Wednesday",
        6 => "Thursday",
        7 => "Friday",
        _ => "Unknown"
    };
}

public class RoutineEntryService : IRoutineEntryService
{
    private readonly IRoutineEntryRepository _routineEntryRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RoutineEntryService(IRoutineEntryRepository routineEntryRepo, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _routineEntryRepo = routineEntryRepo;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PagedResult<RoutineEntryListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
        => await _routineEntryRepo.GetPagedAsync(page, pageSize, search, cancellationToken);

    public async Task<RoutineEntryUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
        => await _routineEntryRepo.GetForEditAsync(id, cancellationToken);

    public async Task<int> CreateAsync(RoutineEntryUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new RoutineEnt.RoutineEntry
        {
            AcademicYearId = dto.AcademicYearId,
            ClassId = dto.ClassId,
            SectionId = dto.SectionId,
            GroupId = dto.GroupId,
            SubjectId = dto.SubjectId,
            TeacherId = dto.TeacherId,
            RoomId = dto.RoomId,
            RoutinePeriodId = dto.RoutinePeriodId,
            DayNumber = dto.DayNumber,
            IsLab = dto.IsLab,
            Note = dto.Note?.Trim(),
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await _routineEntryRepo.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await LogAuditAsync("Create", "RoutineEntry", entity.Id, null, $"Teacher={dto.TeacherId},Period={dto.RoutinePeriodId},Day={dto.DayNumber}", cancellationToken);

        return entity.Id;
    }

    public async Task UpdateAsync(RoutineEntryUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _routineEntryRepo.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Routine entry not found.");

        entity.AcademicYearId = dto.AcademicYearId;
        entity.ClassId = dto.ClassId;
        entity.SectionId = dto.SectionId;
        entity.GroupId = dto.GroupId;
        entity.SubjectId = dto.SubjectId;
        entity.TeacherId = dto.TeacherId;
        entity.RoomId = dto.RoomId;
        entity.RoutinePeriodId = dto.RoutinePeriodId;
        entity.DayNumber = dto.DayNumber;
        entity.IsLab = dto.IsLab;
        entity.Note = dto.Note?.Trim();
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await LogAuditAsync("Update", "RoutineEntry", entity.Id, null, $"Teacher={dto.TeacherId},Period={dto.RoutinePeriodId},Day={dto.DayNumber}", cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _routineEntryRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Routine entry not found.");

        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await LogAuditAsync("Delete", "RoutineEntry", id, null, $"Soft-deleted entry {id}", cancellationToken);
    }

    public async Task<PagedResult<RoutineEntryListItemDto>> GetGridAsync(int academicYearId, int? classId = null, int? sectionId = null, int? groupId = null, int? teacherId = null, int? roomId = null, int page = 1, int pageSize = 100, CancellationToken cancellationToken = default)
        => await _routineEntryRepo.GetGridAsync(academicYearId, classId, sectionId, groupId, teacherId, roomId, page, pageSize, cancellationToken);

    public async Task<bool> ValidateEntryAsync(RoutineEntryUpsertDto dto, CancellationToken cancellationToken = default)
        => await _routineEntryRepo.ValidateEntryAsync(dto, cancellationToken);

    public async Task UpdateEntryAsync(int id, int roomId, int routinePeriodId, int dayNumber, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _routineEntryRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Routine entry not found.");

        entity.RoomId = roomId;
        entity.RoutinePeriodId = routinePeriodId;
        entity.DayNumber = dayNumber;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await LogAuditAsync("UpdateEntry", "RoutineEntry", id, null, $"Room={roomId},Period={routinePeriodId},Day={dayNumber}", cancellationToken);
    }

    public async Task SwapEntriesAsync(int entryId1, int entryId2, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entry1 = await _routineEntryRepo.FirstOrDefaultAsync(x => x.Id == entryId1 && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("First routine entry not found.");

        var entry2 = await _routineEntryRepo.FirstOrDefaultAsync(x => x.Id == entryId2 && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Second routine entry not found.");

        (entry1.RoutinePeriodId, entry2.RoutinePeriodId) = (entry2.RoutinePeriodId, entry1.RoutinePeriodId);
        (entry1.RoomId, entry2.RoomId) = (entry2.RoomId, entry1.RoomId);
        (entry1.DayNumber, entry2.DayNumber) = (entry2.DayNumber, entry1.DayNumber);

        entry1.UpdatedBy = updatedBy;
        entry1.UpdatedAt = DateTime.UtcNow;
        entry2.UpdatedBy = updatedBy;
        entry2.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await LogAuditAsync("Swap", "RoutineEntry", null, null, $"Swapped entries {entryId1}<->{entryId2}", cancellationToken);
    }

    public async Task MoveEntryAsync(int entryId, int targetPeriodId, int targetDayNumber, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _routineEntryRepo.FirstOrDefaultAsync(x => x.Id == entryId && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Routine entry not found.");

        entity.RoutinePeriodId = targetPeriodId;
        entity.DayNumber = targetDayNumber;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await LogAuditAsync("Move", "RoutineEntry", entryId, null, $"To Period={targetPeriodId},Day={targetDayNumber}", cancellationToken);
    }

    public async Task BulkDeleteAsync(List<int> ids, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entities = await _routineEntryRepo.Query()
            .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
            .ToListAsync(cancellationToken);
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await LogAuditAsync("BulkDelete", "RoutineEntry", null, null, $"Deleted {ids.Count} entries", cancellationToken);
    }

    public async Task BulkUpdateAsync(List<int> ids, int roomId, int routinePeriodId, int dayNumber, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entities = await _routineEntryRepo.Query()
            .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
            .ToListAsync(cancellationToken);
        foreach (var entity in entities)
        {
            entity.RoomId = roomId;
            entity.RoutinePeriodId = routinePeriodId;
            entity.DayNumber = dayNumber;
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await LogAuditAsync("BulkUpdate", "RoutineEntry", null, null, $"Updated {ids.Count} entries: Room={roomId},Period={routinePeriodId},Day={dayNumber}", cancellationToken);
    }

    private async Task LogAuditAsync(string action, string entity, int? entityId, string? oldValue, string? newValue, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userIdStr = httpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        int? userId = userIdStr != null && int.TryParse(userIdStr, out var uid) ? uid : null;

        var details = entityId.HasValue
            ? $"[{entity}#{entityId}] {action}"
            : $"[{entity}] {action}";

        if (oldValue != null || newValue != null)
            details += $" | Old: {oldValue} | New: {newValue}";

        var log = new AuditLog
        {
            UserId = userId,
            Module = "Routine",
            Action = $"{entity}.{action}",
            IpAddress = httpContext?.Connection?.RemoteIpAddress?.ToString(),
            Details = details.Length > 1000 ? details[..1000] : details,
            CreatedBy = httpContext?.User?.Identity?.Name ?? "system",
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<AuditLog>().AddAsync(log, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static string GetDayName(int dayNumber) => dayNumber switch
    {
        1 => "Saturday",
        2 => "Sunday",
        3 => "Monday",
        4 => "Tuesday",
        5 => "Wednesday",
        6 => "Thursday",
        7 => "Friday",
        _ => "Unknown"
    };
}

public class RoutineGenerationService : IRoutineGenerationService
{
    private readonly IRoutineGenerationRepository _routineGenerationRepo;
    private readonly IRoutineEntryRepository _routineEntryRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RoutineGenerationService(
        IRoutineGenerationRepository routineGenerationRepo,
        IRoutineEntryRepository routineEntryRepo,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor)
    {
        _routineGenerationRepo = routineGenerationRepo;
        _routineEntryRepo = routineEntryRepo;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PagedResult<RoutineGenerationListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
        => await _routineGenerationRepo.GetPagedAsync(page, pageSize, search, cancellationToken);

    public async Task<RoutineGenerationListItemDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
        => await _routineGenerationRepo.GetForEditAsync(id, cancellationToken);

    public async Task<int> CreateAsync(RoutineGenerationListItemDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new RoutineEnt.RoutineGeneration
        {
            AcademicYearId = dto.AcademicYearId,
            Status = dto.Status,
            TotalAssignments = dto.TotalAssignments,
            SuccessfulAssignments = dto.SuccessfulAssignments,
            FailedAssignments = dto.FailedAssignments,
            ConflictsDetected = dto.ConflictsDetected,
            ErrorMessage = dto.ErrorMessage,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await _routineGenerationRepo.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await LogAuditAsync("Create", "RoutineGeneration", entity.Id, null, $"Year={dto.AcademicYearId},Status={dto.Status}", cancellationToken);

        return entity.Id;
    }

    public async Task UpdateAsync(RoutineGenerationListItemDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _routineGenerationRepo.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Routine generation not found.");

        entity.Status = dto.Status;
        entity.TotalAssignments = dto.TotalAssignments;
        entity.SuccessfulAssignments = dto.SuccessfulAssignments;
        entity.FailedAssignments = dto.FailedAssignments;
        entity.ConflictsDetected = dto.ConflictsDetected;
        entity.ErrorMessage = dto.ErrorMessage;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await LogAuditAsync("Update", "RoutineGeneration", dto.Id, null, $"Status={dto.Status}", cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _routineGenerationRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Routine generation not found.");

        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await LogAuditAsync("Delete", "RoutineGeneration", id, null, $"Soft-deleted generation {id}", cancellationToken);
    }

    public async Task<int> GenerateAsync(int academicYearId, string createdBy, CancellationToken cancellationToken = default)
    {
        var generation = new RoutineEnt.RoutineGeneration
        {
            AcademicYearId = academicYearId,
            Status = "Running",
            StartedAt = DateTime.UtcNow,
            TotalAssignments = 0,
            SuccessfulAssignments = 0,
            FailedAssignments = 0,
            ConflictsDetected = 0,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await _routineGenerationRepo.AddAsync(generation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await LogAuditAsync("GenerateStart", "RoutineGeneration", generation.Id, null, $"Year={academicYearId}", cancellationToken);

        try
        {
            var engine = new RoutineSchedulingEngine(_unitOfWork);
            var genResult = await engine.GenerateAsync(academicYearId, createdBy, cancellationToken);

            var result = new SchedulingResult
            {
                Success = genResult.Success,
                Partial = !genResult.Success && genResult.PlacedTokens > 0,
                TotalAssignments = genResult.TotalTokens,
                SuccessfulAssignments = genResult.PlacedTokens,
                FailedAssignments = genResult.ConflictTokens,
                ConflictsDetected = genResult.Conflicts.Count
            };

            generation.Status = result.Success ? "Completed" : result.Partial ? "Partial" : "Failed";
            generation.CompletedAt = DateTime.UtcNow;
            generation.TotalAssignments = result.TotalAssignments;
            generation.SuccessfulAssignments = result.SuccessfulAssignments;
            generation.FailedAssignments = result.FailedAssignments;
            generation.ConflictsDetected = result.ConflictsDetected;
            generation.UpdatedBy = createdBy;
            generation.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await LogAuditAsync("GenerateComplete", "RoutineGeneration", generation.Id, null, $"Status={generation.Status},Success={result.SuccessfulAssignments},Failed={result.FailedAssignments}", cancellationToken);
        }
        catch (Exception ex)
        {
            generation.Status = "Failed";
            generation.CompletedAt = DateTime.UtcNow;
            generation.ErrorMessage = ex.Message.Length > 4000 ? ex.Message[..4000] : ex.Message;
            generation.UpdatedBy = createdBy;
            generation.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var errorMsg = ex.Message.Length > 200 ? ex.Message[..200] : ex.Message;
            await LogAuditAsync("GenerateComplete", "RoutineGeneration", generation.Id, null, $"Status=Failed,Error={errorMsg}", cancellationToken);
        }

        return generation.Id;
    }

    public async Task<List<RoutineConflictListItemDto>> GetConflictsAsync(int generationId, CancellationToken cancellationToken = default)
        => await _routineGenerationRepo.GetConflictsAsync(generationId, cancellationToken);

    private async Task LogAuditAsync(string action, string entity, int? entityId, string? oldValue, string? newValue, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userIdStr = httpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        int? userId = userIdStr != null && int.TryParse(userIdStr, out var uid) ? uid : null;

        var details = entityId.HasValue
            ? $"[{entity}#{entityId}] {action}"
            : $"[{entity}] {action}";

        if (oldValue != null || newValue != null)
            details += $" | Old: {oldValue} | New: {newValue}";

        var log = new AuditLog
        {
            UserId = userId,
            Module = "Routine",
            Action = $"{entity}.{action}",
            IpAddress = httpContext?.Connection?.RemoteIpAddress?.ToString(),
            Details = details.Length > 1000 ? details[..1000] : details,
            CreatedBy = httpContext?.User?.Identity?.Name ?? "system",
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<AuditLog>().AddAsync(log, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static string GetDayName(int dayNumber) => dayNumber switch
    {
        1 => "Saturday",
        2 => "Sunday",
        3 => "Monday",
        4 => "Tuesday",
        5 => "Wednesday",
        6 => "Thursday",
        7 => "Friday",
        _ => "Unknown"
    };
}

public class RoutineVersionService : IRoutineVersionService
{
    private readonly IRoutineVersionRepository _routineVersionRepo;
    private readonly IRoutineEntryRepository _routineEntryRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private static readonly string PublishedCachePrefix = "RoutinePublished_";

    public RoutineVersionService(
        IRoutineVersionRepository routineVersionRepo,
        IRoutineEntryRepository routineEntryRepo,
        IUnitOfWork unitOfWork,
        IMemoryCache cache,
        IHttpContextAccessor httpContextAccessor)
    {
        _routineVersionRepo = routineVersionRepo;
        _routineEntryRepo = routineEntryRepo;
        _unitOfWork = unitOfWork;
        _cache = cache;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PagedResult<RoutineVersionListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
        => await _routineVersionRepo.GetPagedAsync(page, pageSize, search, cancellationToken);

    public async Task<RoutineVersionUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
        => await _routineVersionRepo.GetForEditAsync(id, cancellationToken);

    public async Task<int> CreateAsync(RoutineVersionUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entryCount = dto.EntryCount;
        if (entryCount == 0)
        {
            entryCount = await _routineEntryRepo.CountAsync(
                x => !x.IsDeleted && x.AcademicYearId == dto.AcademicYearId, cancellationToken);
        }

        var entity = new RoutineEnt.RoutineVersion
        {
            AcademicYearId = dto.AcademicYearId,
            Name = dto.Name.Trim(),
            Status = "Draft",
            EntryCount = entryCount,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await _routineVersionRepo.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await LogAuditAsync("Create", "RoutineVersion", entity.Id, null, $"Year={dto.AcademicYearId},Name={dto.Name}", cancellationToken);

        return entity.Id;
    }

    public async Task UpdateAsync(RoutineVersionUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _routineVersionRepo.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Routine version not found.");

        entity.Name = dto.Name.Trim();
        entity.Status = dto.Status;
        entity.EntryCount = dto.EntryCount;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await LogAuditAsync("Update", "RoutineVersion", dto.Id, null, $"Name={dto.Name},Status={dto.Status}", cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _routineVersionRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Routine version not found.");

        if (entity.Status != "Draft" && entity.Status != "Archived")
            throw new InvalidOperationException("Only draft or archived versions can be deleted.");

        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await LogAuditAsync("Delete", "RoutineVersion", id, null, $"Soft-deleted version {id}", cancellationToken);
    }

    public async Task<RoutineVersionListItemDto?> PublishAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _routineVersionRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity == null) return null;

        entity.Status = "Published";
        entity.PublishedAt = DateTime.UtcNow;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _cache.Remove($"{PublishedCachePrefix}{entity.AcademicYearId}");
        await LogAuditAsync("Publish", "RoutineVersion", id, null, $"Version {id} published", cancellationToken);

        return await GetListItemAsync(id, cancellationToken);
    }

    public async Task<RoutineVersionListItemDto?> ApproveAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _routineVersionRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity == null) return null;

        entity.Status = "Approved";
        entity.ApprovedAt = DateTime.UtcNow;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _cache.Remove($"{PublishedCachePrefix}{entity.AcademicYearId}");
        await LogAuditAsync("Approve", "RoutineVersion", id, null, $"Version {id} approved", cancellationToken);

        return await GetListItemAsync(id, cancellationToken);
    }

    public async Task ArchiveAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _routineVersionRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Routine version not found.");

        entity.Status = "Archived";
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _cache.Remove($"{PublishedCachePrefix}{entity.AcademicYearId}");
        await LogAuditAsync("Archive", "RoutineVersion", id, null, $"Version {id} archived", cancellationToken);
    }

    public async Task<RoutineVersionListItemDto?> GetPublishedAsync(int academicYearId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{PublishedCachePrefix}{academicYearId}";
        if (_cache.TryGetValue(cacheKey, out RoutineVersionListItemDto? cached) && cached != null)
            return cached;

        var result = await _routineVersionRepo.GetPublishedAsync(academicYearId, cancellationToken);

        if (result != null)
        {
            _cache.Set(cacheKey, result, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(5)
            });
        }

        return result;
    }

    private async Task<RoutineVersionListItemDto?> GetListItemAsync(int id, CancellationToken cancellationToken)
    {
        return await _routineVersionRepo.Query()
            .AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new RoutineVersionListItemDto
            {
                Id = x.Id,
                AcademicYearId = x.AcademicYearId,
                AcademicYearName = x.AcademicYear!.Name,
                Name = x.Name,
                Status = x.Status,
                EntryCount = x.EntryCount,
                PublishedAt = x.PublishedAt.HasValue ? x.PublishedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                ApprovedAt = x.ApprovedAt.HasValue ? x.ApprovedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task LogAuditAsync(string action, string entity, int? entityId, string? oldValue, string? newValue, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userIdStr = httpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        int? userId = userIdStr != null && int.TryParse(userIdStr, out var uid) ? uid : null;

        var details = entityId.HasValue
            ? $"[{entity}#{entityId}] {action}"
            : $"[{entity}] {action}";

        if (oldValue != null || newValue != null)
            details += $" | Old: {oldValue} | New: {newValue}";

        var log = new AuditLog
        {
            UserId = userId,
            Module = "Routine",
            Action = $"{entity}.{action}",
            IpAddress = httpContext?.Connection?.RemoteIpAddress?.ToString(),
            Details = details.Length > 1000 ? details[..1000] : details,
            CreatedBy = httpContext?.User?.Identity?.Name ?? "system",
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<AuditLog>().AddAsync(log, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public class RoutineEngineService : IRoutineEngineService
{
    private readonly IRoutineDashboardRepository _dashboardRepo;
    private readonly IRoutineAnalyticsRepository _analyticsRepo;
    private readonly ITeacherLoadRepository _teacherLoadRepo;
    private readonly IRoomUtilizationRepository _roomUtilizationRepo;
    private readonly IRoutineEntryRepository _routineEntryRepo;
    private readonly IRoutineGenerationRepository _routineGenerationRepo;
    private readonly IRoutineVersionRepository _routineVersionRepo;
    private readonly IRoutineConflictRepository _routineConflictRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RoutineEngineService(
        IRoutineDashboardRepository dashboardRepo,
        IRoutineAnalyticsRepository analyticsRepo,
        ITeacherLoadRepository teacherLoadRepo,
        IRoomUtilizationRepository roomUtilizationRepo,
        IRoutineEntryRepository routineEntryRepo,
        IRoutineGenerationRepository routineGenerationRepo,
        IRoutineVersionRepository routineVersionRepo,
        IRoutineConflictRepository routineConflictRepo,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor)
    {
        _dashboardRepo = dashboardRepo;
        _analyticsRepo = analyticsRepo;
        _teacherLoadRepo = teacherLoadRepo;
        _roomUtilizationRepo = roomUtilizationRepo;
        _routineEntryRepo = routineEntryRepo;
        _routineGenerationRepo = routineGenerationRepo;
        _routineVersionRepo = routineVersionRepo;
        _routineConflictRepo = routineConflictRepo;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<RoutineGenerationListItemDto> GenerateRoutineAsync(int academicYearId, string createdBy, CancellationToken cancellationToken = default)
    {
        var existingEntries = await _routineEntryRepo.Query()
            .Where(x => !x.IsDeleted && x.AcademicYearId == academicYearId)
            .ToListAsync(cancellationToken);

        foreach (var entry in existingEntries)
            entry.IsDeleted = true;

        if (existingEntries.Count > 0)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        var generation = new RoutineEnt.RoutineGeneration
        {
            AcademicYearId = academicYearId,
            Status = "Running",
            StartedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await _routineGenerationRepo.AddAsync(generation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await LogAuditAsync("GenerateStart", "RoutineGeneration", generation.Id, null, $"Year={academicYearId}", cancellationToken);

        SchedulingResult result;
        try
        {
            var engine = new RoutineSchedulingEngine(_unitOfWork);
            var genResult = await engine.GenerateAsync(academicYearId, createdBy, cancellationToken);
            result = new SchedulingResult
            {
                Success = genResult.Success,
                Partial = !genResult.Success && genResult.PlacedTokens > 0,
                TotalAssignments = genResult.TotalTokens,
                SuccessfulAssignments = genResult.PlacedTokens,
                FailedAssignments = genResult.ConflictTokens,
                ConflictsDetected = genResult.Conflicts.Count
            };
        }
        catch (Exception ex)
        {
            generation.Status = "Failed";
            generation.CompletedAt = DateTime.UtcNow;
            generation.ErrorMessage = ex.Message.Length > 4000 ? ex.Message[..4000] : ex.Message;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var errMsg = ex.Message.Length > 200 ? ex.Message[..200] : ex.Message;
            await LogAuditAsync("GenerateComplete", "RoutineGeneration", generation.Id, null, $"Status=Failed,Error={errMsg}", cancellationToken);

            return await MapGenerationToDto(generation, cancellationToken);
        }

        generation.Status = result.Success ? "Completed" : result.Partial ? "Partial" : "Failed";
        generation.CompletedAt = DateTime.UtcNow;
        generation.TotalAssignments = result.TotalAssignments;
        generation.SuccessfulAssignments = result.SuccessfulAssignments;
        generation.FailedAssignments = result.FailedAssignments;
        generation.ConflictsDetected = result.ConflictsDetected;

        var conflicts = await DetectConflictsInternalAsync(academicYearId, generation.Id, cancellationToken);
        generation.ConflictsDetected = conflicts.Count;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await LogAuditAsync("GenerateComplete", "RoutineGeneration", generation.Id, null, $"Status={generation.Status},Success={result.SuccessfulAssignments},Failed={result.FailedAssignments}", cancellationToken);

        var entryCount = await _routineEntryRepo.CountAsync(
            x => !x.IsDeleted && x.AcademicYearId == academicYearId, cancellationToken);

        var version = new RoutineEnt.RoutineVersion
        {
            AcademicYearId = academicYearId,
            Name = $"Auto-Generated {DateTime.UtcNow:yyyy-MM-dd HH:mm}",
            Status = "Draft",
            EntryCount = entryCount,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await _routineVersionRepo.AddAsync(version, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await MapGenerationToDto(generation, cancellationToken);
    }

    public async Task<List<RoutineConflictListItemDto>> ValidateRoutineAsync(int academicYearId, CancellationToken cancellationToken = default)
    {
        var conflicts = new List<RoutineConflictListItemDto>();

        var entries = await _routineEntryRepo.Query()
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.AcademicYearId == academicYearId)
            .ToListAsync(cancellationToken);

        var teacherSlots = new HashSet<string>();
        var roomSlots = new HashSet<string>();
        var classSlots = new HashSet<string>();

        foreach (var entry in entries)
        {
            var teacherKey = $"{entry.DayNumber}|{entry.RoutinePeriodId}|{entry.TeacherId}";
            if (!teacherSlots.Add(teacherKey))
            {
                conflicts.Add(new RoutineConflictListItemDto
                {
                    ConflictType = "TeacherConflict",
                    Description = $"Teacher ID {entry.TeacherId} is double-booked at day {entry.DayNumber}, period {entry.RoutinePeriodId}",
                    TeacherId = entry.TeacherId,
                    DayNumber = entry.DayNumber,
                    RoutinePeriodId = entry.RoutinePeriodId
                });
            }

            var roomKey = $"{entry.DayNumber}|{entry.RoutinePeriodId}|{entry.RoomId}";
            if (!roomSlots.Add(roomKey))
            {
                conflicts.Add(new RoutineConflictListItemDto
                {
                    ConflictType = "RoomConflict",
                    Description = $"Room ID {entry.RoomId} is double-booked at day {entry.DayNumber}, period {entry.RoutinePeriodId}",
                    RoomId = entry.RoomId,
                    DayNumber = entry.DayNumber,
                    RoutinePeriodId = entry.RoutinePeriodId
                });
            }

            var classKey = $"{entry.DayNumber}|{entry.RoutinePeriodId}|{entry.ClassId}|{entry.SectionId}|{entry.GroupId}";
            if (!classSlots.Add(classKey))
            {
                conflicts.Add(new RoutineConflictListItemDto
                {
                    ConflictType = "StudentConflict",
                    Description = $"Class {entry.ClassId} is double-booked at day {entry.DayNumber}, period {entry.RoutinePeriodId}",
                    ClassId = entry.ClassId,
                    DayNumber = entry.DayNumber,
                    RoutinePeriodId = entry.RoutinePeriodId
                });
            }
        }

        return conflicts;
    }

    public async Task<List<RoutineConflictListItemDto>> DetectConflictsAsync(int generationId, CancellationToken cancellationToken = default)
    {
        var generation = await _routineGenerationRepo.FirstOrDefaultAsync(x => x.Id == generationId && !x.IsDeleted, cancellationToken);

        if (generation == null)
            return new List<RoutineConflictListItemDto>();

        return await DetectConflictsInternalAsync(generation.AcademicYearId, generationId, cancellationToken);
    }

    public async Task<List<TeacherLoadDto>> GetTeacherLoadSummaryAsync(int academicYearId, CancellationToken cancellationToken = default)
        => await _teacherLoadRepo.GetTeacherLoadSummaryAsync(academicYearId);

    public async Task<List<RoomUtilizationDto>> GetRoomUtilizationAsync(int academicYearId, CancellationToken cancellationToken = default)
        => await _roomUtilizationRepo.GetRoomUtilizationAsync(academicYearId);

    public async Task<List<SubjectDistributionDto>> GetSubjectDistributionAsync(int academicYearId, CancellationToken cancellationToken = default)
    {
        return await _routineEntryRepo.Query()
            .AsNoTracking()
            .Include(x => x.Subject)
            .Where(x => !x.IsDeleted && x.AcademicYearId == academicYearId)
            .GroupBy(x => x.Subject!.Name)
            .Select(g => new SubjectDistributionDto
            {
                SubjectName = g.Key,
                TotalPeriods = g.Count()
            })
            .OrderByDescending(x => x.TotalPeriods)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<RoutineConflictListItemDto>> CheckHolidayConflictsAsync(int academicYearId, CancellationToken cancellationToken = default)
    {
        var conflicts = new List<RoutineConflictListItemDto>();

        var holidays = await _unitOfWork.Repository<AcademicCalendar>().Query()
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.AcademicYearId == academicYearId && x.IsHoliday)
            .Select(x => x.Date)
            .ToListAsync(cancellationToken);

        var academicYear = await _unitOfWork.Repository<AcademicYear>().Query()
            .AsNoTracking()
            .Where(x => x.Id == academicYearId)
            .FirstOrDefaultAsync(cancellationToken);

        var holidayMasters = await _unitOfWork.Repository<HolidayMaster>().Query()
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsActive)
            .ToListAsync(cancellationToken);

        var holidayDayNumbers = holidays
            .Select(h => (int)h.DayOfWeek)
            .Distinct()
            .ToHashSet();

        if (academicYear != null)
        {
            var masterHolidaysInRange = holidayMasters
                .Where(h => h.HolidayDate >= DateOnly.FromDateTime(academicYear.StartsOn)
                         && h.HolidayDate <= DateOnly.FromDateTime(academicYear.EndsOn))
                .Select(h => (int)h.HolidayDate.DayOfWeek)
                .Distinct();

            foreach (var dn in masterHolidaysInRange)
                holidayDayNumbers.Add(dn);
        }

        if (holidayDayNumbers.Count == 0)
            return conflicts;

        var entries = await _routineEntryRepo.Query()
            .AsNoTracking()
            .Include(x => x.Subject)
            .Include(x => x.Teacher).ThenInclude(t => t!.Employee)
            .Include(x => x.RoutinePeriod)
            .Include(x => x.Class)
            .Where(x => !x.IsDeleted && x.AcademicYearId == academicYearId)
            .ToListAsync(cancellationToken);

        foreach (var entry in entries)
        {
            if (holidayDayNumbers.Contains(entry.DayNumber))
            {
                conflicts.Add(new RoutineConflictListItemDto
                {
                    ConflictType = "HolidayConflict",
                    Description = $"Entry for '{entry.Subject?.Name}' on {GetDayName(entry.DayNumber)} falls on a holiday",
                    TeacherId = entry.TeacherId,
                    TeacherName = entry.Teacher?.Employee?.FullName,
                    SubjectId = entry.SubjectId,
                    SubjectName = entry.Subject?.Name,
                    ClassId = entry.ClassId,
                    ClassName = entry.Class?.Name,
                    RoutinePeriodId = entry.RoutinePeriodId,
                    PeriodName = entry.RoutinePeriod?.Name,
                    DayNumber = entry.DayNumber,
                    DayName = GetDayName(entry.DayNumber),
                    IsResolved = false
                });
            }
        }

        return conflicts;
    }

    public async Task<RoutineAnalyticsViewModel> GetAnalyticsAsync(int academicYearId, CancellationToken cancellationToken = default)
    {
        var teacherLoad = await _teacherLoadRepo.GetTeacherLoadSummaryAsync(academicYearId);
        var roomUtil = await _roomUtilizationRepo.GetRoomUtilizationAsync(academicYearId);
        var subjectDist = await GetSubjectDistributionAsync(academicYearId, cancellationToken);

        var totalConflicts = 0;
        var teacherConflicts = 0;
        var roomConflicts = 0;
        var studentConflicts = 0;

        var generationIds = await _routineGenerationRepo.Query()
            .AsNoTracking()
            .Where(g => !g.IsDeleted && g.AcademicYearId == academicYearId)
            .Select(g => g.Id)
            .ToListAsync(cancellationToken);

        if (generationIds.Count > 0)
        {
            var conflicts = await _routineConflictRepo.Query()
                .AsNoTracking()
                .Where(c => generationIds.Contains(c.GenerationId ?? 0))
                .ToListAsync(cancellationToken);

            totalConflicts = conflicts.Count;
            teacherConflicts = conflicts.Count(c => c.ConflictType == "TeacherConflict");
            roomConflicts = conflicts.Count(c => c.ConflictType == "RoomConflict");
            studentConflicts = conflicts.Count(c => c.ConflictType == "StudentConflict");
        }

        return new RoutineAnalyticsViewModel
        {
            TeacherLoadSummary = teacherLoad,
            RoomUtilization = roomUtil,
            SubjectDistribution = subjectDist,
            TotalConflicts = totalConflicts,
            TeacherConflicts = teacherConflicts,
            RoomConflicts = roomConflicts,
            StudentConflicts = studentConflicts
        };
    }

    public async Task<List<TeacherWorkloadListItemDto>> GetWorkloadSummaryAsync(int academicYearId, CancellationToken ct = default)
    {
        var loadData = await _teacherLoadRepo.GetTeacherLoadSummaryAsync(academicYearId);
        if (loadData == null || loadData.Count == 0)
        {
            loadData = await GetFallbackWorkloadAsync(academicYearId, ct);
        }

        var employees = await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>().Query()
            .Where(e => !e.IsDeleted && e.IsTeachingStaff)
            .Select(e => new { e.Id, e.EmployeeCode, e.DepartmentId })
            .AsNoTracking()
            .ToListAsync(ct);

        var departments = await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Employee.Department>().Query()
            .Where(d => !d.IsDeleted)
            .Select(d => new { d.Id, d.Name })
            .AsNoTracking()
            .ToListAsync(ct);

        var deptLookup = departments.ToDictionary(d => d.Id, d => d.Name);
        var empLookup = employees.ToDictionary(e => e.Id, e => new { e.EmployeeCode, e.DepartmentId });

        return loadData.Select(t => {
            empLookup.TryGetValue(t.TeacherId, out var emp);
            var overloadStatus = CalculateOverloadStatus(t.TotalPeriodsPerWeek, t.MaxPeriodsPerDay);
            var deptName = emp != null && deptLookup.TryGetValue(emp.DepartmentId, out var dn) ? dn : null;
            return new TeacherWorkloadListItemDto
            {
                TeacherId = t.TeacherId,
                TeacherName = t.TeacherName,
                EmployeeCode = emp?.EmployeeCode ?? "",
                Department = deptName,
                TotalPeriodsPerWeek = t.TotalPeriodsPerWeek,
                TotalClasses = t.TotalClasses,
                TotalSubjects = t.TotalSubjects,
                MaxPeriodsPerDay = t.MaxPeriodsPerDay,
                WorkingDays = t.WorkingDays,
                AveragePerDay = t.AveragePerDay,
                UtilizationPercent = t.UtilizationPercent,
                OverloadStatus = overloadStatus,
                RoutineEntryCount = t.TotalPeriodsPerWeek
            };
        }).OrderByDescending(x => x.TotalPeriodsPerWeek).ToList();
    }

    public async Task<TeacherWorkloadDetailDto?> GetTeacherWorkloadDetailAsync(int teacherId, int academicYearId, CancellationToken ct = default)
    {
        var loadData = await _teacherLoadRepo.GetTeacherLoadSummaryAsync(academicYearId);
        var teacherLoad = loadData?.FirstOrDefault(t => t.TeacherId == teacherId);

        if (teacherLoad == null) return null;

        var teacher = await _unitOfWork.Repository<Teacher>().Query()
            .Include(t => t.Employee)
            .ThenInclude(e => e.Department)
            .Include(t => t.Employee.Designation)
            .FirstOrDefaultAsync(t => t.Id == teacherId && !t.IsDeleted, ct);

        if (teacher?.Employee == null) return null;

        var entries = await _unitOfWork.Repository<RoutineEntry>().Query()
            .Include(re => re.RoutinePeriod)
            .Include(re => re.Subject)
            .Include(re => re.Class)
            .Include(re => re.Section)
            .Include(re => re.Room)
            .Where(re => re.TeacherId == teacherId && re.AcademicYearId == academicYearId && !re.IsDeleted)
            .OrderBy(re => re.DayNumber).ThenBy(re => re.RoutinePeriod.PeriodNumber)
            .AsNoTracking()
            .ToListAsync(ct);

        var dayNames = new[] { "", "Saturday", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
        var daySchedules = entries.GroupBy(e => e.DayNumber)
            .Select(g => new TeacherDayScheduleDto
            {
                DayNumber = g.Key,
                DayName = g.Key >= 1 && g.Key <= 7 ? dayNames[g.Key] : $"Day {g.Key}",
                PeriodCount = g.Count(),
                Periods = g.Select(e => new TeacherPeriodDto
                {
                    RoutineEntryId = e.Id,
                    PeriodName = e.RoutinePeriod?.Name ?? "",
                    StartTime = e.RoutinePeriod?.StartTime.ToString(@"hh\:mm") ?? "",
                    EndTime = e.RoutinePeriod?.EndTime.ToString(@"hh\:mm") ?? "",
                    SubjectName = e.Subject?.Name ?? "",
                    ClassName = e.Class?.Name ?? "",
                    SectionName = e.Section?.Name ?? "",
                    RoomNo = e.Room?.RoomNo ?? "",
                    IsBreak = e.RoutinePeriod?.IsBreak ?? false
                }).ToList()
            })
            .OrderBy(d => d.DayNumber)
            .ToList();

        return new TeacherWorkloadDetailDto
        {
            TeacherId = teacherLoad.TeacherId,
            TeacherName = teacherLoad.TeacherName,
            EmployeeCode = teacher.Employee.EmployeeCode,
            Department = teacher.Employee.Department?.Name,
            Designation = teacher.Employee.Designation?.Name,
            TotalPeriodsPerWeek = teacherLoad.TotalPeriodsPerWeek,
            TotalClasses = teacherLoad.TotalClasses,
            TotalSubjects = teacherLoad.TotalSubjects,
            MaxPeriodsPerDay = teacherLoad.MaxPeriodsPerDay,
            WorkingDays = teacherLoad.WorkingDays,
            AveragePerDay = teacherLoad.AveragePerDay,
            UtilizationPercent = teacherLoad.UtilizationPercent,
            WeeklyPeriodsByDay = teacherLoad.WeeklyPeriodsByDay,
            DaySchedules = daySchedules
        };
    }

    public async Task<int> GetOverloadedTeacherCountAsync(int academicYearId, int maxPeriodsPerDay = 8, CancellationToken ct = default)
    {
        var loadData = await _teacherLoadRepo.GetTeacherLoadSummaryAsync(academicYearId);
        return loadData?.Count(t => t.MaxPeriodsPerDay > maxPeriodsPerDay || t.TotalPeriodsPerWeek > maxPeriodsPerDay * 5) ?? 0;
    }

    private string CalculateOverloadStatus(int totalPeriods, int maxPerDay)
    {
        if (maxPerDay > 8 || totalPeriods > 40) return "Critical";
        if (maxPerDay > 6 || totalPeriods > 35) return "Warning";
        return "Normal";
    }

    private async Task<List<TeacherLoadDto>> GetFallbackWorkloadAsync(int academicYearId, CancellationToken ct)
    {
        var requirements = await _unitOfWork.Repository<SubjectRequirement>().Query()
            .Include(sr => sr.Teacher)
            .ThenInclude(t => t.Employee)
            .Where(sr => sr.AcademicYearId == academicYearId && !sr.IsDeleted)
            .AsNoTracking()
            .ToListAsync(ct);

        return requirements.GroupBy(sr => sr.TeacherId)
            .Select(g => {
                var teacher = g.First().Teacher;
                var totalPeriods = g.Sum(sr => sr.PeriodsPerWeek);
                return new TeacherLoadDto
                {
                    TeacherId = g.Key,
                    TeacherName = teacher?.Employee?.FullName ?? "Unknown",
                    TotalPeriodsPerWeek = totalPeriods,
                    TotalClasses = g.Select(sr => sr.ClassId).Distinct().Count(),
                    TotalSubjects = g.Select(sr => sr.SubjectId).Distinct().Count(),
                    MaxPeriodsPerDay = (int)Math.Ceiling(totalPeriods / 5.0),
                    WorkingDays = 5,
                    AveragePerDay = Math.Round(totalPeriods / 5.0, 1),
                    UtilizationPercent = Math.Round(totalPeriods * 100.0 / 40.0, 1),
                    WeeklyPeriodsByDay = new Dictionary<int, int>()
                };
            }).OrderByDescending(t => t.TotalPeriodsPerWeek).ToList();
    }

    public async Task<RoutineDashboardDto> GetDashboardAsync(int academicYearId, CancellationToken cancellationToken = default)
    {
        var dashData = await _dashboardRepo.GetDashboardAsync(academicYearId);
        dashData.TeacherLoadSummary = await _teacherLoadRepo.GetTeacherLoadSummaryAsync(academicYearId);
        dashData.RoomUtilization = await _roomUtilizationRepo.GetRoomUtilizationAsync(academicYearId);
        return dashData;
    }

    // ── Cross-entity lookup methods ─────────────────────────────────

    public async Task<AcademicYear?> GetCurrentAcademicYearAsync(CancellationToken ct)
    {
        return await _unitOfWork.Repository<AcademicYear>().Query()
            .AsNoTracking()
            .Where(y => !y.IsDeleted && y.IsActive)
            .OrderByDescending(y => y.StartsOn)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<RoutineGeneration?> GetGenerationByIdAsync(int id, CancellationToken ct)
    {
        return await _routineGenerationRepo.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<List<AcademicYearItem>> GetAcademicYearItemsAsync(CancellationToken ct)
    {
        return await _unitOfWork.Repository<AcademicYear>().Query()
            .AsNoTracking()
            .Where(y => !y.IsDeleted)
            .OrderByDescending(y => y.StartsOn)
            .Select(y => new AcademicYearItem { Id = y.Id, Name = y.Name, IsActive = y.IsActive })
            .ToListAsync(ct);
    }

    public async Task<StudentModel?> GetStudentByUserIdAsync(int userId, CancellationToken ct)
    {
        return await _unitOfWork.Repository<StudentModel>().Query()
            .AsNoTracking()
            .Include(s => s.Class)
            .Include(s => s.Section)
            .Include(s => s.StudentGroup)
            .FirstOrDefaultAsync(s => s.UserId == userId, ct);
    }

    public async Task<(ApplicationUser? User, Teacher? Teacher)> GetUserAndTeacherAsync(int userId, CancellationToken ct)
    {
        var user = await _unitOfWork.Repository<ApplicationUser>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        Teacher? teacher = null;
        if (user?.EmployeeId != null)
        {
            teacher = await _unitOfWork.Repository<Teacher>().Query()
                .AsNoTracking()
                .Include(t => t.Employee)
                .FirstOrDefaultAsync(t => t.EmployeeId == user.EmployeeId, ct);
        }

        return (user, teacher);
    }

    public async Task<List<TeacherLookupDto>> GetTeacherLookupAsync(CancellationToken ct)
    {
        return await _unitOfWork.Repository<Teacher>().Query()
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Include(x => x.Employee)
            .OrderBy(x => x.Employee!.FullName)
            .Select(x => new TeacherLookupDto(x.Id, x.Employee!.FullName))
            .ToListAsync(ct);
    }

    public async Task<List<ClassItem>> GetClassItemsAsync(CancellationToken ct)
    {
        return await _unitOfWork.Repository<SchoolClass>().Query()
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.SortOrder)
            .Select(c => new ClassItem { Id = c.Id, Name = c.Name })
            .ToListAsync(ct);
    }

    public async Task<List<SubjectLookupDto>> GetSubjectLookupAsync(CancellationToken ct)
    {
        return await _unitOfWork.Repository<Subject>().Query()
            .AsNoTracking()
            .Where(s => !s.IsDeleted)
            .OrderBy(s => s.Name)
            .Select(s => new SubjectLookupDto(s.Id, s.Name))
            .ToListAsync(ct);
    }

    public async Task<List<PeriodLookupDto>> GetPeriodLookupAsync(CancellationToken ct)
    {
        return await _unitOfWork.Repository<RoutineEnt.RoutinePeriod>().Query()
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.IsActive)
            .OrderBy(p => p.PeriodNumber)
            .Select(p => new PeriodLookupDto(p.Id, p.Name, p.StartTime.ToString(@"hh\:mm"), p.EndTime.ToString(@"hh\:mm")))
            .ToListAsync(ct);
    }

    public async Task<List<RoutineEntryLookupDto>> GetRoutineEntryLookupAsync(CancellationToken ct)
    {
        return await _unitOfWork.Repository<RoutineEnt.RoutineEntry>().Query()
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Include(x => x.Subject)
            .Include(x => x.Class)
            .Include(x => x.Teacher).ThenInclude(t => t!.Employee)
            .Include(x => x.RoutinePeriod)
            .OrderBy(x => x.DayNumber).ThenBy(x => x.RoutinePeriod!.PeriodNumber)
            .Select(x => new RoutineEntryLookupDto(
                x.Id,
                x.Subject!.Name + " - " + x.Class!.Name + " (" + x.Teacher!.Employee!.FullName + ") [" + x.DayNumber + "/" + x.RoutinePeriod!.Name + "]"))
            .ToListAsync(ct);
    }

    public async Task<List<SectionItem>> GetSectionsByClassAsync(int classId, CancellationToken ct)
    {
        return await _unitOfWork.Repository<Section>().Query()
            .AsNoTracking()
            .Where(s => s.SchoolClassId == classId && !s.IsDeleted)
            .OrderBy(s => s.Name)
            .Select(s => new SectionItem { Id = s.Id, Name = s.Name })
            .ToListAsync(ct);
    }

    public async Task<List<GroupLookupDto>> GetGroupsByClassAsync(int classId, CancellationToken ct)
    {
        var sectionIds = await _unitOfWork.Repository<Section>().Query()
            .AsNoTracking()
            .Where(s => s.SchoolClassId == classId && s.StudentGroupId != null && !s.IsDeleted)
            .Select(s => s.StudentGroupId!.Value)
            .Distinct()
            .ToListAsync(ct);

        return await _unitOfWork.Repository<StudentGroup>().Query()
            .AsNoTracking()
            .Where(g => sectionIds.Contains(g.Id) && !g.IsDeleted)
            .OrderBy(g => g.DisplayOrder)
            .Select(g => new GroupLookupDto(g.Id, g.Name))
            .ToListAsync(ct);
    }

    public async Task<List<RoomItem>> GetRoomItemsAsync(CancellationToken ct)
    {
        return await _unitOfWork.Repository<RoutineEnt.Room>().Query()
            .AsNoTracking()
            .Where(r => !r.IsDeleted)
            .OrderBy(r => r.RoomNo)
            .Select(r => new RoomItem { Id = r.Id, RoomNo = r.RoomNo, Name = r.Name })
            .ToListAsync(ct);
    }

    public async Task<PagedResult<RoutineConflictListItemDto>> GetConflictsPagedAsync(int page, int size, bool? unresolvedOnly, CancellationToken ct)
    {
        IQueryable<RoutineEnt.RoutineConflict> query = _unitOfWork.Repository<RoutineEnt.RoutineConflict>().Query()
            .AsNoTracking()
            .Include(x => x.Teacher).ThenInclude(x => x.Employee)
            .Include(x => x.Room)
            .Include(x => x.Subject)
            .Include(x => x.RoutinePeriod);

        if (unresolvedOnly == true)
            query = query.Where(x => !x.IsResolved);

        var totalItems = await query.CountAsync(ct);

        var conflicts = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(x => new RoutineConflictListItemDto
            {
                Id = x.Id,
                GenerationId = x.GenerationId,
                ConflictType = x.ConflictType,
                Description = x.Description,
                TeacherName = x.Teacher != null ? x.Teacher.Employee.FullName : null,
                RoomNo = x.Room != null ? x.Room.RoomNo : null,
                SubjectName = x.Subject != null ? x.Subject.Name : null,
                PeriodName = x.RoutinePeriod != null ? x.RoutinePeriod.Name : null,
                DayName = x.DayNumber.HasValue ? GetDayName(x.DayNumber.Value) : null,
                IsResolved = x.IsResolved
            })
            .ToListAsync(ct);

        return new PagedResult<RoutineConflictListItemDto>
        {
            Items = conflicts,
            TotalItems = totalItems,
            PageSize = size,
            Page = page
        };
    }

    private async Task<List<RoutineConflictListItemDto>> DetectConflictsInternalAsync(int academicYearId, int? generationId, CancellationToken cancellationToken)
    {
        var existing = await _routineConflictRepo.Query()
            .Where(x => x.GenerationId == generationId)
            .ToListAsync(cancellationToken);

        foreach (var c in existing)
            _routineConflictRepo.Remove(c);

        var entries = await _routineEntryRepo.Query()
            .AsNoTracking()
            .Include(x => x.Teacher).ThenInclude(t => t!.Employee)
            .Include(x => x.Room)
            .Include(x => x.Subject)
            .Include(x => x.Class)
            .Include(x => x.RoutinePeriod)
            .Where(x => !x.IsDeleted && x.AcademicYearId == academicYearId)
            .ToListAsync(cancellationToken);

        var holidayDayNumbers = await GetHolidayDayNumbersAsync(academicYearId, cancellationToken);

        var conflicts = new List<RoutineConflictListItemDto>();
        var teacherSlots = new Dictionary<string, RoutineEnt.RoutineEntry>();
        var roomSlots = new Dictionary<string, RoutineEnt.RoutineEntry>();
        var classSlots = new Dictionary<string, RoutineEnt.RoutineEntry>();
        var addedConflictKeys = new HashSet<string>();

        foreach (var entry in entries)
        {
            if (holidayDayNumbers.Contains(entry.DayNumber))
            {
                var holidayConflictKey = $"HOL|{entry.Id}";
                if (addedConflictKeys.Add(holidayConflictKey))
                {
                    conflicts.Add(new RoutineConflictListItemDto
                    {
                        GenerationId = generationId,
                        ConflictType = "HolidayConflict",
                        Description = $"Entry for '{entry.Subject?.Name ?? "Subject"}' falls on a holiday at {GetDayName(entry.DayNumber)}, period '{entry.RoutinePeriod?.Name}'",
                        TeacherId = entry.TeacherId,
                        TeacherName = entry.Teacher?.Employee?.FullName,
                        SubjectId = entry.SubjectId,
                        SubjectName = entry.Subject?.Name,
                        ClassId = entry.ClassId,
                        ClassName = entry.Class?.Name,
                        RoomId = entry.RoomId,
                        RoomNo = entry.Room?.RoomNo,
                        RoutinePeriodId = entry.RoutinePeriodId,
                        PeriodName = entry.RoutinePeriod?.Name,
                        DayNumber = entry.DayNumber,
                        DayName = GetDayName(entry.DayNumber),
                        IsResolved = false
                    });
                }
            }

            var teacherKey = $"{entry.DayNumber}|{entry.RoutinePeriodId}|{entry.TeacherId}";
            if (teacherSlots.TryGetValue(teacherKey, out var existingEntry))
            {
                var conflictKey = $"TCH|{teacherKey}";
                if (addedConflictKeys.Add(conflictKey))
                {
                    conflicts.Add(new RoutineConflictListItemDto
                    {
                        GenerationId = generationId,
                        ConflictType = "TeacherConflict",
                        Description = $"Teacher '{existingEntry.Teacher?.Employee?.FullName ?? "Unknown"}' is double-booked at {GetDayName(entry.DayNumber)}, period '{existingEntry.RoutinePeriod?.Name}'",
                        TeacherId = existingEntry.TeacherId,
                        TeacherName = existingEntry.Teacher?.Employee?.FullName,
                        DayNumber = existingEntry.DayNumber,
                        RoutinePeriodId = existingEntry.RoutinePeriodId,
                        PeriodName = existingEntry.RoutinePeriod?.Name,
                        IsResolved = false
                    });
                }
            }
            else
            {
                teacherSlots[teacherKey] = entry;
            }

            var roomKey = $"{entry.DayNumber}|{entry.RoutinePeriodId}|{entry.RoomId}";
            if (roomSlots.TryGetValue(roomKey, out existingEntry))
            {
                var conflictKey = $"ROM|{roomKey}";
                if (addedConflictKeys.Add(conflictKey))
                {
                    conflicts.Add(new RoutineConflictListItemDto
                    {
                        GenerationId = generationId,
                        ConflictType = "RoomConflict",
                        Description = $"Room '{existingEntry.Room?.RoomNo}' is double-booked at {GetDayName(entry.DayNumber)}, period '{existingEntry.RoutinePeriod?.Name}'",
                        RoomId = existingEntry.RoomId,
                        RoomNo = existingEntry.Room?.RoomNo,
                        DayNumber = existingEntry.DayNumber,
                        RoutinePeriodId = existingEntry.RoutinePeriodId,
                        PeriodName = existingEntry.RoutinePeriod?.Name,
                        IsResolved = false
                    });
                }
            }
            else
            {
                roomSlots[roomKey] = entry;
            }

            var classKey = $"{entry.DayNumber}|{entry.RoutinePeriodId}|{entry.ClassId}|{entry.SectionId}|{entry.GroupId}";
            if (classSlots.TryGetValue(classKey, out existingEntry))
            {
                var conflictKey = $"CLS|{classKey}";
                if (addedConflictKeys.Add(conflictKey))
                {
                    conflicts.Add(new RoutineConflictListItemDto
                    {
                        GenerationId = generationId,
                        ConflictType = "StudentConflict",
                        Description = $"Class '{existingEntry.Class?.Name}' is double-booked at {GetDayName(entry.DayNumber)}, period '{existingEntry.RoutinePeriod?.Name}'",
                        ClassId = existingEntry.ClassId,
                        ClassName = existingEntry.Class?.Name,
                        DayNumber = existingEntry.DayNumber,
                        RoutinePeriodId = existingEntry.RoutinePeriodId,
                        PeriodName = existingEntry.RoutinePeriod?.Name,
                        IsResolved = false
                    });
                }
            }
            else
            {
                classSlots[classKey] = entry;
            }
        }

        foreach (var conflict in conflicts)
        {
            var entity = new RoutineEnt.RoutineConflict
            {
                GenerationId = generationId,
                ConflictType = conflict.ConflictType,
                Description = conflict.Description,
                TeacherId = conflict.TeacherId,
                RoomId = conflict.RoomId,
                SubjectId = conflict.SubjectId,
                ClassId = conflict.ClassId,
                RoutinePeriodId = conflict.RoutinePeriodId,
                DayNumber = conflict.DayNumber,
                IsResolved = false,
                CreatedBy = "system",
                CreatedAt = DateTime.UtcNow
            };

            await _routineConflictRepo.AddAsync(entity, cancellationToken);
        }

        if (conflicts.Count > 0)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return conflicts;
    }

    private async Task<HashSet<int>> GetHolidayDayNumbersAsync(int academicYearId, CancellationToken cancellationToken)
    {
        var holidays = await _unitOfWork.Repository<AcademicCalendar>().Query()
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.AcademicYearId == academicYearId && x.IsHoliday)
            .Select(x => x.Date)
            .ToListAsync(cancellationToken);

        var academicYear = await _unitOfWork.Repository<AcademicYear>().Query()
            .AsNoTracking()
            .Where(x => x.Id == academicYearId)
            .FirstOrDefaultAsync(cancellationToken);

        var holidayMasters = await _unitOfWork.Repository<HolidayMaster>().Query()
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsActive)
            .ToListAsync(cancellationToken);

        var dayNumbers = holidays
            .Select(h => (int)h.DayOfWeek)
            .Distinct()
            .ToHashSet();

        if (academicYear != null)
        {
            var masterDays = holidayMasters
                .Where(h => h.HolidayDate >= DateOnly.FromDateTime(academicYear.StartsOn)
                         && h.HolidayDate <= DateOnly.FromDateTime(academicYear.EndsOn))
                .Select(h => (int)h.HolidayDate.DayOfWeek)
                .Distinct();

            foreach (var dn in masterDays)
                dayNumbers.Add(dn);
        }

        return dayNumbers;
    }

    private async Task LogAuditAsync(string action, string entity, int? entityId, string? oldValue, string? newValue, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userIdStr = httpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        int? userId = userIdStr != null && int.TryParse(userIdStr, out var uid) ? uid : null;

        var details = entityId.HasValue
            ? $"[{entity}#{entityId}] {action}"
            : $"[{entity}] {action}";

        if (oldValue != null || newValue != null)
            details += $" | Old: {oldValue} | New: {newValue}";

        var log = new AuditLog
        {
            UserId = userId,
            Module = "Routine",
            Action = $"{entity}.{action}",
            IpAddress = httpContext?.Connection?.RemoteIpAddress?.ToString(),
            Details = details.Length > 1000 ? details[..1000] : details,
            CreatedBy = httpContext?.User?.Identity?.Name ?? "system",
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<AuditLog>().AddAsync(log, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<RoutineGenerationListItemDto> MapGenerationToDto(RoutineEnt.RoutineGeneration generation, CancellationToken cancellationToken)
    {
        var yearName = await _unitOfWork.Repository<AcademicYear>().Query()
            .AsNoTracking()
            .Where(y => y.Id == generation.AcademicYearId)
            .Select(y => y.Name)
            .FirstOrDefaultAsync(cancellationToken);

        return new RoutineGenerationListItemDto
        {
            Id = generation.Id,
            AcademicYearId = generation.AcademicYearId,
            AcademicYearName = yearName ?? "",
            Status = generation.Status,
            StartedAt = generation.StartedAt?.ToString("yyyy-MM-dd HH:mm:ss"),
            CompletedAt = generation.CompletedAt?.ToString("yyyy-MM-dd HH:mm:ss"),
            TotalAssignments = generation.TotalAssignments,
            SuccessfulAssignments = generation.SuccessfulAssignments,
            FailedAssignments = generation.FailedAssignments,
            ConflictsDetected = generation.ConflictsDetected,
            ErrorMessage = generation.ErrorMessage
        };
    }

    private static string GetDayName(int dayNumber) => dayNumber switch
    {
        1 => "Saturday",
        2 => "Sunday",
        3 => "Monday",
        4 => "Tuesday",
        5 => "Wednesday",
        6 => "Thursday",
        7 => "Friday",
        _ => "Unknown"
    };
}

internal class SchedulingResult
{
    public bool Success { get; set; }
    public bool Partial { get; set; }
    public int TotalAssignments { get; set; }
    public int SuccessfulAssignments { get; set; }
    public int FailedAssignments { get; set; }
    public int ConflictsDetected { get; set; }
}

public class SubstituteService : ISubstituteService
{
    private readonly ISubstituteAssignmentRepository _substituteRepo;
    private readonly IRoutineEntryRepository _routineEntryRepo;
    private readonly IUnitOfWork _unitOfWork;

    public SubstituteService(
        ISubstituteAssignmentRepository substituteRepo,
        IRoutineEntryRepository routineEntryRepo,
        IUnitOfWork unitOfWork)
    {
        _substituteRepo = substituteRepo;
        _routineEntryRepo = routineEntryRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<SubstituteAssignmentListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
        => await _substituteRepo.GetPagedAsync(page, pageSize, search, cancellationToken);

    public async Task<SubstituteAssignmentUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
        => await _substituteRepo.GetForEditAsync(id, cancellationToken);

    public async Task<int> CreateAsync(SubstituteAssignmentUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var routineEntry = await _routineEntryRepo.Query()
            .Include(x => x.RoutinePeriod)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == dto.RoutineEntryId && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Routine entry not found.");

        var entity = new RoutineEnt.SubstituteAssignment
        {
            RoutineEntryId = dto.RoutineEntryId,
            OriginalTeacherId = routineEntry.TeacherId,
            SubstituteTeacherId = dto.SubstituteTeacherId,
            AssignedById = int.TryParse(createdBy, out var uid) ? uid : 0,
            AssignmentDate = DateTime.UtcNow,
            EffectiveDate = dto.EffectiveDate,
            PeriodNumber = routineEntry.RoutinePeriod?.PeriodNumber,
            DayNumber = routineEntry.DayNumber,
            Status = "Pending",
            Reason = dto.Reason?.Trim(),
            Notes = dto.Notes?.Trim(),
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await _substituteRepo.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(SubstituteAssignmentUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _substituteRepo.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Substitute assignment not found.");

        entity.SubstituteTeacherId = dto.SubstituteTeacherId;
        entity.EffectiveDate = dto.EffectiveDate;
        entity.Reason = dto.Reason?.Trim();
        entity.Notes = dto.Notes?.Trim();
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _substituteRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Substitute assignment not found.");

        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<SubstituteAssignmentListItemDto>> GetByTeacherAsync(int teacherId, CancellationToken cancellationToken = default)
        => await _substituteRepo.GetByTeacherAsync(teacherId, cancellationToken);

    public async Task<List<SubstituteAssignmentListItemDto>> GetPendingAsync(CancellationToken cancellationToken = default)
        => await _substituteRepo.GetPendingAsync(cancellationToken);

    public async Task ApproveAsync(int id, string approvedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _substituteRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Substitute assignment not found.");

        if (entity.Status != "Pending")
            throw new InvalidOperationException("Only pending assignments can be approved.");

        entity.Status = "Approved";
        entity.ApprovedAt = DateTime.UtcNow;
        entity.UpdatedBy = approvedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeclineAsync(int id, string reason, CancellationToken cancellationToken = default)
    {
        var entity = await _substituteRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Substitute assignment not found.");

        if (entity.Status != "Pending")
            throw new InvalidOperationException("Only pending assignments can be declined.");

        entity.Status = "Declined";
        entity.Reason = reason?.Trim();
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
