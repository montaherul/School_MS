using Microsoft.AspNetCore.Authorization;
using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Models.Entities.Communication;
using SchoolManagementSystem.Services.Interfaces.Base;

namespace SchoolManagementSystem.Controllers.Communication;

[Authorize(Roles = "Super Admin,Principal,Assistant Head,Senior Lecturer,Lecturer,Student")]
public class CommunicationController : GenericCrudController<Notice>
{
    public CommunicationController(IBaseService<Notice> service) : base(service, "Notice / News") { }

    protected override IQueryable<Notice> ApplySecurityFilters(IQueryable<Notice> query)
    {
        if (User.IsInRole("Student"))
        {
            return query.Where(n => n.AudienceRole == "All" || n.AudienceRole == "Student");
        }
        return query;
    }
}

