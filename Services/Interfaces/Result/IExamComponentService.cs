using SchoolManagementSystem.Models.DTOs.Exam;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IExamComponentService
{
    Task<List<ExamComponentListDto>> GetAllAsync(bool includeInactive = false);
    Task<ExamComponentListDto?> GetByIdAsync(int id);
    Task<ExamComponentListDto> CreateAsync(ExamComponentUpsertDto dto, string createdBy);
    Task<ExamComponentListDto?> UpdateAsync(int id, ExamComponentUpsertDto dto, string updatedBy);
    Task<bool> DeleteAsync(int id);
    Task<bool> ToggleActiveAsync(int id);
}
