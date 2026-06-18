using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Fees;

public class StudentFeeAssignmentListItemDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentNo { get; set; } = string.Empty;
    public int FeeStructureId { get; set; }
    public string FeeStructureName { get; set; } = string.Empty;
    public int? AcademicYearId { get; set; }
    public string? AcademicYearName { get; set; }
    public decimal? CustomAmount { get; set; }
    public bool IsActive { get; set; }
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
    public int TotalRecords { get; set; }
}

public class StudentFeeAssignmentUpsertDto
{
    public int Id { get; set; }

    [Required]
    public int StudentId { get; set; }

    [Required]
    public int FeeStructureId { get; set; }

    public int? AcademicYearId { get; set; }

    public decimal? CustomAmount { get; set; }
    public bool IsActive { get; set; } = true;
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
}
