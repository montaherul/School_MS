using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.DTOs.Result;

namespace SchoolManagementSystem.Repositories.Interfaces.Result;

public interface IMarkEntryRepository : IBaseRepository<MarkEntry>
{
    Task<List<MarkEntrySheetDto>> GetMarkEntrySheetAsync(int examId, int classId, int sectionId, int subjectId, CancellationToken ct);
    Task<List<MarksEntryStudentDto>> GetMarksEntryListAsync(int examId, int classId, int sectionId, int subjectId, CancellationToken ct, int? optionalSubjectId = null);
}