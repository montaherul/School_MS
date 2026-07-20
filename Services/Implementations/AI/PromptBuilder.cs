using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.AI;

namespace SchoolManagementSystem.Services.Implementations.AI;

public class PromptBuilder
{
    private readonly PromptTemplateLoader _templateLoader;
    private readonly TokenBudgetManager _tokenBudget;
    private readonly ILogger<PromptBuilder> _logger;

    public PromptBuilder(PromptTemplateLoader templateLoader, TokenBudgetManager tokenBudget, ILogger<PromptBuilder> logger)
    {
        _templateLoader = templateLoader;
        _tokenBudget = tokenBudget;
        _logger = logger;
    }

    public (string SystemPrompt, List<MessageDto> History) Build(AiContextDto context, List<MessageDto> conversationHistory, string userMessage)
    {
        var variables = new Dictionary<string, string>
        {
            ["SchoolName"] = context.SchoolName,
            ["StudentName"] = context.StudentName,
            ["StudentNo"] = context.StudentNo,
            ["ClassName"] = context.ClassName,
            ["SectionName"] = context.SectionName,
            ["GroupName"] = context.GroupName ?? "N/A",
            ["AcademicYear"] = context.AcademicYear,
            ["Subjects"] = string.Join(", ", context.Subjects)
        };

        var systemPrompt = _templateLoader.Render("StudentSystemPrompt.md", variables);

        var trimmedHistory = _tokenBudget.TrimHistory(systemPrompt, conversationHistory, userMessage, 8000);

        return (systemPrompt, trimmedHistory);
    }
}
