using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Enums;
using StudentEntity = SchoolManagementSystem.Models.Entities.Student.Student;

namespace SchoolManagementSystem.Models.Entities.Attendance;

public class AttendanceRecord : BaseEntity
{
    public int StudentId { get; set; }
    public StudentEntity? Student { get; set; }
    public int SchoolClassId { get; set; }
    public int SectionId { get; set; }
    public DateOnly AttendanceDate { get; set; }
    public int? PeriodNo { get; set; }
    public AttendanceStatus Status { get; set; }

    [MaxLength(240)]
    public string? Remarks { get; set; }

    public int? CreatedByUserId { get; set; }
    public int? UpdatedByUserId { get; set; }
}

