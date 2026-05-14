namespace SchoolManagementSystem.Constants;

/// <summary>
/// Centralized permission constants. Use these instead of magic strings in [RequirePermission] attributes.
/// </summary>
public static class Permissions
{
    // ── Employee ────────────────────────────────────────────────────────────
    public static class Employee
    {
        public const string View   = "Employee.View";
        public const string Create = "Employee.Create";
        public const string Update = "Employee.Update";
        public const string Delete = "Employee.Delete";
    }

    // ── Student ─────────────────────────────────────────────────────────────
    public static class Student
    {
        public const string View   = "Student.View";
        public const string Create = "Student.Create";
        public const string Update = "Student.Update";
        public const string Delete = "Student.Delete";
    }

    // ── Admission ───────────────────────────────────────────────────────────
    public static class Admission
    {
        public const string View    = "Admission.View";
        public const string Create  = "Admission.Create";
        public const string Update  = "Admission.Update";
        public const string Approve = "Admission.Approve";
        public const string Reject  = "Admission.Reject";
        public const string Delete  = "Admission.Delete";
    }

    // ── Result & Marks ──────────────────────────────────────────────────────
    public static class Result
    {
        public const string View    = "Result.View";
        public const string Create  = "Result.Create";
        public const string Update  = "Result.Update";
        public const string Delete  = "Result.Delete";
        public const string Enter   = "Result.Enter";
        public const string Approve = "Result.Approve";
        public const string Publish = "Result.Publish";
        public const string Lock    = "Result.Lock";
    }

    // ── Attendance ──────────────────────────────────────────────────────────
    public static class Attendance
    {
        public const string View   = "Attendance.View";
        public const string Mark   = "Attendance.Mark";
        public const string Update = "Attendance.Update";
    }

    // ── Leave ────────────────────────────────────────────────────────────────
    public static class Leave
    {
        public const string View     = "Leave.View";
        public const string ViewSelf = "Leave.ViewSelf";
        public const string Apply    = "Leave.Apply";
        public const string Approve  = "Leave.Approve";
        public const string Reject   = "Leave.Reject";
    }

    // ── Payroll ──────────────────────────────────────────────────────────────
    public static class Payroll
    {
        public const string View     = "Payroll.View";
        public const string ViewSelf = "Payroll.ViewSelf";
        public const string Generate = "Payroll.Generate";
        public const string Approve  = "Payroll.Approve";
        public const string Pay      = "Payroll.Pay";
        public const string Configure = "Payroll.Configure";
    }

    // ── Fees ─────────────────────────────────────────────────────────────────
    public static class Fees
    {
        public const string View    = "Fees.View";
        public const string Create  = "Fees.Create";
        public const string Collect = "Fees.Collect";
        public const string Waive   = "Fees.Waive";
    }

    // ── Exam ─────────────────────────────────────────────────────────────────
    public static class Exam
    {
        public const string View    = "Exam.View";
        public const string Create  = "Exam.Create";
        public const string Update  = "Exam.Update";
        public const string Delete  = "Exam.Delete";
        public const string Publish = "Exam.Publish";
    }

    // ── Settings ─────────────────────────────────────────────────────────────
    public static class Settings
    {
        public const string View   = "Settings.View";
        public const string Update = "Settings.Update";
    }

    // ── Reports ──────────────────────────────────────────────────────────────
    public static class Reports
    {
        public const string View     = "Reports.View";
        public const string Generate = "Reports.Generate";
        public const string Export   = "Reports.Export";
    }

    // ── Communication ────────────────────────────────────────────────────────
    public static class Communication
    {
        public const string View   = "Communication.View";
        public const string Send   = "Communication.Send";
        public const string Manage = "Communication.Manage";
    }

    // ── Library ──────────────────────────────────────────────────────────────
    public static class Library
    {
        public const string View  = "Library.View";
        public const string Issue = "Library.Issue";
        public const string Manage = "Library.Manage";
    }

    // ── Transport ────────────────────────────────────────────────────────────
    public static class Transport
    {
        public const string View   = "Transport.View";
        public const string Manage = "Transport.Manage";
    }

    // ── Dashboard ────────────────────────────────────────────────────────────
    public static class Dashboard
    {
        public const string View = "Dashboard.View";
    }
}
