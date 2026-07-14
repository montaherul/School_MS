namespace SchoolManagementSystem.Services.Interfaces.Audit;

public interface IAuditService
{
    Task LogAsync(int? userId, string module, string action, string? details, CancellationToken ct = default);
}
