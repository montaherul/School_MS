using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Admission;
using SchoolManagementSystem.Services.Interfaces.Admissions;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Data;

namespace SchoolManagementSystem.Services.Implementations.Admissions;

public class DocumentVerificationService : IDocumentVerificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAdmissionRepository _admissionRepository;
    private readonly ILogger<DocumentVerificationService> _logger;

    private static readonly HashSet<string> _allowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        { ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx" };
    private const long _maxFileSize = 10 * 1024 * 1024;

    public DocumentVerificationService(
        IUnitOfWork unitOfWork,
        IAdmissionRepository admissionRepository,
        ILogger<DocumentVerificationService> logger)
    {
        _unitOfWork = unitOfWork;
        _admissionRepository = admissionRepository;
        _logger = logger;
    }

    public async Task<DocumentSummaryDto> GetDocumentsByApplicationAsync(int applicationId, CancellationToken ct = default)
    {
        var docs = await _unitOfWork.Repository<AdmissionDocument>().Query().AsNoTracking()
            .Where(d => d.AdmissionApplicationId == applicationId && !d.IsDeleted)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(ct);

        return new DocumentSummaryDto
        {
            TotalDocuments = docs.Count,
            VerifiedCount = docs.Count(d => d.VerificationStatus == DocumentVerificationStatus.Verified),
            PendingCount = docs.Count(d => d.VerificationStatus == DocumentVerificationStatus.Pending),
            RejectedCount = docs.Count(d => d.VerificationStatus == DocumentVerificationStatus.Rejected),
            ExpiredCount = docs.Count(d => d.VerificationStatus == DocumentVerificationStatus.Expired),
            Documents = docs.Select(d => new DocumentDto
            {
                Id = d.Id,
                ApplicationId = d.AdmissionApplicationId,
                DocumentType = d.DocumentType,
                FilePath = d.FilePath,
                OriginalFileName = d.OriginalFileName,
                FileSize = d.FileSize,
                ContentType = d.ContentType,
                VerificationStatus = d.VerificationStatus.ToString(),
                VerifiedAt = d.VerifiedAt,
                VerifiedBy = d.VerifiedBy,
                VerificationRemarks = d.VerificationRemarks,
                VersionNumber = d.VersionNumber,
                UploadedAt = d.CreatedAt
            }).ToList()
        };
    }

    public async Task<DocumentDto> VerifyDocumentAsync(int documentId, DocumentVerificationStatus status, string verifiedBy, string? remarks = null, CancellationToken ct = default)
    {
        var doc = await _unitOfWork.Repository<AdmissionDocument>().FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted, ct)
            ?? throw new InvalidOperationException("Document not found.");

        doc.VerificationStatus = status;
        doc.VerifiedAt = DateTime.UtcNow;
        doc.VerifiedBy = verifiedBy;
        doc.VerificationRemarks = remarks;
        doc.UpdatedBy = verifiedBy;
        doc.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<AdmissionDocument>().Update(doc);
        await _unitOfWork.SaveChangesAsync(ct);

        // Check if all documents are verified
        var allVerified = await AreAllDocumentsVerifiedAsync(doc.AdmissionApplicationId, ct);
        if (allVerified)
        {
            var app = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == doc.AdmissionApplicationId && !x.IsDeleted, ct);
            if (app != null)
            {
                app.AllDocumentsVerified = true;
                app.DocumentsVerifiedAt = DateTime.UtcNow;
                app.DocumentsVerifiedBy = verifiedBy;
                _admissionRepository.Update(app);
                await _unitOfWork.SaveChangesAsync(ct);
            }
        }

        return MapToDto(doc);
    }

    public async Task<DocumentDto> UploadDocumentAsync(int applicationId, string documentType, IFormFile file, string uploadedBy, CancellationToken ct = default)
    {
        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(ext) || !_allowedExtensions.Contains(ext))
            throw new InvalidOperationException($"File type '{ext}' is not allowed.");

        if (file.Length == 0 || file.Length > _maxFileSize)
            throw new InvalidOperationException("File size must be between 1 byte and 10 MB.");

        var subFolder = $"admissions/documents/{applicationId}";
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", subFolder);
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        var safeName = $"{Guid.NewGuid():N}{ext}";
        var filePath = Path.Combine(folderPath, safeName);
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream, ct);

        var doc = new AdmissionDocument
        {
            AdmissionApplicationId = applicationId,
            DocumentType = documentType,
            FilePath = $"/uploads/{subFolder}/{safeName}",
            OriginalFileName = file.FileName,
            FileSize = file.Length,
            ContentType = file.ContentType,
            VerificationStatus = DocumentVerificationStatus.Pending,
            VersionNumber = 1,
            CreatedBy = uploadedBy,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<AdmissionDocument>().AddAsync(doc, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return MapToDto(doc);
    }

    public async Task<bool> DeleteDocumentAsync(int documentId, string deletedBy, CancellationToken ct = default)
    {
        var doc = await _unitOfWork.Repository<AdmissionDocument>().FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted, ct);
        if (doc == null) return false;

        doc.IsDeleted = true;
        doc.UpdatedBy = deletedBy;
        doc.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<AdmissionDocument>().Update(doc);
        await _unitOfWork.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> RequestReUploadAsync(int documentId, string requestedBy, string? remarks = null, CancellationToken ct = default)
    {
        var doc = await _unitOfWork.Repository<AdmissionDocument>().FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted, ct);
        if (doc == null) return false;

        doc.VerificationStatus = DocumentVerificationStatus.ReUploadRequested;
        doc.VerificationRemarks = remarks;
        doc.VerifiedBy = requestedBy;
        doc.VerifiedAt = DateTime.UtcNow;
        doc.UpdatedBy = requestedBy;
        doc.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<AdmissionDocument>().Update(doc);
        await _unitOfWork.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> AreAllDocumentsVerifiedAsync(int applicationId, CancellationToken ct = default)
    {
        var docs = await _unitOfWork.Repository<AdmissionDocument>().Query().AsNoTracking()
            .Where(d => d.AdmissionApplicationId == applicationId && !d.IsDeleted)
            .ToListAsync(ct);

        return docs.Count > 0 && docs.All(d => d.VerificationStatus == DocumentVerificationStatus.Verified);
    }

    private static DocumentDto MapToDto(AdmissionDocument doc) => new()
    {
        Id = doc.Id,
        ApplicationId = doc.AdmissionApplicationId,
        DocumentType = doc.DocumentType,
        FilePath = doc.FilePath,
        OriginalFileName = doc.OriginalFileName,
        FileSize = doc.FileSize,
        ContentType = doc.ContentType,
        VerificationStatus = doc.VerificationStatus.ToString(),
        VerifiedAt = doc.VerifiedAt,
        VerifiedBy = doc.VerifiedBy,
        VerificationRemarks = doc.VerificationRemarks,
        VersionNumber = doc.VersionNumber,
        UploadedAt = doc.CreatedAt
    };
}
