using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;

namespace SchoolManagementSystem.Controllers.Fees;

public class PaymentController : GenericCrudController<Payment>
{
    private const string ViewPath = "~/Views/Fee/Payment";
    public PaymentController(IPaymentService service) : base(service, "Payment") { }

    [RequirePermission("Payments.Read")]
    public override async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);

        var result = await _service.GetPagedAsync(page, pageSize, search, User, cancellationToken);

        var model = new SchoolManagementSystem.Models.ViewModels.Shared.CrudListViewModel
        {
            ModuleName = _moduleName,
            ControllerName = "Payment",
            Columns = new System.Collections.Generic.List<string>(),
            Rows = result.Items.Select(x => new System.Collections.Generic.Dictionary<string, string?> { ["Id"] = x.Id.ToString() }).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalItems = result.TotalItems,
            Search = search
        };
        return View($"{ViewPath}/Index.cshtml", model);
    }

    [RequirePermission("Payments.Create")]
    public override IActionResult Create()
    {
        return base.Create();
    }

    [RequirePermission("Payments.Update")]
    public override Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
    {
        return base.Edit(id, cancellationToken);
    }

    public override async Task<IActionResult> CreateEdit(int? id = null, CancellationToken cancellationToken = default)
    {
        if (!Can(id is > 0 ? "Payments.Update" : "Payments.Create"))
        {
            return Forbid();
        }

        var entity = id is > 0
            ? await _service.GetByIdAsync(id.Value, cancellationToken)
            : new Payment();

        if (entity is null)
        {
            return NotFound();
        }

        return View($"{ViewPath}/CreateEdit.cshtml", entity);
    }

    public override async Task<IActionResult> Save(IFormCollection form, CancellationToken cancellationToken = default)
    {
        var isUpdate = int.TryParse(form["Id"], out var id) && id > 0;
        if (!Can(isUpdate ? "Payments.Update" : "Payments.Create"))
        {
            return Forbid();
        }

        return await base.Save(form, cancellationToken);
    }

    [RequirePermission("Payments.Read")]
    public override async Task<IActionResult> Details(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _service.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View($"{ViewPath}/Details.cshtml", entity);
    }

    [RequirePermission("Payments.Delete")]
    public override async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _service.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View($"{ViewPath}/Delete.cshtml", entity);
    }

    [RequirePermission("Payments.Delete")]
    public override Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken = default)
    {
        return base.DeleteConfirmed(id, cancellationToken);
    }

    protected override IQueryable<Payment> ApplySecurityFilters(IQueryable<Payment> query)
    {
        return ((IPaymentService)_service).GetPaymentsForUser(query, User);
    }

    private bool Can(string permissionCode)
    {
        return User.IsInRole("Super Admin") || User.HasClaim("Permission", permissionCode);
    }
}
