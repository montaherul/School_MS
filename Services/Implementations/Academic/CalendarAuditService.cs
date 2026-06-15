using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class CalendarAuditService : ICalendarAuditService
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CalendarAuditService(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor)
    {
        _uow = uow;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogAsync(string action, string entity, int? entityId, string? oldValue, string? newValue, CancellationToken ct = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userIdStr = httpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        int? userId = userIdStr != null && int.TryParse(userIdStr, out var uid) ? uid : null;

        var details = entityId.HasValue
            ? $"[{entity}#{entityId}] {action}"
            : $"[{entity}] {action}";

        if (oldValue != null || newValue != null)
        {
            details += $" | Old: {oldValue} | New: {newValue}";
        }

        var log = new AuditLog
        {
            UserId = userId,
            Module = "Calendar",
            Action = $"{entity}.{action}",
            IpAddress = httpContext?.Connection?.RemoteIpAddress?.ToString(),
            Details = details.Length > 1000 ? details[..1000] : details,
            CreatedBy = httpContext?.User?.Identity?.Name ?? "system",
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<AuditLog>().AddAsync(log, ct);
        await _uow.SaveChangesAsync(ct);
    }
}
