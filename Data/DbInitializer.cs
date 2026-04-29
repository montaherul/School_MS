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
            new Role { Id = 1, Name = "Super Admin", Description = "System owner with all permissions", CreatedAt = createdAt },
            new Role { Id = 2, Name = "Principal", Description = "Final approval and all modules", CreatedAt = createdAt },
            new Role { Id = 3, Name = "Assistant Head", Description = "Academic operations", CreatedAt = createdAt },
            new Role { Id = 4, Name = "Senior Lecturer", Description = "Teaching and review", CreatedAt = createdAt },
            new Role { Id = 5, Name = "Lecturer", Description = "Teaching operations", CreatedAt = createdAt },
            new Role { Id = 6, Name = "Office Staff", Description = "Admission, fees, reports", CreatedAt = createdAt },
            new Role { Id = 7, Name = "Student", Description = "Student portal access", CreatedAt = createdAt });

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
            "Admission", "Student", "Exam", "Result", "Communication", "System"
        };
        var actions = new[] { "View", "Create", "Edit", "Delete", "Approve", "Assign", "Publish", "Export", "Manage" };
        var permissions = modules.SelectMany(module => actions
            .Select(action => new Permission
            {
                Id = permissionId++,
                Module = module,
                ModuleName = module,
                Action = action,
                Code = $"{module}.{action}",
                CanCreate = action is "Create" or "Manage",
                CanRead = action is "View" or "Export" or "Manage",
                CanUpdate = action is "Edit" or "Approve" or "Assign" or "Publish" or "Manage",
                CanDelete = action is "Delete" or "Manage",
                CreatedAt = createdAt
            })).ToArray();
        modelBuilder.Entity<Permission>().HasData(permissions);
        var adminRolePermissions = permissions.Select(p => new RolePermission { RoleId = 1, PermissionId = p.Id });
        var teacherRolePermissions = permissions
            .Where(p =>
                p.Code is "Dashboard.View" or "Classes.View" or "Students.View" or "Attendance.View" or "Attendance.Create" or "Marks.View" or "Marks.Create" or "Assignments.View" or "Assignments.Create" ||
                p.Code is "Reports.View" or "Exam.View" or "Exams.View")
            .Select(p => new RolePermission { RoleId = 5, PermissionId = p.Id });
        var officeRolePermissions = permissions
            .Where(p =>
                p.ModuleName is "Dashboard" or "Admissions" or "Admission" or "Students" or "Student" or "Fees" or "Payments" or "Reports" &&
                p.Action is not "Delete")
            .Select(p => new RolePermission { RoleId = 6, PermissionId = p.Id });
        var studentRolePermissions = permissions
            .Where(p =>
                p.Code is "Dashboard.View" or "Students.View" or "Student.View" or "Attendance.View" or "Marks.View" or "Assignments.View" or "Assignments.Create" or "Notifications.View" or "Fees.View")
            .Select(p => new RolePermission { RoleId = 7, PermissionId = p.Id });
        modelBuilder.Entity<RolePermission>().HasData(adminRolePermissions.Concat(teacherRolePermissions).Concat(officeRolePermissions).Concat(studentRolePermissions));

        modelBuilder.Entity<AcademicYear>().HasData(new AcademicYear { Id = 1, Name = "2026", StartsOn = new DateTime(2026, 1, 1), EndsOn = new DateTime(2026, 12, 31), IsActive = true, CreatedAt = createdAt });
        //modelBuilder.Entity<SchoolClass>().HasData(
        //    new SchoolClass { Id = 1, Name = "Class One", SortOrder = 1, CreatedAt = createdAt },
        //    new SchoolClass { Id = 2, Name = "Class Two", SortOrder = 2, CreatedAt = createdAt },

        //    new SchoolClass { Id = 3, Name = "Class Three", SortOrder = 3, CreatedAt = createdAt });


        modelBuilder.Entity<SchoolClass>().HasData(
            Enumerable.Range(1, 10).Select(i => new SchoolClass
            {
                Id = i,
                Name = $"Class {GetClassName(i)}",
                SortOrder = i,
                CreatedAt = createdAt
            })
        );

        var sections = new List<Section>();
        int id = 1;

        for (int cls = 1; cls <= 10; cls++)
        {
            // Default sections
            sections.Add(new Section { Id = id++, SchoolClassId = cls, Name = "A", CreatedAt = createdAt });
            sections.Add(new Section { Id = id++, SchoolClassId = cls, Name = "B", CreatedAt = createdAt });

            // Special groups for Class 9 & 10
            if (cls == 9 || cls == 10)
            {
                sections.Add(new Section { Id = id++, SchoolClassId = cls, Name = "Science", CreatedAt = createdAt });
                sections.Add(new Section { Id = id++, SchoolClassId = cls, Name = "Business Studies", CreatedAt = createdAt });
                sections.Add(new Section { Id = id++, SchoolClassId = cls, Name = "Humanities", CreatedAt = createdAt });
            }
        }

        modelBuilder.Entity<Section>().HasData(sections);





        //modelBuilder.Entity<Section>().HasData(
        //    new Section { Id = 1, SchoolClassId = 1, Name = "A", CreatedAt = createdAt },
        //    new Section { Id = 2, SchoolClassId = 1, Name = "B", CreatedAt = createdAt },

        //    new Section { Id = 3, SchoolClassId = 2, Name = "A", CreatedAt = createdAt });
        modelBuilder.Entity<Subject>().HasData(
            new Subject { Id = 1, Code = "BAN", Name = "Bangla", CreatedAt = createdAt },
            new Subject { Id = 2, Code = "ENG", Name = "English", CreatedAt = createdAt },
            new Subject { Id = 3, Code = "MAT", Name = "Mathematics", CreatedAt = createdAt },
            new Subject { Id = 4, Code = "SCI", Name = "General Science", CreatedAt = createdAt },
            new Subject { Id = 5, Code = "REL", Name = "Religion", CreatedAt = createdAt },
            new Subject { Id = 6, Code = "SOC", Name = "Social Science", CreatedAt = createdAt },
            new Subject { Id = 7, Code = "ICT", Name = "ICT", CreatedAt = createdAt },

            // Class 9–10 Science
            new Subject { Id = 8, Code = "PHY", Name = "Physics", CreatedAt = createdAt },
            new Subject { Id = 9, Code = "CHE", Name = "Chemistry", CreatedAt = createdAt },
            new Subject { Id = 10, Code = "BIO", Name = "Biology", CreatedAt = createdAt },
            new Subject { Id = 11, Code = "HMA", Name = "Higher Math", CreatedAt = createdAt },

            // Business Studies
            new Subject { Id = 12, Code = "ACC", Name = "Accounting", CreatedAt = createdAt },
            new Subject { Id = 13, Code = "FIN", Name = "Finance", CreatedAt = createdAt },
            new Subject { Id = 14, Code = "BUS", Name = "Business Entrepreneurship", CreatedAt = createdAt },

            // Humanities
            new Subject { Id = 15, Code = "HIS", Name = "History", CreatedAt = createdAt },
            new Subject { Id = 16, Code = "GEO", Name = "Geography", CreatedAt = createdAt },
            new Subject { Id = 17, Code = "CIV", Name = "Civics", CreatedAt = createdAt }
        );
        modelBuilder.Entity<TeacherProfile>().HasData(
            new TeacherProfile { Id = 1, EmployeeNo = "T-0001", FullName = "Senior Lecturer", Designation = "Senior Lecturer", Phone = "01000000001", CreatedAt = createdAt },
            new TeacherProfile { Id = 2, EmployeeNo = "T-0002", FullName = "Class Teacher", Designation = "Lecturer", Phone = "01000000002", CreatedAt = createdAt });

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
            new MarkEntry { Id = 1, ExamId = 1, StudentId = 1, SubjectId = 1, MarksObtained = 86, EnteredByTeacherId = 1, Status = PublishStatus.Published, CreatedAt = createdAt },
            new MarkEntry { Id = 2, ExamId = 1, StudentId = 2, SubjectId = 1, MarksObtained = 78, EnteredByTeacherId = 1, Status = PublishStatus.Published, CreatedAt = createdAt });

        modelBuilder.Entity<FeeInvoice>().HasData(
            new FeeInvoice { Id = 1, InvoiceNo = "INV-2026-0001", StudentId = 1, DueDate = new DateOnly(2026, 5, 10), TotalAmount = 2500, PaidAmount = 2500, Status = PaymentStatus.Paid, CreatedAt = createdAt },
            new FeeInvoice { Id = 2, InvoiceNo = "INV-2026-0002", StudentId = 2, DueDate = new DateOnly(2026, 5, 10), TotalAmount = 2500, PaidAmount = 1000, Status = PaymentStatus.Partial, CreatedAt = createdAt });
        modelBuilder.Entity<Payment>().HasData(new Payment { Id = 1, FeeInvoiceId = 1, Amount = 2500, Method = PaymentMethod.Cash, PaidAt = createdAt, CreatedAt = createdAt });

        modelBuilder.Entity<Notice>().HasData(new Notice { Id = 1, Title = "Welcome to the 2026 academic session", Body = "Classes and office activities are active.", AudienceRole = "All", PublishAt = createdAt, CreatedAt = createdAt });
        modelBuilder.Entity<Book>().HasData(new Book { Id = 1, AccessionNo = "B-0001", Title = "Primary Mathematics", Author = "Academic Board", TotalCopies = 10, AvailableCopies = 8, CreatedAt = createdAt });
        modelBuilder.Entity<SchoolProfile>().HasData(new SchoolProfile { Id = 1, Name = "School Management System", Address = "Dhaka, Bangladesh", Phone = "01000000000", Email = "info@school.local", CreatedAt = createdAt });
    }
}
