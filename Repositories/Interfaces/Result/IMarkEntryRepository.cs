using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.ViewModels.Result;

namespace SchoolManagementSystem.Repositories.Interfaces.Result;

public interface IMarkEntryRepository : IBaseRepository<MarkEntry>
{
    Task<List<StudentMarkViewModel>> GetMarkEntrySheetAsync(int examId, int classId, int sectionId, int subjectId, CancellationToken ct);
}
