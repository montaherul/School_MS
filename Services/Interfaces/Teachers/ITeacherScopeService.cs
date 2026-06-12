namespace SchoolManagementSystem.Services.Interfaces.Teachers;

public interface ITeacherScopeService
{
    Task<bool> HasClassAccessAsync(int userId, int classId, int sectionId, int? groupId = null, CancellationToken ct = default);
    Task<bool> HasSubjectAccessAsync(int userId, int subjectId, int classId, int sectionId, int? groupId = null, CancellationToken ct = default);
    Task<bool> HasStudentAccessAsync(int userId, int studentId, CancellationToken ct = default);
    
    Task<IEnumerable<int>> GetAssignedClassIdsAsync(int userId, CancellationToken ct = default);
    Task<IEnumerable<int>> GetAssignedSectionIdsAsync(int userId, int classId, CancellationToken ct = default);
    Task<IEnumerable<int>> GetAssignedSubjectIdsAsync(int userId, int classId, int sectionId, int? groupId = null, CancellationToken ct = default);
}

