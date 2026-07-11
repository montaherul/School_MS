using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.ViewModels.Attendance;
using SchoolManagementSystem.Repositories.Interfaces.Attendance;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Attendance
{
    public class AttendanceReportService : IAttendanceReportService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILeaveApplicationRepository _leaveRepo;

        public AttendanceReportService(
            IUnitOfWork uow,
            ILeaveApplicationRepository leaveRepo)
        {
            _uow = uow;
            _leaveRepo = leaveRepo;
        }

        public async Task<AttendanceDashboardVm> GetDashboardSummaryAsync(CancellationToken ct = default)
        {
            var summary = await _uow.Repository<AttendanceRecord>()
                .ExecuteStoredProcAsync<DashboardAttendanceSummaryDto>("sp_GetAttendanceDashboardSummary");

            var data = summary.FirstOrDefault() ?? new DashboardAttendanceSummaryDto();

            var pendingLeaves = await _leaveRepo.Query().CountAsync(
                l => l.ApprovalStatus == LeaveStatus.Pending, ct);

            return new AttendanceDashboardVm
            {
                TotalPresentStudents = data.StudentPresent + data.StudentLate,
                TotalAbsentStudents = data.StudentAbsent,
                TotalPresentEmployees = data.EmployeePresent + data.EmployeeLate,
                TotalAbsentEmployees = data.EmployeeAbsent,
                PendingLeaveRequests = pendingLeaves,
                StudentAttendancePercentage = data.TotalStudents > 0
                    ? Math.Round((double)(data.StudentPresent + data.StudentLate) / data.TotalStudents * 100, 2)
                    : 0,
                EmployeeAttendancePercentage = data.TotalEmployees > 0
                    ? Math.Round((double)(data.EmployeePresent + data.EmployeeLate) / data.TotalEmployees * 100, 2)
                    : 0
            };
        }

        public async Task<byte[]> GenerateStudentMonthlyPdfAsync(int classId, int sectionId, int year, int month, CancellationToken ct = default, int? studentGroupId = null)
            => await GenerateStudentAttendancePdfAsync(
                classId,
                sectionId,
                new DateTime(year, month, 1),
                new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                ct,
                studentGroupId);

        public async Task<byte[]> GenerateStudentYearlyPdfAsync(int classId, int sectionId, int year, CancellationToken ct = default, int? studentGroupId = null)
            => await GenerateStudentAttendancePdfAsync(
                classId,
                sectionId,
                new DateTime(year, 1, 1),
                new DateTime(year, 12, 31),
                ct,
                studentGroupId);

        public async Task<byte[]> GenerateStudentAttendancePdfAsync(int classId, int sectionId, DateTime fromDate, DateTime toDate, CancellationToken ct = default, int? studentGroupId = null)
        {
            var from = DateOnly.FromDateTime(fromDate.Date);
            var to = DateOnly.FromDateTime(toDate.Date);
            var query = _uow.Repository<AttendanceRecord>().Query()
                .AsNoTracking()
                .Include(a => a.Student).ThenInclude(s => s!.Class)
                .Include(a => a.Student).ThenInclude(s => s!.Section)
                .Include(a => a.Student).ThenInclude(s => s!.StudentGroup)
                .Where(a => !a.IsDeleted
                    && a.SchoolClassId == classId
                    && a.SectionId == sectionId
                    && a.AttendanceDate >= from
                    && a.AttendanceDate <= to);

            if (studentGroupId.HasValue)
            {
                query = query.Where(a => a.Student != null && a.Student.StudentGroupId == studentGroupId.Value);
            }

            var rows = await query
                .OrderBy(a => a.AttendanceDate)
                .ThenBy(a => a.Student != null ? a.Student.RollNumber : 0)
                .Select(a => new
                {
                    a.AttendanceDate,
                    a.Status,
                    a.Remarks,
                    StudentNo = a.Student != null ? a.Student.StudentNo : "",
                    StudentName = a.Student != null ? a.Student.FullName : "",
                    RollNumber = a.Student != null ? a.Student.RollNumber.ToString() : "",
                    ClassName = a.Student != null && a.Student.Class != null ? a.Student.Class.Name : "",
                    SectionName = a.Student != null && a.Student.Section != null ? a.Student.Section.Name : "",
                    GroupName = a.Student != null && a.Student.StudentGroup != null ? a.Student.StudentGroup.Name : ""
                })
                .ToListAsync(ct);

            return BuildPdf(
                "Student Attendance Report",
                $"{from:yyyy-MM-dd} to {to:yyyy-MM-dd}",
                new[] { "Date", "Student No", "Name", "Roll", "Class", "Section", "Group", "Status", "Remarks" },
                rows.Select(r => new[]
                {
                    r.AttendanceDate.ToString("yyyy-MM-dd"),
                    r.StudentNo,
                    r.StudentName,
                    r.RollNumber,
                    r.ClassName,
                    r.SectionName,
                    r.GroupName,
                    r.Status.ToString(),
                    r.Remarks ?? ""
                }));
        }

        public async Task<byte[]> GenerateEmployeeMonthlyPdfAsync(int year, int month, CancellationToken ct = default)
            => await GenerateEmployeePdfAsync(
                new DateTime(year, month, 1),
                new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                "Employee Attendance Report",
                ct);

        public async Task<byte[]> GenerateEmployeeYearlyPdfAsync(int year, CancellationToken ct = default)
            => await GenerateEmployeePdfAsync(
                new DateTime(year, 1, 1),
                new DateTime(year, 12, 31),
                "Yearly Employee Attendance Report",
                ct);

        public async Task<byte[]> GenerateClassAttendancePdfAsync(int classId, DateTime fromDate, DateTime toDate, CancellationToken ct = default)
        {
            var from = DateOnly.FromDateTime(fromDate.Date);
            var to = DateOnly.FromDateTime(toDate.Date);
            var className = await _uow.Repository<SchoolClass>().Query().AsNoTracking()
                .Where(c => c.Id == classId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync(ct) ?? classId.ToString();

            return await GenerateStudentScopeSummaryPdfAsync(
                $"Class Attendance Report - {className}",
                from,
                to,
                _uow.Repository<AttendanceRecord>().Query().AsNoTracking().Where(a => a.SchoolClassId == classId),
                ct);
        }

        public async Task<byte[]> GenerateSectionAttendancePdfAsync(int classId, int sectionId, DateTime fromDate, DateTime toDate, CancellationToken ct = default)
        {
            var from = DateOnly.FromDateTime(fromDate.Date);
            var to = DateOnly.FromDateTime(toDate.Date);
            var sectionName = await _uow.Repository<Section>().Query().AsNoTracking()
                .Where(s => s.Id == sectionId)
                .Select(s => s.Name)
                .FirstOrDefaultAsync(ct) ?? sectionId.ToString();

            return await GenerateStudentScopeSummaryPdfAsync(
                $"Section Attendance Report - {sectionName}",
                from,
                to,
                _uow.Repository<AttendanceRecord>().Query().AsNoTracking().Where(a => a.SchoolClassId == classId && a.SectionId == sectionId),
                ct);
        }

        public async Task<byte[]> GenerateGroupAttendancePdfAsync(int classId, int sectionId, int studentGroupId, DateTime fromDate, DateTime toDate, CancellationToken ct = default)
        {
            var from = DateOnly.FromDateTime(fromDate.Date);
            var to = DateOnly.FromDateTime(toDate.Date);
            var groupName = await _uow.Repository<StudentGroup>().Query().AsNoTracking()
                .Where(g => g.Id == studentGroupId)
                .Select(g => g.Name)
                .FirstOrDefaultAsync(ct) ?? studentGroupId.ToString();

            return await GenerateStudentScopeSummaryPdfAsync(
                $"Group Attendance Report - {groupName}",
                from,
                to,
                _uow.Repository<AttendanceRecord>().Query().AsNoTracking()
                    .Include(a => a.Student)
                    .Where(a => a.SchoolClassId == classId
                        && a.SectionId == sectionId
                        && a.Student != null
                        && a.Student.StudentGroupId == studentGroupId),
                ct);
        }

        private async Task<byte[]> GenerateEmployeePdfAsync(DateTime fromDate, DateTime toDate, string title, CancellationToken ct)
        {
            var rows = await _uow.Repository<EmployeeAttendance>().Query()
                .AsNoTracking()
                .Include(a => a.Employee).ThenInclude(e => e!.Department)
                .Include(a => a.Employee).ThenInclude(e => e!.Designation)
                .Where(a => !a.IsDeleted
                    && a.AttendanceDate.Date >= fromDate.Date
                    && a.AttendanceDate.Date <= toDate.Date)
                .OrderBy(a => a.AttendanceDate)
                .ThenBy(a => a.Employee != null ? a.Employee.EmployeeCode : "")
                .Select(a => new
                {
                    a.AttendanceDate,
                    a.Status,
                    a.CheckInTime,
                    a.CheckOutTime,
                    a.Remarks,
                    EmployeeCode = a.Employee != null ? a.Employee.EmployeeCode : "",
                    EmployeeName = a.Employee != null ? a.Employee.FullName : "",
                    Department = a.Employee != null && a.Employee.Department != null ? a.Employee.Department.Name : "",
                    Designation = a.Employee != null && a.Employee.Designation != null ? a.Employee.Designation.Name : ""
                })
                .ToListAsync(ct);

            return BuildPdf(
                title,
                $"{fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}",
                new[] { "Date", "Employee No", "Name", "Department", "Designation", "In", "Out", "Status", "Remarks" },
                rows.Select(r => new[]
                {
                    r.AttendanceDate.ToString("yyyy-MM-dd"),
                    r.EmployeeCode,
                    r.EmployeeName,
                    r.Department,
                    r.Designation,
                    r.CheckInTime?.ToString(@"hh\:mm") ?? "",
                    r.CheckOutTime?.ToString(@"hh\:mm") ?? "",
                    r.Status.ToString(),
                    r.Remarks ?? ""
                }));
        }

        private async Task<byte[]> GenerateStudentScopeSummaryPdfAsync(
            string title,
            DateOnly from,
            DateOnly to,
            IQueryable<SchoolManagementSystem.Models.Entities.Attendance.AttendanceRecord> query,
            CancellationToken ct)
        {
            var rows = await query
                .Where(a => !a.IsDeleted && a.AttendanceDate >= from && a.AttendanceDate <= to)
                .Include(a => a.Student).ThenInclude(s => s!.Class)
                .Include(a => a.Student).ThenInclude(s => s!.Section)
                .Include(a => a.Student).ThenInclude(s => s!.StudentGroup)
                .GroupBy(a => new
                {
                    a.StudentId,
                    StudentNo = a.Student != null ? a.Student.StudentNo : "",
                    StudentName = a.Student != null ? a.Student.FullName : "",
                    RollNumber = a.Student != null ? a.Student.RollNumber.ToString() : "",
                    ClassName = a.Student != null && a.Student.Class != null ? a.Student.Class.Name : "",
                    SectionName = a.Student != null && a.Student.Section != null ? a.Student.Section.Name : "",
                    GroupName = a.Student != null && a.Student.StudentGroup != null ? a.Student.StudentGroup.Name : ""
                })
                .Select(g => new
                {
                    g.Key.StudentNo,
                    g.Key.StudentName,
                    g.Key.RollNumber,
                    g.Key.ClassName,
                    g.Key.SectionName,
                    g.Key.GroupName,
                    Total = g.Count(),
                    Present = g.Count(a => a.Status == AttendanceStatus.Present),
                    Late = g.Count(a => a.Status == AttendanceStatus.Late),
                    Absent = g.Count(a => a.Status == AttendanceStatus.Absent),
                    Leave = g.Count(a => a.Status == AttendanceStatus.Leave)
                })
                .OrderBy(r => r.ClassName)
                .ThenBy(r => r.SectionName)
                .ThenBy(r => r.RollNumber)
                .ToListAsync(ct);

            return BuildPdf(
                title,
                $"{from:yyyy-MM-dd} to {to:yyyy-MM-dd}",
                new[] { "Student No", "Name", "Roll", "Class", "Section", "Group", "Total", "Present", "Late", "Absent", "Leave", "%" },
                rows.Select(r =>
                {
                    var pct = r.Total == 0 ? 0 : Math.Round((double)(r.Present + r.Late) / r.Total * 100, 2);
                    return new[]
                    {
                        r.StudentNo,
                        r.StudentName,
                        r.RollNumber,
                        r.ClassName,
                        r.SectionName,
                        r.GroupName,
                        r.Total.ToString(),
                        r.Present.ToString(),
                        r.Late.ToString(),
                        r.Absent.ToString(),
                        r.Leave.ToString(),
                        pct.ToString("0.##")
                    };
                }));
        }

        private static byte[] BuildPdf(string title, string subtitle, string[] headers, IEnumerable<string[]> rows)
        {
            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf, PageSize.A4.Rotate());
            document.SetMargins(24, 24, 24, 24);

            document.Add(new Paragraph(title)
                .SetBold()
                .SetFontSize(16)
                .SetTextAlignment(TextAlignment.CENTER));

            document.Add(new Paragraph(subtitle)
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.CENTER));

            var table = new Table(UnitValue.CreatePercentArray(headers.Length)).UseAllAvailableWidth();
            foreach (var header in headers)
            {
                table.AddHeaderCell(new Cell()
                    .Add(new Paragraph(header).SetBold().SetFontColor(ColorConstants.WHITE))
                    .SetBackgroundColor(ColorConstants.BLUE)
                    .SetFontSize(8));
            }

            var rowCount = 0;
            foreach (var row in rows)
            {
                foreach (var value in row)
                {
                    table.AddCell(new Cell().Add(new Paragraph(value)).SetFontSize(8));
                }
                rowCount++;
            }

            if (rowCount == 0)
            {
                table.AddCell(new Cell(1, headers.Length)
                    .Add(new Paragraph("No attendance records found."))
                    .SetTextAlignment(TextAlignment.CENTER));
            }

            document.Add(table);
            document.Close();
            return stream.ToArray();
        }
    }
}
