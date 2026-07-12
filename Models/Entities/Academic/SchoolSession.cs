using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
namespace SchoolManagementSystem.Models.Entities.Academic;

public class SchoolSession : BaseEntity
{
    public int AcademicYearId { get; set; }
    public AcademicYear? AcademicYear { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsActive { get; set; } = true;
}
