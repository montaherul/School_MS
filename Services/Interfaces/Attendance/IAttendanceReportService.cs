using System.Threading;
using System.Threading.Tasks;
using SchoolManagementSystem.Models.ViewModels.Attendance;

namespace SchoolManagementSystem.Services.Interfaces.Attendance
{
    public interface IAttendanceReportService
    {
        Task<AttendanceDashboardVm> GetDashboardSummaryAsync(CancellationToken ct = default);
        Task<byte[]> GenerateStudentMonthlyPdfAsync(int classId, int sectionId, int year, int month, CancellationToken ct = default, int? studentGroupId = null);
        Task<byte[]> GenerateEmployeeMonthlyPdfAsync(int year, int month, CancellationToken ct = default);
        Task<byte[]> GenerateStudentAttendancePdfAsync(int classId, int sectionId, DateTime fromDate, DateTime toDate, CancellationToken ct = default, int? studentGroupId = null);
        Task<byte[]> GenerateStudentYearlyPdfAsync(int classId, int sectionId, int year, CancellationToken ct = default, int? studentGroupId = null);
        Task<byte[]> GenerateEmployeeYearlyPdfAsync(int year, CancellationToken ct = default);
        Task<byte[]> GenerateClassAttendancePdfAsync(int classId, DateTime fromDate, DateTime toDate, CancellationToken ct = default);
        Task<byte[]> GenerateSectionAttendancePdfAsync(int classId, int sectionId, DateTime fromDate, DateTime toDate, CancellationToken ct = default);
        Task<byte[]> GenerateGroupAttendancePdfAsync(int classId, int sectionId, int studentGroupId, DateTime fromDate, DateTime toDate, CancellationToken ct = default);
    }
}
