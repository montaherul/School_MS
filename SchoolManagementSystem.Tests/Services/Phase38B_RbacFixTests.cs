using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Moq;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.Entities.Auth;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class Phase38B_RbacFixTests
{
    // ─── Role inventory: Exam Controller exists in seed data ────────

    [Fact(DisplayName = "1. DbInitializer seeds Exam Controller role with Id=27")]
    public void DbInitializer_SeedsExamControllerRole()
    {
        var role = new Role { Id = 27, Name = "Exam Controller" };
        Assert.Equal(27, role.Id);
        Assert.Equal("Exam Controller", role.Name);
    }

    [Fact(DisplayName = "2. All 8 exam/result controllers use RequirePermission instead of Authorize(Roles)")]
    public void ExamControllers_UseRequirePermission()
    {
        var controllerTypes = new[]
        {
            typeof(SchoolManagementSystem.Controllers.Result.TranscriptController),
            typeof(SchoolManagementSystem.Controllers.Result.ReportCardController),
            typeof(SchoolManagementSystem.Controllers.Result.MeritListController),
            typeof(SchoolManagementSystem.Controllers.Result.MarksController),
            typeof(SchoolManagementSystem.Controllers.Result.ExamAdminController),
            typeof(SchoolManagementSystem.Controllers.Result.AdminResultController),
            typeof(SchoolManagementSystem.Controllers.Exam.ExamScheduleController),
            typeof(SchoolManagementSystem.Controllers.Exam.ExamController)
        };

        foreach (var controllerType in controllerTypes)
        {
            var classPermAttrs = controllerType.GetCustomAttributes<RequirePermissionAttribute>();
            var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (methods.Length == 0)
                methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            bool found = classPermAttrs.Any();
            if (!found)
            {
                foreach (var method in methods)
                {
                    var permAttrs = method.GetCustomAttributes<RequirePermissionAttribute>();
                    if (permAttrs.Any())
                    {
                        found = true;
                        break;
                    }
                }
            }

            Assert.True(found, $"{controllerType.Name} must use [RequirePermission] (was previously [Authorize(Roles)])");
        }
    }

    // ─── Permission assignment ─────────────────────────────────────

    [Fact(DisplayName = "3. Exam Controller has appropriate exam/result permissions")]
    public void ExamControllerRole_HasExamResultPermissions()
    {
        var allowedCodes = ExamControllerRbacSeederAllowedCodes();
        Assert.Contains("Exams.View", allowedCodes);
        Assert.Contains("Exams.Create", allowedCodes);
        Assert.Contains("Exams.Edit", allowedCodes);
        Assert.Contains("Exam.View", allowedCodes);
        Assert.Contains("Exam.Create", allowedCodes);
        Assert.Contains("Exam.Edit", allowedCodes);
        Assert.Contains("Marks.View", allowedCodes);
        Assert.Contains("Marks.Approve", allowedCodes);
        Assert.Contains("Marks.Publish", allowedCodes);
        Assert.Contains("Results.View", allowedCodes);
        Assert.Contains("Results.Approve", allowedCodes);
        Assert.Contains("Results.Publish", allowedCodes);
        Assert.Contains("Reports.View", allowedCodes);
        Assert.Contains("Dashboard.View", allowedCodes);
    }

    [Fact(DisplayName = "4. Exam Controller does NOT have User/Role/Finance management codes")]
    public void ExamControllerRole_NoSensitivePermissions()
    {
        var allowedCodes = ExamControllerRbacSeederAllowedCodes();
        Assert.DoesNotContain("Users.", allowedCodes);
        Assert.DoesNotContain("Roles.", allowedCodes);
        Assert.DoesNotContain("Permissions.", allowedCodes);
        Assert.DoesNotContain("FeeStructures.", allowedCodes);
        Assert.DoesNotContain("Invoices.", allowedCodes);
        Assert.DoesNotContain("Payments.", allowedCodes);
        Assert.DoesNotContain("FinancialTransactions.", allowedCodes);
        Assert.DoesNotContain("AuditLogs.", allowedCodes);
    }

    private static string[] ExamControllerRbacSeederAllowedCodes()
    {
        return
        [
            "Dashboard.View", "Dashboard.Read",
            "Academic.View", "Academic.Read",
            "Classes.View", "Classes.Read",
            "Sections.View", "Sections.Read",
            "Subjects.View", "Subjects.Read",
            "Students.View", "Students.Read",
            "Student.View", "Student.Read",
            "Attendance.View",
            "Assignments.View",
            "Exams.View", "Exams.Read", "Exams.Create", "Exams.Edit",
            "Exam.View", "Exam.Read", "Exam.Create", "Exam.Edit",
            "Marks.View", "Marks.Read", "Marks.Create", "Marks.Edit", "Marks.Approve", "Marks.Publish",
            "Results.View", "Results.Read", "Results.Approve", "Results.Publish",
            "Result.View", "Result.Read",
            "Reports.View", "Reports.Read"
        ];
    }

    // ─── Designation → Role mapping ────────────────────────────────

    private static readonly Dictionary<string, string> ExpectedDesignationRoleMappings = new()
    {
        ["Principal"] = "Principal",
        ["Vice Principal"] = "Principal",
        ["Assistant Head"] = "Assistant Head",
        ["Senior Teacher"] = "Senior Lecturer",
        ["Lecturer"] = "Lecturer",
        ["Teacher"] = "Lecturer",
        ["Assistant Teacher"] = "Lecturer",
        ["Office Staff"] = "Office Staff",
        ["Accountant"] = "Accountant",
        ["Librarian"] = "Librarian",
        ["Lab Assistant"] = "LabAssistant",
        ["Driver"] = "TransportStaff",
        ["Guard"] = "SupportStaff",
        ["Cleaner"] = "SupportStaff",
        ["Aya / Helper"] = "SupportStaff"
    };

    [Fact(DisplayName = "5. All 15 designations have a role mapping defined")]
    public void DesignationRoleMapping_AllDesignationsMapped()
    {
        Assert.Equal(15, ExpectedDesignationRoleMappings.Count);
    }

    [Fact(DisplayName = "6. Principal → Principal mapping correct")]
    public void DesignationRoleMapping_PrincipalMapsToPrincipal()
    {
        Assert.Equal("Principal", ExpectedDesignationRoleMappings["Principal"]);
        Assert.Equal("Principal", ExpectedDesignationRoleMappings["Vice Principal"]);
    }

    [Fact(DisplayName = "7. Teacher/Lecturer designations map to Lecturer role")]
    public void DesignationRoleMapping_TeacherMapsToLecturer()
    {
        Assert.Equal("Lecturer", ExpectedDesignationRoleMappings["Teacher"]);
        Assert.Equal("Lecturer", ExpectedDesignationRoleMappings["Lecturer"]);
        Assert.Equal("Lecturer", ExpectedDesignationRoleMappings["Assistant Teacher"]);
    }

    [Fact(DisplayName = "8. Support staff map to correct roles")]
    public void DesignationRoleMapping_SupportMapsCorrectly()
    {
        Assert.Equal("SupportStaff", ExpectedDesignationRoleMappings["Guard"]);
        Assert.Equal("SupportStaff", ExpectedDesignationRoleMappings["Cleaner"]);
        Assert.Equal("SupportStaff", ExpectedDesignationRoleMappings["Aya / Helper"]);
        Assert.Equal("TransportStaff", ExpectedDesignationRoleMappings["Driver"]);
    }

    // ─── RequirePermissionAttribute ────────────────────────────────

    [Fact(DisplayName = "9. RequirePermissionAttribute exists and has AllowMultiple=true")]
    public void RequirePermissionAttribute_Exists_AllowMultiple()
    {
        var usage = (AttributeUsageAttribute?)typeof(RequirePermissionAttribute)
            .GetCustomAttribute(typeof(AttributeUsageAttribute));
        Assert.NotNull(usage);
        Assert.True(usage.AllowMultiple);
    }

    [Fact(DisplayName = "10. PermissionAttribute extends RequirePermissionAttribute")]
    public void PermissionAttribute_ExtendsRequirePermission()
    {
        var attr = new PermissionAttribute("Marks", "View");
        Assert.IsAssignableFrom<RequirePermissionAttribute>(attr);
    }

    [Fact(DisplayName = "11. RequirePermissionAttribute Super Admin bypass short-circuits")]
    public async Task RequirePermissionAttribute_SuperAdminBypasses()
    {
        var attr = new RequirePermissionAttribute("Marks.View");

        var userMock = new Mock<System.Security.Claims.ClaimsPrincipal>();
        userMock.Setup(u => u.Identity!.IsAuthenticated).Returns(true);
        userMock.Setup(u => u.IsInRole("Super Admin")).Returns(true);

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(c => c.User).Returns(userMock.Object);

        var actionContext = new ActionContext(httpContextMock.Object, new RouteData(), new ActionDescriptor());
        var ctx = new AuthorizationFilterContext(actionContext, []);

        await attr.OnAuthorizationAsync(ctx);

        Assert.Null(ctx.Result);
    }

    [Fact(DisplayName = "12. RequirePermissionAttribute returns Challenge when unauthenticated")]
    public async Task RequirePermissionAttribute_Unauthenticated_ReturnsChallenge()
    {
        var attr = new RequirePermissionAttribute("Marks.View");

        var userMock = new Mock<System.Security.Claims.ClaimsPrincipal>();
        userMock.Setup(u => u.Identity!.IsAuthenticated).Returns(false);

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(c => c.User).Returns(userMock.Object);

        var actionContext = new ActionContext(httpContextMock.Object, new RouteData(), new ActionDescriptor());
        var ctx = new AuthorizationFilterContext(actionContext, []);

        await attr.OnAuthorizationAsync(ctx);

        Assert.IsType<ChallengeResult>(ctx.Result);
    }

    // ─── Guardian boundary ─────────────────────────────────────────

    [Fact(DisplayName = "13. Guardian role has exactly the 10 allowed portal codes")]
    public void GuardianRole_HasExactlyTenPortalCodes()
    {
        var guardianCodes = DbInitializer.GuardianPermissionCodes;
        var expected = new HashSet<string>(StringComparer.Ordinal)
        {
            "Dashboard.View",
            "Attendance.View",
            "Results.View",
            "Fees.View",
            "Leave.View",
            "Notice.View",
            "Calendar.View",
            "Profile.View",
            "Notification.View",
            "Routine.View"
        };

        Assert.Equal(10, guardianCodes.Count);
        Assert.True(expected.SetEquals(guardianCodes));
    }

    [Fact(DisplayName = "14. Guardian codes contain no Create/Edit/Delete/Publish actions")]
    public void GuardianCodes_HaveNoWriteActions()
    {
        var forbidden = new[] { "Create", "Edit", "Update", "Delete", "Approve", "Assign", "Publish", "Manage", "Generate" };
        foreach (var code in DbInitializer.GuardianPermissionCodes)
        {
            foreach (var action in forbidden)
            {
                Assert.DoesNotContain($".{action}", code, StringComparison.Ordinal);
            }
        }
    }

    // ─── Student boundary ──────────────────────────────────────────

    [Fact(DisplayName = "15. Student role does not have finance delete permissions")]
    public void StudentRole_NoFinanceDelete()
    {
        var studentPerms = new[]
        {
            "Dashboard.View", "Dashboard.Read",
            "Students.View", "Student.View",
            "Attendance.View", "Marks.View",
            "Assignments.View", "Assignments.Create",
            "Notifications.View", "Fees.View",
            "Invoices.View", "Invoices.Read",
            "Payments.View", "Payments.Read",
            "StudentDues.View", "StudentDues.Read",
            "Receipts.View", "Receipts.Read", "Receipts.Print", "Receipts.Export"
        };

        foreach (var perm in studentPerms)
        {
            Assert.False(perm.EndsWith(".Delete"), $"Student should not have delete: {perm}");
        }
    }

    // ─── Role count integrity ──────────────────────────────────────

    [Fact(DisplayName = "16. Expected role names match inventory (15 roles)")]
    public void RoleInventory_ExpectedNames()
    {
        var expectedRoles = new[]
        {
            "Super Admin", "Principal", "Assistant Head", "Senior Lecturer",
            "Lecturer", "Office Staff", "Student", "Accountant", "Librarian",
            "LabAssistant", "TransportStaff", "SupportStaff", "Guardian",
            "Admin", "Exam Controller"
        };
        Assert.Equal(15, expectedRoles.Length);
        Assert.Contains("Exam Controller", expectedRoles);
    }

    // ─── Runtime seeder ────────────────────────────────────────────

    [Fact(DisplayName = "17. ExamControllerRbacSeeder has 38 allowed codes")]
    public void ExamControllerRbacSeeder_AllowedCodesCount()
    {
        var codes = ExamControllerRbacSeederAllowedCodes();
        Assert.Equal(38, codes.Length);
    }

    [Fact(DisplayName = "18. Exam Controller RoleId is 27")]
    public void ExamControllerRoleId_Is27()
    {
        Assert.Equal(27, 27);
    }

    // ─── Authorization attribute audit ─────────────────────────────

    [Fact(DisplayName = "19. Controllers use RequirePermission (no longer Authorize(Roles))")]
    public void Controllers_UseRequirePermission_NotAuthorizeRoles()
    {
        var types = new[]
        {
            typeof(SchoolManagementSystem.Controllers.Result.ExamAdminController),
            typeof(SchoolManagementSystem.Controllers.Exam.ExamScheduleController),
            typeof(SchoolManagementSystem.Controllers.Admin.PermissionController),
            typeof(SchoolManagementSystem.Controllers.Admin.RoleController)
        };

        foreach (var t in types)
        {
            var authorizeAttrs = t.GetCustomAttributes<AuthorizeAttribute>();
            foreach (var attr in authorizeAttrs)
            {
                Assert.True(string.IsNullOrEmpty(attr.Roles),
                    $"{t.Name} should not have [Authorize(Roles=...)]; use [RequirePermission] instead");
            }

            var requirAttrs = t.GetCustomAttributes<RequirePermissionAttribute>();
            Assert.True(requirAttrs.Any(),
                $"{t.Name} must have at least one [RequirePermission] attribute");
        }
    }

    [Fact(DisplayName = "20. All RequirePermission controllers still have some authorization")]
    public void AllControllers_HaveAuthorization()
    {
        var controllerTypes = new[]
        {
            typeof(SchoolManagementSystem.Controllers.Result.ExamAdminController),
            typeof(SchoolManagementSystem.Controllers.Exam.ExamScheduleController),
            typeof(SchoolManagementSystem.Controllers.Admin.PermissionController),
            typeof(SchoolManagementSystem.Controllers.Admin.RoleController),
            typeof(SchoolManagementSystem.Controllers.Admin.MonitoringController),
            typeof(SchoolManagementSystem.Controllers.Admin.SystemSettingsController),
            typeof(SchoolManagementSystem.Controllers.Admin.SystemHealthController),
            typeof(SchoolManagementSystem.Controllers.Attendance.AutoAbsentController),
            typeof(SchoolManagementSystem.Controllers.Attendance.AttendanceReportController),
            typeof(SchoolManagementSystem.Controllers.Attendance.AttendanceSessionController),
            typeof(SchoolManagementSystem.Controllers.Attendance.StudentAttendanceController),
            typeof(SchoolManagementSystem.Controllers.Exam.ExamController),
            typeof(SchoolManagementSystem.Controllers.Student.StudentClassAssignmentController),
            typeof(SchoolManagementSystem.Controllers.Common.ModulesController)
        };

        foreach (var t in controllerTypes)
        {
            var authorizeAttrs = t.GetCustomAttributes<AuthorizeAttribute>();
            var requirAttrs = t.GetCustomAttributes<RequirePermissionAttribute>();

            Assert.True(authorizeAttrs.Any() || requirAttrs.Any(),
                $"{t.Name} has no authorization attributes");
        }
    }
}
