using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Services.Interfaces.Base;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;
<<<<<<< HEAD
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Constants;
using Microsoft.AspNetCore.Mvc;
=======
>>>>>>> d8b24e6 (attendece and website curtomize)

namespace SchoolManagementSystem.Controllers.Exam;

public class ExamController : GenericCrudController<ExamEntity>
{
    public ExamController(IBaseService<ExamEntity> service) : base(service, "Exam") { }
<<<<<<< HEAD

    [RequirePermission(Permissions.Exam.View)]
    public override Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default)
        => base.Index(page, pageSize, search, cancellationToken);

    [RequirePermission(Permissions.Exam.Create)]
    public override Task<IActionResult> CreateEdit(int? id = null, CancellationToken cancellationToken = default)
        => base.CreateEdit(id, cancellationToken);

    [RequirePermission(Permissions.Exam.Update)]
    public override Task<IActionResult> Save(IFormCollection form, CancellationToken cancellationToken = default)
        => base.Save(form, cancellationToken);

    [RequirePermission(Permissions.Exam.Delete)]
    public override Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        => base.Delete(id, cancellationToken);
}
=======
}

>>>>>>> d8b24e6 (attendece and website curtomize)
