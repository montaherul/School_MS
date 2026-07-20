namespace SchoolManagementSystem.Services.Interfaces.AI;

public interface IAISettingsCache
{
    Task<string> GetApiKeyAsync();
    Task<string> GetEndpointAsync();
    Task<string> GetModelAsync();
    Task<int> GetMaxTokensAsync();
    Task<double> GetTemperatureAsync();
    Task<int> GetRetryCountAsync();
    Task<int> GetTimeoutSecondsAsync();
    Task<decimal> GetCostPerPromptTokenAsync();
    Task<decimal> GetCostPerCompletionTokenAsync();
}
