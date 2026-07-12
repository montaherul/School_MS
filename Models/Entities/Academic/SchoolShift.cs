using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
namespace SchoolManagementSystem.Models.Entities.Academic;

public class SchoolShift : BaseEntity
{
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(10)]
    public string Code { get; set; } = string.Empty;

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
