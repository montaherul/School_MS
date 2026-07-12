namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IReportCardService
{
    Task<byte[]?> GenerateReportCardPdfAsync(int examId, int studentId, CancellationToken ct = default);
    Task<byte[]?> GenerateReportCardPdfAsync(int examId, int studentId, bool isAdmin, CancellationToken ct = default);
    Task<bool> IsResultBlockedForStudentAsync(int studentId, CancellationToken cancellationToken = default);
    Task<byte[]> GenerateBulkReportCardsAsync(int examId, int? classId, int? sectionId, string format, CancellationToken ct = default);
    Task<int> GetReportCardCountAsync(int examId, int? classId, int? sectionId, CancellationToken ct = default);
    Task<int> AddToPrintQueueAsync(int examId, int? classId, int? sectionId, int totalItems, string requestedBy, CancellationToken ct = default);
}

