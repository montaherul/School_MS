using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Employee;

public class EmployeePayrollService : IEmployeePayrollService
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<EmployeePayrollService> _logger;

    public EmployeePayrollService(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor, ILogger<EmployeePayrollService> logger)
    {
        _uow = uow;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<List<EmployeeSalaryDto>> GetSalariesByEmployeeIdAsync(int employeeId, CancellationToken ct)
    {
        var salaries = await _uow.Repository<EmployeeSalary>()
            .Query()
            .Include(s => s.Employee)
            .Where(s => s.EmployeeId == employeeId && !s.IsDeleted)
            .OrderByDescending(s => s.EffectiveFrom)
            .Select(s => new EmployeeSalaryDto
            {
                Id = s.Id,
                EmployeeId = s.EmployeeId,
                EmployeeName = s.Employee != null ? s.Employee.FullName : null,
                BasicSalary = s.BasicSalary,
                HouseRent = s.HouseRent,
                MedicalAllowance = s.MedicalAllowance,
                TransportAllowance = s.TransportAllowance,
                OtherAllowance = s.OtherAllowance,
                Deduction = s.Deduction,
                TotalSalary = s.TotalSalary,
                EffectiveFrom = s.EffectiveFrom
            })
            .AsNoTracking()
            .ToListAsync(ct);

        return salaries;
    }

    public async Task<EmployeeSalaryDto?> GetSalaryByIdAsync(int id, CancellationToken ct)
    {
        var s = await _uow.Repository<EmployeeSalary>()
            .Query()
            .Include(s => s.Employee)
            .Where(s => s.Id == id && !s.IsDeleted)
            .Select(s => new EmployeeSalaryDto
            {
                Id = s.Id,
                EmployeeId = s.EmployeeId,
                EmployeeName = s.Employee != null ? s.Employee.FullName : null,
                BasicSalary = s.BasicSalary,
                HouseRent = s.HouseRent,
                MedicalAllowance = s.MedicalAllowance,
                TransportAllowance = s.TransportAllowance,
                OtherAllowance = s.OtherAllowance,
                Deduction = s.Deduction,
                TotalSalary = s.TotalSalary,
                EffectiveFrom = s.EffectiveFrom
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        return s;
    }

    public async Task SaveSalaryAsync(EmployeeSalaryDto dto, CancellationToken ct)
    {
        var repo = _uow.Repository<EmployeeSalary>();
        var userName = GetCurrentUserName();

        if (dto.Id > 0)
        {
            var entity = await repo.GetByIdAsync(dto.Id, ct);
            if (entity != null)
            {
                entity.BasicSalary = dto.BasicSalary;
                entity.HouseRent = dto.HouseRent;
                entity.MedicalAllowance = dto.MedicalAllowance;
                entity.TransportAllowance = dto.TransportAllowance;
                entity.OtherAllowance = dto.OtherAllowance;
                entity.Deduction = dto.Deduction;
                entity.TotalSalary = dto.BasicSalary + dto.HouseRent + dto.MedicalAllowance + dto.TransportAllowance + dto.OtherAllowance - dto.Deduction;
                entity.EffectiveFrom = dto.EffectiveFrom;
                entity.UpdatedBy = userName;
                entity.UpdatedAt = DateTime.UtcNow;
                repo.Update(entity);
            }
        }
        else
        {
            var entity = new EmployeeSalary
            {
                EmployeeId = dto.EmployeeId,
                BasicSalary = dto.BasicSalary,
                HouseRent = dto.HouseRent,
                MedicalAllowance = dto.MedicalAllowance,
                TransportAllowance = dto.TransportAllowance,
                OtherAllowance = dto.OtherAllowance,
                Deduction = dto.Deduction,
                TotalSalary = dto.BasicSalary + dto.HouseRent + dto.MedicalAllowance + dto.TransportAllowance + dto.OtherAllowance - dto.Deduction,
                EffectiveFrom = dto.EffectiveFrom,
                CreatedBy = userName,
                CreatedAt = DateTime.UtcNow
            };
            await repo.AddAsync(entity, ct);
        }

        await _uow.SaveChangesAsync(ct);
        await LogAuditAsync("Employee.Salary", dto.Id > 0 ? "Update" : "Create", dto.EmployeeId.ToString(), $"Salary {(dto.Id > 0 ? "updated" : "created")}: {dto.TotalSalary:N0} effective {dto.EffectiveFrom:yyyy-MM-dd}", ct);
    }

    public async Task DeleteSalaryAsync(int id, CancellationToken ct)
    {
        var repo = _uow.Repository<EmployeeSalary>();
        var entity = await repo.GetByIdAsync(id, ct);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.UpdatedBy = GetCurrentUserName();
            entity.UpdatedAt = DateTime.UtcNow;
            repo.Update(entity);
            await _uow.SaveChangesAsync(ct);
            await LogAuditAsync("Employee.Salary", "Delete", entity.EmployeeId.ToString(), $"Salary record deleted (effective: {entity.EffectiveFrom:yyyy-MM-dd})", ct);
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
