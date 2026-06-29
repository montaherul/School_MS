using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Website;

public class AdmissionFeeStructure : BaseEntity
{
    public int SchoolClassId { get; set; }
    public SchoolClass? SchoolClass { get; set; }

    [MaxLength(50)]
    public string? ClassName { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AdmissionFee { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal MonthlyFee { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal SessionFee { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal ExamFee { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal OtherFee { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal RegistrationFee { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal DevelopmentFee { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal LibraryFee { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal LaboratoryFee { get; set; } = 0;

    public int DisplayOrder { get; set; } = 0;

    public bool IsActive { get; set; } = true;
}