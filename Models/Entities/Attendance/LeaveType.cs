using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.Entities.Attendance
{
    public class LeaveType
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int MaxDays { get; set; }

        public bool IsPaid { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
