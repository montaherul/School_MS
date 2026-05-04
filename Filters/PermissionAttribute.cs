namespace SchoolManagementSystem.Filters;

public class PermissionAttribute : RequirePermissionAttribute
{
    public PermissionAttribute(string moduleName, string action)
        : base($"{moduleName}.{action}")
    {
    }
}
