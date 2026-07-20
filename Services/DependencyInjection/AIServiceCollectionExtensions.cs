using SchoolManagementSystem.Repositories.Implementations.AI;
using SchoolManagementSystem.Repositories.Interfaces.AI;
using SchoolManagementSystem.Services.Implementations.AI;
using SchoolManagementSystem.Services.Implementations.AI.Background;
using SchoolManagementSystem.Services.Implementations.AI.Security;
using SchoolManagementSystem.Services.Interfaces.AI;

namespace SchoolManagementSystem.Extensions;

public static class AIServiceCollectionExtensions
{
    public static IServiceCollection AddAIModule(this IServiceCollection services)
    {
        // AI settings cache (reads from DB AISettings + AIProvider tables)
        services.AddScoped<IAISettingsCache, AISettingsCache>();
        services.AddSingleton<IAICacheInvalidator, AICacheInvalidator>();

        // OpenAI client abstraction (swappable provider)
        services.AddHttpClient<IOpenAIClient, OpenAIClient>();
        services.AddScoped<IOpenAIService, OpenAIService>();

        // Repositories
        services.AddScoped<IAIConversationRepository, AIConversationRepository>();
        services.AddScoped<IAIMessageRepository, AIMessageRepository>();
        services.AddScoped<IAIUsageRepository, AIUsageRepository>();
        services.AddScoped<IAIContextRepository, AIContextRepository>();
        services.AddScoped<IAIAdminRepository, AIAdminRepository>();

        // Services
        services.AddScoped<IAIChatService, AIChatService>();
        services.AddScoped<IAIContextService, AIContextService>();
        services.AddScoped<IAIAdminService, AIAdminService>();
        services.AddScoped<IAIFeatureService, AIFeatureService>();

        // Infrastructure
        services.AddScoped<PromptBuilder>();
        services.AddScoped<PromptTemplateLoader>();
        services.AddScoped<TokenBudgetManager>();
        services.AddScoped<InputGuard>();

        // Background usage logging
        services.AddSingleton<UsageLoggingChannel>();
        services.AddHostedService<UsageLoggingWorker>();

        return services;
    }
}
