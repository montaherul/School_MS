namespace SchoolManagementSystem.Models.DTOs.Attendance
{
    public class DashboardAttendanceSummaryDto
    {
        public int TotalStudents { get; set; }
        public int StudentPresent { get; set; }
        public int StudentAbsent { get; set; }
        public int StudentLate { get; set; }
        public int StudentLeave { get; set; }
        public decimal StudentAttendancePercentage { get; set; }

        public int TotalEmployees { get; set; }
        public int EmployeePresent { get; set; }
        public int EmployeeAbsent { get; set; }
        public int EmployeeLate { get; set; }
        public int EmployeeLeave { get; set; }
        
        public int ClassesMissingAttendance { get; set; }
        public int LockedSessions { get; set; }
    }
}