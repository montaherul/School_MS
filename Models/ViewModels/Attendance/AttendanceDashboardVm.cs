namespace SchoolManagementSystem.Models.ViewModels.Attendance
{
    public class AttendanceDashboardVm
    {
        public int TotalPresentStudents { get; set; }
        public int TotalAbsentStudents { get; set; }
        public int TotalPresentEmployees { get; set; }
        public int TotalAbsentEmployees { get; set; }
        public int PendingLeaveRequests { get; set; }
        public double StudentAttendancePercentage { get; set; }
        public double EmployeeAttendancePercentage { get; set; }
    }
}
