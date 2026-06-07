namespace SchoolManagementSystem.Models.DTOs.Attendance
{
    public class StudentAttendanceStatsDto
    {
        public int StudentId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int WorkingDays { get; set; }
        public int RecordedDays { get; set; }
        public int Present { get; set; }
        public int Late { get; set; }
        public int Absent { get; set; }
        public int Leave { get; set; }
        public int CountedAsPresent { get; set; }
        public double AttendancePercentage { get; set; }
    }

    public class EmployeeAttendanceStatsDto
    {
        public int EmployeeId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int WorkingDays { get; set; }
        public int RecordedDays { get; set; }
        public int Present { get; set; }
        public int Late { get; set; }
        public int Absent { get; set; }
        public int Leave { get; set; }
        public int CountedAsPresent { get; set; }
        public double AttendancePercentage { get; set; }
    }
}
