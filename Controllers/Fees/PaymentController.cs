
using Microsoft.AspNetCore.Authorization;
using SchoolManagementSystem.Controllers.Common;

using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Filters;

using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;

namespace SchoolManagementSystem.Controllers.Fees;

public class PaymentController : GenericCrudController<Payment>
{
    public PaymentController(IPaymentService service) : base(service, "Payment") { }



    [RequirePermission("Payments.Read")]
    public override Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default)
    {
        return base.Index(page, pageSize, search, cancellationToken);
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

    public override Task<IActionResult> CreateEdit(int? id = null, CancellationToken cancellationToken = default)
    {
        if (!Can(id is > 0 ? "Payments.Update" : "Payments.Create"))
        {
            return Task.FromResult<IActionResult>(Forbid());
        }

        return base.CreateEdit(id, cancellationToken);
    }

    public override Task<IActionResult> Save(IFormCollection form, CancellationToken cancellationToken = default)
    {
        var isUpdate = int.TryParse(form["Id"], out var id) && id > 0;
        if (!Can(isUpdate ? "Payments.Update" : "Payments.Create"))
        {
            return Task.FromResult<IActionResult>(Forbid());
        }

        return base.Save(form, cancellationToken);
    }

    [RequirePermission("Payments.Read")]
    public override Task<IActionResult> Details(int id, CancellationToken cancellationToken = default)
    {
        return base.Details(id, cancellationToken);
    }

    [RequirePermission("Payments.Delete")]
    public override Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        return base.Delete(id, cancellationToken);
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

}



    private bool Can(string permissionCode)
    {
        return User.IsInRole("Super Admin") || User.HasClaim("Permission", permissionCode);
    }
}

