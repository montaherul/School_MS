using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IExamWizardService
{
    // Legacy methods (kept for backward compatibility)
    Task<List<ExamWizardSubjectDto>> LoadSubjectsAsync(int academicYearId, List<int> classIds, ExamTerm term, CancellationToken ct = default);
    Task<ExamWizardStateDto?> LoadPreviousExamTemplateAsync(int academicYearId, ExamTerm term, CancellationToken ct = default);
    Task<ExamWizardStateDto?> LoadExamByIdAsync(int examId, CancellationToken ct = default);
    Task<ExamWizardResultDto> CreateExamsFromWizardAsync(ExamWizardCreateRequest request, string userId, CancellationToken ct = default);
    Task<List<ExamWizardComponentDto>> GetComponentsForSubjectAsync(int subjectId, int? classId, CancellationToken ct = default);
    Task<Dictionary<int, int?>> GetTeacherAssignmentsAsync(int academicYearId, List<int> classIds, List<int> subjectIds, CancellationToken ct = default);

    /// <summary>Loads standard NCTB subject/component/mark configurations based on class and group.</summary>
    Task<ExamWizardStateDto?> LoadNctbTemplateAsync(int academicYearId, int classId, ExamTerm term, CancellationToken ct = default);

    /// <summary>Saves the exam configuration as a reusable template.</summary>
    Task<ExamTemplateDto> SaveTemplateAsync(SaveTemplateRequest request, string userId, CancellationToken ct = default);

    /// <summary>Loads a saved template by ID.</summary>
    Task<ExamWizardStateDto?> LoadTemplateAsync(int templateId, CancellationToken ct = default);

    /// <summary>Lists saved templates with optional filters.</summary>
    Task<List<ExamTemplateListItemDto>> ListTemplatesAsync(int? academicYearId, ExamTerm? term, CancellationToken ct = default);

    /// <summary>Deletes a saved template.</summary>
    Task<bool> DeleteTemplateAsync(int templateId, CancellationToken ct = default);

    // New SP-based methods for Enterprise Exam Creation Wizard
    Task<ExamCreationPreviewDto> GetExamCreationPreviewAsync(int academicYearId, List<int> classIds, CancellationToken ct = default);
    Task<ExamClassHierarchyDto> GetExamClassHierarchyAsync(int academicYearId, List<int> classIds, CancellationToken ct = default);
    Task<List<ExamTeacherAssignmentDto>> GetExamTeacherAssignmentsAsync(int academicYearId, List<int> classIds, CancellationToken ct = default);
    Task<ExamValidationResultDto> ValidateExamCreationAsync(int academicYearId, string examName, ExamTerm term, List<int> classIds, DateOnly startDate, DateOnly endDate, CancellationToken ct = default);
    Task<ExamCreateResultDto> CreateExamHierarchyAsync(ExamCreateHierarchyRequest request, string userId, CancellationToken ct = default);
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