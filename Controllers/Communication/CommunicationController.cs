using Microsoft.AspNetCore.Authorization;
using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Models.Entities.Communication;
using SchoolManagementSystem.Services.Interfaces.Base;
<<<<<<< HEAD
using SchoolManagementSystem.Constants;
=======
>>>>>>> d8b24e6 (attendece and website curtomize)

namespace SchoolManagementSystem.Controllers.Communication;

[Authorize(Roles = "SuperAdmin,Admin,Principal,Teacher,Student")]
public class CommunicationController : GenericCrudController<Notice>
{
    public CommunicationController(IBaseService<Notice> service) : base(service, "Notice / News") { }

    protected override IQueryable<Notice> ApplySecurityFilters(IQueryable<Notice> query)
    {
        if (User.IsInRole(Roles.Student))
        {
            return query.Where(n => n.AudienceRole == "All" || n.AudienceRole == Roles.Student);
        }
        return query;
    }
}

