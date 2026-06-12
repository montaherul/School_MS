using SchoolManagementSystem.Models.ViewModels.Exam;

namespace SchoolManagementSystem.Services.Interfaces.Exam;

public interface IAdmitCardService
{
    Task GenerateAdmitCardsAsync(int examId);
    Task<AdmitCardViewModel> GetAdmitCardAsync(int examId, int studentId);
    Task<byte[]> GenerateAdmitCardPdfAsync(int examId, int studentId);
    Task<byte[]> GenerateBulkAdmitCardsPdfAsync(int examId, int? sectionId);
    Task<bool> IsAdmitCardGeneratedAsync(int examId);
}
