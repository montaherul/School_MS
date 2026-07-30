using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Finance;
using SchoolManagementSystem.Services.Interfaces.Finance;

namespace SchoolManagementSystem.Controllers.Finance;

[Authorize]
public class CashFlowController : Controller
{
    private const string ViewPath = "~/Views/Finance/CashFlow";
    private readonly ICashFlowService _service;

    public CashFlowController(ICashFlowService service) { _service = service; }

    [RequirePermission("CashFlow.Read")]
    public IActionResult Index() => View($"{ViewPath}/Index.cshtml");

    [HttpGet]
    [RequirePermission("CashFlow.Read")]
    public async Task<IActionResult> GetStatement(int year, int? month = null, int? periodType = 3, int? fromMonth = null, int? toMonth = null)
    {
        var result = await _service.GetCashFlowStatementAsync(year, month, periodType);
        return Json(result);
    }
}
