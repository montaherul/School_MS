using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Routine;
using SchoolManagementSystem.Models.Entities.Routine;
using SchoolManagementSystem.Repositories.Interfaces;

namespace SchoolManagementSystem.Repositories.Interfaces.Routine;

public interface IRoutinePeriodRepository : IBaseRepository<RoutinePeriod>
{
    Task<PagedResult<RoutinePeriodListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<RoutinePeriodUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<List<RoutinePeriodListItemDto>> GetActivePeriodsAsync(CancellationToken cancellationToken = default);
}

public interface IRoomRepository : IBaseRepository<Room>
{
    Task<PagedResult<RoomListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<RoomUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<List<RoomListItemDto>> GetActiveRoomsAsync(CancellationToken cancellationToken = default);
    Task<List<string>> GetRoomTypesAsync();
}

public interface ISubjectRequirementRepository : IBaseRepository<SubjectRequirement>
{
    Task<PagedResult<SubjectRequirementListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<SubjectRequirementUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<List<SubjectRequirementListItemDto>> GetByClassAsync(int classId, int? sectionId = null, int? groupId = null, CancellationToken cancellationToken = default);
}

public interface IRoutineEntryRepository : IBaseRepository<RoutineEntry>
{
    Task<PagedResult<RoutineEntryListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<RoutineEntryUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<RoutineEntryListItemDto>> GetGridAsync(int academicYearId, int? classId = null, int? sectionId = null, int? groupId = null, int? teacherId = null, int? roomId = null, int page = 1, int pageSize = 100, CancellationToken cancellationToken = default);
    Task<bool> ValidateEntryAsync(RoutineEntryUpsertDto dto, CancellationToken cancellationToken = default);
    Task<PagedResult<RoutineEntryListItemDto>> GetEntriesPagedSpAsync(int academicYearId, int page = 1, int pageSize = 50, string? search = null, int? classId = null, int? sectionId = null, int? groupId = null, int? teacherId = null, int? roomId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<RoutineEntryListItemDto>> GetClassRoutineGridAsync(int academicYearId, int classId, int? sectionId = null, int? groupId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<RoutineEntryListItemDto>> GetTeacherRoutineGridAsync(int academicYearId, int teacherId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RoutineEntryListItemDto>> GetRoomScheduleGridAsync(int roomId, int? dayNumber = null, CancellationToken cancellationToken = default);
}

public interface IWorkingDayRepository : IBaseRepository<WorkingDay>
{
    Task<PagedResult<WorkingDayListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<WorkingDayUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<List<WorkingDayListItemDto>> GetByAcademicYearAsync(int academicYearId, CancellationToken cancellationToken = default);
}

public interface ITeacherAvailabilityRepository : IBaseRepository<TeacherAvailability>
{
    Task<PagedResult<TeacherAvailabilityListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<TeacherAvailabilityUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<List<TeacherAvailabilityListItemDto>> GetByTeacherAsync(int teacherId, CancellationToken cancellationToken = default);
}

public interface IRoutineGenerationRepository : IBaseRepository<RoutineGeneration>
{
    Task<PagedResult<RoutineGenerationListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<RoutineGenerationListItemDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> GenerateAsync(int academicYearId, string createdBy, CancellationToken cancellationToken = default);
    Task<List<RoutineConflictListItemDto>> GetConflictsAsync(int generationId, CancellationToken cancellationToken = default);
}

public interface IRoutineConflictRepository : IBaseRepository<RoutineConflict> { }

public interface IRoutineVersionRepository : IBaseRepository<RoutineVersion>
{
    Task<PagedResult<RoutineVersionListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<RoutineVersionUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<RoutineVersionListItemDto?> GetPublishedAsync(int academicYearId, CancellationToken cancellationToken = default);
}

public interface ISubstituteAssignmentRepository : IBaseRepository<SubstituteAssignment>
{
    Task<PagedResult<SubstituteAssignmentListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<SubstituteAssignmentUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<List<SubstituteAssignmentListItemDto>> GetByTeacherAsync(int teacherId, CancellationToken cancellationToken = default);
    Task<List<SubstituteAssignmentListItemDto>> GetPendingAsync(CancellationToken cancellationToken = default);
}
