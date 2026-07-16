using SchoolManagementSystem.Models.DTOs.SchoolPay;

namespace SchoolManagementSystem.Services.Interfaces.SchoolPay;

public interface IOperationsCenterService
{
    Task<SchoolPayOperationsDto> GetOperationsDataAsync(CancellationToken ct = default);
}
