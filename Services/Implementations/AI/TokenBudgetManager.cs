using SchoolManagementSystem.Models.DTOs.AI;
using Microsoft.Extensions.Logging;

namespace SchoolManagementSystem.Services.Implementations.AI;

public class TokenBudgetManager
{
    private readonly ILogger<TokenBudgetManager> _logger;

    private const int TokensPerChar = 4;

    public TokenBudgetManager(ILogger<TokenBudgetManager> logger)
    {
        _logger = logger;
    }

    public int EstimateTokens(string text) => text.Length / TokensPerChar + 1;

    public List<MessageDto> TrimHistory(string systemPrompt, List<MessageDto> history, string userMessage, int maxTokens)
    {
        var systemTokens = EstimateTokens(systemPrompt);
        var userTokens = EstimateTokens(userMessage);
        var available = maxTokens - systemTokens - userTokens - 512;

        if (available <= 0)
        {
            _logger.LogWarning("System prompt + user message exceeds budget; returning empty history");
            return new List<MessageDto>();
        }

        var ordered = history.OrderByDescending(m => m.CreatedAt).ToList();
        var result = new List<MessageDto>();
        var used = 0;

        foreach (var msg in ordered)
        {
            var tokens = EstimateTokens(msg.Content) + 4;
            if (used + tokens > available) break;
            result.Add(msg);
            used += tokens;
        }

        result.Reverse();
        return result;
    }
}
