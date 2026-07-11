using SchoolManagementSystem.Models.DTOs.Academic;

namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface IAcademicReportService
{
    Task<AcademicReportViewModel> GetReportAsync(AcademicReportFilterDto filter, CancellationToken ct = default);
    Task<byte[]> ExportPdfAsync(AcademicReportFilterDto filter, CancellationToken ct = default);
    Task<byte[]> ExportExcelAsync(AcademicReportFilterDto filter, CancellationToken ct = default);
}
