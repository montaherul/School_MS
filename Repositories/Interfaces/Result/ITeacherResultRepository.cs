using SchoolManagementSystem.Models.DTOs.Result;

namespace SchoolManagementSystem.Repositories.Interfaces.Result;

public interface ITeacherResultRepository
{
    Task<List<TeacherAssignedExamDto>> GetTeacherAssignedExamsAsync(int teacherId, int? academicYearId, CancellationToken ct = default);
    Task<List<TeacherAssignedSubjectDto>> GetTeacherAssignedSubjectsAsync(int teacherId, int classId, int? sectionId, int? groupId, CancellationToken ct = default);
    Task<TeacherMarksEntrySheetDto> GetTeacherMarksEntrySheetAsync(int teacherId, int examId, int classId, int sectionId, int subjectId, int? groupId, CancellationToken ct = default);
    Task<TeacherResultSummaryDto> GetTeacherResultSummaryAsync(int teacherId, int examId, int subjectId, int classId, int sectionId, int? groupId, CancellationToken ct = default);
    Task<List<TeacherExportRowDto>> GetTeacherExportSheetAsync(int teacherId, int examId, int subjectId, int classId, int sectionId, int? groupId, CancellationToken ct = default);
}
