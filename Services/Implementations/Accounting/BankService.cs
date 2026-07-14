using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Accounting;
using SchoolManagementSystem.Repositories.Interfaces.Accounting;
using SchoolManagementSystem.Services.Interfaces.Accounting;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Services.Implementations.Accounting;

public class BankService : IBankService
{
    private readonly IUnitOfWork _uow;
    private readonly IBankTransactionRepository _repo;

    public BankService(IUnitOfWork uow, IBankTransactionRepository repo)
    {
        _uow = uow;
        _repo = repo;
    }

    public async Task<PagedResult<BankBookEntryDto>> GetBankBookAsync(int? accountId, int? bankType, DateTime? from, DateTime? to, int? periodId, int page, int pageSize, CancellationToken ct)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        var (items, total) = await _repo.GetBankBookAsync(accountId, bankType, from, to, periodId, page, pageSize, ct);
        return new PagedResult<BankBookEntryDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public async Task<BankBookSummaryDto> GetSummaryAsync(int? accountId, int? bankType, DateTime? from, DateTime? to, int? periodId, CancellationToken ct)
        => await _repo.GetBankBookSummaryAsync(accountId, bankType, from, to, periodId, ct);

    public async Task<int> CreateTransactionAsync(BankTransactionDto dto, string createdBy, CancellationToken ct)
    {
        var entity = new BankTransaction
        {
            CreatedBy = createdBy,
            AccountId = dto.AccountId,
            BankAccountType = (BankAccountType)dto.BankAccountType,
            TransactionDate = dto.TransactionDate,
            TransactionType = (BankTransactionType)dto.TransactionType,
            Amount = dto.Amount,
            ReferenceNo = dto.ReferenceNo,
            ChequeNo = dto.ChequeNo,
            Description = dto.Description,
            CounterParty = dto.CounterParty,
            FinancialPeriodId = dto.FinancialPeriodId
        };
        await _uow.Repository<BankTransaction>().AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task ReconcileAsync(string transactionIds, string reconciledBy, CancellationToken ct)
    {
        await _repo.ReconcileTransactionsAsync(transactionIds, reconciledBy, ct);
    }

    public Task<List<BankReconciliationDto>> GetUnreconciledAsync(int? accountId, CancellationToken ct)
        => _repo.GetUnreconciledAsync(accountId, ct);
}
