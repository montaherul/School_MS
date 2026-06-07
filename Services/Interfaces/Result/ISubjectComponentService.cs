using SchoolManagementSystem.Models.Entities.Exam;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface ISubjectComponentService
{
    Task<List<SubjectComponent>> GetComponentsByClassSubjectAsync(int classSubjectId);
    Task<SubjectComponent?> CreateComponentAsync(SubjectComponent component);
    Task<SubjectComponent?> UpdateComponentAsync(SubjectComponent component);
    Task<bool> DeleteComponentAsync(int componentId);
    Task<List<SubjectComponent>> GetComponentsForSubjectAsync(int subjectId, int classId, int? groupId = null);
}
