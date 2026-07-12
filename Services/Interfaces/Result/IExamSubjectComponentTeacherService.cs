using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SchoolManagementSystem.Models.DTOs.Exam;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IExamSubjectComponentTeacherService
{
    Task<bool> IsCustomizationEnabledAsync(CancellationToken ct = default);
    Task<List<TeacherExamSubjectDto>> GetTeacherExamSubjectsAsync(int teacherId, CancellationToken ct = default);
    Task<List<TeacherExamSubjectComponentDto>> GetExamSubjectComponentsAsync(int teacherId, int examSubjectId, CancellationToken ct = default);
    Task<bool> CanCustomizeAsync(int teacherId, int examSubjectId, CancellationToken ct = default);
    Task<bool> UpdateComponentAsync(int teacherId, TeacherExamSubjectComponentUpsertDto dto, string updatedBy, CancellationToken ct = default);
    Task<bool> UpdateComponentsBulkAsync(int teacherId, int examSubjectId, List<TeacherExamSubjectComponentUpsertDto> components, string updatedBy, CancellationToken ct = default);
    Task<TeacherMarksEntryGridConfigDto?> GetMarksEntryGridConfigAsync(int teacherId, int examSubjectId, CancellationToken ct = default);
}