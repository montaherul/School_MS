using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Academic;

#pragma warning disable CS8618

public class TransferCertificateListItemDto
{
    public int Id { get; set; }
    public string CertificateNo { get; set; }
    public int StudentId { get; set; }
    public int OldClassId { get; set; }
    public int? OldSectionId { get; set; }
    public string NewSchoolName { get; set; }
    public DateTime IssueDate { get; set; }
    public string Reason { get; set; }
    public bool IsActive { get; set; }
    public int TotalRecords { get; set; }
}

public class TransferCertificateUpsertDto
{
    public int Id { get; set; }

    [Required]
    public int StudentId { get; set; }

    [Required]
    public int OldClassId { get; set; }

    public int? OldSectionId { get; set; }

    [Required]
    [StringLength(200)]
    public string NewSchoolName { get; set; } = string.Empty;

    [StringLength(40)]
    public string CertificateNo { get; set; } = string.Empty;

    [Required]
    public DateTime IssueDate { get; set; } = DateTime.Today;

    [StringLength(500)]
    public string Reason { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public int NewClassId { get; set; }
    public int NewSectionId { get; set; }
    public int? NewStudentGroupId { get; set; }
}
