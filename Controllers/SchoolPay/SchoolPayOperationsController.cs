using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Controllers.SchoolPay;

[Authorize]
[Route("SchoolPay/Operations")]
public class SchoolPayOperationsController : Controller
{
    private readonly IOperationsCenterService _operationsService;

    public SchoolPayOperationsController(IOperationsCenterService operationsService)
    {
        _operationsService = operationsService;
    }

    [RequirePermission("SchoolPay.ViewTransactions")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var data = await _operationsService.GetOperationsDataAsync(ct);
        return View(data);
    }
}
