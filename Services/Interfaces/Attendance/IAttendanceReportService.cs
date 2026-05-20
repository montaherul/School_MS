using System.Threading;
using System.Threading.Tasks;
using SchoolManagementSystem.Models.ViewModels.Attendance;

namespace SchoolManagementSystem.Services.Interfaces.Attendance
{
    public interface IAttendanceReportService
    {
        Task<AttendanceDashboardVm> GetDashboardSummaryAsync(CancellationToken ct = default);
        Task<byte[]> GenerateStudentMonthlyPdfAsync(int classId, int sectionId, int year, int month, CancellationToken ct = default);
        Task<byte[]> GenerateEmployeeMonthlyPdfAsync(int year, int month, CancellationToken ct = default);
    }
}
