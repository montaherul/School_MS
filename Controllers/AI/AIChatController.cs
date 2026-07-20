using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.DTOs.AI;
using SchoolManagementSystem.Models.ViewModels.AI;
using SchoolManagementSystem.Services.Interfaces.AI;
using SchoolManagementSystem.Services.Interfaces.Students;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.AI;

[Authorize]
[Route("AI/[controller]")]
public class AIChatController : Controller
{
    private readonly IAIChatService _chatService;
    private readonly IOpenAIService _openAiService;
    private readonly IAIFeatureService _aiFeature;
    private readonly IStudentService _studentService;

    public AIChatController(IAIChatService chatService, IOpenAIService openAiService, IAIFeatureService aiFeature, IStudentService studentService)
    {
        _chatService = chatService;
        _openAiService = openAiService;
        _aiFeature = aiFeature;
        _studentService = studentService;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Student";

    private async Task<int> GetStudentIdAsync(CancellationToken ct)
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(raw, out var userId) || userId == 0) return 0;
        var id = await _studentService.GetStudentIdByUserIdAsync(userId, ct);
        return id ?? 0;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        if (!await _aiFeature.IsFeatureEnabledAsync("AI.Feature.Chat", ct))
        {
            TempData["ErrorMessage"] = "AI Chat is currently disabled by the administration.";
            return View("~/Views/AI/Chat/Index.cshtml", new AIChatViewModel());
        }

        var studentId = await GetStudentIdAsync(ct);
        if (studentId == 0)
        {
            TempData["ErrorMessage"] = "AI Chat is only available for students.";
            return RedirectToAction("Index", "Dashboard");
        }

        var result = await _chatService.GetConversationsAsync(studentId, 1, 20, ct);
        if (result.IsFailure)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return View("~/Views/AI/Chat/Index.cshtml", new AIChatViewModel());
        }

        var (conversations, totalPages) = result.Data!;
        return View("~/Views/AI/Chat/Index.cshtml", new AIChatViewModel
        {
            Conversations = conversations,
            Page = 1,
            TotalPages = totalPages
        });
    }

    [HttpGet("Chat/{conversationId}")]
    public async Task<IActionResult> Chat(int conversationId, CancellationToken ct)
    {
        var studentId = await GetStudentIdAsync(ct);
        if (studentId == 0)
        {
            TempData["ErrorMessage"] = "AI Chat is only available for students.";
            return RedirectToAction(nameof(Index));
        }

        var convResult = await _chatService.GetConversationAsync(conversationId, studentId, ct);
        if (convResult.IsFailure) return NotFound();

        var msgsResult = await _chatService.GetMessagesAsync(conversationId, studentId, ct);
        var listResult = await _chatService.GetConversationsAsync(studentId, 1, 20, ct);

        if (listResult.IsFailure)
            return NotFound();

        return View("~/Views/AI/Chat/Index.cshtml", new AIChatViewModel
        {
            ActiveConversationId = conversationId,
            ActiveConversationTitle = convResult.Data!.Title,
            Conversations = listResult.Data.Items,
            Messages = msgsResult.Data ?? new List<MessageDto>(),
            Page = 1,
            TotalPages = listResult.Data.TotalPages
        });
    }

    [HttpPost("New")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> New(CancellationToken ct)
    {
        var studentId = await GetStudentIdAsync(ct);
        if (studentId == 0)
        {
            TempData["ErrorMessage"] = "AI Chat is only available for students.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _chatService.CreateConversationAsync(studentId, GetUserId(), ct);
        if (result.IsFailure)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Chat), new { conversationId = result.Data!.Id });
    }

    [HttpPost("Send")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(int conversationId, string message, CancellationToken ct)
    {
        var studentId = await GetStudentIdAsync(ct);
        if (studentId == 0) return Json(new { success = false, error = "Unauthorized." });

        var result = await _chatService.SendMessageAsync(conversationId, studentId, message, GetUserId(), ct);
        if (result.IsFailure)
            return Json(new { success = false, error = result.ErrorMessage });

        return Json(new { success = true, content = result.Data!.Content });
    }

    [HttpPost("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int conversationId, CancellationToken ct)
    {
        var studentId = await GetStudentIdAsync(ct);
        if (studentId == 0)
        {
            TempData["ErrorMessage"] = "AI Chat is only available for students.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _chatService.DeleteConversationAsync(conversationId, studentId, GetUserId(), ct);
        if (result.IsFailure)
            TempData["ErrorMessage"] = result.ErrorMessage;
        else
            TempData["SuccessMessage"] = "Conversation deleted.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("List")]
    public async Task<IActionResult> List(int page = 1, CancellationToken ct = default)
    {
        var studentId = await GetStudentIdAsync(ct);
        if (studentId == 0) return Json(new { data = new List<object>(), totalPages = 0 });

        var result = await _chatService.GetConversationsAsync(studentId, page, 20, ct);
        if (result.IsFailure)
            return Json(new { data = new List<object>(), totalPages = 0 });

        var (conversations, totalPages) = result.Data!;
        return Json(new { data = conversations, totalPages });
    }

    [HttpGet("Messages/{conversationId}")]
    public async Task<IActionResult> Messages(int conversationId, CancellationToken ct = default)
    {
        var studentId = await GetStudentIdAsync(ct);
        if (studentId == 0) return Json(new { data = new List<object>() });

        var result = await _chatService.GetMessagesAsync(conversationId, studentId, ct);
        return Json(new { data = result.Data ?? new List<MessageDto>() });
    }

    [HttpGet("Stream/{conversationId}")]
    public async Task Stream(int conversationId, string message, CancellationToken ct)
    {
        var studentId = await GetStudentIdAsync(ct);
        if (studentId == 0) return;

        Response.Headers["Content-Type"] = "text/event-stream";
        Response.Headers["Cache-Control"] = "no-cache";
        Response.Headers["Connection"] = "keep-alive";

        var sendResult = await _chatService.SendMessageAsync(conversationId, studentId, message, GetUserId(), ct);
        if (sendResult.IsFailure)
        {
            await Response.WriteAsync($"data: {{\"error\":\"{sendResult.ErrorMessage}\"}}\n\n", ct);
            await Response.WriteAsync("event: error\ndata: connection_closed\n\n", ct);
            return;
        }

        var content = sendResult.Data!.Content;
        var chunkSize = 50;

        for (int i = 0; i < content.Length; i += chunkSize)
        {
            var chunk = content.Substring(i, Math.Min(chunkSize, content.Length - i));
            var escaped = System.Text.Json.JsonSerializer.Serialize(chunk);
            await Response.WriteAsync($"data: {escaped}\n\n", ct);
            await Response.Body.FlushAsync(ct);
            await Task.Delay(15, ct);
        }

        await Response.WriteAsync("data: [DONE]\n\n", ct);
    }
}
