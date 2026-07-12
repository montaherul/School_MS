using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.DTOs.Exam;

namespace SchoolManagementSystem.Repositories.Interfaces.Result;

public interface IExamRepository : IBaseRepository<Exam>
{
    Task<IEnumerable<ExamListDto>> GetExamsForAdminAsync(int academicYearId, CancellationToken ct);
    Task<ExamDashboardDto> GetDashboardDataAsync(int academicYearId, CancellationToken ct);
    Task<List<ExamStatusDistributionDto>> GetStatusDistributionAsync(int academicYearId, CancellationToken ct);
    Task<List<ExamPassRateDto>> GetExamPassRatesAsync(int academicYearId, CancellationToken ct);
    Task<(IEnumerable<ExamListDto> Items, int TotalCount)> GetExamListAsync(int academicYearId, string? searchTerm, int? status, int pageNumber, int pageSize, string sortColumn, string sortDirection, CancellationToken ct);
    Task<ExamDetailsDto?> GetExamDetailsAsync(int examId, CancellationToken ct);
    Task<ExamReadinessReportDto> GetExamReadinessReportAsync(int academicYearId, CancellationToken ct);
}