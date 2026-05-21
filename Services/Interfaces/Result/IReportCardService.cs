namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IReportCardService
{
    Task<byte[]?> GenerateReportCardPdfAsync(int examId, int studentId, CancellationToken ct = default);
}

