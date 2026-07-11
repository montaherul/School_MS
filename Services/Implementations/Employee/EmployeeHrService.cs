using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Employee;

public class EmployeeHrService : IEmployeeHrService
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<EmployeeHrService> _logger;

    public EmployeeHrService(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor, ILogger<EmployeeHrService> logger)
    {
        _uow = uow;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    // ── Bank Accounts ──
    public async Task<List<EmployeeBankAccountDto>> GetBankAccountsAsync(int employeeId, CancellationToken ct)
    {
        var accounts = await _uow.Repository<EmployeeBankAccount>()
            .Query()
            .Where(b => b.EmployeeId == employeeId && !b.IsDeleted)
            .Select(b => new EmployeeBankAccountDto
            {
                Id = b.Id,
                EmployeeId = b.EmployeeId,
                BankName = b.BankName,
                BranchName = b.BranchName,
                AccountNumber = b.AccountNumber,
                RoutingNumber = b.RoutingNumber,
                AccountType = b.AccountType,
                IsDefault = b.IsDefault,
                IsActive = b.IsActive
            })
            .AsNoTracking()
            .ToListAsync(ct);
        return accounts;
    }

    public async Task SaveBankAccountAsync(EmployeeBankAccountDto dto, CancellationToken ct)
    {
        var repo = _uow.Repository<EmployeeBankAccount>();
        if (dto.Id > 0)
        {
            var entity = await repo.GetByIdAsync(dto.Id, ct);
            if (entity != null)
            {
                entity.BankName = dto.BankName;
                entity.BranchName = dto.BranchName;
                entity.AccountNumber = dto.AccountNumber;
                entity.RoutingNumber = dto.RoutingNumber;
                entity.AccountType = dto.AccountType;
                entity.IsDefault = dto.IsDefault;
                entity.IsActive = dto.IsActive;
                entity.UpdatedBy = GetCurrentUserName();
                entity.UpdatedAt = DateTime.UtcNow;
                repo.Update(entity);
            }
        }
        else
        {
            if (dto.IsDefault)
            {
                var allAccounts = await _uow.Repository<EmployeeBankAccount>()
                    .Query().Where(b => b.EmployeeId == dto.EmployeeId && !b.IsDeleted).ToListAsync(ct);
                foreach (var a in allAccounts) a.IsDefault = false;
            }
            await repo.AddAsync(new EmployeeBankAccount
            {
                EmployeeId = dto.EmployeeId,
                BankName = dto.BankName,
                BranchName = dto.BranchName,
                AccountNumber = dto.AccountNumber,
                RoutingNumber = dto.RoutingNumber,
                AccountType = dto.AccountType,
                IsDefault = dto.IsDefault,
                IsActive = dto.IsActive,
                CreatedBy = GetCurrentUserName(),
                CreatedAt = DateTime.UtcNow
            }, ct);
        }
        await _uow.SaveChangesAsync(ct);
        await LogAuditAsync("Employee.BankAccount", dto.Id > 0 ? "Update" : "Create", dto.EmployeeId.ToString(), $"Bank account {(dto.Id > 0 ? "updated" : "created")}: {dto.BankName} - {dto.AccountNumber}", ct);
    }

    public async Task DeleteBankAccountAsync(int id, CancellationToken ct)
    {
        var entity = await _uow.Repository<EmployeeBankAccount>().GetByIdAsync(id, ct);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.UpdatedBy = GetCurrentUserName();
            entity.UpdatedAt = DateTime.UtcNow;
            _uow.Repository<EmployeeBankAccount>().Update(entity);
            await _uow.SaveChangesAsync(ct);
            await LogAuditAsync("Employee.BankAccount", "Delete", entity.EmployeeId.ToString(), $"Bank account deleted: {entity.BankName} - {entity.AccountNumber}", ct);
        }
    }

    // ── Promotions ──
    public async Task<List<EmployeePromotionDto>> GetPromotionsAsync(int employeeId, CancellationToken ct)
    {
        var repo = _uow.Repository<EmployeePromotion>();
        var q = repo.Query().Where(p => p.EmployeeId == employeeId && !p.IsDeleted).OrderByDescending(p => p.PromotionDate);
        var items = await q.Select(p => new EmployeePromotionDto
        {
            Id = p.Id, EmployeeId = p.EmployeeId, PreviousDesignationId = p.PreviousDesignationId, NewDesignationId = p.NewDesignationId,
            Reason = p.Reason, PromotionDate = p.PromotionDate, PreviousSalary = p.PreviousSalary, NewSalary = p.NewSalary, Remarks = p.Remarks
        }).AsNoTracking().ToListAsync(ct);
        return items;
    }

    public async Task SavePromotionAsync(EmployeePromotionDto dto, CancellationToken ct)
    {
        var repo = _uow.Repository<EmployeePromotion>();
        var userName = GetCurrentUserName();
        if (dto.Id > 0)
        {
            var entity = await repo.GetByIdAsync(dto.Id, ct);
            if (entity != null)
            {
                entity.PreviousDesignationId = dto.PreviousDesignationId; entity.NewDesignationId = dto.NewDesignationId;
                entity.Reason = dto.Reason; entity.PromotionDate = dto.PromotionDate;
                entity.PreviousSalary = dto.PreviousSalary; entity.NewSalary = dto.NewSalary; entity.Remarks = dto.Remarks;
                entity.UpdatedBy = userName; entity.UpdatedAt = DateTime.UtcNow;
                repo.Update(entity);
            }
        }
        else
        {
            await repo.AddAsync(new EmployeePromotion
            {
                EmployeeId = dto.EmployeeId, PreviousDesignationId = dto.PreviousDesignationId, NewDesignationId = dto.NewDesignationId,
                Reason = dto.Reason, PromotionDate = dto.PromotionDate, PreviousSalary = dto.PreviousSalary, NewSalary = dto.NewSalary, Remarks = dto.Remarks,
                CreatedBy = userName, CreatedAt = DateTime.UtcNow
            }, ct);
        }
        await _uow.SaveChangesAsync(ct);
        await LogAuditAsync("Employee.Promotion", dto.Id > 0 ? "Update" : "Create", dto.EmployeeId.ToString(), $"Promotion {(dto.Id > 0 ? "updated" : "created")} on {dto.PromotionDate:yyyy-MM-dd}", ct);
    }

    public async Task DeletePromotionAsync(int id, CancellationToken ct)
    {
        var entity = await _uow.Repository<EmployeePromotion>().GetByIdAsync(id, ct);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.UpdatedBy = GetCurrentUserName(); entity.UpdatedAt = DateTime.UtcNow;
            _uow.Repository<EmployeePromotion>().Update(entity);
            await _uow.SaveChangesAsync(ct);
            await LogAuditAsync("Employee.Promotion", "Delete", entity.EmployeeId.ToString(), $"Promotion deleted (date: {entity.PromotionDate:yyyy-MM-dd})", ct);
        }
    }

    // ── Transfers ──
    public async Task<List<EmployeeTransferDto>> GetTransfersAsync(int employeeId, CancellationToken ct)
    {
        var q = _uow.Repository<EmployeeTransfer>().Query().Where(t => t.EmployeeId == employeeId && !t.IsDeleted).OrderByDescending(t => t.TransferDate);
        return await q.Select(t => new EmployeeTransferDto
        {
            Id = t.Id, EmployeeId = t.EmployeeId, FromDepartmentId = t.FromDepartmentId, ToDepartmentId = t.ToDepartmentId,
            Reason = t.Reason, TransferDate = t.TransferDate, Remarks = t.Remarks
        }).AsNoTracking().ToListAsync(ct);
    }

    public async Task SaveTransferAsync(EmployeeTransferDto dto, CancellationToken ct)
    {
        var repo = _uow.Repository<EmployeeTransfer>();
        var userName = GetCurrentUserName();
        if (dto.Id > 0)
        {
            var entity = await repo.GetByIdAsync(dto.Id, ct);
            if (entity != null)
            {
                entity.FromDepartmentId = dto.FromDepartmentId; entity.ToDepartmentId = dto.ToDepartmentId;
                entity.Reason = dto.Reason; entity.TransferDate = dto.TransferDate; entity.Remarks = dto.Remarks;
                entity.UpdatedBy = userName; entity.UpdatedAt = DateTime.UtcNow;
                repo.Update(entity);
            }
        }
        else
        {
            await repo.AddAsync(new EmployeeTransfer
            {
                EmployeeId = dto.EmployeeId, FromDepartmentId = dto.FromDepartmentId, ToDepartmentId = dto.ToDepartmentId,
                Reason = dto.Reason, TransferDate = dto.TransferDate, Remarks = dto.Remarks,
                CreatedBy = userName, CreatedAt = DateTime.UtcNow
            }, ct);
        }
        await _uow.SaveChangesAsync(ct);
        await LogAuditAsync("Employee.Transfer", dto.Id > 0 ? "Update" : "Create", dto.EmployeeId.ToString(), $"Transfer {(dto.Id > 0 ? "updated" : "created")} on {dto.TransferDate:yyyy-MM-dd}", ct);
    }

    public async Task DeleteTransferAsync(int id, CancellationToken ct)
    {
        var entity = await _uow.Repository<EmployeeTransfer>().GetByIdAsync(id, ct);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.UpdatedBy = GetCurrentUserName(); entity.UpdatedAt = DateTime.UtcNow;
            _uow.Repository<EmployeeTransfer>().Update(entity);
            await _uow.SaveChangesAsync(ct);
            await LogAuditAsync("Employee.Transfer", "Delete", entity.EmployeeId.ToString(), $"Transfer deleted (date: {entity.TransferDate:yyyy-MM-dd})", ct);
        }
    }

    // ── Training ──
    public async Task<List<EmployeeTrainingDto>> GetTrainingsAsync(int employeeId, CancellationToken ct)
    {
        var q = _uow.Repository<EmployeeTraining>().Query().Where(t => t.EmployeeId == employeeId && !t.IsDeleted).OrderByDescending(t => t.StartDate);
        return await q.Select(t => new EmployeeTrainingDto
        {
            Id = t.Id, EmployeeId = t.EmployeeId, TrainingName = t.TrainingName, InstitutionName = t.InstitutionName,
            Duration = t.Duration, StartDate = t.StartDate, EndDate = t.EndDate, CertificatePath = t.CertificatePath, Remarks = t.Remarks
        }).AsNoTracking().ToListAsync(ct);
    }

    public async Task SaveTrainingAsync(EmployeeTrainingDto dto, CancellationToken ct)
    {
        var repo = _uow.Repository<EmployeeTraining>();
        var userName = GetCurrentUserName();
        if (dto.Id > 0)
        {
            var entity = await repo.GetByIdAsync(dto.Id, ct);
            if (entity != null)
            {
                entity.TrainingName = dto.TrainingName; entity.InstitutionName = dto.InstitutionName;
                entity.Duration = dto.Duration; entity.StartDate = dto.StartDate; entity.EndDate = dto.EndDate; entity.Remarks = dto.Remarks;
                entity.UpdatedBy = userName; entity.UpdatedAt = DateTime.UtcNow;
                repo.Update(entity);
            }
        }
        else
        {
            await repo.AddAsync(new EmployeeTraining
            {
                EmployeeId = dto.EmployeeId, TrainingName = dto.TrainingName, InstitutionName = dto.InstitutionName,
                Duration = dto.Duration, StartDate = dto.StartDate, EndDate = dto.EndDate, Remarks = dto.Remarks,
                CreatedBy = userName, CreatedAt = DateTime.UtcNow
            }, ct);
        }
        await _uow.SaveChangesAsync(ct);
        await LogAuditAsync("Employee.Training", dto.Id > 0 ? "Update" : "Create", dto.EmployeeId.ToString(), $"Training {(dto.Id > 0 ? "updated" : "created")}: {dto.TrainingName}", ct);
    }

    public async Task DeleteTrainingAsync(int id, CancellationToken ct)
    {
        var entity = await _uow.Repository<EmployeeTraining>().GetByIdAsync(id, ct);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.UpdatedBy = GetCurrentUserName(); entity.UpdatedAt = DateTime.UtcNow;
            _uow.Repository<EmployeeTraining>().Update(entity);
            await _uow.SaveChangesAsync(ct);
            await LogAuditAsync("Employee.Training", "Delete", entity.EmployeeId.ToString(), $"Training deleted: {entity.TrainingName}", ct);
        }
    }

    // ── Awards ──
    public async Task<List<EmployeeAwardDto>> GetAwardsAsync(int employeeId, CancellationToken ct)
    {
        var q = _uow.Repository<EmployeeAward>().Query().Where(a => a.EmployeeId == employeeId && !a.IsDeleted).OrderByDescending(a => a.AwardDate);
        return await q.Select(a => new EmployeeAwardDto
        {
            Id = a.Id, EmployeeId = a.EmployeeId, AwardName = a.AwardName, AwardedBy = a.AwardedBy,
            AwardDate = a.AwardDate, Description = a.Description, CertificatePath = a.CertificatePath
        }).AsNoTracking().ToListAsync(ct);
    }

    public async Task SaveAwardAsync(EmployeeAwardDto dto, CancellationToken ct)
    {
        var repo = _uow.Repository<EmployeeAward>();
        var userName = GetCurrentUserName();
        if (dto.Id > 0)
        {
            var entity = await repo.GetByIdAsync(dto.Id, ct);
            if (entity != null)
            {
                entity.AwardName = dto.AwardName; entity.AwardedBy = dto.AwardedBy; entity.AwardDate = dto.AwardDate;
                entity.Description = dto.Description;
                entity.UpdatedBy = userName; entity.UpdatedAt = DateTime.UtcNow;
                repo.Update(entity);
            }
        }
        else
        {
            await repo.AddAsync(new EmployeeAward
            {
                EmployeeId = dto.EmployeeId, AwardName = dto.AwardName, AwardedBy = dto.AwardedBy,
                AwardDate = dto.AwardDate, Description = dto.Description,
                CreatedBy = userName, CreatedAt = DateTime.UtcNow
            }, ct);
        }
        await _uow.SaveChangesAsync(ct);
        await LogAuditAsync("Employee.Award", dto.Id > 0 ? "Update" : "Create", dto.EmployeeId.ToString(), $"Award {(dto.Id > 0 ? "updated" : "created")}: {dto.AwardName}", ct);
    }

    public async Task DeleteAwardAsync(int id, CancellationToken ct)
    {
        var entity = await _uow.Repository<EmployeeAward>().GetByIdAsync(id, ct);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.UpdatedBy = GetCurrentUserName(); entity.UpdatedAt = DateTime.UtcNow;
            _uow.Repository<EmployeeAward>().Update(entity);
            await _uow.SaveChangesAsync(ct);
            await LogAuditAsync("Employee.Award", "Delete", entity.EmployeeId.ToString(), $"Award deleted: {entity.AwardName}", ct);
        }
    }

    // ── Disciplinary Actions ──
    public async Task<List<EmployeeDisciplinaryActionDto>> GetDisciplinaryActionsAsync(int employeeId, CancellationToken ct)
    {
        var q = _uow.Repository<EmployeeDisciplinaryAction>().Query().Where(d => d.EmployeeId == employeeId && !d.IsDeleted).OrderByDescending(d => d.ActionDate);
        return await q.Select(d => new EmployeeDisciplinaryActionDto
        {
            Id = d.Id, EmployeeId = d.EmployeeId, ActionType = d.ActionType, Reason = d.Reason,
            ActionDate = d.ActionDate, Description = d.Description, DocumentPath = d.DocumentPath,
            IsResolved = d.IsResolved, ResolvedAt = d.ResolvedAt, ResolutionRemarks = d.ResolutionRemarks
        }).AsNoTracking().ToListAsync(ct);
    }

    public async Task SaveDisciplinaryActionAsync(EmployeeDisciplinaryActionDto dto, CancellationToken ct)
    {
        var repo = _uow.Repository<EmployeeDisciplinaryAction>();
        var userName = GetCurrentUserName();
        if (dto.Id > 0)
        {
            var entity = await repo.GetByIdAsync(dto.Id, ct);
            if (entity != null)
            {
                entity.ActionType = dto.ActionType; entity.Reason = dto.Reason; entity.ActionDate = dto.ActionDate;
                entity.Description = dto.Description; entity.IsResolved = dto.IsResolved;
                entity.ResolvedAt = dto.ResolvedAt; entity.ResolutionRemarks = dto.ResolutionRemarks;
                entity.UpdatedBy = userName; entity.UpdatedAt = DateTime.UtcNow;
                repo.Update(entity);
            }
        }
        else
        {
            await repo.AddAsync(new EmployeeDisciplinaryAction
            {
                EmployeeId = dto.EmployeeId, ActionType = dto.ActionType, Reason = dto.Reason,
                ActionDate = dto.ActionDate, Description = dto.Description,
                CreatedBy = userName, CreatedAt = DateTime.UtcNow
            }, ct);
        }
        await _uow.SaveChangesAsync(ct);
        await LogAuditAsync("Employee.Disciplinary", dto.Id > 0 ? "Update" : "Create", dto.EmployeeId.ToString(), $"Disciplinary action {(dto.Id > 0 ? "updated" : "created")}: {dto.ActionType}", ct);
    }

    public async Task DeleteDisciplinaryActionAsync(int id, CancellationToken ct)
    {
        var entity = await _uow.Repository<EmployeeDisciplinaryAction>().GetByIdAsync(id, ct);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.UpdatedBy = GetCurrentUserName(); entity.UpdatedAt = DateTime.UtcNow;
            _uow.Repository<EmployeeDisciplinaryAction>().Update(entity);
            await _uow.SaveChangesAsync(ct);
            await LogAuditAsync("Employee.Disciplinary", "Delete", entity.EmployeeId.ToString(), $"Disciplinary action deleted: {entity.ActionType}", ct);
        }
    }

    public async Task ResolveDisciplinaryActionAsync(int id, string resolutionRemarks, CancellationToken ct)
    {
        var entity = await _uow.Repository<EmployeeDisciplinaryAction>().GetByIdAsync(id, ct);
        if (entity != null)
        {
            entity.IsResolved = true;
            entity.ResolvedAt = DateTime.UtcNow;
            entity.ResolutionRemarks = resolutionRemarks;
            entity.UpdatedBy = GetCurrentUserName();
            entity.UpdatedAt = DateTime.UtcNow;
            _uow.Repository<EmployeeDisciplinaryAction>().Update(entity);
            await _uow.SaveChangesAsync(ct);
            await LogAuditAsync("Employee.Disciplinary", "Resolve", entity.EmployeeId.ToString(), $"Disciplinary action resolved: {entity.ActionType}", ct);
        }
    }

    // ── Helpers ──

    private string GetCurrentUserName()
        => _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "system";

    private async Task LogAuditAsync(string module, string action, string entityId, string details, CancellationToken ct = default)
    {
        var httpContext = _httpContextAccessor?.HttpContext;
        var userIdStr = httpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        int? userId = userIdStr != null && int.TryParse(userIdStr, out var uid) ? uid : null;

        var log = new AuditLog
        {
            UserId = userId,
            Module = module,
            Action = action,
            IpAddress = httpContext?.Connection?.RemoteIpAddress?.ToString(),
            Details = details.Length > 1000 ? details[..1000] : details,
            CreatedBy = GetCurrentUserName(),
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<AuditLog>().AddAsync(log, ct);
        await _uow.SaveChangesAsync(ct);
    }
}
