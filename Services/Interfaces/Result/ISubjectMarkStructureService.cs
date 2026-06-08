using SchoolManagementSystem.Models.DTOs.Exam;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface ISubjectMarkStructureService
{
    Task<List<SubjectMarkStructureDto>> GetByExamAsync(int examId);
    Task<List<SubjectMarkStructureDto>> GetBySubjectAsync(int examId, int subjectId);
    Task<SubjectMarkStructureDto?> GetByIdAsync(int id);
    Task<SubjectMarkStructureDto> CreateAsync(SubjectMarkStructureUpsertDto dto, string createdBy);
    Task<SubjectMarkStructureDto?> UpdateAsync(int id, SubjectMarkStructureUpsertDto dto, string updatedBy);
    Task<bool> DeleteAsync(int id);
    Task<bool> SaveBulkAsync(int examId, int subjectId, List<SubjectMarkStructureUpsertDto> items, string updatedBy);
    Task<List<ComponentColumnDto>> GetGridColumnsAsync(int examId, int subjectId, int? classId = null, int? studentGroupId = null);
}
