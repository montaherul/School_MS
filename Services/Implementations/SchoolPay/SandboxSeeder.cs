using SchoolManagementSystem.Models.Entities.SchoolPay;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public static class SandboxSeeder
{
    public static async Task SeedSandboxProviderAsync(ISchoolPayRepository repo, string createdBy = "System")
    {
        var existing = await repo.GetProviderEntityByCodeAsync("SANDBOX");
        if (existing != null) return;

        var provider = new PaymentProvider
        {
            Code = "SANDBOX",
            Name = "Sandbox Simulator",
            Description = "Test payment provider for development and testing",
            Status = ProviderStatus.Active,
            IsActive = true,
            IsSandbox = true,
            Priority = 999,
            SupportsRefund = true,
            SupportsSettlement = false,
            MaxRetryAttempts = 1,
            ClassName = "SandboxProvider",
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        var configs = new List<PaymentProviderConfiguration>
        {
            new() { Key = "mode", Value = "sandbox", IsActive = true, CreatedBy = createdBy, CreatedAt = DateTime.UtcNow }
        };

        await repo.CreateProviderAsync(provider, configs);
    }
}
