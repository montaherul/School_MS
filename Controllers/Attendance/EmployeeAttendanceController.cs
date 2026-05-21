using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.Services.Interfaces.Attendance;

namespace SchoolManagementSystem.Controllers.Attendance
{
    [Authorize]
    public class EmployeeAttendanceController : Controller
    {
        private readonly IEmployeeAttendanceService _service;
        private readonly IDepartmentService _departmentService;
        private readonly IDesignationService _designationService;

        public EmployeeAttendanceController(
            IEmployeeAttendanceService service,
            IDepartmentService departmentService,
            IDesignationService designationService)
        {
            _service = service;
            _departmentService = departmentService;
            _designationService = designationService;
        }

        [RequirePermission("Attendance.View")]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            ViewBag.Departments = await _departmentService.GetAllAsync(ct);
            ViewBag.Designations = await _designationService.GetAllAsync(ct);
            return View();
        }

        [HttpGet]
        [RequirePermission("Attendance.View")]
        public async Task<IActionResult> LoadAttendance(
            int page = 1,
            int size = 25,
            DateTime? attendanceDate = null,
            int? departmentId = null,
            int? designationId = null,
            string? employeeType = null,
            bool? isTeachingStaff = null,
            CancellationToken ct = default)
        {
            var filter = new EmployeeAttendanceFilterDto
            {
                AttendanceDate = (attendanceDate ?? DateTime.Today).Date,
                DepartmentId = departmentId,
                DesignationId = designationId,
                EmployeeType = employeeType,
                IsTeachingStaff = isTeachingStaff
            };

            var result = await _service.LoadAttendanceAsync(filter, page, size, ct);
            return Json(new
            {
                data = result.Data,
                last_page = Math.Max(1, (int)Math.Ceiling((double)result.TotalRecords / size)),
                total_records = result.TotalRecords,
                summary = result.Summary
            });
        }

        [HttpGet]
        [RequirePermission("Attendance.View")]
        public async Task<IActionResult> GetPagedData(int page = 1, int size = 10, string? date = null, CancellationToken ct = default)
        {
            DateTime? parsedDate = null;
            if (DateTime.TryParse(date, out var d)) parsedDate = d;

            var filter = new EmployeeAttendanceFilterDto { AttendanceDate = (parsedDate ?? DateTime.Today).Date };
            var result = await _service.LoadAttendanceAsync(filter, page, size, ct);
            return Json(new { data = result.Data, last_page = Math.Ceiling((double)result.TotalRecords / size), total_records = result.TotalRecords, summary = result.Summary });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Attendance.Create")]
        public async Task<IActionResult> SaveAttendance([FromBody] EmployeeAttendanceBulkDto dto, CancellationToken ct)
        {
            try
            {
                var userName = User.Identity?.Name ?? "Unknown";
                await _service.SaveAttendanceAsync(dto, userName, ct);
                return Json(new { success = true, message = "Employee attendance saved successfully." });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Attendance.Create")]
        public async Task<IActionResult> Mark([FromBody] EmployeeAttendanceDto dto, CancellationToken ct)
        {
            try
            {
                var userName = User.Identity?.Name ?? "Unknown";
                await _service.MarkStatusAsync(dto.EmployeeId, dto.AttendanceDate, dto.Status, dto.Remarks, userName, ct);
                return Json(new { success = true, message = "Attendance marked successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Attendance.Create")]
        public async Task<IActionResult> BulkMark([FromBody] EmployeeAttendanceBulkDto dto, CancellationToken ct)
        {
            try
            {
                var userName = User.Identity?.Name ?? "Unknown";
                await _service.SaveAttendanceAsync(dto, userName, ct);
                return Json(new { success = true, message = "Bulk attendance saved successfully." });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [RequirePermission("Attendance.View")]
        public async Task<IActionResult> AttendanceHistory(int employeeId, int? year = null, int? month = null, CancellationToken ct = default)
        {
            var today = DateTime.Today;
            var y = year ?? today.Year;
            var m = month ?? today.Month;
            var rows = await _service.GetAttendanceHistoryAsync(employeeId, y, m, ct);
            var summary = await _service.GetMonthlySummaryAsync(employeeId, y, m, ct);
            return Json(new { data = rows, summary });
        }

        [HttpGet]
        [RequirePermission("Attendance.View")]
        public async Task<IActionResult> MonthlySummary(int employeeId, int? year = null, int? month = null, CancellationToken ct = default)
        {
            var today = DateTime.Today;
            var summary = await _service.GetMonthlySummaryAsync(employeeId, year ?? today.Year, month ?? today.Month, ct);
            return Json(summary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Attendance.Create")]
        public async Task<IActionResult> CheckIn([FromBody] EmployeeAttendanceDto dto, CancellationToken ct)
        {
            try
            {
                var userName = User.Identity?.Name ?? "Unknown";
                var time = DateTime.Now.TimeOfDay;
                await _service.CheckInAsync(dto.EmployeeId, DateTime.Today, time, userName, ct);
                return Json(new { success = true, message = "Checked in successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Attendance.Create")]
        public async Task<IActionResult> CheckOut([FromBody] EmployeeAttendanceDto dto, CancellationToken ct)
        {
            try
            {
                var userName = User.Identity?.Name ?? "Unknown";
                var time = DateTime.Now.TimeOfDay;
                await _service.CheckOutAsync(dto.EmployeeId, DateTime.Today, time, userName, ct);
                return Json(new { success = true, message = "Checked out successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
