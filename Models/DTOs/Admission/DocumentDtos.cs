using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.DTOs.Admission;

public class DocumentVerificationRequest
{
    public int DocumentId { get; set; }
    public DocumentVerificationStatus Status { get; set; }
    public string? Remarks { get; set; }
}

public class DocumentUploadRequest
{
    public int ApplicationId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
}

public class DocumentDto
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string? OriginalFileName { get; set; }
    public long FileSize { get; set; }
    public string? ContentType { get; set; }
    public string VerificationStatus { get; set; } = string.Empty;
    public DateTime? VerifiedAt { get; set; }
    public string? VerifiedBy { get; set; }
    public string? VerificationRemarks { get; set; }
    public int VersionNumber { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class DocumentSummaryDto
{
    public int TotalDocuments { get; set; }
    public int VerifiedCount { get; set; }
    public int PendingCount { get; set; }
    public int RejectedCount { get; set; }
    public int ExpiredCount { get; set; }
    public List<DocumentDto> Documents { get; set; } = new();
}
