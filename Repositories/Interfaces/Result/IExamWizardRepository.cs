using SchoolManagementSystem.Models.DTOs.Exam;

namespace SchoolManagementSystem.Repositories.Interfaces.Result;

public interface IExamWizardRepository
{
    Task<ExamCreationPreviewDto> GetExamCreationPreviewAsync(int academicYearId, List<int> classIds, CancellationToken ct = default);
    Task<ExamClassHierarchyDto> GetExamClassHierarchyAsync(int academicYearId, List<int> classIds, CancellationToken ct = default);
    Task<List<ExamTeacherAssignmentDto>> GetExamTeacherAssignmentsAsync(int academicYearId, List<int> classIds, CancellationToken ct = default);
    Task<ExamValidationResultDto> GetExamValidationAsync(int academicYearId, string examName, int examTerm, List<int> classIds, DateOnly startDate, DateOnly endDate, CancellationToken ct = default);
    Task<ExamCreateResultDto> CreateExamHierarchyAsync(ExamCreateHierarchyRequest request, CancellationToken ct = default);
    Task<ExamCreationReadinessDto> GetExamReadinessAsync(int academicYearId, List<int> classIds, CancellationToken ct = default);
    Task<ExamStatisticsDto> GetExamStatisticsAsync(int academicYearId, List<int> classIds, CancellationToken ct = default);
    Task<ExamScheduleResultDto> GenerateExamScheduleAsync(int examId, DateOnly startDate, DateOnly endDate, string userId, CancellationToken ct = default);
    Task<List<ExamConflictDto>> GetExamConflictsAsync(int examId, CancellationToken ct = default);
    
    // Fix Issues methods
    Task<ExamFixResultDto> AssignTeacherToExamSubjectAsync(int academicYearId, int subjectId, int classId, int? sectionId, int? studentGroupId, int teacherId, string userId, CancellationToken ct = default);
    Task<ExamFixResultDto> ConfigureExamSubjectComponentsAsync(int examSubjectId, string componentsJson, string userId, CancellationToken ct = default);
    Task<ExamFixResultDto> AddSectionsToClassAsync(int classId, string sectionNamesJson, int? studentGroupId, string userId, CancellationToken ct = default);
    Task<ExamFixResultDto> MapSubjectToClassAsync(int subjectId, int classId, int? studentGroupId, decimal fullMarks, decimal passMarks, bool isOptional, int displayOrder, string userId, CancellationToken ct = default);
    Task<ExamFixResultDto> ConfigureSubjectMarkStructureAsync(int subjectId, int? classId, int? studentGroupId, string componentsJson, string userId, CancellationToken ct = default);
    
    // Publish readiness check
    Task<ExamPublishReadinessDto> CheckExamPublishReadinessAsync(int examId, CancellationToken ct = default);
}