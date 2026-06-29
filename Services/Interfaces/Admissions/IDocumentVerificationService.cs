using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Services.Interfaces.Admissions;

public interface IDocumentVerificationService
{
    Task<DocumentSummaryDto> GetDocumentsByApplicationAsync(int applicationId, CancellationToken ct = default);
    Task<DocumentDto> VerifyDocumentAsync(int documentId, DocumentVerificationStatus status, string verifiedBy, string? remarks = null, CancellationToken ct = default);
    Task<DocumentDto> UploadDocumentAsync(int applicationId, string documentType, IFormFile file, string uploadedBy, CancellationToken ct = default);
    Task<bool> DeleteDocumentAsync(int documentId, string deletedBy, CancellationToken ct = default);
    Task<bool> RequestReUploadAsync(int documentId, string requestedBy, string? remarks = null, CancellationToken ct = default);
    Task<bool> AreAllDocumentsVerifiedAsync(int applicationId, CancellationToken ct = default);
}
