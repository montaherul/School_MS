using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.Common;
using SchoolManagementSystem.Models.DTOs.AI;
using SchoolManagementSystem.Services.Interfaces.AI;

namespace SchoolManagementSystem.Services.Implementations.AI;

public class OpenAIService : IOpenAIService
{
    private readonly IOpenAIClient _client;
    private readonly ILogger<OpenAIService> _logger;

    public OpenAIService(IOpenAIClient client, ILogger<OpenAIService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Result<AiResponseDto>> SendMessageAsync(string systemPrompt, List<MessageDto> conversationHistory, string userMessage, CancellationToken ct)
    {
        try
        {
            var response = await _client.SendMessageAsync(systemPrompt, conversationHistory, userMessage, ct);
            return Result<AiResponseDto>.Success(response);
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("OpenAI request timed out");
            return Result<AiResponseDto>.Fail("Request timed out. Please try again.", "TIMEOUT");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "OpenAI API request failed");
            return Result<AiResponseDto>.Fail("AI service temporarily unavailable. Please try again later.", "API_UNAVAILABLE");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenAI critical error");
            return Result<AiResponseDto>.Fail("An unexpected error occurred.", "INTERNAL_ERROR");
        }
    }
}

public class OpenAIClient : IOpenAIClient
{
    private readonly HttpClient _httpClient;
    private readonly IAISettingsCache _settings;
    private readonly ILogger<OpenAIClient> _logger;

    public OpenAIClient(HttpClient httpClient, IAISettingsCache settings, ILogger<OpenAIClient> logger)
    {
        _httpClient = httpClient;
        _settings = settings;
        _logger = logger;
    }

    public async Task<AiResponseDto> SendMessageAsync(string systemPrompt, List<MessageDto> conversationHistory, string userMessage, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        var apiKey = await _settings.GetApiKeyAsync();
        var endpoint = await _settings.GetEndpointAsync();
        var model = await _settings.GetModelAsync();
        var maxTokens = await _settings.GetMaxTokensAsync();
        var temperature = await _settings.GetTemperatureAsync();
        var retryCount = await _settings.GetRetryCountAsync();
        var timeoutSeconds = await _settings.GetTimeoutSecondsAsync();
        var costPerPromptToken = await _settings.GetCostPerPromptTokenAsync();
        var costPerCompletionToken = await _settings.GetCostPerCompletionTokenAsync();

        _httpClient.BaseAddress ??= new Uri(endpoint);
        if (_httpClient.DefaultRequestHeaders.Authorization == null)
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        var maxAttempts = Math.Max(1, retryCount);

        var attempt = 0;
        while (attempt < maxAttempts)
        {
            attempt++;
            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

                var messages = new List<object>
                {
                    new { role = "system", content = systemPrompt }
                };

                foreach (var msg in conversationHistory)
                    messages.Add(new { role = msg.Role, content = msg.Content });

                messages.Add(new { role = "user", content = userMessage });

                var requestBody = new
                {
                    model,
                    messages,
                    max_tokens = maxTokens,
                    temperature
                };

                var json = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation("OpenAI request (attempt {Attempt}/{MaxAttempts}): Model={Model}, HistoryMsgs={Count}",
                    attempt, maxAttempts, model, conversationHistory.Count);

                var response = await _httpClient.PostAsync("", content, cts.Token);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync(cts.Token);
                sw.Stop();

                using var doc = JsonDocument.Parse(responseJson);
                var root = doc.RootElement;

                var outputText = root.TryGetProperty("choices", out var choices)
                    ? choices[0].TryGetProperty("message", out var msgEl)
                        ? msgEl.TryGetProperty("content", out var c) ? c.GetString() ?? string.Empty : string.Empty
                        : choices[0].TryGetProperty("text", out var t) ? t.GetString() ?? string.Empty : string.Empty
                    : root.TryGetProperty("output", out var output)
                        ? output.TryGetProperty("text", out var text)
                            ? text.GetString() ?? string.Empty
                            : output.GetString() ?? string.Empty
                        : string.Empty;

                var promptTokens = 0;
                var completionTokens = 0;
                if (root.TryGetProperty("usage", out var usage))
                {
                    promptTokens = usage.TryGetProperty("input_tokens", out var pt) ? pt.GetInt32() : 0;
                    completionTokens = usage.TryGetProperty("output_tokens", out var ct2) ? ct2.GetInt32() : 0;
                    if (promptTokens == 0 && usage.TryGetProperty("prompt_tokens", out var pt2))
                        promptTokens = pt2.GetInt32();
                    if (completionTokens == 0 && usage.TryGetProperty("completion_tokens", out var ct3))
                        completionTokens = ct3.GetInt32();
                }

                var estimatedCost = (promptTokens * costPerPromptToken) + (completionTokens * costPerCompletionToken);

                _logger.LogInformation("OpenAI response: Tokens={TotalTokens}, Latency={LatencyMs}ms",
                    promptTokens + completionTokens, sw.ElapsedMilliseconds);

                return new AiResponseDto
                {
                    Content = outputText,
                    PromptTokens = promptTokens,
                    CompletionTokens = completionTokens,
                    Model = model,
                    LatencyMs = (int)sw.ElapsedMilliseconds,
                    EstimatedCost = estimatedCost
                };
            }
            catch (TaskCanceledException) when (attempt < maxAttempts)
            {
                _logger.LogWarning("OpenAI request timed out (attempt {Attempt}/{MaxAttempts})", attempt, maxAttempts);
            }
            catch (HttpRequestException ex) when (attempt < maxAttempts)
            {
                _logger.LogWarning(ex, "OpenAI request failed (attempt {Attempt}/{MaxAttempts})", attempt, maxAttempts);
                await Task.Delay(TimeSpan.FromSeconds(1 * attempt), ct);
            }
        }

        sw.Stop();
        _logger.LogError("OpenAI failed after {MaxAttempts} attempts", maxAttempts);
        throw new InvalidOperationException($"OpenAI API request failed after {maxAttempts} attempts.");
    }

}
