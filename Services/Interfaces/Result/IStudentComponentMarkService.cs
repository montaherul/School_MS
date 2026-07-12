using SchoolManagementSystem.Models.Entities.Result;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IStudentComponentMarkService
{
    Task<StudentComponentMark?> GetAsync(int examId, int studentId, int examSubjectComponentId);
    Task<List<StudentComponentMark>> GetByStudentAsync(int examId, int studentId);
    Task<List<StudentComponentMark>> GetByExamSubjectAsync(int examSubjectId);
    Task<List<StudentComponentMark>> GetByExamAsync(int examId);
    Task UpsertAsync(StudentComponentMark mark, string updatedBy);
    Task UpsertBatchAsync(List<StudentComponentMark> marks, string updatedBy);
    Task<bool> DeleteAsync(int id);
    Task<bool> HasMarksAsync(int examSubjectComponentId);
}
