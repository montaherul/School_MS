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

namespace SchoolManagementSystem.Data;

public static class DbInitializer
{
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

            new Role { Id = 1, Name = "SuperAdmin", Description = "Full system owner with all permissions", Priority = 100, IsSystemRole = true, IsActive = true, CreatedAt = createdAt },
            new Role { Id = 2, Name = "Admin", Description = "Administrative operations", Priority = 95, IsSystemRole = true, IsActive = true, CreatedAt = createdAt },
            new Role { Id = 3, Name = "Principal", Description = "Academic + operational authority", Priority = 90, IsSystemRole = true, IsActive = true, CreatedAt = createdAt },
            new Role { Id = 4, Name = "HRManager", Description = "Human resource management", Priority = 80, IsSystemRole = true, IsActive = true, CreatedAt = createdAt },
            new Role { Id = 5, Name = "Accountant", Description = "Financial operations", Priority = 70, IsSystemRole = true, IsActive = true, CreatedAt = createdAt },
            new Role { Id = 6, Name = "Teacher", Description = "Academic teaching operations", Priority = 60, IsSystemRole = true, IsActive = true, CreatedAt = createdAt },
            new Role { Id = 7, Name = "Librarian", Description = "Library management", Priority = 50, IsSystemRole = true, IsActive = true, CreatedAt = createdAt },
            new Role { Id = 8, Name = "Staff", Description = "General employee operations", Priority = 40, IsSystemRole = true, IsActive = true, CreatedAt = createdAt },
            new Role { Id = 9, Name = "Student", Description = "Student portal access", Priority = 10, IsSystemRole = true, IsActive = true, CreatedAt = createdAt },
            new Role { Id = 10, Name = "Parent", Description = "Parent portal access", Priority = 5, IsSystemRole = true, IsActive = true, CreatedAt = createdAt });

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
            new Role { Id = 26, Name = "Admin", Description = "Administrator", CreatedAt = createdAt });


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

            "Admission", "Student", "Exam", "Result", "Communication", "System", "AuditLogs"

            "Admission", "Student", "Exam", "Result", "Communication", "System", "AuditLogs",
            "FeeStructures", "Invoices", "Scholarships", "Waivers", "StudentDues", "FinancialTransactions",
            "FinanceReports", "FinanceConfiguration", "FinanceDashboard", "Receipts"
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
        modelBuilder.Entity<Permission>().HasData(permissions);

        var superAdminPermissions = permissions.Select(p => new RolePermission { RoleId = 1, PermissionId = p.Id });
        var adminPermissions = permissions.Select(p => new RolePermission { RoleId = 2, PermissionId = p.Id });
        var principalPermissions = permissions.Select(p => new RolePermission { RoleId = 3, PermissionId = p.Id });
        
        var hrManagerPermissions = permissions
            .Where(p => p.ModuleName is "Dashboard" or "Employee" or "Attendance" or "Leave" or "Payroll" or "HR" or "Reports")
            .Select(p => new RolePermission { RoleId = 4, PermissionId = p.Id });

        var accountantPermissions = permissions
            .Where(p => p.ModuleName is "Dashboard" or "Fees" or "Payments" or "Payroll" or "Reports")
            .Select(p => new RolePermission { RoleId = 5, PermissionId = p.Id });

        var teacherPermissions = permissions


        var financeModules = new HashSet<string>
        {
            "FeeStructures", "Invoices", "Payments", "Scholarships", "Waivers", "StudentDues",
            "FinancialTransactions", "FinanceReports", "FinanceConfiguration", "FinanceDashboard", "Receipts"
        };

        var adminRolePermissions = permissions.Select(p => new RolePermission { RoleId = 1, PermissionId = p.Id });
        var principalRolePermissions = permissions
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
        var assistantHeadRolePermissions = permissions
            .Where(p =>
                p.ModuleName is "Dashboard" or "Academic" or "Classes" or "Sections" or "Subjects" or "Admissions" or "Admission" or "Students" or "Student" or "Attendance" or "Exams" or "Exam" or "Marks" or "Result" or "Communication" or "Reports")
            .Select(p => new RolePermission { RoleId = 3, PermissionId = p.Id });
        var teacherRolePermissions = permissions

            .Where(p =>
                p.Code is "Dashboard.View" or "Classes.View" or "Students.View" or "Attendance.View" or "Attendance.Create" or "Marks.View" or "Marks.Create" or "Assignments.View" or "Assignments.Create" ||
                p.ModuleName is "Exam" or "Result" or "Reports")
            .Select(p => new RolePermission { RoleId = 6, PermissionId = p.Id });

        var librarianPermissions = permissions
            .Where(p => p.ModuleName is "Dashboard" or "Library" or "Reports")
            .Select(p => new RolePermission { RoleId = 7, PermissionId = p.Id });

        var staffPermissions = permissions
            .Where(p =>
                p.ModuleName is "Dashboard" or "Admissions" or "Admission" or "Students" or "Student" or "Fees" or "Payments" or "Reports" &&
                p.Action is not "Delete")
            .Select(p => new RolePermission { RoleId = 8, PermissionId = p.Id });

        var studentPermissions = permissions
            .Where(p =>

                p.Code is "Dashboard.View" or "Students.View" or "Student.View" or "Attendance.View" or "Marks.View" or "Assignments.View" or "Assignments.Create" or "Notifications.View" or "Fees.View")
            .Select(p => new RolePermission { RoleId = 9, PermissionId = p.Id });

        var parentPermissions = permissions
            .Where(p =>
                p.Code is "Dashboard.View" or "Student.View" or "Attendance.View" or "Marks.View" or "Notifications.View" or "Fees.View")
            .Select(p => new RolePermission { RoleId = 10, PermissionId = p.Id });

        modelBuilder.Entity<RolePermission>().HasData(
            superAdminPermissions
            .Concat(adminPermissions)
            .Concat(principalPermissions)
            .Concat(hrManagerPermissions)
            .Concat(accountantPermissions)
            .Concat(teacherPermissions)
            .Concat(librarianPermissions)
            .Concat(staffPermissions)
            .Concat(studentPermissions)
            .Concat(parentPermissions));

                p.Code is "Dashboard.View" or "Dashboard.Read" or "Students.View" or "Student.View" or "Attendance.View" or "Marks.View" or "Assignments.View" or "Assignments.Create" or "Notifications.View" or "Fees.View" or
                    "Invoices.View" or "Invoices.Read" or "Payments.View" or "Payments.Read" or "StudentDues.View" or "StudentDues.Read" or
                    "Receipts.View" or "Receipts.Read" or "Receipts.Print" or "Receipts.Export")
            .Select(p => new RolePermission { RoleId = 7, PermissionId = p.Id });
        var accountantRolePermissions = permissions
            .Where(p => financeModules.Contains(p.ModuleName) || p.Code is "Dashboard.View" or "Dashboard.Read")
            .Select(p => new RolePermission { RoleId = 20, PermissionId = p.Id });
        var applicationAdminRolePermissions = permissions.Select(p => new RolePermission { RoleId = 26, PermissionId = p.Id });
        modelBuilder.Entity<RolePermission>().HasData(
            adminRolePermissions
            .Concat(principalRolePermissions)
            .Concat(assistantHeadRolePermissions)
            .Concat(teacherRolePermissions)
            .Concat(officeRolePermissions)
            .Concat(studentRolePermissions)
            .Concat(accountantRolePermissions)
            .Concat(applicationAdminRolePermissions));


        modelBuilder.Entity<AcademicYear>().HasData(new AcademicYear { Id = 1, Name = "2026", StartsOn = new DateTime(2026, 1, 1), EndsOn = new DateTime(2026, 12, 31), IsActive = true, CreatedAt = createdAt });
        //modelBuilder.Entity<SchoolClass>().HasData(
        //    new SchoolClass { Id = 1, Name = "Class One", SortOrder = 1, CreatedAt = createdAt },
        //    new SchoolClass { Id = 2, Name = "Class Two", SortOrder = 2, CreatedAt = createdAt },

        //    new SchoolClass { Id = 3, Name = "Class Three", SortOrder = 3, CreatedAt = createdAt });


        modelBuilder.Entity<SchoolClass>().HasData(
     new SchoolClass { Id = 1, Name = "Class One", SortOrder = 1, CreatedAt = createdAt },
     new SchoolClass { Id = 2, Name = "Class Two", SortOrder = 2, CreatedAt = createdAt },
     new SchoolClass { Id = 3, Name = "Class Three", SortOrder = 3, CreatedAt = createdAt },
     new SchoolClass { Id = 4, Name = "Class Four", SortOrder = 4, CreatedAt = createdAt },
     new SchoolClass { Id = 5, Name = "Class Five", SortOrder = 5, CreatedAt = createdAt },
     new SchoolClass { Id = 6, Name = "Class Six", SortOrder = 6, CreatedAt = createdAt },
     new SchoolClass { Id = 7, Name = "Class Seven", SortOrder = 7, CreatedAt = createdAt },
     new SchoolClass { Id = 8, Name = "Class Eight", SortOrder = 8, CreatedAt = createdAt },
     new SchoolClass { Id = 9, Name = "Class Nine", SortOrder = 9, CreatedAt = createdAt },
     new SchoolClass { Id = 10, Name = "Class Ten", SortOrder = 10, CreatedAt = createdAt }
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
        foreach (var cls in new[] { 9, 10 })
        {
            var groups = new[] { "Science", "Business Studies", "Humanities" };
            foreach (var groupName in groups)
            {
                int groupId = id++;
                // Parent group section (ParentSectionId = null = it IS the group)
                sections.Add(new Section
                {
                    Id = groupId,
                    SchoolClassId = cls,
                    Name = groupName,
                    ParentSectionId = null,
                    CreatedAt = createdAt
                });
                // Leaf sub-sections A and B
                sections.Add(new Section
                {
                    Id = id++,
                    SchoolClassId = cls,
                    Name = $"{groupName} A",
                    ParentSectionId = groupId,
                    CreatedAt = createdAt
                });
                sections.Add(new Section
                {
                    Id = id++,
                    SchoolClassId = cls,
                    Name = $"{groupName} B",
                    ParentSectionId = groupId,
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


       
       new Subject { Id = 9, Code = "BAN1", Name = "বাংলা ১ম পত্র", CreatedAt = createdAt },
       new Subject { Id = 10, Code = "BAN2", Name = "বাংলা ২য় পত্র", CreatedAt = createdAt },
       new Subject { Id = 11, Code = "ENG1", Name = "ইংরেজি ১ম পত্র", CreatedAt = createdAt },
       new Subject { Id = 12, Code = "ENG2", Name = "ইংরেজি ২য় পত্র", CreatedAt = createdAt },
       new Subject { Id = 13, Code = "SCI", Name = "বিজ্ঞান", CreatedAt = createdAt },
       new Subject { Id = 14, Code = "ICT", Name = "তথ্য ও যোগাযোগ প্রযুক্তি", CreatedAt = createdAt },
       new Subject { Id = 15, Code = "AGR", Name = "কৃষি শিক্ষা", CreatedAt = createdAt },

            // Primary Subjects
            new Subject { Id = 1, Code = "BAN", Name = "বাংলা", NameBn = "বাংলা", CreatedAt = createdAt },
            new Subject { Id = 2, Code = "ENG", Name = "English", NameBn = "ইংরেজি", CreatedAt = createdAt },
            new Subject { Id = 3, Code = "MAT", Name = "Mathematics", NameBn = "গণিত", CreatedAt = createdAt },
            new Subject { Id = 4, Code = "GSCI", Name = "General Science", NameBn = "সাধারণ বিজ্ঞান", CreatedAt = createdAt },
            new Subject { Id = 5, Code = "SOC", Name = "Bangladesh and Global Studies", NameBn = "বাংলাদেশ ও বিশ্ব পরিচয়", CreatedAt = createdAt },
            new Subject { Id = 6, Code = "REL", Name = "Religion and Moral Education", NameBn = "ধর্ম ও নৈতিক শিক্ষা", CreatedAt = createdAt },
            new Subject { Id = 7, Code = "ART", Name = "Arts and Crafts", NameBn = "চারুকলা", CreatedAt = createdAt },
            new Subject { Id = 8, Code = "PE", Name = "Physical Education", NameBn = "শারীরিক শিক্ষা", CreatedAt = createdAt },


            // SSC Common Subjects
            new Subject { Id = 9, Code = "BAN1", Name = "Bangla 1st Paper", NameBn = "বাংলা ১ম পত্র", CreatedAt = createdAt },
            new Subject { Id = 10, Code = "BAN2", Name = "Bangla 2nd Paper", NameBn = "বাংলা ২য় পত্র", CreatedAt = createdAt },
            new Subject { Id = 11, Code = "ENG1", Name = "English 1st Paper", NameBn = "ইংরেজি ১ম পত্র", CreatedAt = createdAt },
            new Subject { Id = 12, Code = "ENG2", Name = "English 2nd Paper", NameBn = "ইংরেজি ২য় পত্র", CreatedAt = createdAt },
            new Subject { Id = 13, Code = "SCI", Name = "Science", NameBn = "বিজ্ঞান", CreatedAt = createdAt },
            new Subject { Id = 14, Code = "ICT", Name = "Information and Communication Technology", NameBn = "তথ্য ও যোগাযোগ প্রযুক্তি", CreatedAt = createdAt },
            new Subject { Id = 15, Code = "AGR", Name = "Agriculture Studies", NameBn = "কৃষি শিক্ষা", CreatedAt = createdAt },

            // Science Group
            new Subject { Id = 16, Code = "PHY", Name = "Physics", NameBn = "পদার্থবিজ্ঞান", CreatedAt = createdAt, SubjectGroup = "Science" },
            new Subject { Id = 17, Code = "CHE", Name = "Chemistry", NameBn = "রসায়ন", CreatedAt = createdAt, SubjectGroup = "Science" },
            new Subject { Id = 18, Code = "BIO", Name = "Biology", NameBn = "জীববিজ্ঞান", CreatedAt = createdAt, SubjectGroup = "Science" },
            new Subject { Id = 19, Code = "HMA", Name = "Higher Mathematics", NameBn = "উচ্চতর গণিত", CreatedAt = createdAt, SubjectGroup = "Science" },

            // Business Studies
            new Subject { Id = 20, Code = "ACC", Name = "Accounting", NameBn = "হিসাববিজ্ঞান", CreatedAt = createdAt, SubjectGroup = "Business Studies" },
            new Subject { Id = 21, Code = "FIN", Name = "Finance and Banking", NameBn = "ফাইন্যান্স", CreatedAt = createdAt, SubjectGroup = "Business Studies" },
            new Subject { Id = 22, Code = "BUS", Name = "Business Entrepreneurship", NameBn = "ব্যবসায় উদ্যোগ", CreatedAt = createdAt, SubjectGroup = "Business Studies" },

            // Humanities
            new Subject { Id = 23, Code = "HIS", Name = "History", NameBn = "ইতিহাস", CreatedAt = createdAt, SubjectGroup = "Humanities" },
            new Subject { Id = 24, Code = "GEO", Name = "Geography and Environment", NameBn = "ভূগোল ও পরিবেশ", CreatedAt = createdAt, SubjectGroup = "Humanities" },
            new Subject { Id = 25, Code = "ECO", Name = "Economics", NameBn = "অর্থনীতি", CreatedAt = createdAt, SubjectGroup = "Humanities" },
            new Subject { Id = 26, Code = "CIV", Name = "Civics", NameBn = "নাগরিকতা", CreatedAt = createdAt, SubjectGroup = "Humanities" },

            // Others
            new Subject { Id = 27, Code = "CAREER", Name = "Career Education", NameBn = "ক্যারিয়ার শিক্ষা", CreatedAt = createdAt },
            new Subject { Id = 28, Code = "HEALTH", Name = "Physical Education, Health and Sports", NameBn = "শারীরিক শিক্ষা, স্বাস্থ্য ও খেলাধুলা", CreatedAt = createdAt },
            new Subject { Id = 29, Code = "HSC", Name = "Home Science", NameBn = "গার্হস্থ্য বিজ্ঞান", CreatedAt = createdAt },

            // Religion & Moral Education
            new Subject { Id = 30, Code = "IRE", Name = "Islam and Moral Education", NameBn = "ইসলাম ও নৈতিক শিক্ষা", CreatedAt = createdAt },
            new Subject { Id = 31, Code = "HRE", Name = "Hindu Religion and Moral Education", NameBn = "হিন্দুধর্ম ও নৈতিক শিক্ষা", CreatedAt = createdAt },
            new Subject { Id = 32, Code = "BRE", Name = "Buddhist Religion and Moral Education", NameBn = "বৌদ্ধধর্ম ও নৈতিক শিক্ষা", CreatedAt = createdAt },
            new Subject { Id = 33, Code = "CRE", Name = "Christian Religion and Moral Education", NameBn = "খ্রিস্টধর্ম ও নৈতিক শিক্ষা", CreatedAt = createdAt }
        );

        modelBuilder.Entity<Student>().HasData(
            new Student { Id = 1, StudentNo = "STU-2026-0001", FullName = "Sample Student One", DateOfBirth = new DateTime(2018, 2, 1), Gender = "Male", FatherName = "Father One", MotherName = "Mother One", MobileNumber = "01700000010", Nationality = "Bangladeshi", Country = "Bangladesh", MaritalStatus = "Single", Religion = "Islam", ClassId = 1, SectionId = 1, RollNumber = 1, Status = StudentStatus.Active, CreatedAt = createdAt },
            new Student { Id = 2, StudentNo = "STU-2026-0002", FullName = "Sample Student Two", DateOfBirth = new DateTime(2018, 5, 11), Gender = "Female", FatherName = "Father Two", MotherName = "Mother Two", MobileNumber = "01700000020", Nationality = "Bangladeshi", Country = "Bangladesh", MaritalStatus = "Single", Religion = "Islam", ClassId = 1, SectionId = 1, RollNumber = 2, Status = StudentStatus.Active, CreatedAt = createdAt });
        modelBuilder.Entity<Guardian>().HasData(
            new Guardian { Id = 1, StudentId = 1, Name = "Guardian One", Relation = "Father", Phone = "01700000001", CreatedAt = createdAt },
            new Guardian { Id = 2, StudentId = 2, Name = "Guardian Two", Relation = "Mother", Phone = "01700000002", CreatedAt = createdAt });

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
            new StudentGroup { Id = 2, Name = "Business Studies", Code = "BS", Description = "Business Studies Group", MinClass = 9, MaxClass = 10, DisplayOrder = 2, IsActive = true, CreatedAt = createdAt },
            new StudentGroup { Id = 3, Name = "Humanities", Code = "HUM", Description = "Humanities Group", MinClass = 9, MaxClass = 10, DisplayOrder = 3, IsActive = true, CreatedAt = createdAt }
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
