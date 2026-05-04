using Microsoft.AspNetCore.Authorization;
using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Communication;

namespace SchoolManagementSystem.Controllers;

[Authorize(Roles = "Super Admin,Principal,Assistant Head,Senior Lecturer,Lecturer,Student")]
public class CommunicationController : GenericCrudController<Notice>
{
    public CommunicationController(SchoolDbContext db) : base(db, "Notice / Communication") { }

    protected override IQueryable<Notice> ApplySecurityFilters(IQueryable<Notice> query)
    {
        if (User.IsInRole("Student"))
        {
            return query.Where(n => n.AudienceRole == "All" || n.AudienceRole == "Student");
        }
        return query;
    }
}
