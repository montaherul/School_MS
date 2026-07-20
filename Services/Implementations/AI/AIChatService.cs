using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.Common;
using SchoolManagementSystem.Models.DTOs.AI;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.AI;
using SchoolManagementSystem.Services.Implementations.AI.Background;
using SchoolManagementSystem.Services.Implementations.AI.Security;
using SchoolManagementSystem.Services.Interfaces.AI;

namespace SchoolManagementSystem.Services.Implementations.AI;

public class AIChatService : IAIChatService
{
    private readonly IAIConversationRepository _conversationRepo;
    private readonly IAIMessageRepository _messageRepo;
    private readonly IAIUsageRepository _usageRepo;
    private readonly IAIContextService _contextService;
    private readonly IOpenAIService _openAiService;
    private readonly PromptBuilder _promptBuilder;
    private readonly InputGuard _inputGuard;
    private readonly UsageLoggingChannel _usageChannel;
    private readonly ILogger<AIChatService> _logger;

    private static readonly ConcurrentDictionary<int, RateLimitState> _rateLimits = new();

    public AIChatService(
        IAIConversationRepository conversationRepo,
        IAIMessageRepository messageRepo,
        IAIUsageRepository usageRepo,
        IAIContextService contextService,
        IOpenAIService openAiService,
        PromptBuilder promptBuilder,
        InputGuard inputGuard,
        UsageLoggingChannel usageChannel,
        ILogger<AIChatService> logger)
    {
        _conversationRepo = conversationRepo;
        _messageRepo = messageRepo;
        _usageRepo = usageRepo;
        _contextService = contextService;
        _openAiService = openAiService;
        _promptBuilder = promptBuilder;
        _inputGuard = inputGuard;
        _usageChannel = usageChannel;
        _logger = logger;
    }

    public async Task<Result<ConversationDetailDto>> CreateConversationAsync(int studentId, string createdBy, CancellationToken ct)
    {
        try
        {
            var result = await _conversationRepo.CreateAsync(studentId, "New Chat", createdBy, ct);
            return Result<ConversationDetailDto>.Success(new ConversationDetailDto
            {
                Id = result.Id,
                StudentId = studentId,
                Title = "New Chat",
                CreatedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create conversation for student {StudentId}", studentId);
            return Result<ConversationDetailDto>.Fail("Failed to create conversation.", "CREATE_FAILED");
        }
    }

    public async Task<Result<(List<ConversationListItemDto> Items, int TotalPages)>> GetConversationsAsync(int studentId, int page, int pageSize, CancellationToken ct)
    {
        try
        {
            var (items, total) = await _conversationRepo.ListPagedAsync(studentId, page, pageSize, ct);
            var totalPages = pageSize > 0 ? (int)Math.Ceiling(total / (double)pageSize) : 0;
            return Result<(List<ConversationListItemDto>, int)>.Success((items, totalPages));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list conversations for student {StudentId}", studentId);
            return Result<(List<ConversationListItemDto> Items, int TotalPages)>.Fail("Failed to load conversations.", "LIST_FAILED");
        }
    }

    public async Task<Result<ConversationDetailDto>> GetConversationAsync(int conversationId, int studentId, CancellationToken ct)
    {
        try
        {
            var conversation = await _conversationRepo.GetAsync(conversationId, studentId, ct);
            if (conversation is null)
                return Result<ConversationDetailDto>.Fail("Conversation not found.", "NOT_FOUND");
            return Result<ConversationDetailDto>.Success(conversation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get conversation {ConvId}", conversationId);
            return Result<ConversationDetailDto>.Fail("Failed to load conversation.", "LOAD_FAILED");
        }
    }

    public async Task<Result<List<MessageDto>>> GetMessagesAsync(int conversationId, int studentId, CancellationToken ct)
    {
        try
        {
            var messages = await _messageRepo.ListAsync(conversationId, studentId, ct);
            return Result<List<MessageDto>>.Success(messages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get messages for conversation {ConvId}", conversationId);
            return Result<List<MessageDto>>.Fail("Failed to load messages.", "LOAD_FAILED");
        }
    }

    public async Task<Result<AiResponseDto>> SendMessageAsync(int conversationId, int studentId, string message, string createdBy, CancellationToken ct)
    {
        try
        {
            // Rate limiting
            if (!CheckRateLimit(studentId))
            {
                _logger.LogWarning("Rate limit exceeded for student {StudentId}", studentId);
                return Result<AiResponseDto>.Fail("You have exceeded the rate limit. Please wait before sending another message.", "RATE_LIMITED");
            }

            // Ownership verification
            var conversation = await _conversationRepo.GetAsync(conversationId, studentId, ct);
            if (conversation is null)
            {
                _logger.LogWarning("Conversation {ConvId} not found for student {StudentId}", conversationId, studentId);
                return Result<AiResponseDto>.Fail("Conversation not found.", "NOT_FOUND");
            }

            // Sanitize input
            var sanitizedMessage = _inputGuard.Sanitize(message);
            if (string.IsNullOrWhiteSpace(sanitizedMessage))
                return Result<AiResponseDto>.Fail("Message cannot be empty.", "EMPTY_MESSAGE");

            // Prompt injection detection
            if (_inputGuard.ContainsPromptInjection(sanitizedMessage))
            {
                _logger.LogWarning("Prompt injection blocked for student {StudentId}", studentId);
                return Result<AiResponseDto>.Fail("Message contains prohibited content.", "CONTENT_BLOCKED");
            }

            // Mask PII
            var cleanedMessage = _inputGuard.MaskPii(sanitizedMessage);

            // Load student context
            var contextResult = await _contextService.GetStudentContextAsync(studentId, ct);
            if (contextResult.IsFailure)
                return Result<AiResponseDto>.Fail(contextResult.ErrorMessage!, "CONTEXT_FAILED");

            var context = contextResult.Data!;

            // Load conversation history
            var history = await _messageRepo.ListAsync(conversationId, studentId, ct);

            // Build prompt with token budget
            var (systemPrompt, trimmedHistory) = _promptBuilder.Build(context, history, cleanedMessage);

            // Save user message
            var userMsgId = await _messageRepo.InsertAsync(conversationId, "user", cleanedMessage, null, null, null, null, createdBy, ct);

            // Call OpenAI
            var responseResult = await _openAiService.SendMessageAsync(systemPrompt, trimmedHistory, cleanedMessage, ct);
            if (responseResult.IsFailure)
                return responseResult;

            var response = responseResult.Data!;

            // Save assistant message
            var assistantMsgId = await _messageRepo.InsertAsync(
                conversationId, "assistant", response.Content,
                response.PromptTokens, response.CompletionTokens, response.Model, response.LatencyMs,
                createdBy, ct);

            // Background usage logging
            await _usageChannel.EnqueueAsync(new UsageLogEntry
            {
                StudentId = studentId,
                ConversationId = conversationId,
                MessageId = assistantMsgId,
                Model = response.Model,
                PromptTokens = response.PromptTokens,
                CompletionTokens = response.CompletionTokens,
                TotalTokens = response.PromptTokens + response.CompletionTokens,
                EstimatedCost = response.EstimatedCost,
                LatencyMs = response.LatencyMs,
                CreatedBy = createdBy
            }, ct);

            // Auto-title on first message
            if (conversation.Title == "New Chat" && !string.IsNullOrWhiteSpace(cleanedMessage))
            {
                var newTitle = cleanedMessage.Length > 55 ? cleanedMessage[..55] + "..." : cleanedMessage;
                await _conversationRepo.UpdateTitleAsync(conversationId, studentId, newTitle, createdBy, ct);
            }

            return Result<AiResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SendMessage failed for student {StudentId}, conversation {ConvId}", studentId, conversationId);
            return Result<AiResponseDto>.Fail("An unexpected error occurred. Please try again.", "INTERNAL_ERROR");
        }
    }

    public async Task<Result<bool>> DeleteConversationAsync(int conversationId, int studentId, string updatedBy, CancellationToken ct)
    {
        try
        {
            await _conversationRepo.DeleteAsync(conversationId, studentId, updatedBy, ct);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete conversation {ConvId}", conversationId);
            return Result<bool>.Fail("Failed to delete conversation.", "DELETE_FAILED");
        }
    }

    public async Task<Result<List<UsageDailySummaryDto>>> GetUsageSummaryAsync(int? studentId, DateOnly? startDate, DateOnly? endDate, CancellationToken ct)
    {
        try
        {
            var summary = await _usageRepo.GetDailySummaryAsync(studentId, startDate, endDate, ct);
            return Result<List<UsageDailySummaryDto>>.Success(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get usage summary");
            return Result<List<UsageDailySummaryDto>>.Fail("Failed to load usage summary.", "LOAD_FAILED");
        }
    }

    private static bool CheckRateLimit(int studentId)
    {
        var now = DateTime.UtcNow;
        var state = _rateLimits.GetOrAdd(studentId, _ => new RateLimitState());

        lock (state)
        {
            // Minutely: 30 requests
            state.MinuteWindow.RemoveAll(t => now - t > TimeSpan.FromMinutes(1));
            if (state.MinuteWindow.Count >= 30)
                return false;

            // Daily: 500 requests
            state.DailyWindow.RemoveAll(t => now - t > TimeSpan.FromDays(1));
            if (state.DailyWindow.Count >= 500)
                return false;

            state.MinuteWindow.Add(now);
            state.DailyWindow.Add(now);
            return true;
        }
    }

    private sealed class RateLimitState
    {
        public List<DateTime> MinuteWindow { get; } = new();
        public List<DateTime> DailyWindow { get; } = new();
    }
}
