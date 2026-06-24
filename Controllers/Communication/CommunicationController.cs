using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.Entities.Communication;
using SchoolManagementSystem.Services.Interfaces.Base;

namespace SchoolManagementSystem.Controllers.Communication;

[RequirePermission("Communication.Manage")]
public class CommunicationController : GenericCrudController<Notice>
{

    public CommunicationController(IBaseService<Notice> service) : base(service, "Notice / News") { }

    protected override IQueryable<Notice> ApplySecurityFilters(IQueryable<Notice> query)
    {
        // Always hide deleted notices
        query = query.Where(n => !n.IsDeleted);

        if (User.IsInRole("Student"))
        {
            // Students only see published notices aimed at them and already published (PublishAt <= now)
            return query.Where(n => n.IsPublished
                                     && n.PublishAt <= DateTime.UtcNow
                                     && (n.AudienceRole == "All" || n.AudienceRole == "Student"));
        }

        return query;
    }

    // Students get a modern card-based index. Admins keep the CRUD list.
    public override async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default)
    {
        if (User.IsInRole("Student"))
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 6, 50);

            var query = _service.Query().AsNoTracking();
            query = ApplySecurityFilters(query);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(n => n.Title.Contains(search) || n.Body.Contains(search));
            }

            var total = await query.CountAsync(cancellationToken);

            var notices = await query
                .OrderByDescending(n => n.PublishAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var model = new SchoolManagementSystem.Models.ViewModels.Communication.NoticeListViewModel
            {
                Notices = notices,
                Page = page,
                PageSize = pageSize,
                TotalItems = total,
                Search = search
            };

            return View("StudentIndex", model);
        }

        return await base.Index(page, pageSize, search, cancellationToken);
    }

    // Student-friendly details view
    public override async Task<IActionResult> Details(int id, CancellationToken cancellationToken = default)
    {
        if (User.IsInRole("Student"))
        {
            var query = _service.Query().AsNoTracking().Where(n => n.Id == id);
            query = ApplySecurityFilters(query);
            var notice = await query.FirstOrDefaultAsync(cancellationToken);
            if (notice == null)
            {
                return NotFound();
            }
            return View("StudentDetails", notice);
        }

        return await base.Details(id, cancellationToken);
    }

    // ADMIN-only actions: restrict create/edit/delete/save endpoints to administrative roles
    [RequirePermission("Communication.Manage")]
    public override IActionResult Create()
    {
        return base.Create();
    }

    [RequirePermission("Communication.Manage")]
    public override Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
    {
        return base.Edit(id, cancellationToken);
    }

    [RequirePermission("Communication.Manage")]
    public override Task<IActionResult> CreateEdit(int? id = null, CancellationToken cancellationToken = default)
    {
        return base.CreateEdit(id, cancellationToken);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Communication.Manage")]
    public override Task<IActionResult> Save(IFormCollection form, CancellationToken cancellationToken = default)
    {
        return base.Save(form, cancellationToken);
    }

    [RequirePermission("Communication.Manage")]
    public override Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        return base.Delete(id, cancellationToken);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Communication.Manage")]
    public override Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken = default)
    {
        return base.DeleteConfirmed(id, cancellationToken);
    }
}

