using SchoolManagementSystem.Models.DTOs.Employee;

namespace SchoolManagementSystem.Services.Interfaces.Employee;

public interface IEmployeeHrService
{
    // Bank Accounts
    Task<List<EmployeeBankAccountDto>> GetBankAccountsAsync(int employeeId, CancellationToken ct);
    Task SaveBankAccountAsync(EmployeeBankAccountDto dto, CancellationToken ct);
    Task DeleteBankAccountAsync(int id, CancellationToken ct);

    // Promotions
    Task<List<EmployeePromotionDto>> GetPromotionsAsync(int employeeId, CancellationToken ct);
    Task SavePromotionAsync(EmployeePromotionDto dto, CancellationToken ct);
    Task DeletePromotionAsync(int id, CancellationToken ct);

    // Transfers
    Task<List<EmployeeTransferDto>> GetTransfersAsync(int employeeId, CancellationToken ct);
    Task SaveTransferAsync(EmployeeTransferDto dto, CancellationToken ct);
    Task DeleteTransferAsync(int id, CancellationToken ct);

    // Training
    Task<List<EmployeeTrainingDto>> GetTrainingsAsync(int employeeId, CancellationToken ct);
    Task SaveTrainingAsync(EmployeeTrainingDto dto, CancellationToken ct);
    Task DeleteTrainingAsync(int id, CancellationToken ct);

    // Awards
    Task<List<EmployeeAwardDto>> GetAwardsAsync(int employeeId, CancellationToken ct);
    Task SaveAwardAsync(EmployeeAwardDto dto, CancellationToken ct);
    Task DeleteAwardAsync(int id, CancellationToken ct);

    // Disciplinary Actions
    Task<List<EmployeeDisciplinaryActionDto>> GetDisciplinaryActionsAsync(int employeeId, CancellationToken ct);
    Task SaveDisciplinaryActionAsync(EmployeeDisciplinaryActionDto dto, CancellationToken ct);
    Task DeleteDisciplinaryActionAsync(int id, CancellationToken ct);
    Task ResolveDisciplinaryActionAsync(int id, string resolutionRemarks, CancellationToken ct);
}
