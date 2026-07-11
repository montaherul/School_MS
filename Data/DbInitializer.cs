using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.Communication;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Library;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Entities.System;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.Entities.Guardian;

namespace SchoolManagementSystem.Data;

public static class DbInitializer
{
    /// <summary>
    /// Guardian role ID (single source of truth).
    /// </summary>
    public const int GuardianRoleId = 25;

    /// <summary>
    /// Parent role ID (alias for Guardian).
    /// </summary>
    public const int ParentRoleId = 31;

    /// <summary>
    /// EXACT permission codes the Guardian role is allowed to have.
    /// This is the single source of truth — both the EF seed AND the runtime
    /// enforcer (<see cref="GuardianRbacEnforcer"/>) read from this set.
    /// Any Guardian permission not in this set must be removed.
    /// DO NOT add Create/Edit/Update/Delete/Approve/Assign/Publish/Manage/Generate
    /// actions here — Guardian is strictly read-only on its own children.
    /// </summary>
    public static readonly IReadOnlySet<string> GuardianPermissionCodes = new HashSet<string>(StringComparer.Ordinal)
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

    private static string GetClassName(int i)
    {
        return i switch
        {
            1 => "One",
            2 => "Two",
            3 => "Three",
            4 => "Four",
            5 => "Five",
            6 => "Six",
            7 => "Seven",
            8 => "Eight",
            9 => "Nine",
            10 => "Ten",
            _ => i.ToString()
        };
    }
    public static void Seed(ModelBuilder modelBuilder)
    {

        var createdAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Super Admin", Description = "System owner with all permissions", CreatedAt = createdAt },
            new Role { Id = 2, Name = "Principal", Description = "Final approval and all modules", CreatedAt = createdAt },
            new Role { Id = 3, Name = "Assistant Head", Description = "Academic operations", CreatedAt = createdAt },
            new Role { Id = 4, Name = "Senior Lecturer", Description = "Teaching and review", CreatedAt = createdAt },
            new Role { Id = 5, Name = "Lecturer", Description = "Teaching operations", CreatedAt = createdAt },
            new Role { Id = 6, Name = "Office Staff", Description = "Admission, fees, reports", CreatedAt = createdAt },
            new Role { Id = 7, Name = "Student", Description = "Student portal access", CreatedAt = createdAt },
            new Role { Id = 20, Name = "Accountant", Description = "Accounts and finance", CreatedAt = createdAt },
            new Role { Id = 21, Name = "Librarian", Description = "Library services", CreatedAt = createdAt },
            new Role { Id = 22, Name = "LabAssistant", Description = "Lab assistance", CreatedAt = createdAt },
            new Role { Id = 23, Name = "TransportStaff", Description = "Transport services", CreatedAt = createdAt },
            new Role { Id = 24, Name = "SupportStaff", Description = "Support and cleaning", CreatedAt = createdAt },
            new Role { Id = 25, Name = "Guardian", Description = "Guardian portal access", CreatedAt = createdAt },
            new Role { Id = 26, Name = "Admin", Description = "Administrator", CreatedAt = createdAt },
            new Role { Id = 27, Name = "Exam Controller", Description = "Exam and result management operations", CreatedAt = createdAt });

        modelBuilder.Entity<ApplicationUser>().HasData(
            new ApplicationUser
            {
                Id = 1,
                UserName = "admin",
                Email = "admin@school.local",
                PasswordHash = "ChangeThisHash",
                Status = AccountStatus.Active,
                IsEmailConfirmed = true,
                ActivationToken = null,
                ActivationTokenExpiry = null,
                CreatedAt = createdAt
            });

        modelBuilder.Entity<UserRole>().HasData(new UserRole { UserId = 1, RoleId = 1 });

        var permissionId = 1;
        var modules = new[]
        {
            "Dashboard", "Users", "Roles", "Permissions", "Admissions", "Students", "Teachers", "Classes",
            "Sections", "Subjects", "Attendance", "Exams", "Marks", "Assignments", "Fees", "Payments",
            "Library", "Transport", "Health", "Notifications", "Reports", "Settings", "Academic",
            "Admission", "Student", "Exam", "Result", "Communication", "System", "AuditLogs",
            "FeeStructures", "Invoices", "Scholarships", "Waivers", "StudentDues", "FinancialTransactions",
            "FinanceReports", "FinanceConfiguration", "FinanceDashboard", "Receipts",
            "Routine",
            // Guardian portal modules (added for Guardian Management System)
            "Results", "Leave", "Notice", "Calendar", "Profile", "Notification"
        };
        var actions = new[]
        {
            "View", "Read", "Create", "Edit", "Update", "Delete", "Approve", "Assign", "Publish", "Export",
            "Print", "Generate", "Manage"
        };
        var permissions = modules.SelectMany(module => actions
            .Select(action => new Permission
            {
                Id = permissionId++,
                Module = module,
                ModuleName = module,
                Action = action,
                Code = $"{module}.{action}",
                CanCreate = action is "Create" or "Generate" or "Manage",
                CanRead = action is "View" or "Read" or "Export" or "Print" or "Generate" or "Manage",
                CanUpdate = action is "Edit" or "Update" or "Approve" or "Assign" or "Publish" or "Manage",
                CanDelete = action is "Delete" or "Manage",
                CreatedAt = createdAt
            })).ToArray();
        // Custom permissions appended at fixed IDs (no longer part of matrix to avoid
        // sequential ID shifts). These are for module/action combos needed by specific
        // roles but absent from the 46x13 matrix above.
        var customPermissions = new Permission[]
        {
            new() { Id = 612, Module = "Library", ModuleName = "Library", Action = "Issue", Code = "Library.Issue",
                CanCreate = false, CanRead = true, CanUpdate = true, CanDelete = false, CreatedAt = createdAt },
            new() { Id = 613, Module = "Library", ModuleName = "Library", Action = "Return", Code = "Library.Return",
                CanCreate = false, CanRead = true, CanUpdate = true, CanDelete = false, CreatedAt = createdAt },
            new() { Id = 614, Module = "Laboratory", ModuleName = "Laboratory", Action = "View", Code = "Laboratory.View",
                CanCreate = false, CanRead = true, CanUpdate = false, CanDelete = false, CreatedAt = createdAt },
            new() { Id = 615, Module = "Laboratory", ModuleName = "Laboratory", Action = "Manage", Code = "Laboratory.Manage",
                CanCreate = true, CanRead = true, CanUpdate = true, CanDelete = true, CreatedAt = createdAt },
            new() { Id = 616, Module = "Calendar", ModuleName = "Calendar", Action = "Regenerate", Code = "Calendar.Regenerate",
                CanCreate = true, CanRead = true, CanUpdate = true, CanDelete = false, CreatedAt = createdAt },
            new() { Id = 617, Module = "Calendar", ModuleName = "Calendar", Action = "Repair", Code = "Calendar.Repair",
                CanCreate = true, CanRead = true, CanUpdate = true, CanDelete = false, CreatedAt = createdAt },
        };
        var allPermissions = permissions.Concat(customPermissions).ToArray();
        modelBuilder.Entity<Permission>().HasData(allPermissions);

        var financeModules = new HashSet<string>
        {
            "FeeStructures", "Invoices", "Payments", "Scholarships", "Waivers", "StudentDues",
            "FinancialTransactions", "FinanceReports", "FinanceConfiguration", "FinanceDashboard", "Receipts"
        };

        var adminRolePermissions = allPermissions.Select(p => new RolePermission { RoleId = 1, PermissionId = p.Id });
        var principalRolePermissions = allPermissions
            .Where(p => !financeModules.Contains(p.ModuleName) ||
                p.Code is
                    "FinanceDashboard.View" or "FinanceDashboard.Read" or
                    "FinanceReports.View" or "FinanceReports.Read" or "FinanceReports.Export" or "FinanceReports.Print" or
                    "Payments.View" or "Payments.Read" or
                    "Invoices.View" or "Invoices.Read" or
                    "StudentDues.View" or "StudentDues.Read" or
                    "Scholarships.View" or "Scholarships.Read" or "Scholarships.Approve" or
                    "Waivers.View" or "Waivers.Read" or "Waivers.Approve")
            .Select(p => new RolePermission { RoleId = 2, PermissionId = p.Id });
        var assistantHeadRolePermissions = allPermissions
            .Where(p =>
                p.ModuleName is "Dashboard" or "Academic" or "Classes" or "Sections" or "Subjects" or "Admissions" or "Admission" or "Students" or "Student" or "Attendance" or "Exams" or "Exam" or "Marks" or "Result" or "Communication" or "Reports" or "Calendar")
            .Select(p => new RolePermission { RoleId = 3, PermissionId = p.Id });
        // Senior Lecturer: senior teaching role — full attendance/marks/results authority
        var seniorLecturerRolePermissions = allPermissions
            .Where(p => p.Code is
                "Dashboard.View" or
                "Attendance.View" or "Attendance.Create" or "Attendance.Edit" or
                "Marks.View" or "Marks.Create" or "Marks.Edit" or
                "Results.View" or "Result.View" or
                "Assignments.View" or "Assignments.Create" or
                "Reports.View" or
                "Exams.View" or "Exam.View")
            .Select(p => new RolePermission { RoleId = 4, PermissionId = p.Id });
        var teacherRolePermissions = allPermissions
            .Where(p =>
                p.Code is "Dashboard.View" or "Classes.View" or "Students.View" or "Attendance.View" or "Attendance.Create" or "Marks.View" or "Marks.Create" or "Assignments.View" or "Assignments.Create" ||
                p.Code is "Reports.View" or "Exam.View" or "Exams.View")
            .Select(p => new RolePermission { RoleId = 5, PermissionId = p.Id });
        var officeRolePermissions = allPermissions
            .Where(p =>
                p.ModuleName is "Dashboard" or "Admissions" or "Admission" or "Students" or "Student" or "Fees" or "Payments" or "Reports" &&
                p.Action is not "Delete")
            .Select(p => new RolePermission { RoleId = 6, PermissionId = p.Id });
        var studentRolePermissions = allPermissions
            .Where(p =>
                p.Code is "Dashboard.View" or "Dashboard.Read" or "Students.View" or "Student.View" or "Attendance.View" or "Marks.View" or "Assignments.View" or "Assignments.Create" or "Notifications.View" or "Fees.View" or
                    "Invoices.View" or "Invoices.Read" or "Payments.View" or "Payments.Read" or "StudentDues.View" or "StudentDues.Read" or
                    "Receipts.View" or "Receipts.Read" or "Receipts.Print" or "Receipts.Export")
            .Select(p => new RolePermission { RoleId = 7, PermissionId = p.Id });
        var accountantRolePermissions = allPermissions.Where(p => financeModules.Contains(p.ModuleName) || p.Code is "Dashboard.View" or "Dashboard.Read").Select(p => new RolePermission { RoleId = 20, PermissionId = p.Id });
        // Librarian: library management (catalog + issue/return books)
        var librarianRolePermissions = allPermissions
            .Where(p => p.Code is
                "Library.View" or "Library.Create" or "Library.Edit" or "Library.Delete" or
                "Library.Issue" or "Library.Return" or
                "Reports.View")
            .Select(p => new RolePermission { RoleId = 21, PermissionId = p.Id });
        // LabAssistant: manage lab equipment
        var labAssistantRolePermissions = allPermissions
            .Where(p => p.Code is "Laboratory.View" or "Laboratory.Manage")
            .Select(p => new RolePermission { RoleId = 22, PermissionId = p.Id });
        // TransportStaff: manage transport routes
        var transportStaffRolePermissions = allPermissions
            .Where(p => p.Code is "Transport.View" or "Transport.Edit")
            .Select(p => new RolePermission { RoleId = 23, PermissionId = p.Id });
        // SupportStaff: minimal dashboard access
        var supportStaffRolePermissions = allPermissions
            .Where(p => p.Code is "Dashboard.View")
            .Select(p => new RolePermission { RoleId = 24, PermissionId = p.Id });
        // Guardian Management System: grant Guardian role ONLY the 9 portal-specific
        // permissions declared in GuardianPermissionCodes (source of truth).
        var guardianRolePermissions = allPermissions
            .Where(p => GuardianPermissionCodes.Contains(p.Code))
            .Select(p => new RolePermission { RoleId = GuardianRoleId, PermissionId = p.Id });
        var applicationAdminRolePermissions = allPermissions.Select(p => new RolePermission { RoleId = 26, PermissionId = p.Id });
        // Exam Controller: exam/result management — admin-level exam/result operations without full system access
        var examControllerRolePermissions = allPermissions
            .Where(p => p.Code is
                "Dashboard.View" or "Dashboard.Read" or
                "Academic.View" or "Academic.Read" or
                "Classes.View" or "Classes.Read" or
                "Sections.View" or "Sections.Read" or
                "Subjects.View" or "Subjects.Read" or
                "Students.View" or "Students.Read" or
                "Student.View" or "Student.Read" or
                "Attendance.View" or
                "Assignments.View" or
                "Exams.View" or "Exams.Read" or "Exams.Create" or "Exams.Edit" or
                "Exam.View" or "Exam.Read" or "Exam.Create" or "Exam.Edit" or
                "Marks.View" or "Marks.Read" or "Marks.Create" or "Marks.Edit" or "Marks.Approve" or "Marks.Publish" or
                "Results.View" or "Results.Read" or "Results.Approve" or "Results.Publish" or
                "Result.View" or "Result.Read" or
                "Reports.View" or "Reports.Read")
            .Select(p => new RolePermission { RoleId = 27, PermissionId = p.Id });
        modelBuilder.Entity<RolePermission>().HasData(
            adminRolePermissions
            .Concat(principalRolePermissions)
            .Concat(assistantHeadRolePermissions)
            .Concat(seniorLecturerRolePermissions)
            .Concat(teacherRolePermissions)
            .Concat(officeRolePermissions)
            .Concat(studentRolePermissions)
            .Concat(accountantRolePermissions)
            .Concat(librarianRolePermissions)
            .Concat(labAssistantRolePermissions)
            .Concat(transportStaffRolePermissions)
            .Concat(supportStaffRolePermissions)
            .Concat(guardianRolePermissions)
            .Concat(applicationAdminRolePermissions)
            .Concat(examControllerRolePermissions));

        modelBuilder.Entity<AcademicYear>().HasData(new AcademicYear { Id = 1, Name = "2026", StartsOn = new DateTime(2026, 1, 1), EndsOn = new DateTime(2026, 12, 31), IsActive = true, CreatedAt = createdAt });
        //modelBuilder.Entity<SchoolClass>().HasData(
        //    new SchoolClass { Id = 1, Name = "Class One", SortOrder = 1, CreatedAt = createdAt },
        //    new SchoolClass { Id = 2, Name = "Class Two", SortOrder = 2, CreatedAt = createdAt },

        //    new SchoolClass { Id = 3, Name = "Class Three", SortOrder = 3, CreatedAt = createdAt });


        modelBuilder.Entity<SchoolClass>().HasData(
     new SchoolClass { Id = 1, Name = "Class One", SortOrder = 1, IsGroupBased = false, CreatedAt = createdAt },
     new SchoolClass { Id = 2, Name = "Class Two", SortOrder = 2, IsGroupBased = false, CreatedAt = createdAt },
     new SchoolClass { Id = 3, Name = "Class Three", SortOrder = 3, IsGroupBased = false, CreatedAt = createdAt },
     new SchoolClass { Id = 4, Name = "Class Four", SortOrder = 4, IsGroupBased = false, CreatedAt = createdAt },
     new SchoolClass { Id = 5, Name = "Class Five", SortOrder = 5, IsGroupBased = false, CreatedAt = createdAt },
     new SchoolClass { Id = 6, Name = "Class Six", SortOrder = 6, IsGroupBased = false, CreatedAt = createdAt },
     new SchoolClass { Id = 7, Name = "Class Seven", SortOrder = 7, IsGroupBased = false, CreatedAt = createdAt },
     new SchoolClass { Id = 8, Name = "Class Eight", SortOrder = 8, IsGroupBased = false, CreatedAt = createdAt },
     new SchoolClass { Id = 9, Name = "Class Nine", SortOrder = 9, IsGroupBased = true, CreatedAt = createdAt },
     new SchoolClass { Id = 10, Name = "Class Ten", SortOrder = 10, IsGroupBased = true, CreatedAt = createdAt }
 );

        var sections = new List<Section>();
        int id = 1;

        // Class 1–8: flat A/B sections (IDs 1–16)
        for (int cls = 1; cls <= 8; cls++)
        {
            sections.Add(new Section { Id = id++, SchoolClassId = cls, Name = "A", CreatedAt = createdAt });
            sections.Add(new Section { Id = id++, SchoolClassId = cls, Name = "B", CreatedAt = createdAt });
        }

        // Class 9 & 10: Group containers + 2 leaf sub-sections each
        // Groups: Science, Business Studies, Humanities
        // Each group gets sub-sections: "{Group} A", "{Group} B"
        // Admin can auto-add "{Group} C" once group reaches 100+ students
        var groupNameToId = new Dictionary<string, int>
        {
            ["Science"] = 1,
            ["Business Studies"] = 2,
            ["Humanities"] = 3
        };
        foreach (var cls in new[] { 9, 10 })
        {
            var groups = new[] { "Science", "Business Studies", "Humanities" };
            foreach (var groupName in groups)
            {
                int groupId = id++;
                int studentGroupId = groupNameToId[groupName];
                // Parent group section (ParentSectionId = null = it IS the group)
                sections.Add(new Section
                {
                    Id = groupId,
                    SchoolClassId = cls,
                    Name = groupName,
                    ParentSectionId = null,
                    StudentGroupId = studentGroupId,
                    CreatedAt = createdAt
                });
                // Leaf sub-sections A and B
                sections.Add(new Section
                {
                    Id = id++,
                    SchoolClassId = cls,
                    Name = $"{groupName} A",
                    ParentSectionId = groupId,
                    StudentGroupId = studentGroupId,
                    CreatedAt = createdAt
                });
                sections.Add(new Section
                {
                    Id = id++,
                    SchoolClassId = cls,
                    Name = $"{groupName} B",
                    ParentSectionId = groupId,
                    StudentGroupId = studentGroupId,
                    CreatedAt = createdAt
                });
            }
        }

        modelBuilder.Entity<Section>().HasData(sections);





        //modelBuilder.Entity<Section>().HasData(
        //    new Section { Id = 1, SchoolClassId = 1, Name = "A", CreatedAt = createdAt },
        //    new Section { Id = 2, SchoolClassId = 1, Name = "B", CreatedAt = createdAt },

        //    new Section { Id = 3, SchoolClassId = 2, Name = "A", CreatedAt = createdAt });
        modelBuilder.Entity<Subject>().HasData(

            // Primary Subjects (General education for classes 1-5)
            new Subject { Id = 1, Code = "BAN", Name = "বাংলা", NameBn = "বাংলা", Category = "Core", CreatedAt = createdAt, SubjectGroup = "General" },
            new Subject { Id = 2, Code = "ENG", Name = "English", NameBn = "ইংরেজি", Category = "Core", CreatedAt = createdAt, SubjectGroup = "General" },
            new Subject { Id = 3, Code = "MAT", Name = "Mathematics", NameBn = "গণিত", Category = "Core", CreatedAt = createdAt, SubjectGroup = "General" },
            new Subject { Id = 4, Code = "GSCI", Name = "General Science", NameBn = "সাধারণ বিজ্ঞান", Category = "Core", CreatedAt = createdAt, SubjectGroup = "General" },
            new Subject { Id = 5, Code = "SOC", Name = "Bangladesh and Global Studies", NameBn = "বাংলাদেশ ও বিশ্ব পরিচয়", Category = "Core", CreatedAt = createdAt, SubjectGroup = "General" },
            new Subject { Id = 6, Code = "REL", Name = "Religion and Moral Education", NameBn = "ধর্ম ও নৈতিক শিক্ষা", Category = "Core", CreatedAt = createdAt, SubjectGroup = "General" },
            new Subject { Id = 7, Code = "ART", Name = "Arts and Crafts", NameBn = "চারুকলা", Category = "Core", CreatedAt = createdAt, SubjectGroup = "General" },
            new Subject { Id = 8, Code = "PE", Name = "Physical Education", NameBn = "শারীরিক শিক্ষা", Category = "Core", CreatedAt = createdAt, SubjectGroup = "General" },

            // SSC Common Subjects (General education for classes 6-10)
            new Subject { Id = 9, Code = "BAN1", Name = "Bangla 1st Paper", NameBn = "বাংলা ১ম পত্র", Category = "Core", CreatedAt = createdAt, SubjectGroup = "General" },
            new Subject { Id = 10, Code = "BAN2", Name = "Bangla 2nd Paper", NameBn = "বাংলা ২য় পত্র", Category = "Core", CreatedAt = createdAt, SubjectGroup = "General" },
            new Subject { Id = 11, Code = "ENG1", Name = "English 1st Paper", NameBn = "ইংরেজি ১ম পত্র", Category = "Core", CreatedAt = createdAt, SubjectGroup = "General" },
            new Subject { Id = 12, Code = "ENG2", Name = "English 2nd Paper", NameBn = "ইংরেজি ২য় পত্র", Category = "Core", CreatedAt = createdAt, SubjectGroup = "General" },
            new Subject { Id = 13, Code = "SCI", Name = "Science", NameBn = "বিজ্ঞান", Category = "Core", CreatedAt = createdAt, SubjectGroup = "General" },
            new Subject { Id = 14, Code = "ICT", Name = "Information and Communication Technology", NameBn = "তথ্য ও যোগাযোগ প্রযুক্তি", Category = "Core", CreatedAt = createdAt, SubjectGroup = "General" },
            new Subject { Id = 15, Code = "AGR", Name = "Agriculture Studies", NameBn = "কৃষি শিক্ষা", Category = "Vocational", CreatedAt = createdAt, SubjectGroup = "General" },

            // Science Group
            new Subject { Id = 16, Code = "PHY", Name = "Physics", NameBn = "পদার্থবিজ্ঞান", Category = "Core", CreatedAt = createdAt, SubjectGroup = "Science" },
            new Subject { Id = 17, Code = "CHE", Name = "Chemistry", NameBn = "রসায়ন", Category = "Core", CreatedAt = createdAt, SubjectGroup = "Science" },
            new Subject { Id = 18, Code = "BIO", Name = "Biology", NameBn = "জীববিজ্ঞান", Category = "Core", CreatedAt = createdAt, SubjectGroup = "Science" },
            new Subject { Id = 19, Code = "HMA", Name = "Higher Mathematics", NameBn = "উচ্চতর গণিত", Category = "Core", CreatedAt = createdAt, SubjectGroup = "Science" },

            // Business Studies
            new Subject { Id = 20, Code = "ACC", Name = "Accounting", NameBn = "হিসাববিজ্ঞান", Category = "Core", CreatedAt = createdAt, SubjectGroup = "BusinessStudies" },
            new Subject { Id = 21, Code = "FIN", Name = "Finance and Banking", NameBn = "ফাইন্যান্স", Category = "Core", CreatedAt = createdAt, SubjectGroup = "BusinessStudies" },
            new Subject { Id = 22, Code = "BUS", Name = "Business Entrepreneurship", NameBn = "ব্যবসায় উদ্যোগ", Category = "Core", CreatedAt = createdAt, SubjectGroup = "BusinessStudies" },

            // Humanities
            new Subject { Id = 23, Code = "HIS", Name = "History", NameBn = "ইতিহাস", Category = "Core", CreatedAt = createdAt, SubjectGroup = "Humanities" },
            new Subject { Id = 24, Code = "GEO", Name = "Geography and Environment", NameBn = "ভূগোল ও পরিবেশ", Category = "Core", CreatedAt = createdAt, SubjectGroup = "Humanities" },
            new Subject { Id = 25, Code = "ECO", Name = "Economics", NameBn = "অর্থনীতি", Category = "Core", CreatedAt = createdAt, SubjectGroup = "Humanities" },
            new Subject { Id = 26, Code = "CIV", Name = "Civics", NameBn = "নাগরিকতা", Category = "Core", CreatedAt = createdAt, SubjectGroup = "Humanities" },

            // Others (General subjects)
            new Subject { Id = 27, Code = "CAREER", Name = "Career Education", NameBn = "ক্যারিয়ার শিক্ষা", Category = "Core", CreatedAt = createdAt, SubjectGroup = "General" },
            new Subject { Id = 28, Code = "HEALTH", Name = "Physical Education, Health and Sports", NameBn = "শারীরিক শিক্ষা, স্বাস্থ্য ও খেলাধুলা", Category = "Core", CreatedAt = createdAt, SubjectGroup = "General" },
            new Subject { Id = 29, Code = "HSC", Name = "Home Science", NameBn = "গার্হস্থ্য বিজ্ঞান", Category = "Vocational", CreatedAt = createdAt, SubjectGroup = "General" },

            // Music (NCTB compulsory for classes 1-8)
            new Subject { Id = 34, Code = "MUS", Name = "Music", NameBn = "সঙ্গীত", Category = "Core", SubjectGroup = "General", IsMandatory = true, CreatedAt = createdAt },

            // Religion & Moral Education
            new Subject { Id = 30, Code = "IRE", Name = "Islam and Moral Education", NameBn = "ইসলাম ও নৈতিক শিক্ষা", Category = "Religion", SubjectGroup = "Religion", IsReligionSubject = true, ReligionType = "Islam", CreatedAt = createdAt },
            new Subject { Id = 31, Code = "HRE", Name = "Hindu Religion and Moral Education", NameBn = "হিন্দুধর্ম ও নৈতিক শিক্ষা", Category = "Religion", SubjectGroup = "Religion", IsReligionSubject = true, ReligionType = "Hindu", CreatedAt = createdAt },
            new Subject { Id = 32, Code = "BRE", Name = "Buddhist Religion and Moral Education", NameBn = "বৌদ্ধধর্ম ও নৈতিক শিক্ষা", Category = "Religion", SubjectGroup = "Religion", IsReligionSubject = true, ReligionType = "Buddhist", CreatedAt = createdAt },
            new Subject { Id = 33, Code = "CRE", Name = "Christian Religion and Moral Education", NameBn = "খ্রিস্টধর্ম ও নৈতিক শিক্ষা", Category = "Religion", SubjectGroup = "Religion", IsReligionSubject = true, ReligionType = "Christian", CreatedAt = createdAt }
        );

        modelBuilder.Entity<Student>().HasData(
            new Student { Id = 1, StudentNo = "STU-2026-0001", FullName = "Sample Student One", DateOfBirth = new DateTime(2018, 2, 1), Gender = "Male", FatherName = "Father One", MotherName = "Mother One", MobileNumber = "01700000010", Nationality = "Bangladeshi", Country = "Bangladesh", MaritalStatus = "Single", Religion = "Islam", AssignedReligionSubjectId = 30, ClassId = 1, SectionId = 1, RollNumber = 1, Status = StudentStatus.Active, CreatedAt = createdAt },
            new Student { Id = 2, StudentNo = "STU-2026-0002", FullName = "Sample Student Two", DateOfBirth = new DateTime(2018, 5, 11), Gender = "Female", FatherName = "Father Two", MotherName = "Mother Two", MobileNumber = "01700000020", Nationality = "Bangladeshi", Country = "Bangladesh", MaritalStatus = "Single", Religion = "Islam", AssignedReligionSubjectId = 30, ClassId = 1, SectionId = 1, RollNumber = 2, Status = StudentStatus.Active, CreatedAt = createdAt });

        modelBuilder.Entity<SchoolManagementSystem.Models.Entities.Guardian.Guardian>().HasData(
            new SchoolManagementSystem.Models.Entities.Guardian.Guardian 
            { 
                Id = 1, 
                GuardianCode = "GRD-00001",
                FirstName = "Guardian", 
                LastName = "One",
                FullName = "Guardian One",
                Gender = "Male",
                RelationType = SchoolManagementSystem.Models.Entities.Guardian.GuardianRelationshipType.Father, 
                MobileNumber = "01700000001", 
                Status = SchoolManagementSystem.Models.Entities.Guardian.GuardianStatus.Active,
                CreatedAt = createdAt 
            },
            new SchoolManagementSystem.Models.Entities.Guardian.Guardian 
            { 
                Id = 2, 
                GuardianCode = "GRD-00002",
                FirstName = "Guardian", 
                LastName = "Two",
                FullName = "Guardian Two",
                Gender = "Female",
                RelationType = SchoolManagementSystem.Models.Entities.Guardian.GuardianRelationshipType.Mother, 
                MobileNumber = "01700000002", 
                Status = SchoolManagementSystem.Models.Entities.Guardian.GuardianStatus.Active,
                CreatedAt = createdAt 
            });

        modelBuilder.Entity<StudentGuardian>().HasData(
            new StudentGuardian { Id = 1, StudentId = 1, GuardianId = 1, Relationship = SchoolManagementSystem.Models.Entities.Guardian.GuardianRelationshipType.Father, IsPrimaryGuardian = true, CreatedAt = createdAt },
            new StudentGuardian { Id = 2, StudentId = 2, GuardianId = 2, Relationship = SchoolManagementSystem.Models.Entities.Guardian.GuardianRelationshipType.Mother, IsPrimaryGuardian = true, CreatedAt = createdAt });

        modelBuilder.Entity<AdmissionApplication>().HasData(
            new AdmissionApplication { Id = 1, ApplicationNo = "APP-2026-0001", ApplicantName = "Pending Applicant", DateOfBirth = new DateTime(2019, 4, 1), Gender = "Female", FatherName = "Applicant Father", MotherName = "Applicant Mother", GuardianName = "Applicant Guardian", ApplicantMobileNumber = "01800000010", FatherOrGuardianMobileNo = "01800000001", Nationality = "Bangladeshi", Country = "Bangladesh", MaritalStatus = "Single", Religion = "Islam", AppliedClassId = 1, Status = AdmissionStatus.Pending, AdmissionFee = 1500, CreatedAt = createdAt });

        modelBuilder.Entity<AttendanceRecord>().HasData(
            new AttendanceRecord { Id = 1, StudentId = 1, SchoolClassId = 1, SectionId = 1, AttendanceDate = new DateOnly(2026, 4, 25), Status = AttendanceStatus.Present, CreatedAt = createdAt },
            new AttendanceRecord { Id = 2, StudentId = 2, SchoolClassId = 1, SectionId = 1, AttendanceDate = new DateOnly(2026, 4, 25), Status = AttendanceStatus.Absent, CreatedAt = createdAt });

        modelBuilder.Entity<Exam>().HasData(new Exam { Id = 1, Name = "Midterm", AcademicYearId = 1, StartsOn = new DateOnly(2026, 6, 1), EndsOn = new DateOnly(2026, 6, 12), CreatedAt = createdAt });
        modelBuilder.Entity<MarkEntry>().HasData(
            new MarkEntry { Id = 1, ExamId = 1, StudentId = 1, SubjectId = 1, MarksObtained = 86, EnteredByTeacherId = 1, Status = ResultWorkflowStatus.Published, CreatedAt = createdAt },
            new MarkEntry { Id = 2, ExamId = 1, StudentId = 2, SubjectId = 1, MarksObtained = 78, EnteredByTeacherId = 1, Status = ResultWorkflowStatus.Published, CreatedAt = createdAt });

        modelBuilder.Entity<FeeInvoice>().HasData(
            new FeeInvoice { Id = 1, InvoiceNo = "INV-2026-0001", StudentId = 1, DueDate = new DateOnly(2026, 5, 10), TotalAmount = 2500, PaidAmount = 2500, Status = PaymentStatus.Paid, CreatedAt = createdAt },
            new FeeInvoice { Id = 2, InvoiceNo = "INV-2026-0002", StudentId = 2, DueDate = new DateOnly(2026, 5, 10), TotalAmount = 2500, PaidAmount = 1000, Status = PaymentStatus.Partial, CreatedAt = createdAt });
        modelBuilder.Entity<Payment>().HasData(new Payment { Id = 1, FeeInvoiceId = 1, Amount = 2500, Method = PaymentMethod.Cash, PaidAt = createdAt, CreatedAt = createdAt });

        modelBuilder.Entity<Notice>().HasData(new Notice { Id = 1, Title = "Welcome to the 2026 academic session", Body = "Classes and office activities are active.", AudienceRole = "All", PublishAt = createdAt, CreatedAt = createdAt });
        modelBuilder.Entity<Book>().HasData(new Book { Id = 1, AccessionNo = "B-0001", Title = "Primary Mathematics", Author = "Academic Board", TotalCopies = 10, AvailableCopies = 8, CreatedAt = createdAt });
        modelBuilder.Entity<SchoolProfile>().HasData(new SchoolProfile { Id = 1, Name = "School Management System", Address = "Dhaka, Bangladesh", Phone = "01000000000", Email = "info@school.local", CreatedAt = createdAt });

        modelBuilder.Entity<StudentGroup>().HasData(
            new StudentGroup { Id = 1, Name = "Science", Code = "SCI", Description = "Science Group", MinClass = 9, MaxClass = 10, DisplayOrder = 1, IsActive = true, CreatedAt = createdAt },
            new StudentGroup { Id = 2, Name = "BusinessStudies", Code = "BS", Description = "Business Studies Group", MinClass = 9, MaxClass = 10, DisplayOrder = 2, IsActive = true, CreatedAt = createdAt },
            new StudentGroup { Id = 3, Name = "Humanities", Code = "HUM", Description = "Humanities Group", MinClass = 9, MaxClass = 10, DisplayOrder = 3, IsActive = true, CreatedAt = createdAt }
        );

        modelBuilder.Entity<LeaveType>().HasData(
            new LeaveType { Id = 1, Name = "Sick Leave", MaxDays = 14, IsPaid = true, IsActive = true },
            new LeaveType { Name = "Casual Leave", Id = 2, MaxDays = 10, IsPaid = true, IsActive = true },
            new LeaveType { Name = "Maternity Leave", Id = 3, MaxDays = 180, IsPaid = true, IsActive = true },
            new LeaveType { Name = "Paternity Leave", Id = 4, MaxDays = 15, IsPaid = true, IsActive = true },
            new LeaveType { Name = "Unpaid Leave", Id = 5, MaxDays = 30, IsPaid = false, IsActive = true }
        );

        // Workflow Definition
        modelBuilder.Entity<WorkflowDefinition>().HasData(
            new WorkflowDefinition { Id = 1, Name = "Standard Admission Workflow", Description = "Default workflow for student admissions (17 states)", IsActive = true, SortOrder = 1, CreatedAt = createdAt }
        );

        // Workflow Transitions (forward flow: ApplicationSubmitted → AdmissionCompleted)
        modelBuilder.Entity<WorkflowTransition>().HasData(
            new WorkflowTransition { Id = 1, WorkflowDefinitionId = 1, FromState = WorkflowState.ApplicationSubmitted, ToState = WorkflowState.DocumentVerification, TransitionType = WorkflowTransitionType.Automatic, RequiresApproval = false, SortOrder = 1, IsActive = true, CreatedAt = createdAt },
            new WorkflowTransition { Id = 2, WorkflowDefinitionId = 1, FromState = WorkflowState.DocumentVerification, ToState = WorkflowState.AcademicReview, TransitionType = WorkflowTransitionType.Automatic, ConditionExpression = "AllDocumentsVerified", RequiresApproval = false, SortOrder = 2, IsActive = true, CreatedAt = createdAt },
            new WorkflowTransition { Id = 3, WorkflowDefinitionId = 1, FromState = WorkflowState.AcademicReview, ToState = WorkflowState.InterviewScheduled, TransitionType = WorkflowTransitionType.ManualApproval, RequiresApproval = true, SortOrder = 3, IsActive = true, CreatedAt = createdAt },
            new WorkflowTransition { Id = 4, WorkflowDefinitionId = 1, FromState = WorkflowState.InterviewScheduled, ToState = WorkflowState.InterviewCompleted, TransitionType = WorkflowTransitionType.Automatic, RequiresApproval = false, SortOrder = 4, IsActive = true, CreatedAt = createdAt },
            new WorkflowTransition { Id = 5, WorkflowDefinitionId = 1, FromState = WorkflowState.InterviewCompleted, ToState = WorkflowState.FeeVerification, TransitionType = WorkflowTransitionType.Automatic, RequiresApproval = false, SortOrder = 5, IsActive = true, CreatedAt = createdAt },
            new WorkflowTransition { Id = 6, WorkflowDefinitionId = 1, FromState = WorkflowState.FeeVerification, ToState = WorkflowState.PrincipalApproval, TransitionType = WorkflowTransitionType.ManualApproval, RequiresApproval = true, SortOrder = 6, IsActive = true, CreatedAt = createdAt },
            new WorkflowTransition { Id = 7, WorkflowDefinitionId = 1, FromState = WorkflowState.PrincipalApproval, ToState = WorkflowState.StudentCreation, TransitionType = WorkflowTransitionType.Automatic, RequiresApproval = false, SortOrder = 7, IsActive = true, CreatedAt = createdAt },
            new WorkflowTransition { Id = 8, WorkflowDefinitionId = 1, FromState = WorkflowState.StudentCreation, ToState = WorkflowState.GuardianCreation, TransitionType = WorkflowTransitionType.Automatic, RequiresApproval = false, SortOrder = 8, IsActive = true, CreatedAt = createdAt },
            new WorkflowTransition { Id = 9, WorkflowDefinitionId = 1, FromState = WorkflowState.GuardianCreation, ToState = WorkflowState.UserProvisioning, TransitionType = WorkflowTransitionType.Automatic, RequiresApproval = false, SortOrder = 9, IsActive = true, CreatedAt = createdAt },
            new WorkflowTransition { Id = 10, WorkflowDefinitionId = 1, FromState = WorkflowState.UserProvisioning, ToState = WorkflowState.StudentIdGeneration, TransitionType = WorkflowTransitionType.Automatic, RequiresApproval = false, SortOrder = 10, IsActive = true, CreatedAt = createdAt },
            new WorkflowTransition { Id = 11, WorkflowDefinitionId = 1, FromState = WorkflowState.StudentIdGeneration, ToState = WorkflowState.IdCardGeneration, TransitionType = WorkflowTransitionType.Automatic, RequiresApproval = false, SortOrder = 11, IsActive = true, CreatedAt = createdAt },
            new WorkflowTransition { Id = 12, WorkflowDefinitionId = 1, FromState = WorkflowState.IdCardGeneration, ToState = WorkflowState.WelcomeEmail, TransitionType = WorkflowTransitionType.Automatic, RequiresApproval = false, SortOrder = 12, IsActive = true, CreatedAt = createdAt },
            new WorkflowTransition { Id = 13, WorkflowDefinitionId = 1, FromState = WorkflowState.WelcomeEmail, ToState = WorkflowState.AdmissionCompleted, TransitionType = WorkflowTransitionType.Automatic, RequiresApproval = false, SortOrder = 13, IsActive = true, CreatedAt = createdAt },
            // Hold/Resume transitions
            new WorkflowTransition { Id = 14, WorkflowDefinitionId = 1, FromState = WorkflowState.ApplicationSubmitted, ToState = WorkflowState.OnHold, TransitionType = WorkflowTransitionType.ManualApproval, RequiresApproval = true, SortOrder = 14, IsActive = true, CreatedAt = createdAt },
            new WorkflowTransition { Id = 15, WorkflowDefinitionId = 1, FromState = WorkflowState.OnHold, ToState = WorkflowState.DocumentVerification, TransitionType = WorkflowTransitionType.ManualApproval, RequiresApproval = true, SortOrder = 15, IsActive = true, CreatedAt = createdAt },
            // Reject transitions (from key states)
            new WorkflowTransition { Id = 16, WorkflowDefinitionId = 1, FromState = WorkflowState.DocumentVerification, ToState = WorkflowState.Rejected, TransitionType = WorkflowTransitionType.ManualApproval, RequiresApproval = true, SortOrder = 16, IsActive = true, CreatedAt = createdAt },
            new WorkflowTransition { Id = 17, WorkflowDefinitionId = 1, FromState = WorkflowState.AcademicReview, ToState = WorkflowState.Rejected, TransitionType = WorkflowTransitionType.ManualApproval, RequiresApproval = true, SortOrder = 17, IsActive = true, CreatedAt = createdAt },
            new WorkflowTransition { Id = 18, WorkflowDefinitionId = 1, FromState = WorkflowState.InterviewCompleted, ToState = WorkflowState.Rejected, TransitionType = WorkflowTransitionType.ManualApproval, RequiresApproval = true, SortOrder = 18, IsActive = true, CreatedAt = createdAt },
            new WorkflowTransition { Id = 19, WorkflowDefinitionId = 1, FromState = WorkflowState.FeeVerification, ToState = WorkflowState.Rejected, TransitionType = WorkflowTransitionType.ManualApproval, RequiresApproval = true, SortOrder = 19, IsActive = true, CreatedAt = createdAt },
            new WorkflowTransition { Id = 20, WorkflowDefinitionId = 1, FromState = WorkflowState.PrincipalApproval, ToState = WorkflowState.Rejected, TransitionType = WorkflowTransitionType.ManualApproval, RequiresApproval = true, SortOrder = 20, IsActive = true, CreatedAt = createdAt }
        );

        // WorkflowInstance for existing test admission (Id=1, Pending status)
        modelBuilder.Entity<WorkflowInstance>().HasData(
            new WorkflowInstance { Id = 1, AdmissionApplicationId = 1, WorkflowDefinitionId = 1, CurrentState = WorkflowState.ApplicationSubmitted, IsCompleted = false, CreatedAt = createdAt }
        );

        // WorkflowHistoryEntry for initial submission
        modelBuilder.Entity<WorkflowHistoryEntry>().HasData(
            new WorkflowHistoryEntry { Id = 1, WorkflowInstanceId = 1, FromState = WorkflowState.ApplicationSubmitted, ToState = WorkflowState.ApplicationSubmitted, Remarks = "Application submitted by applicant", ActionedBy = "applicant", ActionedAt = createdAt, CreatedAt = createdAt }
        );

        modelBuilder.Entity<GradingRule>().HasData(
            new GradingRule { Id = 1, Grade = "A+", MinMarks = 80, MaxMarks = 100, GradePoint = 5.0m, CreatedAt = createdAt },
            new GradingRule { Id = 2, Grade = "A", MinMarks = 70, MaxMarks = 79, GradePoint = 4.0m, CreatedAt = createdAt },
            new GradingRule { Id = 3, Grade = "A-", MinMarks = 60, MaxMarks = 69, GradePoint = 3.5m, CreatedAt = createdAt },
            new GradingRule { Id = 4, Grade = "B", MinMarks = 50, MaxMarks = 59, GradePoint = 3.0m, CreatedAt = createdAt },
            new GradingRule { Id = 5, Grade = "C", MinMarks = 40, MaxMarks = 49, GradePoint = 2.0m, CreatedAt = createdAt },
            new GradingRule { Id = 6, Grade = "D", MinMarks = 33, MaxMarks = 39, GradePoint = 1.0m, CreatedAt = createdAt },
            new GradingRule { Id = 7, Grade = "F", MinMarks = 0, MaxMarks = 32, GradePoint = 0.0m, CreatedAt = createdAt }
        );
    }
}
