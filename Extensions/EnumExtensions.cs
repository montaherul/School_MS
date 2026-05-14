using System.ComponentModel.DataAnnotations;
using System.Reflection;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Extensions;

/// <summary>
/// Extension methods for enum display helpers — GetDisplayName(), GetDescription().
/// </summary>
public static class EnumExtensions
{
    /// <summary>Returns the [Display(Name = "...")] attribute value, or the enum member name.</summary>
    public static string GetDisplayName(this Enum value)
    {
        var member = value.GetType()
            .GetMember(value.ToString())
            .FirstOrDefault();

        var display = member?.GetCustomAttribute<DisplayAttribute>();
        return display?.Name ?? value.ToString();
    }

    /// <summary>Returns a human-readable label with spaces inserted before capital letters.</summary>
    public static string ToLabel(this Enum value)
    {
        var name = value.ToString();
        return string.Concat(name.Select((c, i) =>
            i > 0 && char.IsUpper(c) ? " " + c.ToString() : c.ToString()));
    }

    /// <summary>Returns a Bootstrap badge CSS class based on common status enums.</summary>
    public static string ToBadgeClass(this Enum value) => value switch
    {
        LeaveStatus.Approved         => "badge bg-success",
        LeaveStatus.Rejected         => "badge bg-danger",
        LeaveStatus.Pending          => "badge bg-warning text-dark",
        LeaveStatus.Cancelled        => "badge bg-secondary",

        AttendanceStatus.Present     => "badge bg-success",
        AttendanceStatus.Absent      => "badge bg-danger",
        AttendanceStatus.Late        => "badge bg-warning text-dark",
        AttendanceStatus.Leave       => "badge bg-info",
        AttendanceStatus.HalfDay     => "badge bg-primary",
        AttendanceStatus.Holiday     => "badge bg-secondary",

        PayrollPaymentStatus.Paid    => "badge bg-success",
        PayrollPaymentStatus.Approved=> "badge bg-info",
        PayrollPaymentStatus.Pending => "badge bg-warning text-dark",
        PayrollPaymentStatus.Cancelled => "badge bg-danger",

        AccountStatus.Active         => "badge bg-success",
        AccountStatus.Inactive       => "badge bg-secondary",
        AccountStatus.Locked         => "badge bg-danger",
        AccountStatus.Pending        => "badge bg-warning text-dark",

        PublishStatus.Published      => "badge bg-success",
        PublishStatus.Approved       => "badge bg-info",
        PublishStatus.PendingApproval=> "badge bg-warning text-dark",
        PublishStatus.Draft          => "badge bg-secondary",

        _ => "badge bg-secondary"
    };

    /// <summary>Returns a Bootstrap text-color class.</summary>
    public static string ToTextClass(this Enum value) => value switch
    {
        LeaveStatus.Approved       => "text-success",
        LeaveStatus.Rejected       => "text-danger",
        LeaveStatus.Pending        => "text-warning",

        AttendanceStatus.Present   => "text-success",
        AttendanceStatus.Absent    => "text-danger",
        AttendanceStatus.Late      => "text-warning",

        _ => "text-secondary"
    };

    /// <summary>Returns a Bootstrap icon class that represents the status.</summary>
    public static string ToIcon(this Enum value) => value switch
    {
        LeaveStatus.Approved       => "bi bi-check-circle-fill text-success",
        LeaveStatus.Rejected       => "bi bi-x-circle-fill text-danger",
        LeaveStatus.Pending        => "bi bi-clock-fill text-warning",
        LeaveStatus.Cancelled      => "bi bi-dash-circle text-secondary",

        AttendanceStatus.Present   => "bi bi-check-circle-fill text-success",
        AttendanceStatus.Absent    => "bi bi-x-circle-fill text-danger",
        AttendanceStatus.Late      => "bi bi-exclamation-circle-fill text-warning",
        AttendanceStatus.Leave     => "bi bi-calendar-x text-info",

        PayrollPaymentStatus.Paid  => "bi bi-cash-stack text-success",
        PayrollPaymentStatus.Pending => "bi bi-hourglass-split text-warning",

        _ => "bi bi-circle text-secondary"
    };

    /// <summary>Gets all values of an enum as a list for dropdowns.</summary>
    public static IEnumerable<(TEnum Value, string Label)> GetSelectList<TEnum>()
        where TEnum : struct, Enum
        => Enum.GetValues<TEnum>().Select(e => (e, e.GetDisplayName()));
}
