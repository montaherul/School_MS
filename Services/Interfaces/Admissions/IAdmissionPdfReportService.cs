using SchoolManagementSystem.Models.DTOs.Admission;

namespace SchoolManagementSystem.Services.Interfaces.Admissions;

public interface IAdmissionPdfReportService
{
    Task<byte[]> GenerateRegisterReportPdfAsync(DateTime? fromDate, DateTime? toDate, int? classId, int? statusId);
    Task<byte[]> GenerateConversionFunnelPdfAsync(int? academicYearId);
}
