using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.Entities.Attendance
{
    public class AttendanceSetting
    {
        [Key]
        public int Id { get; set; }

        public TimeSpan SchoolStartTime { get; set; } = new TimeSpan(8, 0, 0);
        public int LateAfterMinutes { get; set; } = 15;
        public int HalfDayAfterMinutes { get; set; } = 240;
        public int RevisionWindowHours { get; set; } = 24;
        public bool CountLateAsPresent { get; set; } = true;
        public bool CountLeaveAsPresent { get; set; }

        [MaxLength(100)]
        public string WorkingDays { get; set; } = "Sun,Mon,Tue,Wed,Thu"; 

        public int AttendanceLockAfterHours { get; set; } = 24; 
        
        public bool AutoAbsentEnabled { get; set; } = true;
    }
}
