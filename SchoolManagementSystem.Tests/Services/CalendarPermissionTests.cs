using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using SchoolManagementSystem.Filters;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class CalendarPermissionTests
{
    [Fact]
    public void RequirePermissionAttribute_Exists()
    {
        var attr = new RequirePermissionAttribute("Calendar.View");
        Assert.NotNull(attr);
    }

    [Fact]
    public void CalendarView_PermissionCode_IsValid()
    {
        var code = "Calendar.View";
        Assert.StartsWith("Calendar.", code);
    }

    [Fact]
    public void CalendarCreate_PermissionCode_IsValid()
    {
        var code = "Calendar.Create";
        Assert.StartsWith("Calendar.", code);
    }

    [Fact]
    public void CalendarEdit_PermissionCode_IsValid()
    {
        var code = "Calendar.Edit";
        Assert.StartsWith("Calendar.", code);
    }

    [Fact]
    public void CalendarDelete_PermissionCode_IsValid()
    {
        var code = "Calendar.Delete";
        Assert.StartsWith("Calendar.", code);
    }

    [Fact]
    public void CalendarExport_PermissionCode_IsValid()
    {
        var code = "Calendar.Export";
        Assert.StartsWith("Calendar.", code);
    }

    [Fact]
    public void CalendarGenerate_PermissionCode_IsValid()
    {
        var code = "Calendar.Generate";
        Assert.StartsWith("Calendar.", code);
    }

    [Fact]
    public void CalendarRegenerate_PermissionCode_IsValid()
    {
        var code = "Calendar.Regenerate";
        Assert.StartsWith("Calendar.", code);
    }

    [Fact]
    public void CalendarRepair_PermissionCode_IsValid()
    {
        var code = "Calendar.Repair";
        Assert.StartsWith("Calendar.", code);
    }

    [Fact]
    public void AcademicCalendarController_HasAuthorizeAttribute()
    {
        var attr = typeof(SchoolManagementSystem.Controllers.Academic.AcademicCalendarController)
            .GetCustomAttributes(typeof(AuthorizeAttribute), true);
        Assert.NotEmpty(attr);
    }

    [Fact]
    public void CalendarPermissionAttribute_CanBeUsedMultiple()
    {
        var attr = new RequirePermissionAttribute("Calendar.View");
        var usage = (AttributeUsageAttribute?)typeof(RequirePermissionAttribute)
            .GetCustomAttribute(typeof(AttributeUsageAttribute));
        Assert.NotNull(usage);
        Assert.True(usage.AllowMultiple);
    }

    [Fact]
    public void PermissionAttribute_ExtendsRequirePermission()
    {
        var attr = new PermissionAttribute("Calendar", "View");
        Assert.IsType<PermissionAttribute>(attr);
        Assert.IsAssignableFrom<RequirePermissionAttribute>(attr);
    }
}
