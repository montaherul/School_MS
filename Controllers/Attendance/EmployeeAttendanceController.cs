using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Helpers.Reports;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Controllers.Attendance
{
    [Authorize]
    public class EmployeeAttendanceController : Controller
    {
        private readonly IEmployeeAttendanceService _service;
        private readonly IDepartmentService _departmentService;
        private readonly IDesignationService _designationService;
        private readonly IUnitOfWork _uow;

        public EmployeeAttendanceController(
            IEmployeeAttendanceService service,
            IDepartmentService departmentService,
            IDesignationService designationService,
            IUnitOfWork uow)
        {
            _service = service;
            _departmentService = departmentService;
            _designationService = designationService;
            _uow = uow;
        }

        private bool IsAdminOrPrincipal()
        {
            return User.IsInRole("Super Admin") || User.IsInRole("Admin") || User.IsInRole("Principal") || User.IsInRole("Assistant Head");
        }

        private async Task<bool> CanAccessEmployeeAsync(int employeeId, CancellationToken ct)
        {
            if (IsAdminOrPrincipal()) return true;

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId)) return false;

            var ownEmployeeId = await _uow.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>().Query()
                .Where(e => e.UserId == userId && !e.IsDeleted)
                .Select(e => e.Id)
                .FirstOrDefaultAsync(ct);

            return ownEmployeeId > 0 && ownEmployeeId == employeeId;
        }

        private async Task<int?> GetLoggedInEmployeeIdAsync(CancellationToken ct)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId)) return null;

            var employeeId = await _uow.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>().Query()
                .Where(e => e.UserId == userId && !e.IsDeleted)
                .Select(e => e.Id)
                .FirstOrDefaultAsync(ct);

            return employeeId > 0 ? employeeId : null;
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
            string? departmentId = null,
            string? designationId = null,
            string? employeeType = null,
            string? isTeachingStaff = null,
            CancellationToken ct = default)
        {
            if (!IsAdminOrPrincipal()) return Forbid();

            var filter = new EmployeeAttendanceFilterDto
            {
                AttendanceDate = (attendanceDate ?? DateTime.Today).Date,
                DepartmentId = int.TryParse(departmentId, out var dId) ? dId : null,
                DesignationId = int.TryParse(designationId, out var desId) ? desId : null,
                EmployeeType = string.IsNullOrEmpty(employeeType) || employeeType == "null" ? null : employeeType,
                IsTeachingStaff = bool.TryParse(isTeachingStaff, out var its) ? its : null
            };

            try
            {
                var result = await _service.LoadAttendanceAsync(filter, page, size, ct);
                return Json(new
                {
                    data = result.Data,
                    last_page = Math.Max(1, (int)Math.Ceiling((double)result.TotalRecords / size)),
                    total_records = result.TotalRecords,
                    summary = result.Summary
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, inner = ex.InnerException?.Message });
            }
        }
        [HttpGet]
        [RequirePermission("Attendance.View")]
        public async Task<IActionResult> GetPagedData(int page = 1, int size = 10, string? date = null, CancellationToken ct = default)
        {
            if (!IsAdminOrPrincipal()) return Forbid();

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
            if (!IsAdminOrPrincipal()) return Forbid();

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
            if (!await CanAccessEmployeeAsync(dto.EmployeeId, ct)) return Forbid();

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
            if (!IsAdminOrPrincipal()) return Forbid();

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
            if (!await CanAccessEmployeeAsync(employeeId, ct)) return Forbid();

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
            if (!await CanAccessEmployeeAsync(employeeId, ct)) return Forbid();

            var today = DateTime.Today;
            var summary = await _service.GetMonthlySummaryAsync(employeeId, year ?? today.Year, month ?? today.Month, ct);
            return Json(summary);
        }

        [HttpGet]
        public async Task<IActionResult> MyAttendance(CancellationToken ct)
        {
            var employeeId = await GetLoggedInEmployeeIdAsync(ct);
            if (employeeId == null)
            {
                return NotFound("Employee profile not found.");
            }

            var today = DateTime.Today;
            var summary = await _service.GetMonthlySummaryAsync(employeeId.Value, today.Year, today.Month, ct);
            return View(summary);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyAttendanceData(int? year = null, int? month = null, CancellationToken ct = default)
        {
            var employeeId = await GetLoggedInEmployeeIdAsync(ct);
            if (employeeId == null)
            {
                return Json(new { success = false, message = "Employee profile not found." });
            }

            var today = DateTime.Today;
            var y = year ?? today.Year;
            var m = month ?? today.Month;
            var history = await _service.GetAttendanceHistoryAsync(employeeId.Value, y, m, ct);
            var summary = await _service.GetMonthlySummaryAsync(employeeId.Value, y, m, ct);
            var todayRecord = history.FirstOrDefault(h => h.AttendanceDate.Date == today);

            return Json(new
            {
                success = true,
                summary,
                history = history.Select(h => new
                {
                    date = h.AttendanceDate.ToString("yyyy-MM-dd"),
                    formattedDate = h.AttendanceDate.ToString("dd MMM yyyy"),
                    status = h.Status,
                    statusName = h.StatusName,
                    checkInTime = h.CheckInTime?.ToString(@"hh\:mm") ?? "",
                    checkOutTime = h.CheckOutTime?.ToString(@"hh\:mm") ?? "",
                    remarks = h.Remarks
                }),
                todayStatus = todayRecord?.StatusName ?? "Not Marked"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Attendance.Create")]
        public async Task<IActionResult> CheckIn([FromBody] EmployeeAttendanceDto dto, CancellationToken ct)
        {
            if (!await CanAccessEmployeeAsync(dto.EmployeeId, ct)) return Forbid();

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
            if (!await CanAccessEmployeeAsync(dto.EmployeeId, ct)) return Forbid();

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

        [HttpGet]
        [RequirePermission("Attendance.View")]
        public async Task<IActionResult> ExportEmployeeAttendanceExcel(DateTime fromDate, DateTime toDate, CancellationToken ct = default)
        {
            if (!IsAdminOrPrincipal()) return Forbid();

            var filter = new EmployeeAttendanceFilterDto
            {
                AttendanceDate = fromDate.Date,
                Page = 1,
                PageSize = 10000
            };
            var result = await _service.LoadAttendanceAsync(filter, 1, 10000, ct);
            var data = result.Data;

            var rows = new List<string[]>
            {
                new[] { "Employee Code", "Name", "Department", "Designation", "Date", "Check-In", "Check-Out", "Status", "Remarks" }
            };
            foreach (var a in data)
            {
                rows.Add(new[]
                {
                    a.EmployeeCode,
                    a.EmployeeName,
                    a.Department,
                    a.Designation,
                    a.AttendanceDate.ToString("yyyy-MM-dd"),
                    a.CheckInTime?.ToString(@"hh\:mm") ?? string.Empty,
                    a.CheckOutTime?.ToString(@"hh\:mm") ?? string.Empty,
                    a.StatusName,
                    a.Remarks ?? string.Empty
                });
            }

            var xlsx = SimpleExcelWriter.WriteWorkbook("Employee Attendance", rows);
            return File(xlsx, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"employee_attendance_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.xlsx");
        }
    }
}
