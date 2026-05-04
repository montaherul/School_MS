using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Transport;

public class TransportRoute : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string PickupDropSchedule { get; set; } = string.Empty;
}

public class Vehicle : BaseEntity
{
    [MaxLength(40)]
    public string RegistrationNo { get; set; } = string.Empty;

    public int Capacity { get; set; }
}

public class Driver : BaseEntity
{
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(30)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(60)]
    public string LicenseNo { get; set; } = string.Empty;
}

public class StudentRouteAssignment : BaseEntity
{
    public int StudentId { get; set; }
    public int TransportRouteId { get; set; }
    public int VehicleId { get; set; }
}
