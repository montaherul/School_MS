using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.Entities.Attendance
{
    public class AttendanceLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Action { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string EntityName { get; set; } = string.Empty;

        public int EntityId { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string IPAddress { get; set; } = string.Empty;
    }
}
