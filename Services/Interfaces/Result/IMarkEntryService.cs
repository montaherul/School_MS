using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Services.Interfaces.Base;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IMarkEntryService : IBaseService<MarkEntry>
{
    Task<MarkEntryDataDto> GetMarkEntryDataAsync(int examId, int subjectId, int classId, int sectionId);
    Task SubmitMarksBatchAsync(MarkBatchDto dto);
    Task<byte[]> GenerateImportTemplateAsync(int examId, int subjectId, int classId, int sectionId);
    Task<ImportResultDto> ImportMarksFromExcelAsync(Stream stream, int examId, int subjectId, int classId, int sectionId, int teacherId, bool saveAsDraft);
    Task<byte[]> ExportMarksToExcelAsync(int examId, int subjectId, int classId, int sectionId, int? groupId);
    Task<string> ExportMarksToCsvAsync(int examId, int subjectId, int classId, int sectionId, int? groupId);

    Task LockMarksAsync(int examId, int subjectId, int classId, int sectionId);
    Task UnlockMarksAsync(int examId, int subjectId, int classId, int sectionId);
    Task<EntryStatusSummaryDto> GetEntryStatusAsync(int examId, int? classId = null);
}
