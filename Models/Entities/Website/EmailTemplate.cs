using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Website;

public class EmailTemplate : BaseEntity
{
    [MaxLength(100)]
    public string? TemplateName { get; set; }

    [MaxLength(500)]
    public string? Subject { get; set; }

    public string? Body { get; set; }

    [MaxLength(500)]
    public string? Placeholders { get; set; }

    [MaxLength(50)]
    public string? Category { get; set; }

    public bool IsActive { get; set; } = true;
}