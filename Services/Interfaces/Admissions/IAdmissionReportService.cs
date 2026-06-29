using SchoolManagementSystem.Models.DTOs.Admission;

namespace SchoolManagementSystem.Services.Interfaces.Admissions;

public interface IAdmissionReportService
{
    Task<AdmissionRegisterReportDto> GetRegisterReportAsync(AdmissionReportRequest request, CancellationToken ct = default);
    Task<byte[]> ExportRegisterToExcelAsync(AdmissionReportRequest request, CancellationToken ct = default);
    Task<byte[]> ExportRegisterToPdfAsync(AdmissionReportRequest request, CancellationToken ct = default);
    Task<ConversionFunnelDto> GetConversionFunnelAsync(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default);
    Task<CollectionReportDto> GetCollectionReportAsync(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default);
}
