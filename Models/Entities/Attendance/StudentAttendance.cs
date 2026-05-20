using System;
using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Academic;

namespace SchoolManagementSystem.Models.Entities.Attendance
{
    public class StudentAttendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }
        public Student.Student? Student { get; set; }

        [Required]
        public int ClassId { get; set; }
        public SchoolClass? Class { get; set; }

        [Required]
        public int SectionId { get; set; }
        public Section? Section { get; set; }

        [Required]
        public DateTime AttendanceDate { get; set; }

        [Required]
        public SchoolManagementSystem.Models.Enums.AttendanceStatus Status { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }

        [Required]
        public string RecordedBy { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
