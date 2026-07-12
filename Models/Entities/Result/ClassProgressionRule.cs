using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Result;

public class ClassProgressionRule : BaseEntity
{
    public int FromClassId { get; set; }
    public Academic.SchoolClass? FromClass { get; set; }

    public int ToClassId { get; set; }
    public Academic.SchoolClass? ToClass { get; set; }

    [MaxLength(20)]
    public string ProgressionType { get; set; } = "Normal";

    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
}
