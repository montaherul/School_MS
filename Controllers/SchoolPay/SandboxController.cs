using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Controllers.SchoolPay;

[AllowAnonymous]
[Route("SchoolPay/Sandbox")]
public class SandboxController : Controller
{
    private readonly ISchoolPayRepository _repo;
    private readonly ILogger<SandboxController> _logger;

    public SandboxController(ISchoolPayRepository repo, ILogger<SandboxController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    [RequirePermission("SchoolPay.Manage")]
    public IActionResult Index()
    {
        return View("~/Views/SchoolPay/Sandbox/Index.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("SchoolPay.Manage")]
    public async Task<IActionResult> Simulate(SchoolPaySandboxTestRequest request, CancellationToken ct)
    {
        var sandboxProvider = await _repo.GetProviderEntityByCodeAsync("SANDBOX", ct);
        var sandboxProviderId = sandboxProvider?.Id ?? 0;
        var txnRef = $"SANDBOX_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..6]}";

        var txn = new SchoolManagementSystem.Models.Entities.SchoolPay.PaymentGatewayTransaction
        {
            PaymentProviderId = sandboxProviderId,
            TransactionReference = txnRef,
            Amount = request.Amount,
            Currency = "BDT",
            InitiatedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        switch (request.Scenario?.ToLower())
        {
            case "success":
                txn.Status = SchoolPayTransactionStatus.Completed;
                txn.CompletedAt = DateTime.UtcNow;
                txn.ProviderTransactionId = $"SANDBOX_{Guid.NewGuid().ToString("N")[..8]}";
                break;

            case "failure":
                txn.Status = SchoolPayTransactionStatus.Failed;
                txn.CompletedAt = DateTime.UtcNow;
                txn.StatusMessage = "Sandbox simulated failure";
                break;

            case "timeout":
                txn.Status = SchoolPayTransactionStatus.Pending;
                txn.StatusMessage = "Sandbox simulated timeout — will expire";
                break;

            case "duplicate":
                txn.Status = SchoolPayTransactionStatus.Completed;
                txn.CompletedAt = DateTime.UtcNow;
                var dup = new SchoolManagementSystem.Models.Entities.SchoolPay.PaymentGatewayTransaction
                {
                    PaymentProviderId = sandboxProviderId,
                    TransactionReference = txnRef,
                    Amount = request.Amount,
                    Currency = "BDT",
                    Status = SchoolPayTransactionStatus.Completed,
                    InitiatedAt = DateTime.UtcNow,
                    CompletedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    ProviderTransactionId = $"SANDBOX_{Guid.NewGuid().ToString("N")[..8]}",
                    StatusMessage = "Duplicate sandbox transaction"
                };
                await _repo.CreateTransactionAsync(dup, ct);
                break;

            default:
                txn.Status = SchoolPayTransactionStatus.Pending;
                break;
        }

        await _repo.CreateTransactionAsync(txn, ct);

        TempData["Success"] = $"Sandbox transaction created: {txnRef} ({request.Scenario})";
        return RedirectToAction(nameof(Index));
    }

    [AllowAnonymous]
    [HttpGet("Gateway")]
    public IActionResult Gateway(string txnRef, decimal amount, string? returnUrl)
    {
        ViewBag.TxnRef = txnRef;
        ViewBag.Amount = amount;
        ViewBag.ReturnUrl = returnUrl;
        return View("~/Views/SchoolPay/Sandbox/Gateway.cshtml");
    }

    [HttpPost("Callback")]
    [AllowAnonymous]
    public async Task<IActionResult> Callback(string txnRef, string status, CancellationToken ct)
    {
        _logger.LogInformation("Sandbox callback: {TxnRef} status={Status}", txnRef, status);

        var transactions = await _repo.GetTransactionsPagedAsync(1, 100, ct: ct);
        var txn = transactions.FirstOrDefault(t => t.TransactionReference == txnRef);
        if (txn != null)
        {
            await _repo.LogAuditEventAsync(txn.Id, "SandboxCallback", $"Simulated callback with status={status}", "Sandbox", null, ct);
        }

        return RedirectToAction("Gateway", new { txnRef, amount = 0 });
    }
}
