using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface ICashierCollectionService
{
    Task<List<StudentSearchResultDto>> SearchStudentsAsync(string searchTerm);
    Task<CashierCollectionDto> GetStudentCollectionDataAsync(int studentId);
    Task<CashierPaymentResultDto> ProcessPaymentAsync(int studentId, List<int> invoiceIds, CashierPaymentDto payment, string createdBy);
}
