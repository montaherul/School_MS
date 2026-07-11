using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Routine;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.Routine;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Entities.Teachers;

namespace SchoolManagementSystem.Services.Interfaces.Routine;

public interface IRoutinePeriodService
{
    Task<PagedResult<RoutinePeriodListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<RoutinePeriodUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(RoutinePeriodUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(RoutinePeriodUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task<List<RoutinePeriodListItemDto>> GetActivePeriodsAsync(CancellationToken cancellationToken = default);
}

public interface IRoomService
{
    Task<PagedResult<RoomListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<RoomUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(RoomUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(RoomUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task<List<RoomListItemDto>> GetActiveRoomsAsync(CancellationToken cancellationToken = default);
    Task<List<string>> GetRoomTypesAsync();
}

public interface ISubjectRequirementService
{
    Task<PagedResult<SubjectRequirementListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<SubjectRequirementUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SubjectRequirementUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(SubjectRequirementUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task<List<SubjectRequirementListItemDto>> GetByClassAsync(int classId, int? sectionId = null, int? groupId = null, CancellationToken cancellationToken = default);
}

public interface IWorkingDayService
{
    Task<PagedResult<WorkingDayListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<WorkingDayUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(WorkingDayUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(WorkingDayUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task<List<WorkingDayListItemDto>> GetByAcademicYearAsync(int academicYearId, CancellationToken cancellationToken = default);
}

public interface ITeacherAvailabilityService
{
    Task<PagedResult<TeacherAvailabilityListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<TeacherAvailabilityUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(TeacherAvailabilityUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(TeacherAvailabilityUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task<List<TeacherAvailabilityListItemDto>> GetByTeacherAsync(int teacherId, CancellationToken cancellationToken = default);
}

public interface IRoutineEntryService
{
    Task<PagedResult<RoutineEntryListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<RoutineEntryUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(RoutineEntryUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(RoutineEntryUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task<PagedResult<RoutineEntryListItemDto>> GetGridAsync(int academicYearId, int? classId = null, int? sectionId = null, int? groupId = null, int? teacherId = null, int? roomId = null, int page = 1, int pageSize = 100, CancellationToken cancellationToken = default);
    Task<bool> ValidateEntryAsync(RoutineEntryUpsertDto dto, CancellationToken cancellationToken = default);
    Task UpdateEntryAsync(int id, int roomId, int routinePeriodId, int dayNumber, string updatedBy, CancellationToken cancellationToken = default);
    Task SwapEntriesAsync(int entryId1, int entryId2, string updatedBy, CancellationToken cancellationToken = default);
    Task MoveEntryAsync(int entryId, int targetPeriodId, int targetDayNumber, string updatedBy, CancellationToken cancellationToken = default);
    Task BulkDeleteAsync(List<int> ids, string updatedBy, CancellationToken cancellationToken = default);
    Task BulkUpdateAsync(List<int> ids, int roomId, int routinePeriodId, int dayNumber, string updatedBy, CancellationToken cancellationToken = default);
}

public interface IRoutineGenerationService
{
    Task<PagedResult<RoutineGenerationListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<RoutineGenerationListItemDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(RoutineGenerationListItemDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(RoutineGenerationListItemDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task<int> GenerateAsync(int academicYearId, string createdBy, CancellationToken cancellationToken = default);
    Task<List<RoutineConflictListItemDto>> GetConflictsAsync(int generationId, CancellationToken cancellationToken = default);
}

public interface IRoutineVersionService
{
    Task<PagedResult<RoutineVersionListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<RoutineVersionUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(RoutineVersionUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(RoutineVersionUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task<RoutineVersionListItemDto?> PublishAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task<RoutineVersionListItemDto?> ApproveAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task ArchiveAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task<RoutineVersionListItemDto?> GetPublishedAsync(int academicYearId, CancellationToken cancellationToken = default);
}

public interface IRoutineEngineService
{
    Task<RoutineGenerationListItemDto> GenerateRoutineAsync(int academicYearId, string createdBy, CancellationToken cancellationToken = default);
    Task<List<RoutineConflictListItemDto>> ValidateRoutineAsync(int academicYearId, CancellationToken cancellationToken = default);
    Task<List<RoutineConflictListItemDto>> DetectConflictsAsync(int generationId, CancellationToken cancellationToken = default);
    Task<RoutineDashboardDto> GetDashboardAsync(int academicYearId, CancellationToken cancellationToken = default);
    Task<List<TeacherLoadDto>> GetTeacherLoadSummaryAsync(int academicYearId, CancellationToken cancellationToken = default);
    Task<List<RoomUtilizationDto>> GetRoomUtilizationAsync(int academicYearId, CancellationToken cancellationToken = default);
    Task<List<SubjectDistributionDto>> GetSubjectDistributionAsync(int academicYearId, CancellationToken cancellationToken = default);
    Task<RoutineAnalyticsViewModel> GetAnalyticsAsync(int academicYearId, CancellationToken cancellationToken = default);
    Task<List<RoutineConflictListItemDto>> CheckHolidayConflictsAsync(int academicYearId, CancellationToken cancellationToken = default);

    // Cross-entity lookup methods
    Task<AcademicYear?> GetCurrentAcademicYearAsync(CancellationToken ct);
    Task<RoutineGeneration?> GetGenerationByIdAsync(int id, CancellationToken ct);
    Task<List<AcademicYearItem>> GetAcademicYearItemsAsync(CancellationToken ct);
    Task<Student?> GetStudentByUserIdAsync(int userId, CancellationToken ct);
    Task<(ApplicationUser? User, Teacher? Teacher)> GetUserAndTeacherAsync(int userId, CancellationToken ct);
    Task<List<TeacherLookupDto>> GetTeacherLookupAsync(CancellationToken ct);
    Task<List<ClassItem>> GetClassItemsAsync(CancellationToken ct);
    Task<List<SubjectLookupDto>> GetSubjectLookupAsync(CancellationToken ct);
    Task<List<PeriodLookupDto>> GetPeriodLookupAsync(CancellationToken ct);
    Task<List<RoutineEntryLookupDto>> GetRoutineEntryLookupAsync(CancellationToken ct);
    Task<List<SectionItem>> GetSectionsByClassAsync(int classId, CancellationToken ct);
    Task<List<GroupLookupDto>> GetGroupsByClassAsync(int classId, CancellationToken ct);
    Task<List<RoomItem>> GetRoomItemsAsync(CancellationToken ct);
    Task<PagedResult<RoutineConflictListItemDto>> GetConflictsPagedAsync(int page, int size, bool? unresolvedOnly, CancellationToken ct);
}

public interface ISubstituteService
{
    Task<PagedResult<SubstituteAssignmentListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<SubstituteAssignmentUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(SubstituteAssignmentUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(SubstituteAssignmentUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task<List<SubstituteAssignmentListItemDto>> GetByTeacherAsync(int teacherId, CancellationToken cancellationToken = default);
    Task<List<SubstituteAssignmentListItemDto>> GetPendingAsync(CancellationToken cancellationToken = default);
    Task ApproveAsync(int id, string approvedBy, CancellationToken cancellationToken = default);
    Task DeclineAsync(int id, string reason, CancellationToken cancellationToken = default);
}
