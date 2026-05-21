using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.ViewModels.Attendance;
using SchoolManagementSystem.Repositories.Interfaces.Attendance;
using SchoolManagementSystem.Services.Interfaces.Attendance;

namespace SchoolManagementSystem.Services.Implementations.Attendance
{
    public class AttendanceReportService : IAttendanceReportService
    {
        private readonly IStudentAttendanceRepository _studentRepo;
        private readonly IEmployeeAttendanceRepository _employeeRepo;
        private readonly ILeaveApplicationRepository _leaveRepo;

        public AttendanceReportService(
            IStudentAttendanceRepository studentRepo,
            IEmployeeAttendanceRepository employeeRepo,
            ILeaveApplicationRepository leaveRepo)
        {
            _studentRepo = studentRepo;
            _employeeRepo = employeeRepo;
            _leaveRepo = leaveRepo;
        }

        public async Task<AttendanceDashboardVm> GetDashboardSummaryAsync(CancellationToken ct = default)
        {
            var today = DateTime.UtcNow.Date;

            var studentPresent = await _studentRepo.Query().CountAsync(a => a.AttendanceDate == today && (a.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Present || a.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Late), ct);
            var studentAbsent = await _studentRepo.Query().CountAsync(a => a.AttendanceDate == today && a.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Absent, ct);

            var employeePresent = await _employeeRepo.Query().CountAsync(a => a.AttendanceDate == today && (a.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Present || a.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Late), ct);
            var employeeAbsent = await _employeeRepo.Query().CountAsync(a => a.AttendanceDate == today && a.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Absent, ct);

            var pendingLeaves = await _leaveRepo.Query().CountAsync(l => l.ApprovalStatus == SchoolManagementSystem.Models.Entities.Attendance.LeaveApplication.ApprovalStatusEnum.Pending, ct);

            return new AttendanceDashboardVm
            {
                TotalPresentStudents = studentPresent,
                TotalAbsentStudents = studentAbsent,
                TotalPresentEmployees = employeePresent,
                TotalAbsentEmployees = employeeAbsent,
                PendingLeaveRequests = pendingLeaves,
                StudentAttendancePercentage = studentPresent + studentAbsent > 0 ? Math.Round(((double)studentPresent / (studentPresent + studentAbsent)) * 100, 2) : 0,
                EmployeeAttendancePercentage = employeePresent + employeeAbsent > 0 ? Math.Round(((double)employeePresent / (employeePresent + employeeAbsent)) * 100, 2) : 0
            };
        }

        public Task<byte[]> GenerateStudentMonthlyPdfAsync(int classId, int sectionId, int year, int month, CancellationToken ct = default)
        {
            // Placeholder for PDF generation using iTextSharp or similar
            return Task.FromResult(Array.Empty<byte>());
        }

        public Task<byte[]> GenerateEmployeeMonthlyPdfAsync(int year, int month, CancellationToken ct = default)
        {
            // Placeholder for PDF generation
            return Task.FromResult(Array.Empty<byte>());
        }
    }
}
