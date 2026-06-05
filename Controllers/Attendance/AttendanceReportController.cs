using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.Services.Interfaces.Teachers;

namespace SchoolManagementSystem.Controllers.Attendance
{
    [Authorize(Roles = "Super Admin,Admin,Principal,Assistant Head,Senior Lecturer,Lecturer,Teacher")]
    public class AttendanceReportController : Controller
    {
        private readonly IAttendanceReportService _service;
        private readonly ITeacherScopeService _teacherScopeService;
        private readonly IAttendanceAuthorizationService _attendanceAuthorizationService;

        public AttendanceReportController(
            IAttendanceReportService service,
            ITeacherScopeService teacherScopeService,
            IAttendanceAuthorizationService attendanceAuthorizationService)
        {
            _service = service;
            _teacherScopeService = teacherScopeService;
            _attendanceAuthorizationService = attendanceAuthorizationService;
        }

        private bool IsAdminOrPrincipal()
        {
            return User.IsInRole("Super Admin") || User.IsInRole("Admin") || User.IsInRole("Principal") || User.IsInRole("Assistant Head");
        }

        public async Task<IActionResult> Dashboard(CancellationToken ct)
        {
            if (!IsAdminOrPrincipal()) return Forbid();

            var summary = await _service.GetDashboardSummaryAsync(ct);
            return View(summary);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadStudentReport(int classId, int sectionId, int year, int month, int? studentGroupId, CancellationToken ct)
        {
            if (!IsAdminOrPrincipal())
            {
                await _attendanceAuthorizationService.EnsureCurrentUserCanManageAttendanceAsync(classId, sectionId, studentGroupId, 0, ct);
            }

            var pdf = await _service.GenerateStudentMonthlyPdfAsync(classId, sectionId, year, month, ct, studentGroupId);
            if (pdf == null || pdf.Length == 0) return NotFound("Report could not be generated or no data found.");

            return File(pdf, "application/pdf", $"StudentAttendance_{year}_{month}.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadStudentYearlyReport(int classId, int sectionId, int year, int? studentGroupId, CancellationToken ct)
        {
            if (!IsAdminOrPrincipal())
            {
                await _attendanceAuthorizationService.EnsureCurrentUserCanManageAttendanceAsync(classId, sectionId, studentGroupId, 0, ct);
            }

            var pdf = await _service.GenerateStudentYearlyPdfAsync(classId, sectionId, year, ct, studentGroupId);
            return File(pdf, "application/pdf", $"StudentAttendance_{classId}_{sectionId}_{year}.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadClassReport(int classId, DateTime fromDate, DateTime toDate, CancellationToken ct)
        {
            if (!IsAdminOrPrincipal()) return Forbid();

            var pdf = await _service.GenerateClassAttendancePdfAsync(classId, fromDate, toDate, ct);
            return File(pdf, "application/pdf", $"ClassAttendance_{classId}_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadSectionReport(int classId, int sectionId, DateTime fromDate, DateTime toDate, CancellationToken ct)
        {
            if (!IsAdminOrPrincipal())
            {
                await _attendanceAuthorizationService.EnsureCurrentUserCanManageAttendanceAsync(classId, sectionId, null, 0, ct);
            }

            var pdf = await _service.GenerateSectionAttendancePdfAsync(classId, sectionId, fromDate, toDate, ct);
            return File(pdf, "application/pdf", $"SectionAttendance_{classId}_{sectionId}_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadGroupReport(int classId, int sectionId, int studentGroupId, DateTime fromDate, DateTime toDate, CancellationToken ct)
        {
            if (!IsAdminOrPrincipal())
            {
                await _attendanceAuthorizationService.EnsureCurrentUserCanManageAttendanceAsync(classId, sectionId, studentGroupId, 0, ct);
            }

            var pdf = await _service.GenerateGroupAttendancePdfAsync(classId, sectionId, studentGroupId, fromDate, toDate, ct);
            return File(pdf, "application/pdf", $"GroupAttendance_{classId}_{sectionId}_{studentGroupId}_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadEmployeeReport(int year, int month, CancellationToken ct)
        {
            if (!IsAdminOrPrincipal()) return Forbid();

            var pdf = await _service.GenerateEmployeeMonthlyPdfAsync(year, month, ct);
            if (pdf == null || pdf.Length == 0) return NotFound("Report could not be generated or no data found.");

            return File(pdf, "application/pdf", $"EmployeeAttendance_{year}_{month}.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadEmployeeYearlyReport(int year, CancellationToken ct)
        {
            if (!IsAdminOrPrincipal()) return Forbid();

            var pdf = await _service.GenerateEmployeeYearlyPdfAsync(year, ct);
            return File(pdf, "application/pdf", $"EmployeeAttendance_{year}.pdf");
        }
    }
}
