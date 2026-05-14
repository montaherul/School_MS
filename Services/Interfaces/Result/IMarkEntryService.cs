using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.ViewModels.Result;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Services.Interfaces.Base;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IMarkEntryService : IBaseService<MarkEntry>
{
    Task<MarkEntryViewModel> GetMarkEntryDataAsync(int examId, int subjectId, int classId, int sectionId);
    Task SubmitMarksBatchAsync(MarkBatchDto dto);
}
