using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.DTOs.Result;

namespace SchoolManagementSystem.Repositories.Interfaces.Result;

public interface IResultPublicationRepository : IBaseRepository<ResultPublication>
{
    Task<(List<PublicationDashboardExamDto> Exams, PublicationDashboardSummaryDto Summary)> GetPublicationDashboardAsync(int academicYearId, CancellationToken ct);
}