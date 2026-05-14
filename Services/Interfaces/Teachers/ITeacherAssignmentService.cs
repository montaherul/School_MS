using SchoolManagementSystem.Models.DTOs.Teacher;

namespace SchoolManagementSystem.Services.Interfaces.Teachers;

public interface ITeacherAssignmentService
{
    Task<IEnumerable<TeacherClassAssignmentDto>> GetTeacherClassAssignmentsAsync(int teacherId, CancellationToken ct = default);
    Task<IEnumerable<TeacherSubjectAssignmentDto>> GetTeacherSubjectAssignmentsAsync(int teacherId, int classId, int sectionId, CancellationToken ct = default);
    Task<IEnumerable<SchoolManagementSystem.Models.Entities.Academic.SchoolClass>> GetClassesByTeacherIdAsync(int teacherId, CancellationToken ct = default);
    Task<bool> AssignClassAsync(int teacherId, int classId, int sectionId, int academicYearId, string createdBy);
    Task<bool> AssignSubjectAsync(int teacherId, int subjectId, int classId, int sectionId, int academicYearId, string createdBy);
    Task RemoveClassAssignmentAsync(int assignmentId);
    Task RemoveSubjectAssignmentAsync(int assignmentId);

    Task<SchoolManagementSystem.Models.Entities.Teachers.Teacher?> GetTeacherWithAssignmentsAsync(int teacherId, CancellationToken ct = default);
    Task<IEnumerable<SchoolManagementSystem.Models.Entities.Academic.SchoolClass>> GetClassesAsync(CancellationToken ct = default);
    Task<IEnumerable<SchoolManagementSystem.Models.Entities.Academic.AcademicYear>> GetAcademicYearsAsync(CancellationToken ct = default);
    Task<IEnumerable<SchoolManagementSystem.Models.Entities.Academic.Subject>> GetSubjectsAsync(CancellationToken ct = default);
}

