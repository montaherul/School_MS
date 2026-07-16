using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Controllers.SchoolPay;

[Authorize]
[Route("SchoolPay/DeadLetter")]
public class DeadLetterQueueController : Controller
{
    private readonly IDeadLetterQueueService _dlqService;

    public DeadLetterQueueController(IDeadLetterQueueService dlqService)
    {
        _dlqService = dlqService;
    }

    [RequirePermission("SchoolPay.ViewTransactions")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var items = await _dlqService.GetAllAsync(ct);
        return View(items);
    }

    [RequirePermission("SchoolPay.ViewTransactions")]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var item = await _dlqService.GetByIdAsync(id, ct);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("SchoolPay.Manage")]
    public async Task<IActionResult> Reprocess(int id, CancellationToken ct)
    {
        await _dlqService.ReprocessAsync(id, ct);
        TempData["Success"] = "Webhook moved back to processing queue";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("SchoolPay.Manage")]
    public async Task<IActionResult> Ignore(int id, CancellationToken ct)
    {
        await _dlqService.IgnoreAsync(id, ct);
        TempData["Success"] = "Webhook ignored";
        return RedirectToAction(nameof(Index));
    }
}
