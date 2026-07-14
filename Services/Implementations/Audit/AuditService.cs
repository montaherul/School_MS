using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Services.Interfaces.Audit;

namespace SchoolManagementSystem.Services.Implementations.Audit;

public class AuditService : IAuditService
{
    private readonly SchoolDbContext _db;

    public AuditService(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task LogAsync(int? userId, string module, string action, string? details, CancellationToken ct = default)
    {
        var log = new AuditLog
        {
            UserId = userId,
            Module = module,
            Action = action,
            Details = details,
            CreatedAt = DateTime.UtcNow
        };
        _db.AuditLogs.Add(log);
        await _db.SaveChangesAsync(ct);
    }
}
