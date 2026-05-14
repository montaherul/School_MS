using SchoolManagementSystem.Models.DTOs.Academic;

namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface IClassRoutineService
{
    Task<IEnumerable<ClassRoutineDto>> GetBySectionAsync(int sectionId, CancellationToken ct = default);
    Task<IEnumerable<ClassRoutineDto>> GetByTeacherAsync(long employeeId, CancellationToken ct = default);
    Task<bool> AddRoutineAsync(ClassRoutineDto dto, string createdBy, CancellationToken ct = default);
    Task DeleteRoutineAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<string>> DetectConflictsAsync(ClassRoutineDto dto, CancellationToken ct = default);
}

public interface ITeacherAcademicService
{
    Task<IEnumerable<TeacherSubjectAssignmentDto>> GetAssignmentsByTeacherAsync(long employeeId, CancellationToken ct = default);
    Task AssignSubjectAsync(TeacherSubjectAssignmentDto dto, string createdBy, CancellationToken ct = default);
    Task<TeacherWorkloadDto> GetWorkloadAsync(long employeeId, CancellationToken ct = default);
}
