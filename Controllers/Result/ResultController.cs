using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Services.Interfaces.Base;
using SchoolManagementSystem.Services.Interfaces.Result;
using Microsoft.AspNetCore.Authorization;
using SchoolManagementSystem.Filters;
using Microsoft.AspNetCore.Mvc;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
public class ResultController : GenericCrudController<MarkEntry>
{
    public ResultController(IMarkEntryService markEntryService) : base(markEntryService, "Result / Marks")
    {
    }

    [RequirePermission("Result.View")]
    public override Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default)
        => base.Index(page, pageSize, search, cancellationToken);

    [RequirePermission("Result.View")]
    public override Task<IActionResult> Details(int id, CancellationToken cancellationToken = default)
        => base.Details(id, cancellationToken);

    [RequirePermission("Result.Create")]
    public override Task<IActionResult> CreateEdit(int? id = null, CancellationToken cancellationToken = default)
        => base.CreateEdit(id, cancellationToken);

    [RequirePermission("Result.Create")]
    public override Task<IActionResult> Save(IFormCollection form, CancellationToken cancellationToken = default)
        => base.Save(form, cancellationToken);

    [RequirePermission("Result.Delete")]
    public override Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        => base.Delete(id, cancellationToken);

    [RequirePermission("Result.Delete")]
    public override Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken = default)
        => base.DeleteConfirmed(id, cancellationToken);
}
