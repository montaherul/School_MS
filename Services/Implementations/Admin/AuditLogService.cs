using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.ViewModels.Admin;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Admin;

public class AuditLogService : IAuditLogService
{
    private readonly IUnitOfWork _unitOfWork;

    public AuditLogService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<AuditLogListItemViewModel>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default)
    {
        var query = _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Auth.AuditLog>().Query()
            .Include(a => a.User)
            .Where(a => !a.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lower = search.ToLower();
            query = query.Where(a =>
                a.Module.ToLower().Contains(lower) ||
                a.Action.ToLower().Contains(lower) ||
                (a.Details ?? string.Empty).ToLower().Contains(lower) ||
                (a.User != null && a.User.UserName.ToLower().Contains(lower)));
        }

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AuditLogListItemViewModel
            {
                Id = a.Id,
                Module = a.Module,
                Action = a.Action,
                IpAddress = a.IpAddress,
                Details = a.Details,
                UserName = a.User != null ? a.User.UserName : null,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync(ct);

        return new PagedResult<AuditLogListItemViewModel>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }
}
