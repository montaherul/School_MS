using System;
using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Attendance
{
    public class AutoAbsentExecutionLog : BaseEntity
    {
        [Required]
        public DateTime ExecutionDate { get; set; }

        [Required]
        public DateTime TargetDate { get; set; }

        public int StudentsProcessed { get; set; }
        public int StudentsMarkedAbsent { get; set; }
        public int EmployeesProcessed { get; set; }
        public int EmployeesMarkedAbsent { get; set; }

        public int HolidaysSkipped { get; set; }
        public int WeeklyOffsSkipped { get; set; }
        public int WorkingDaysEvaluated { get; set; }

        [MaxLength(40)]
        public string Status { get; set; } = "Success";

        [MaxLength(2000)]
        public string? Message { get; set; }

        public int DurationMs { get; set; }
    }
}
