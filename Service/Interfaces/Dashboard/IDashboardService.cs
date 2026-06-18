using SchoolManagementSystem.Models.ViewModels.Dashboard;

namespace SchoolManagementSystem.Service.Interfaces.Dashboard;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<StudentDashboardViewModel> GetStudentDashboardAsync(int userId, CancellationToken cancellationToken = default);
    Task<TeacherDashboardViewModel> GetTeacherDashboardAsync(int userId, CancellationToken cancellationToken = default);
    Task<GuardianDashboardViewModel> GetGuardianDashboardAsync(int userId, CancellationToken cancellationToken = default);
    Task<ExamControllerDashboardViewModel> GetExamControllerDashboardAsync(CancellationToken cancellationToken = default);
}
