using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Health;

public class MedicalRecord : BaseEntity
{
    public int StudentId { get; set; }

    [MaxLength(120)]
    public string BloodGroup { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Allergies { get; set; }

    [MaxLength(120)]
    public string EmergencyContactName { get; set; } = string.Empty;

    [MaxLength(30)]
    public string EmergencyContactPhone { get; set; } = string.Empty;
}

public class VaccinationRecord : BaseEntity
{
    public int StudentId { get; set; }

    [MaxLength(120)]
    public string VaccineName { get; set; } = string.Empty;

    public DateOnly VaccinatedOn { get; set; }
    public DateOnly? NextDueOn { get; set; }
}
