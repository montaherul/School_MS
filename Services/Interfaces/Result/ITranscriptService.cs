using SchoolManagementSystem.Models.DTOs.Result;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface ITranscriptService
{
    Task<StudentTranscriptDto?> GetStudentTranscriptAsync(int studentId, int academicYearId);
    Task<byte[]?> GenerateTranscriptPdfAsync(int studentId, int academicYearId);
    Task<bool> IsResultBlockedForStudentAsync(int studentId, CancellationToken cancellationToken = default);
    Task<bool> HasGuardianAccessAsync(int userId, int studentId, CancellationToken ct = default);
}
