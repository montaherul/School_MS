using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.DTOs.Teacher;
using SchoolManagementSystem.Models.Entities.Teachers;

namespace SchoolManagementSystem.Services.Interfaces.Teachers;

public interface ITeacherAssignmentService
{
    Task<IEnumerable<TeacherClassAssignmentDto>> GetTeacherClassAssignmentsAsync(int teacherId, CancellationToken ct = default);
    Task<IEnumerable<TeacherSubjectAssignmentDto>> GetTeacherSubjectAssignmentsAsync(int teacherId, int classId, int sectionId, CancellationToken ct = default);
    Task<IEnumerable<SchoolClass>> GetClassesByTeacherIdAsync(int teacherId, CancellationToken ct = default);
    Task<bool> AssignClassAsync(int teacherId, int classId, int? groupId, int sectionId, int academicYearId, string createdBy);
    Task<bool> AssignSubjectAsync(int teacherId, int subjectId, int? groupId, int classId, int sectionId, int academicYearId, string createdBy);
    Task RemoveClassAssignmentAsync(int assignmentId);
    Task RemoveSubjectAssignmentAsync(int assignmentId);
    Task<Teacher?> GetTeacherWithAssignmentsAsync(int teacherId, CancellationToken ct = default);
    Task<IEnumerable<SchoolClass>> GetClassesAsync(CancellationToken ct = default);
    Task<IEnumerable<AcademicYear>> GetAcademicYearsAsync(CancellationToken ct = default);
    Task<IEnumerable<Subject>> GetSubjectsAsync(CancellationToken ct = default);
    Task<IEnumerable<StudentGroup>> GetStudentGroupsAsync(CancellationToken ct = default);

    // New methods for Bangladesh curriculum filtering
    Task<IEnumerable<StudentGroup>> GetTeacherAssignedGroupsAsync(int teacherId, int classId, CancellationToken ct = default);
    Task<IEnumerable<Section>> GetTeacherAssignedSectionsAsync(int teacherId, int classId, int? groupId, CancellationToken ct = default);
    Task<IEnumerable<Subject>> GetTeacherAssignedSubjectsAsync(int teacherId, int classId, int? groupId, int? sectionId, CancellationToken ct = default);
}

