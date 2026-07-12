using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Routine;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.Routine;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Repositories.Interfaces.Routine;
using RoutineEnt = SchoolManagementSystem.Models.Entities.Routine;

namespace SchoolManagementSystem.Repositories.Implementations.Routine;

public class RoutinePeriodRepository : BaseRepository<RoutinePeriod>, IRoutinePeriodRepository
{
    public RoutinePeriodRepository(SchoolDbContext db) : base(db) { }

    public async Task<PagedResult<RoutinePeriodListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var query = _set.AsNoTracking().Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.Name.Contains(search));

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.PeriodNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new RoutinePeriodListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                StartTime = x.StartTime.ToString(@"hh\:mm"),
                EndTime = x.EndTime.ToString(@"hh\:mm"),
                PeriodNumber = x.PeriodNumber,
                IsBreak = x.IsBreak,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<RoutinePeriodListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };
    }

    public async Task<RoutinePeriodUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _set.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (entity == null) return null;

        return new RoutinePeriodUpsertDto
        {
            Id = entity.Id,
            Name = entity.Name,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            PeriodNumber = entity.PeriodNumber,
            IsBreak = entity.IsBreak,
            IsActive = entity.IsActive
        };
    }

    public async Task<List<RoutinePeriodListItemDto>> GetActivePeriodsAsync(CancellationToken cancellationToken = default)
    {
        return await _set.AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsActive)
            .OrderBy(x => x.PeriodNumber)
            .Select(x => new RoutinePeriodListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                StartTime = x.StartTime.ToString(@"hh\:mm"),
                EndTime = x.EndTime.ToString(@"hh\:mm"),
                PeriodNumber = x.PeriodNumber,
                IsBreak = x.IsBreak,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);
    }
}

public class RoomRepository : BaseRepository<Room>, IRoomRepository
{
    public RoomRepository(SchoolDbContext db) : base(db) { }

    public async Task<PagedResult<RoomListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var query = _set.AsNoTracking().Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.RoomNo.Contains(search) || x.Name!.Contains(search) || x.Building!.Contains(search));

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.RoomNo)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new RoomListItemDto
            {
                Id = x.Id,
                RoomNo = x.RoomNo,
                Name = x.Name,
                Capacity = x.Capacity,
                Building = x.Building,
                Floor = x.Floor,
                RoomType = x.RoomType,
                IsLab = x.IsLab,
                RequiresDoublePeriod = x.RequiresDoublePeriod,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<RoomListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };
    }

    public async Task<RoomUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _set.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (entity == null) return null;

        return new RoomUpsertDto
        {
            Id = entity.Id,
            RoomNo = entity.RoomNo,
            Name = entity.Name,
            Capacity = entity.Capacity,
            Building = entity.Building,
            Floor = entity.Floor,
            RoomType = entity.RoomType,
            IsLab = entity.IsLab,
            RequiresDoublePeriod = entity.RequiresDoublePeriod,
            IsActive = entity.IsActive
        };
    }

    public async Task<List<RoomListItemDto>> GetActiveRoomsAsync(CancellationToken cancellationToken = default)
    {
        return await _set.AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsActive)
            .OrderBy(x => x.RoomNo)
            .Select(x => new RoomListItemDto
            {
                Id = x.Id,
                RoomNo = x.RoomNo,
                Name = x.Name,
                Capacity = x.Capacity,
                Building = x.Building,
                Floor = x.Floor,
                RoomType = x.RoomType,
                IsLab = x.IsLab,
                RequiresDoublePeriod = x.RequiresDoublePeriod,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);
    }

    public Task<List<string>> GetRoomTypesAsync()
    {
        var names = Enum.GetNames<SchoolManagementSystem.Models.Enums.RoomType>();
        return Task.FromResult(names.ToList());
    }
}

public class SubjectRequirementRepository : BaseRepository<SubjectRequirement>, ISubjectRequirementRepository
{
    public SubjectRequirementRepository(SchoolDbContext db) : base(db) { }

    public async Task<PagedResult<SubjectRequirementListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var query = _set.AsNoTracking()
            .Include(x => x.Class)
            .Include(x => x.Section)
            .Include(x => x.Group)
            .Include(x => x.Subject)
            .Include(x => x.Teacher).ThenInclude(t => t!.Employee)
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.Class!.Name.Contains(search) || x.Subject!.Name.Contains(search) || x.Teacher!.Employee!.FullName.Contains(search));

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.ClassId).ThenBy(x => x.SubjectId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new SubjectRequirementListItemDto
            {
                Id = x.Id,
                AcademicYearId = x.AcademicYearId,
                ClassId = x.ClassId,
                ClassName = x.Class!.Name,
                SectionId = x.SectionId,
                SectionName = x.Section!.Name,
                GroupId = x.GroupId,
                GroupName = x.Group!.Name,
                SubjectId = x.SubjectId,
                SubjectName = x.Subject!.Name,
                TeacherId = x.TeacherId,
                TeacherName = x.Teacher!.Employee!.FullName,
                PeriodsPerWeek = x.PeriodsPerWeek,
                RequiresLab = x.RequiresLab,
                RequiresDoublePeriod = x.RequiresDoublePeriod,
                Priority = x.Priority,
                MaxConsecutive = x.MaxConsecutive
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<SubjectRequirementListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };
    }

    public async Task<SubjectRequirementUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _set.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (entity == null) return null;

        return new SubjectRequirementUpsertDto
        {
            Id = entity.Id,
            AcademicYearId = entity.AcademicYearId,
            ClassId = entity.ClassId,
            SectionId = entity.SectionId,
            GroupId = entity.GroupId,
            SubjectId = entity.SubjectId,
            TeacherId = entity.TeacherId,
            PeriodsPerWeek = entity.PeriodsPerWeek,
            RequiresLab = entity.RequiresLab,
            RequiresDoublePeriod = entity.RequiresDoublePeriod,
            Priority = entity.Priority,
            MaxConsecutive = entity.MaxConsecutive
        };
    }

    public async Task<List<SubjectRequirementListItemDto>> GetByClassAsync(int classId, int? sectionId = null, int? groupId = null, CancellationToken cancellationToken = default)
    {
        var query = _set.AsNoTracking()
            .Include(x => x.Class)
            .Include(x => x.Section)
            .Include(x => x.Group)
            .Include(x => x.Subject)
            .Include(x => x.Teacher).ThenInclude(t => t!.Employee)
            .Where(x => !x.IsDeleted && x.ClassId == classId);

        if (sectionId.HasValue)
            query = query.Where(x => x.SectionId == sectionId.Value);
        if (groupId.HasValue)
            query = query.Where(x => x.GroupId == groupId.Value);

        return await query
            .OrderBy(x => x.Priority).ThenBy(x => x.Subject!.Name)
            .Select(x => new SubjectRequirementListItemDto
            {
                Id = x.Id,
                AcademicYearId = x.AcademicYearId,
                ClassId = x.ClassId,
                ClassName = x.Class!.Name,
                SectionId = x.SectionId,
                SectionName = x.Section!.Name,
                GroupId = x.GroupId,
                GroupName = x.Group!.Name,
                SubjectId = x.SubjectId,
                SubjectName = x.Subject!.Name,
                TeacherId = x.TeacherId,
                TeacherName = x.Teacher!.Employee!.FullName,
                PeriodsPerWeek = x.PeriodsPerWeek,
                RequiresLab = x.RequiresLab,
                RequiresDoublePeriod = x.RequiresDoublePeriod,
                Priority = x.Priority,
                MaxConsecutive = x.MaxConsecutive
            })
            .ToListAsync(cancellationToken);
    }
}

public class RoutineEntryRepository : BaseRepository<RoutineEntry>, IRoutineEntryRepository
{
    public RoutineEntryRepository(SchoolDbContext db) : base(db) { }

    public async Task<PagedResult<RoutineEntryListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var query = _set.AsNoTracking()
            .Include(x => x.AcademicYear)
            .Include(x => x.Class)
            .Include(x => x.Section)
            .Include(x => x.Group)
            .Include(x => x.Subject)
            .Include(x => x.Teacher).ThenInclude(t => t!.Employee)
            .Include(x => x.Room)
            .Include(x => x.RoutinePeriod)
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x =>
                x.Subject!.Name.Contains(search) ||
                x.Teacher!.Employee!.FullName.Contains(search) ||
                x.Room!.RoomNo.Contains(search) ||
                x.Class!.Name.Contains(search));

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.DayNumber).ThenBy(x => x.RoutinePeriod!.PeriodNumber).ThenBy(x => x.Class!.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new RoutineEntryListItemDto
            {
                Id = x.Id,
                AcademicYearId = x.AcademicYearId,
                AcademicYearName = x.AcademicYear!.Name,
                ClassId = x.ClassId,
                ClassName = x.Class!.Name,
                SectionId = x.SectionId,
                SectionName = x.Section!.Name,
                GroupId = x.GroupId,
                GroupName = x.Group!.Name,
                SubjectId = x.SubjectId,
                SubjectName = x.Subject!.Name,
                TeacherId = x.TeacherId,
                TeacherName = x.Teacher!.Employee!.FullName,
                RoomId = x.RoomId,
                RoomNo = x.Room!.RoomNo,
                RoutinePeriodId = x.RoutinePeriodId,
                PeriodName = x.RoutinePeriod!.Name,
                DayNumber = x.DayNumber,
                DayName = GetDayName(x.DayNumber),
                IsLab = x.IsLab,
                Note = x.Note
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<RoutineEntryListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };
    }

    public async Task<RoutineEntryUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _set.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (entity == null) return null;

        return new RoutineEntryUpsertDto
        {
            Id = entity.Id,
            AcademicYearId = entity.AcademicYearId,
            ClassId = entity.ClassId,
            SectionId = entity.SectionId,
            GroupId = entity.GroupId,
            SubjectId = entity.SubjectId,
            TeacherId = entity.TeacherId,
            RoomId = entity.RoomId,
            RoutinePeriodId = entity.RoutinePeriodId,
            DayNumber = entity.DayNumber,
            IsLab = entity.IsLab,
            Note = entity.Note
        };
    }

    public async Task<PagedResult<RoutineEntryListItemDto>> GetGridAsync(int academicYearId, int? classId = null, int? sectionId = null, int? groupId = null, int? teacherId = null, int? roomId = null, int page = 1, int pageSize = 100, CancellationToken cancellationToken = default)
    {
        var query = _set.AsNoTracking()
            .Include(x => x.AcademicYear)
            .Include(x => x.Class)
            .Include(x => x.Section)
            .Include(x => x.Group)
            .Include(x => x.Subject)
            .Include(x => x.Teacher).ThenInclude(t => t!.Employee)
            .Include(x => x.Room)
            .Include(x => x.RoutinePeriod)
            .Where(x => !x.IsDeleted && x.AcademicYearId == academicYearId);

        if (classId.HasValue)
            query = query.Where(x => x.ClassId == classId.Value);
        if (sectionId.HasValue)
            query = query.Where(x => x.SectionId == sectionId.Value);
        if (groupId.HasValue)
            query = query.Where(x => x.GroupId == groupId.Value);
        if (teacherId.HasValue)
            query = query.Where(x => x.TeacherId == teacherId.Value);
        if (roomId.HasValue)
            query = query.Where(x => x.RoomId == roomId.Value);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.DayNumber).ThenBy(x => x.RoutinePeriod!.PeriodNumber).ThenBy(x => x.Class!.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new RoutineEntryListItemDto
            {
                Id = x.Id,
                AcademicYearId = x.AcademicYearId,
                AcademicYearName = x.AcademicYear!.Name,
                ClassId = x.ClassId,
                ClassName = x.Class!.Name,
                SectionId = x.SectionId,
                SectionName = x.Section!.Name,
                GroupId = x.GroupId,
                GroupName = x.Group!.Name,
                SubjectId = x.SubjectId,
                SubjectName = x.Subject!.Name,
                TeacherId = x.TeacherId,
                TeacherName = x.Teacher!.Employee!.FullName,
                RoomId = x.RoomId,
                RoomNo = x.Room!.RoomNo,
                RoutinePeriodId = x.RoutinePeriodId,
                PeriodName = x.RoutinePeriod!.Name,
                DayNumber = x.DayNumber,
                DayName = GetDayName(x.DayNumber),
                IsLab = x.IsLab,
                Note = x.Note
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<RoutineEntryListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };
    }

    public async Task<bool> ValidateEntryAsync(RoutineEntryUpsertDto dto, CancellationToken cancellationToken = default)
    {
        var teacherConflict = await _set.AsNoTracking()
            .AnyAsync(x => !x.IsDeleted
                && x.AcademicYearId == dto.AcademicYearId
                && x.DayNumber == dto.DayNumber
                && x.RoutinePeriodId == dto.RoutinePeriodId
                && x.TeacherId == dto.TeacherId
                && x.Id != dto.Id, cancellationToken);

        if (teacherConflict) return false;

        var roomConflict = await _set.AsNoTracking()
            .AnyAsync(x => !x.IsDeleted
                && x.AcademicYearId == dto.AcademicYearId
                && x.DayNumber == dto.DayNumber
                && x.RoutinePeriodId == dto.RoutinePeriodId
                && x.RoomId == dto.RoomId
                && x.Id != dto.Id, cancellationToken);

        if (roomConflict) return false;

        var classConflict = await _set.AsNoTracking()
            .AnyAsync(x => !x.IsDeleted
                && x.AcademicYearId == dto.AcademicYearId
                && x.DayNumber == dto.DayNumber
                && x.RoutinePeriodId == dto.RoutinePeriodId
                && x.ClassId == dto.ClassId
                && x.SectionId == dto.SectionId
                && x.GroupId == dto.GroupId
                && x.Id != dto.Id, cancellationToken);

        return !classConflict;
    }

    public async Task<PagedResult<RoutineEntryListItemDto>> GetEntriesPagedSpAsync(int academicYearId, int page = 1, int pageSize = 50, string? search = null, int? classId = null, int? sectionId = null, int? groupId = null, int? teacherId = null, int? roomId = null, CancellationToken cancellationToken = default)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetRoutineEntriesPaged";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@AcademicYearId", academicYearId);
        AddParameter(command, "@PageNumber", page);
        AddParameter(command, "@PageSize", pageSize);
        AddParameter(command, "@SearchTerm", search);
        AddParameter(command, "@ClassId", classId);
        AddParameter(command, "@SectionId", sectionId);
        AddParameter(command, "@GroupId", groupId);
        AddParameter(command, "@TeacherId", teacherId);
        AddParameter(command, "@RoomId", roomId);

        await using var lease = await OpenConnectionAsync(command.Connection!, cancellationToken);
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var items = new List<RoutineEntryListItemDto>();
        int totalRecords = 0;
        while (await reader.ReadAsync(cancellationToken))
        {
            totalRecords = GetInt32(reader, "TotalRecords");
            items.Add(new RoutineEntryListItemDto
            {
                Id = GetInt32(reader, "Id"),
                AcademicYearId = GetInt32(reader, "AcademicYearId"),
                AcademicYearName = GetString(reader, "AcademicYearName"),
                ClassId = GetInt32(reader, "ClassId"),
                ClassName = GetString(reader, "ClassName"),
                SectionId = GetNullableInt32(reader, "SectionId"),
                SectionName = GetNullableString(reader, "SectionName"),
                GroupId = GetNullableInt32(reader, "GroupId"),
                GroupName = GetNullableString(reader, "GroupName"),
                SubjectId = GetInt32(reader, "SubjectId"),
                SubjectName = GetString(reader, "SubjectName"),
                TeacherId = GetInt32(reader, "TeacherId"),
                TeacherName = GetString(reader, "TeacherName"),
                RoomId = GetInt32(reader, "RoomId"),
                RoomNo = GetString(reader, "RoomNo"),
                RoutinePeriodId = GetInt32(reader, "RoutinePeriodId"),
                PeriodName = GetString(reader, "PeriodName"),
                DayNumber = GetInt32(reader, "DayNumber"),
                DayName = GetString(reader, "DayName"),
                IsLab = GetBoolean(reader, "IsLab"),
                Note = GetNullableString(reader, "Note")
            });
        }

        return new PagedResult<RoutineEntryListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalRecords
        };
    }

    public async Task<IEnumerable<RoutineEntryListItemDto>> GetClassRoutineGridAsync(int academicYearId, int classId, int? sectionId = null, int? groupId = null, CancellationToken cancellationToken = default)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetClassRoutineGrid";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@AcademicYearId", academicYearId);
        AddParameter(command, "@ClassId", classId);
        AddParameter(command, "@SectionId", sectionId);
        AddParameter(command, "@GroupId", groupId);

        await using var lease = await OpenConnectionAsync(command.Connection!, cancellationToken);
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var items = new List<RoutineEntryListItemDto>();
        while (await reader.ReadAsync(cancellationToken))
        {
            items.Add(new RoutineEntryListItemDto
            {
                DayNumber = GetInt32(reader, "DayNumber"),
                RoutinePeriodId = GetInt32(reader, "RoutinePeriodId"),
                PeriodName = GetString(reader, "PeriodName"),
                SubjectName = GetString(reader, "SubjectName"),
                TeacherName = GetString(reader, "TeacherName"),
                RoomNo = GetString(reader, "RoomNo")
            });
        }

        return items;
    }

    public async Task<IEnumerable<RoutineEntryListItemDto>> GetTeacherRoutineGridAsync(int academicYearId, int teacherId, CancellationToken cancellationToken = default)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetTeacherRoutineGrid";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@AcademicYearId", academicYearId);
        AddParameter(command, "@TeacherId", teacherId);

        await using var lease = await OpenConnectionAsync(command.Connection!, cancellationToken);
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var items = new List<RoutineEntryListItemDto>();
        while (await reader.ReadAsync(cancellationToken))
        {
            items.Add(new RoutineEntryListItemDto
            {
                DayNumber = GetInt32(reader, "DayNumber"),
                RoutinePeriodId = GetInt32(reader, "RoutinePeriodId"),
                PeriodName = GetString(reader, "PeriodName"),
                ClassName = GetString(reader, "ClassName"),
                SectionName = GetNullableString(reader, "SectionName"),
                SubjectName = GetString(reader, "SubjectName"),
                RoomNo = GetString(reader, "RoomNo")
            });
        }

        return items;
    }

    public async Task<IEnumerable<RoutineEntryListItemDto>> GetRoomScheduleGridAsync(int roomId, int? dayNumber = null, CancellationToken cancellationToken = default)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetRoomScheduleGrid";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@RoomId", roomId);
        AddParameter(command, "@DayNumber", dayNumber);

        await using var lease = await OpenConnectionAsync(command.Connection!, cancellationToken);
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var items = new List<RoutineEntryListItemDto>();
        while (await reader.ReadAsync(cancellationToken))
        {
            items.Add(new RoutineEntryListItemDto
            {
                RoutinePeriodId = GetInt32(reader, "RoutinePeriodId"),
                PeriodName = GetString(reader, "PeriodName"),
                ClassName = GetString(reader, "ClassName"),
                SubjectName = GetString(reader, "SubjectName"),
                TeacherName = GetString(reader, "TeacherName")
            });
        }

        return items;
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

public class WorkingDayRepository : BaseRepository<WorkingDay>, IWorkingDayRepository
{
    public WorkingDayRepository(SchoolDbContext db) : base(db) { }

    public async Task<PagedResult<WorkingDayListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var query = _set.AsNoTracking().Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.DayName.Contains(search));

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.DayNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new WorkingDayListItemDto
            {
                Id = x.Id,
                AcademicYearId = x.AcademicYearId,
                DayName = x.DayName,
                DayNumber = x.DayNumber,
                IsWorkingDay = x.IsWorkingDay
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<WorkingDayListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };
    }

    public async Task<WorkingDayUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _set.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (entity == null) return null;

        return new WorkingDayUpsertDto
        {
            Id = entity.Id,
            AcademicYearId = entity.AcademicYearId,
            DayName = entity.DayName,
            DayNumber = entity.DayNumber,
            IsWorkingDay = entity.IsWorkingDay
        };
    }

    public async Task<List<WorkingDayListItemDto>> GetByAcademicYearAsync(int academicYearId, CancellationToken cancellationToken = default)
    {
        return await _set.AsNoTracking()
            .Where(x => !x.IsDeleted && x.AcademicYearId == academicYearId)
            .OrderBy(x => x.DayNumber)
            .Select(x => new WorkingDayListItemDto
            {
                Id = x.Id,
                AcademicYearId = x.AcademicYearId,
                DayName = x.DayName,
                DayNumber = x.DayNumber,
                IsWorkingDay = x.IsWorkingDay
            })
            .ToListAsync(cancellationToken);
    }
}

public class TeacherAvailabilityRepository : BaseRepository<TeacherAvailability>, ITeacherAvailabilityRepository
{
    public TeacherAvailabilityRepository(SchoolDbContext db) : base(db) { }

    public async Task<PagedResult<TeacherAvailabilityListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var query = _set.AsNoTracking()
            .Include(x => x.Teacher).ThenInclude(t => t!.Employee)
            .Include(x => x.RoutinePeriod)
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.Teacher!.Employee!.FullName.Contains(search));

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.TeacherId).ThenBy(x => x.DayNumber).ThenBy(x => x.RoutinePeriod!.PeriodNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new TeacherAvailabilityListItemDto
            {
                Id = x.Id,
                TeacherId = x.TeacherId,
                TeacherName = x.Teacher!.Employee!.FullName,
                RoutinePeriodId = x.RoutinePeriodId,
                PeriodName = x.RoutinePeriod!.Name,
                DayNumber = x.DayNumber,
                DayName = GetDayName(x.DayNumber),
                IsAvailable = x.IsAvailable
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<TeacherAvailabilityListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };
    }

    public async Task<TeacherAvailabilityUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _set.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (entity == null) return null;

        return new TeacherAvailabilityUpsertDto
        {
            Id = entity.Id,
            TeacherId = entity.TeacherId,
            RoutinePeriodId = entity.RoutinePeriodId,
            DayNumber = entity.DayNumber,
            IsAvailable = entity.IsAvailable
        };
    }

    public async Task<List<TeacherAvailabilityListItemDto>> GetByTeacherAsync(int teacherId, CancellationToken cancellationToken = default)
    {
        return await _set.AsNoTracking()
            .Include(x => x.Teacher).ThenInclude(t => t!.Employee)
            .Include(x => x.RoutinePeriod)
            .Where(x => !x.IsDeleted && x.TeacherId == teacherId)
            .OrderBy(x => x.DayNumber).ThenBy(x => x.RoutinePeriod!.PeriodNumber)
            .Select(x => new TeacherAvailabilityListItemDto
            {
                Id = x.Id,
                TeacherId = x.TeacherId,
                TeacherName = x.Teacher!.Employee!.FullName,
                RoutinePeriodId = x.RoutinePeriodId,
                PeriodName = x.RoutinePeriod!.Name,
                DayNumber = x.DayNumber,
                DayName = GetDayName(x.DayNumber),
                IsAvailable = x.IsAvailable
            })
            .ToListAsync(cancellationToken);
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

public class RoutineGenerationRepository : BaseRepository<RoutineGeneration>, IRoutineGenerationRepository
{
    public RoutineGenerationRepository(SchoolDbContext db) : base(db) { }

    public async Task<PagedResult<RoutineGenerationListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var query = _set.AsNoTracking().Where(x => !x.IsDeleted);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new RoutineGenerationListItemDto
            {
                Id = x.Id,
                AcademicYearId = x.AcademicYearId,
                AcademicYearName = "",
                Status = x.Status,
                StartedAt = x.StartedAt.HasValue ? x.StartedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                CompletedAt = x.CompletedAt.HasValue ? x.CompletedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                TotalAssignments = x.TotalAssignments,
                SuccessfulAssignments = x.SuccessfulAssignments,
                FailedAssignments = x.FailedAssignments,
                ConflictsDetected = x.ConflictsDetected,
                ErrorMessage = x.ErrorMessage
            })
            .ToListAsync(cancellationToken);

        var yearIds = items.Select(i => i.AcademicYearId).Distinct().ToList();
        var years = await _db.Set<AcademicYear>().AsNoTracking()
            .Where(y => yearIds.Contains(y.Id))
            .ToDictionaryAsync(y => y.Id, y => y.Name, cancellationToken);

        foreach (var item in items)
        {
            if (years.TryGetValue(item.AcademicYearId, out var name))
                item.AcademicYearName = name;
        }

        return new PagedResult<RoutineGenerationListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };
    }

    public async Task<RoutineGenerationListItemDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _set.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (entity == null) return null;

        var yearName = await _db.Set<AcademicYear>().AsNoTracking()
            .Where(y => y.Id == entity.AcademicYearId)
            .Select(y => y.Name)
            .FirstOrDefaultAsync(cancellationToken);

        return new RoutineGenerationListItemDto
        {
            Id = entity.Id,
            AcademicYearId = entity.AcademicYearId,
            AcademicYearName = yearName ?? "",
            Status = entity.Status,
            StartedAt = entity.StartedAt?.ToString("yyyy-MM-dd HH:mm:ss"),
            CompletedAt = entity.CompletedAt?.ToString("yyyy-MM-dd HH:mm:ss"),
            TotalAssignments = entity.TotalAssignments,
            SuccessfulAssignments = entity.SuccessfulAssignments,
            FailedAssignments = entity.FailedAssignments,
            ConflictsDetected = entity.ConflictsDetected,
            ErrorMessage = entity.ErrorMessage
        };
    }

    public async Task<int> GenerateAsync(int academicYearId, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new RoutineGeneration
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

        await _set.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<List<RoutineConflictListItemDto>> GetConflictsAsync(int generationId, CancellationToken cancellationToken = default)
    {
        return await _db.Set<RoutineConflict>().AsNoTracking()
            .Include(x => x.Teacher).ThenInclude(t => t!.Employee)
            .Include(x => x.Room)
            .Include(x => x.Subject)
            .Include(x => x.Class)
            .Include(x => x.RoutinePeriod)
            .Where(x => x.GenerationId == generationId)
            .OrderBy(x => x.ConflictType)
            .Select(x => new RoutineConflictListItemDto
            {
                Id = x.Id,
                GenerationId = x.GenerationId,
                ConflictType = x.ConflictType,
                Description = x.Description,
                TeacherId = x.TeacherId,
                TeacherName = x.Teacher!.Employee!.FullName,
                RoomId = x.RoomId,
                RoomNo = x.Room!.RoomNo,
                SubjectId = x.SubjectId,
                SubjectName = x.Subject!.Name,
                ClassId = x.ClassId,
                ClassName = x.Class!.Name,
                RoutinePeriodId = x.RoutinePeriodId,
                PeriodName = x.RoutinePeriod!.Name,
                DayNumber = x.DayNumber,
                DayName = x.DayNumber.HasValue ? GetDayName(x.DayNumber.Value) : null,
                IsResolved = x.IsResolved
            })
            .ToListAsync(cancellationToken);
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

public class RoutineConflictRepository : BaseRepository<RoutineConflict>, IRoutineConflictRepository
{
    public RoutineConflictRepository(SchoolDbContext db) : base(db) { }
}

public class RoutineVersionRepository : BaseRepository<RoutineVersion>, IRoutineVersionRepository
{
    public RoutineVersionRepository(SchoolDbContext db) : base(db) { }

    public async Task<PagedResult<RoutineVersionListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var query = _set.AsNoTracking()
            .Include(x => x.AcademicYear)
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.Name.Contains(search));

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
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
            .ToListAsync(cancellationToken);

        return new PagedResult<RoutineVersionListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };
    }

    public async Task<RoutineVersionUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _set.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (entity == null) return null;

        return new RoutineVersionUpsertDto
        {
            Id = entity.Id,
            AcademicYearId = entity.AcademicYearId,
            Name = entity.Name,
            Status = entity.Status,
            EntryCount = entity.EntryCount
        };
    }

    public async Task<RoutineVersionListItemDto?> GetPublishedAsync(int academicYearId, CancellationToken cancellationToken = default)
    {
        return await _set.AsNoTracking()
            .Include(x => x.AcademicYear)
            .Where(x => !x.IsDeleted
                && x.AcademicYearId == academicYearId
                && x.Status == "Published")
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
}

public class SubstituteAssignmentRepository : BaseRepository<SubstituteAssignment>, ISubstituteAssignmentRepository
{
    public SubstituteAssignmentRepository(SchoolDbContext db) : base(db) { }

    public async Task<PagedResult<SubstituteAssignmentListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var query = _set.AsNoTracking()
            .Include(x => x.RoutineEntry).ThenInclude(e => e!.Subject)
            .Include(x => x.RoutineEntry).ThenInclude(e => e!.Class)
            .Include(x => x.OriginalTeacher).ThenInclude(t => t!.Employee)
            .Include(x => x.SubstituteTeacher).ThenInclude(t => t!.Employee)
            .Include(x => x.AssignedBy)
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x =>
                x.RoutineEntry!.Subject!.Name.Contains(search) ||
                x.OriginalTeacher!.Employee!.FullName.Contains(search) ||
                x.SubstituteTeacher!.Employee!.FullName.Contains(search) ||
                x.RoutineEntry!.Class!.Name.Contains(search));

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.AssignmentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new SubstituteAssignmentListItemDto
            {
                Id = x.Id,
                RoutineEntryId = x.RoutineEntryId,
                SubjectName = x.RoutineEntry!.Subject!.Name,
                ClassName = x.RoutineEntry!.Class!.Name,
                OriginalTeacherName = x.OriginalTeacher!.Employee!.FullName,
                SubstituteTeacherName = x.SubstituteTeacher!.Employee!.FullName,
                AssignedByName = x.AssignedBy!.UserName,
                AssignmentDate = x.AssignmentDate,
                EffectiveDate = x.EffectiveDate,
                Status = x.Status,
                Reason = x.Reason,
                ApprovedAt = x.ApprovedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<SubstituteAssignmentListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };
    }

    public async Task<SubstituteAssignmentUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _set.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (entity == null) return null;

        return new SubstituteAssignmentUpsertDto
        {
            Id = entity.Id,
            RoutineEntryId = entity.RoutineEntryId,
            SubstituteTeacherId = entity.SubstituteTeacherId,
            EffectiveDate = entity.EffectiveDate,
            Reason = entity.Reason,
            Notes = entity.Notes
        };
    }

    public async Task<List<SubstituteAssignmentListItemDto>> GetByTeacherAsync(int teacherId, CancellationToken cancellationToken = default)
    {
        return await _set.AsNoTracking()
            .Include(x => x.RoutineEntry).ThenInclude(e => e!.Subject)
            .Include(x => x.RoutineEntry).ThenInclude(e => e!.Class)
            .Include(x => x.OriginalTeacher).ThenInclude(t => t!.Employee)
            .Include(x => x.SubstituteTeacher).ThenInclude(t => t!.Employee)
            .Include(x => x.AssignedBy)
            .Where(x => !x.IsDeleted && x.SubstituteTeacherId == teacherId)
            .OrderByDescending(x => x.AssignmentDate)
            .Select(x => new SubstituteAssignmentListItemDto
            {
                Id = x.Id,
                RoutineEntryId = x.RoutineEntryId,
                SubjectName = x.RoutineEntry!.Subject!.Name,
                ClassName = x.RoutineEntry!.Class!.Name,
                OriginalTeacherName = x.OriginalTeacher!.Employee!.FullName,
                SubstituteTeacherName = x.SubstituteTeacher!.Employee!.FullName,
                AssignedByName = x.AssignedBy!.UserName,
                AssignmentDate = x.AssignmentDate,
                EffectiveDate = x.EffectiveDate,
                Status = x.Status,
                Reason = x.Reason,
                ApprovedAt = x.ApprovedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<SubstituteAssignmentListItemDto>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        return await _set.AsNoTracking()
            .Include(x => x.RoutineEntry).ThenInclude(e => e!.Subject)
            .Include(x => x.RoutineEntry).ThenInclude(e => e!.Class)
            .Include(x => x.OriginalTeacher).ThenInclude(t => t!.Employee)
            .Include(x => x.SubstituteTeacher).ThenInclude(t => t!.Employee)
            .Include(x => x.AssignedBy)
            .Where(x => !x.IsDeleted && x.Status == "Pending")
            .OrderByDescending(x => x.AssignmentDate)
            .Select(x => new SubstituteAssignmentListItemDto
            {
                Id = x.Id,
                RoutineEntryId = x.RoutineEntryId,
                SubjectName = x.RoutineEntry!.Subject!.Name,
                ClassName = x.RoutineEntry!.Class!.Name,
                OriginalTeacherName = x.OriginalTeacher!.Employee!.FullName,
                SubstituteTeacherName = x.SubstituteTeacher!.Employee!.FullName,
                AssignedByName = x.AssignedBy!.UserName,
                AssignmentDate = x.AssignmentDate,
                EffectiveDate = x.EffectiveDate,
                Status = x.Status,
                Reason = x.Reason,
                ApprovedAt = x.ApprovedAt
            })
            .ToListAsync(cancellationToken);
    }
}

public class RoutineDashboardRepository : IRoutineDashboardRepository
{
    private readonly SchoolDbContext _db;

    public RoutineDashboardRepository(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task<RoutineDashboardDto> GetDashboardAsync(int academicYearId)
    {
        var result = _db.Database
            .SqlQueryRaw<RoutineDashboardDto>("EXEC sp_GetRoutineDashboard @p0", academicYearId)
            .AsEnumerable()
            .FirstOrDefault();

        return result ?? new RoutineDashboardDto();
    }
}

public class RoutineAnalyticsRepository : IRoutineAnalyticsRepository
{
    private readonly SchoolDbContext _db;

    public RoutineAnalyticsRepository(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task<RoutineAnalyticsViewModel> GetAnalyticsAsync(int academicYearId)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetRoutineAnalytics";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@AcademicYearId", academicYearId));

        await using var lease = await OpenConnectionAsync(command.Connection!, CancellationToken.None);
        using var reader = await command.ExecuteReaderAsync(CancellationToken.None);

        var result = new RoutineAnalyticsViewModel();

        // RS1: Teacher load distribution
        while (await reader.ReadAsync(CancellationToken.None))
        {
            result.TeacherConflicts = GetInt32(reader, "Overloaded");
            result.StudentConflicts = GetInt32(reader, "Normal");
            result.RoomConflicts = GetInt32(reader, "Underloaded");
        }

        // RS2: Room utilization ranges
        await reader.NextResultAsync(CancellationToken.None);
        while (await reader.ReadAsync(CancellationToken.None))
        {
            // stored for reference; not directly mapped to current ViewModel properties
        }

        // RS3: Period utilization
        await reader.NextResultAsync(CancellationToken.None);
        while (await reader.ReadAsync(CancellationToken.None))
        {
            // period utilization data available for future use
        }

        // RS4: Conflict summary by type
        await reader.NextResultAsync(CancellationToken.None);
        while (await reader.ReadAsync(CancellationToken.None))
        {
            var conflictType = GetString(reader, "ConflictType");
            var unresolvedCount = GetInt32(reader, "UnresolvedCount");
            result.TotalConflicts += unresolvedCount;
            if (conflictType == "TeacherConflict")
                result.TeacherConflicts += unresolvedCount;
            else if (conflictType == "RoomConflict")
                result.RoomConflicts += unresolvedCount;
            else if (conflictType == "StudentConflict")
                result.StudentConflicts += unresolvedCount;
        }

        return result;
    }

    private static async Task<IAsyncDisposable> OpenConnectionAsync(System.Data.Common.DbConnection connection, CancellationToken ct)
    {
        var wasClosed = connection.State == ConnectionState.Closed;
        if (wasClosed) await connection.OpenAsync(ct);
        return new ConnectionLease(connection, wasClosed);
    }

    private static int GetInt32(System.Data.Common.DbDataReader reader, string name)
        => reader.IsDBNull(reader.GetOrdinal(name)) ? 0 : Convert.ToInt32(reader[name]);

    private static string GetString(System.Data.Common.DbDataReader reader, string name)
        => reader.IsDBNull(reader.GetOrdinal(name)) ? string.Empty : Convert.ToString(reader[name]) ?? string.Empty;

    private sealed class ConnectionLease : IAsyncDisposable
    {
        private readonly System.Data.Common.DbConnection _connection;
        private readonly bool _closeOnDispose;
        public ConnectionLease(System.Data.Common.DbConnection connection, bool closeOnDispose) { _connection = connection; _closeOnDispose = closeOnDispose; }
        public async ValueTask DisposeAsync() { if (_closeOnDispose) await _connection.CloseAsync(); }
    }
}

public class TeacherLoadRepository : ITeacherLoadRepository
{
    private readonly SchoolDbContext _db;

    public TeacherLoadRepository(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task<List<TeacherLoadDto>> GetTeacherLoadSummaryAsync(int academicYearId)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetTeacherLoadSummary";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@AcademicYearId", academicYearId));

        await using var lease = await OpenConnectionAsync(command.Connection!, CancellationToken.None);
        using var reader = await command.ExecuteReaderAsync(CancellationToken.None);

        var items = new List<TeacherLoadDto>();
        while (await reader.ReadAsync(CancellationToken.None))
        {
            items.Add(new TeacherLoadDto
            {
                TeacherId = GetInt32(reader, "TeacherId"),
                TeacherName = GetString(reader, "TeacherName"),
                TotalPeriodsPerWeek = GetInt32(reader, "TotalPeriodsPerWeek"),
                TotalClasses = GetInt32(reader, "ClassesCount"),
                TotalSubjects = GetInt32(reader, "SubjectsCount"),
                UtilizationPercent = (double)GetDecimal(reader, "UtilizationPercent"),
                MaxPeriodsPerDay = GetInt32(reader, "MaxPeriodsPerDay"),
                WorkingDays = GetInt32(reader, "WorkingDays"),
                AveragePerDay = (double)GetDecimal(reader, "AveragePerDay"),
                WeeklyPeriodsByDay = ParsePeriodsByDayJson(GetString(reader, "PeriodsByDay"))
            });
        }

        return items;
    }

    private static Dictionary<int, int> ParsePeriodsByDayJson(string json)
    {
        if (string.IsNullOrEmpty(json) || json == "[]")
            return new Dictionary<int, int>();

        try
        {
            var entries = JsonSerializer.Deserialize<List<DayPeriodEntry>>(json);
            if (entries == null || entries.Count == 0)
                return new Dictionary<int, int>();

            return entries.ToDictionary(e => e.Key, e => e.Value);
        }
        catch
        {
            return new Dictionary<int, int>();
        }
    }

    private sealed class DayPeriodEntry
    {
        public int Key { get; set; }
        public int Value { get; set; }
    }

    private static async Task<IAsyncDisposable> OpenConnectionAsync(System.Data.Common.DbConnection connection, CancellationToken ct)
    {
        var wasClosed = connection.State == ConnectionState.Closed;
        if (wasClosed) await connection.OpenAsync(ct);
        return new ConnectionLease(connection, wasClosed);
    }

    private static int GetInt32(System.Data.Common.DbDataReader reader, string name)
        => reader.IsDBNull(reader.GetOrdinal(name)) ? 0 : Convert.ToInt32(reader[name]);

    private static string GetString(System.Data.Common.DbDataReader reader, string name)
        => reader.IsDBNull(reader.GetOrdinal(name)) ? string.Empty : Convert.ToString(reader[name]) ?? string.Empty;

    private static decimal GetDecimal(System.Data.Common.DbDataReader reader, string name)
        => reader.IsDBNull(reader.GetOrdinal(name)) ? 0m : Convert.ToDecimal(reader[name]);

    private sealed class ConnectionLease : IAsyncDisposable
    {
        private readonly System.Data.Common.DbConnection _connection;
        private readonly bool _closeOnDispose;
        public ConnectionLease(System.Data.Common.DbConnection connection, bool closeOnDispose) { _connection = connection; _closeOnDispose = closeOnDispose; }
        public async ValueTask DisposeAsync() { if (_closeOnDispose) await _connection.CloseAsync(); }
    }
}

public class RoomUtilizationRepository : IRoomUtilizationRepository
{
    private readonly SchoolDbContext _db;

    public RoomUtilizationRepository(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task<List<RoomUtilizationDto>> GetRoomUtilizationAsync(int academicYearId)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetRoomUtilization";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@AcademicYearId", academicYearId));

        await using var lease = await OpenConnectionAsync(command.Connection!, CancellationToken.None);
        using var reader = await command.ExecuteReaderAsync(CancellationToken.None);

        var items = new List<RoomUtilizationDto>();
        while (await reader.ReadAsync(CancellationToken.None))
        {
            items.Add(new RoomUtilizationDto
            {
                RoomId = GetInt32(reader, "RoomId"),
                RoomNo = GetString(reader, "RoomNo"),
                Building = GetNullableString(reader, "Building"),
                Capacity = GetInt32(reader, "Capacity"),
                TotalSlotsPerWeek = GetInt32(reader, "TotalPeriodsPerWeek"),
                UsedSlots = GetInt32(reader, "UsedPeriods"),
                UtilizationPercent = (double)GetDecimal(reader, "UtilizationPercent")
            });
        }

        return items;
    }

    private static async Task<IAsyncDisposable> OpenConnectionAsync(System.Data.Common.DbConnection connection, CancellationToken ct)
    {
        var wasClosed = connection.State == ConnectionState.Closed;
        if (wasClosed) await connection.OpenAsync(ct);
        return new ConnectionLease(connection, wasClosed);
    }

    private static int GetInt32(System.Data.Common.DbDataReader reader, string name)
        => reader.IsDBNull(reader.GetOrdinal(name)) ? 0 : Convert.ToInt32(reader[name]);

    private static string GetString(System.Data.Common.DbDataReader reader, string name)
        => reader.IsDBNull(reader.GetOrdinal(name)) ? string.Empty : Convert.ToString(reader[name]) ?? string.Empty;

    private static string? GetNullableString(System.Data.Common.DbDataReader reader, string name)
        => reader.IsDBNull(reader.GetOrdinal(name)) ? null : Convert.ToString(reader[name]);

    private static decimal GetDecimal(System.Data.Common.DbDataReader reader, string name)
        => reader.IsDBNull(reader.GetOrdinal(name)) ? 0m : Convert.ToDecimal(reader[name]);

    private sealed class ConnectionLease : IAsyncDisposable
    {
        private readonly System.Data.Common.DbConnection _connection;
        private readonly bool _closeOnDispose;
        public ConnectionLease(System.Data.Common.DbConnection connection, bool closeOnDispose) { _connection = connection; _closeOnDispose = closeOnDispose; }
        public async ValueTask DisposeAsync() { if (_closeOnDispose) await _connection.CloseAsync(); }
    }
}
