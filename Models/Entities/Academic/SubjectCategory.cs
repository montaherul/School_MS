using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
namespace SchoolManagementSystem.Models.Entities.Academic;

public class SubjectCategory : BaseEntity
{
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
