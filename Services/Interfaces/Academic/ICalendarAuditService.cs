namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface ICalendarAuditService
{
    Task LogAsync(string action, string entity, int? entityId, string? oldValue, string? newValue, CancellationToken ct = default);
}
