using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.Fees;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class FeeLedgerController : Controller
{
    private readonly IFeeLedgerService _service;
    private readonly IFeeSecurityService _security;
    public FeeLedgerController(IFeeLedgerService service, IFeeSecurityService security) { _service = service; _security = security; }

    [RequirePermission("FeeLedger.Read")]
    public IActionResult Index() { return View(); }

    [HttpGet]
    [RequirePermission("FeeLedger.Read")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null, int? studentId = null, int? transactionType = null)
    {
        if (_security.HasStudentRole(User)) studentId = _security.GetCurrentStudentId(User);
        var result = await _service.GetPagedAsync(page, size, search, studentId, transactionType);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

}
