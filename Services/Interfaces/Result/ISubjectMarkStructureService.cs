using SchoolManagementSystem.Models.DTOs.Exam;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public class ComponentPreviewDto
{
    public int SubjectId { get; set; }
    public string Preview { get; set; } = string.Empty;
    public List<ComponentDetailDto> Components { get; set; } = [];
}

public class ComponentDetailDto
{
    public string Name { get; set; } = string.Empty;
    public decimal FullMarks { get; set; }
}

public interface ISubjectMarkStructureService
{
    Task<List<SubjectMarkStructureDto>> GetBySubjectAsync(int subjectId);
    Task<SubjectMarkStructureDto?> GetByIdAsync(int id);
    Task<SubjectMarkStructureDto> CreateAsync(SubjectMarkStructureUpsertDto dto, string createdBy);
    Task<SubjectMarkStructureDto?> UpdateAsync(int id, SubjectMarkStructureUpsertDto dto, string updatedBy);
    Task<bool> DeleteAsync(int id);
    Task<bool> SaveBulkAsync(int subjectId, List<SubjectMarkStructureUpsertDto> items, string updatedBy);
    Task<List<ComponentColumnDto>> GetGridColumnsAsync(int subjectId, int? classId = null, int? studentGroupId = null);
    Task<List<ComponentPreviewDto>> GetComponentPreviewsAsync(List<int> subjectIds, CancellationToken ct = default);
}
