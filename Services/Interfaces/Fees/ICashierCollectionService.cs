using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface ICashierCollectionService
{
    Task<List<StudentSearchResultDto>> SearchStudentsAsync(string searchTerm, CancellationToken ct = default);
    Task<CashierCollectionDto> GetStudentCollectionDataAsync(int studentId, CancellationToken ct = default);
    Task<CashierPaymentResultDto> ProcessPaymentAsync(int studentId, List<int> invoiceIds, CashierPaymentDto payment, string createdBy, CancellationToken ct = default);
}
