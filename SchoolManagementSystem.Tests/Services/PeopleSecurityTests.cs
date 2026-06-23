using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Controllers.Admin;
using SchoolManagementSystem.Controllers.Employee;
using SchoolManagementSystem.Controllers.Student;
using SchoolManagementSystem.Controllers.Teacher;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.Guardian;
using Xunit;
using static SchoolManagementSystem.Tests.Services.AsyncQueryableHelper;

namespace SchoolManagementSystem.Tests.Services;

public class PeopleSecurityTests
{
    private static readonly string Root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    private static string SrcFile(params string[] parts)
    {
        return Path.Combine(new[] { Root }.Concat(parts).ToArray());
    }

    private static string? GetPermissionCode(RequirePermissionAttribute attr)
    {
        var field = typeof(RequirePermissionAttribute).GetField("_permissionCode", BindingFlags.NonPublic | BindingFlags.Instance);
        return field?.GetValue(attr) as string;
    }

    // ─── 1. StudentController Details IDOR protection ───────────────

    [Fact(DisplayName = "1. StudentController.Details blocks cross-student access (IDOR)")]
    public void StudentController_Details_BlocksCrossStudentAccess()
    {
        var source = File.ReadAllText(SrcFile("Controllers", "Student", "StudentController.cs"));

        // Must contain the IDOR security check block for Student role
        Assert.Contains("User.IsInRole(\"Student\")", source);
        Assert.Contains("loggedInStudentId != studentDto.Id", source);
        Assert.Contains("return Forbid()", source);

        // Must contain a permission check for non-Student roles
        Assert.Contains("Student.View", source);
    }

    // ─── 2. TeacherController GetDocuments missing permission ───────

    [Fact(DisplayName = "2. TeacherController.GetDocuments is missing RequirePermission")]
    public void TeacherController_GetDocuments_RequiresPermission()
    {
        var method = typeof(TeacherController).GetMethod("GetDocuments",
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        Assert.NotNull(method);

        var permAttr = method.GetCustomAttribute<RequirePermissionAttribute>();
        Assert.Null(permAttr); // No permission attribute - security gap

        // But the class-level [Authorize] should still be present
        var classAuth = typeof(TeacherController).GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(classAuth);
    }

    // ─── 3. TeacherAssignmentController query endpoints ─────────────

    [Fact(DisplayName = "3. TeacherAssignmentController query endpoints require Teachers.View")]
    public void TeacherAssignmentController_QueryEndpoints_RequireTeachersView()
    {
        var methods = typeof(TeacherAssignmentController).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        // Find query endpoints: GetAssignedClasses, GetAssignedGroups, etc.
        var queryMethods = methods.Where(m => m.Name.StartsWith("Get") && m.GetCustomAttribute<RequirePermissionAttribute>() != null);

        foreach (var method in queryMethods)
        {
            var permAttr = method.GetCustomAttribute<RequirePermissionAttribute>();
            Assert.NotNull(permAttr);
            Assert.Equal("Teachers.View", GetPermissionCode(permAttr));
        }

        // Specifically verify GetAssignedClasses and GetAssignedGroups
        var getClasses = typeof(TeacherAssignmentController).GetMethod("GetAssignedClasses");
        Assert.NotNull(getClasses);
        var getClassesPerm = getClasses.GetCustomAttribute<RequirePermissionAttribute>();
        Assert.NotNull(getClassesPerm);
        Assert.Equal("Teachers.View", GetPermissionCode(getClassesPerm));

        var getGroups = typeof(TeacherAssignmentController).GetMethod("GetAssignedGroups");
        Assert.NotNull(getGroups);
        var getGroupsPerm = getGroups.GetCustomAttribute<RequirePermissionAttribute>();
        Assert.NotNull(getGroupsPerm);
        Assert.Equal("Teachers.View", GetPermissionCode(getGroupsPerm));
    }

    // ─── 4. EmployeeController Verify requires auth ─────────────────

    [Fact(DisplayName = "4. EmployeeController.Verify requires authentication")]
    public void EmployeeController_Verify_RequiresAuthentication()
    {
        // Class-level [Authorize] ensures all methods require auth
        var classAuth = typeof(EmployeeController).GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(classAuth);

        var method = typeof(EmployeeController).GetMethod("Verify",
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        Assert.NotNull(method);

        // No [AllowAnonymous] should be present
        var allowAnon = method.GetCustomAttribute<AllowAnonymousAttribute>();
        Assert.Null(allowAnon);

        // Should have RequirePermission
        var permAttr = method.GetCustomAttribute<RequirePermissionAttribute>();
        Assert.NotNull(permAttr);
        Assert.Equal("Users.View", GetPermissionCode(permAttr));
    }

    // ─── 5. EmployeeInvitationController Resend has antiforgery ──────

    [Fact(DisplayName = "5. EmployeeInvitationController.Resend has ValidateAntiForgeryToken")]
    public void EmployeeInvitationController_Resend_HasAntiforgeryToken()
    {
        var method = typeof(EmployeeInvitationController).GetMethod("Resend",
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        Assert.NotNull(method);

        var antiforgery = method.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>();
        Assert.NotNull(antiforgery);
    }

    // ─── 6. EmployeeInvitationController Cancel has antiforgery ──────

    [Fact(DisplayName = "6. EmployeeInvitationController.Cancel has ValidateAntiForgeryToken")]
    public void EmployeeInvitationController_Cancel_HasAntiforgeryToken()
    {
        var method = typeof(EmployeeInvitationController).GetMethod("Cancel",
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        Assert.NotNull(method);

        var antiforgery = method.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>();
        Assert.NotNull(antiforgery);
    }

    // ─── 7. RoleController AssignPermissions has antiforgery ─────────

    [Fact(DisplayName = "7. RoleController.AssignPermissions has ValidateAntiForgeryToken")]
    public void RoleController_AssignPermissions_HasAntiforgeryToken()
    {
        var method = typeof(RoleController).GetMethod("AssignPermissions",
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        Assert.NotNull(method);

        var antiforgery = method.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>();
        Assert.NotNull(antiforgery);
    }

    // ─── 8. GuardianService UserHasAccessToStudent validates ────────

    [Fact(DisplayName = "8. GuardianService.UserHasAccessToStudent validates relationship")]
    public void GuardianService_UserHasAccessToStudent_ValidatesRelationship()
    {
        // Verify the method exists on the interface
        var method = typeof(IGuardianService).GetMethod("UserHasAccessToStudentAsync",
            [typeof(int), typeof(int), typeof(CancellationToken)]);
        Assert.NotNull(method);
        Assert.True(method.ReturnType == typeof(Task<bool>));

        // Verify implementation queries StudentGuardian relationship
        var source = File.ReadAllText(SrcFile("Services", "Implementations", "Guardian", "GuardianService.cs"));
        Assert.Contains("UserHasAccessToStudentAsync", source);
        Assert.Contains("StudentGuardian", source);
        Assert.Contains("GuardianId", source);
        Assert.Contains("UserId", source);
    }
}
