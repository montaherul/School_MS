using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Academic;

public class SubjectListItemDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;


    public string NameBn { get; set; } = string.Empty;

    public string? SubjectGroup { get; set; }
    public bool IsReligionSubject { get; set; }
    public string? ReligionType { get; set; }
    public bool IsOptional { get; set; }
    public bool IsPractical { get; set; }
    public bool IsActive { get; set; }
}

public class SubjectUpsertDto
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(30)]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;



    [Required]
    [StringLength(100)]
    public string NameBn { get; set; } = string.Empty;


    public string? SubjectGroup { get; set; }
    public bool IsReligionSubject { get; set; }
    public string? ReligionType { get; set; }
    public bool IsOptional { get; set; }
    public bool IsPractical { get; set; }
    public bool IsActive { get; set; } = true;
}

