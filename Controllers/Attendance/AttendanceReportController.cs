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

        public AttendanceReportController(IAttendanceReportService service, ITeacherScopeService teacherScopeService)
        {
            _service = service;
            _teacherScopeService = teacherScopeService;
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
        public async Task<IActionResult> DownloadStudentReport(int classId, int sectionId, int year, int month, CancellationToken ct)
        {
            if (!IsAdminOrPrincipal())
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdStr, out var userId))
                {
                    var hasAccess = await _teacherScopeService.HasClassAccessAsync(userId, classId, sectionId, ct);
                    if (!hasAccess) return Forbid();
                }
                else
                {
                    return Forbid();
                }
            }

            var pdf = await _service.GenerateStudentMonthlyPdfAsync(classId, sectionId, year, month, ct);
            if (pdf == null || pdf.Length == 0) return NotFound("Report could not be generated or no data found.");

            return File(pdf, "application/pdf", $"StudentAttendance_{year}_{month}.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadEmployeeReport(int year, int month, CancellationToken ct)
        {
            if (!IsAdminOrPrincipal()) return Forbid();

            var pdf = await _service.GenerateEmployeeMonthlyPdfAsync(year, month, ct);
            if (pdf == null || pdf.Length == 0) return NotFound("Report could not be generated or no data found.");

            return File(pdf, "application/pdf", $"EmployeeAttendance_{year}_{month}.pdf");
        }
    }
}
