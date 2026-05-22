using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Entities.Assignment;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.Communication;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Health;
using SchoolManagementSystem.Models.Entities.Library;
using SchoolManagementSystem.Models.Entities.Notification;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Entities.System;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Models.Entities.Transport;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.Entities.Website;


namespace SchoolManagementSystem.Data;

public class SchoolDbContext : DbContext
{
    public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options) { }

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<AdmissionApplication> Admissions => Set<AdmissionApplication>();
    public DbSet<AdmissionDocument> AdmissionDocuments => Set<AdmissionDocument>();
    public DbSet<AdmissionListResultDto> AdmissionListResults => Set<AdmissionListResultDto>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Guardian> Guardians => Set<Guardian>();
    public DbSet<StudentDocument> StudentDocuments => Set<StudentDocument>();
    public DbSet<StudentPromotion> StudentPromotions => Set<StudentPromotion>();
    public DbSet<TransferCertificate> TransferCertificates => Set<TransferCertificate>();
    public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();
    public DbSet<SchoolClass> Classes => Set<SchoolClass>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<StudentListItemDto> StudentListItemResults => Set<StudentListItemDto>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<TeacherAttendance> TeacherAttendances => Set<TeacherAttendance>();
    public DbSet<TeacherLeave> TeacherLeaves => Set<TeacherLeave>();
    public DbSet<TeacherDocument> TeacherDocuments => Set<TeacherDocument>();
    public DbSet<TeacherSalary> TeacherSalaries => Set<TeacherSalary>();
    public DbSet<TeacherPerformance> TeacherPerformances => Set<TeacherPerformance>();
    public DbSet<ClassSubjectTeacher> ClassSubjectTeachers => Set<ClassSubjectTeacher>();
    public DbSet<Syllabus> Syllabi => Set<Syllabus>();
    public DbSet<LessonPlan> LessonPlans => Set<LessonPlan>();
    public DbSet<StudyMaterial> StudyMaterials => Set<StudyMaterial>();
    public DbSet<AttendanceRecord> Attendance => Set<AttendanceRecord>();
    public DbSet<LeaveApplication> LeaveApplications => Set<LeaveApplication>();
    public DbSet<StudentAttendance> StudentAttendances => Set<StudentAttendance>();
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<AttendanceSetting> AttendanceSettings => Set<AttendanceSetting>();
    public DbSet<AttendanceLog> AttendanceLogs => Set<AttendanceLog>();
    public DbSet<AttendanceNotificationLog> AttendanceNotificationLogs => Set<AttendanceNotificationLog>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<ExamSubject> ExamSubjects => Set<ExamSubject>();
    public DbSet<ExamSchedule> ExamSchedules => Set<ExamSchedule>();
    public DbSet<AdmitCard> AdmitCards => Set<AdmitCard>();
    public DbSet<SeatingPlan> SeatingPlans => Set<SeatingPlan>();
    public DbSet<MarkEntry> Marks => Set<MarkEntry>();
    public DbSet<GradingRule> GradingRules => Set<GradingRule>();
    public DbSet<ResultPublication> ResultPublications => Set<ResultPublication>();
    public DbSet<ReportCard> ReportCards => Set<ReportCard>();
    public DbSet<StudentSubjectResult> StudentSubjectResults => Set<StudentSubjectResult>();
    public DbSet<StudentExamResult> StudentExamResults => Set<StudentExamResult>();
    public DbSet<FinalResult> FinalResults => Set<FinalResult>();
    public DbSet<ResultAuditLog> ResultAuditLogs => Set<ResultAuditLog>();
    public DbSet<ReEvaluationRequest> ReEvaluationRequests => Set<ReEvaluationRequest>();
    public DbSet<AssignmentTask> Assignments => Set<AssignmentTask>();
    public DbSet<AssignmentSubmission> AssignmentSubmissions => Set<AssignmentSubmission>();
    public DbSet<FeeStructure> FeeStructures => Set<FeeStructure>();
    public DbSet<FeeInvoice> FeeInvoices => Set<FeeInvoice>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<FineRule> FineRules => Set<FineRule>();
    public DbSet<Notice> Notices => Set<Notice>();
    public DbSet<MessageThread> MessageThreads => Set<MessageThread>();
    public DbSet<MessageItem> MessageItems => Set<MessageItem>();
    public DbSet<Circular> Circulars => Set<Circular>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<BookIssue> BookIssues => Set<BookIssue>();
    public DbSet<BookReservation> BookReservations => Set<BookReservation>();
    public DbSet<TransportRoute> TransportRoutes => Set<TransportRoute>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<StudentRouteAssignment> StudentRouteAssignments => Set<StudentRouteAssignment>();
    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
    public DbSet<VaccinationRecord> VaccinationRecords => Set<VaccinationRecord>();
    public DbSet<NotificationMessage> Notifications => Set<NotificationMessage>();
    public DbSet<SchoolProfile> SchoolProfiles => Set<SchoolProfile>();
    public DbSet<SystemLog> SystemLogs => Set<SystemLog>();
    public DbSet<BackupRecord> BackupRecords => Set<BackupRecord>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<StudentGroup> StudentGroups => Set<StudentGroup>();

    public DbSet<TeacherClassAssignment> TeacherClassAssignments => Set<TeacherClassAssignment>();
    public DbSet<TeacherSubjectAssignment> TeacherSubjectAssignments => Set<TeacherSubjectAssignment>();
    public DbSet<TeacherAcademicProfile> TeacherAcademicProfiles => Set<TeacherAcademicProfile>();
    public DbSet<TeacherAssignmentLog> TeacherAssignmentLogs => Set<TeacherAssignmentLog>();
    public DbSet<TeacherTimetable> TeacherTimetables => Set<TeacherTimetable>();

    // Employee Module DbSets
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Designation> Designations => Set<Designation>();
    public DbSet<DesignationRoleMapping> DesignationRoleMappings => Set<DesignationRoleMapping>();
    public DbSet<EmployeeQualification> EmployeeQualifications => Set<EmployeeQualification>();
    public DbSet<EmployeeDocument> EmployeeDocuments => Set<EmployeeDocument>();
    public DbSet<EmployeeExperience> EmployeeExperiences => Set<EmployeeExperience>();
    public DbSet<EmployeeAttendance> EmployeeAttendances => Set<EmployeeAttendance>();
    public DbSet<EmployeeSalary> EmployeeSalaries => Set<EmployeeSalary>();
    public DbSet<EmployeeAcademicAssignment> EmployeeAcademicAssignments => Set<EmployeeAcademicAssignment>();

    // Public Website DbSets
    public DbSet<SchoolSetting> SchoolSettings => Set<SchoolSetting>();
    public DbSet<WebsitePage> WebsitePages => Set<WebsitePage>();
    public DbSet<Slider> Sliders => Set<Slider>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Gallery> Galleries => Set<Gallery>();
    public DbSet<GalleryImage> GalleryImages => Set<GalleryImage>();
    public DbSet<Announcement> Announcements => Set<Announcement>();

    public DbSet<ClassSubject> ClassSubjects => Set<ClassSubject>();

    // Exam Configuration DbSets
    public DbSet<ExamType> ExamTypes => Set<ExamType>();
    public DbSet<ExamConfiguration> ExamConfigurations => Set<ExamConfiguration>();
    public DbSet<SubjectComponent> SubjectComponents => Set<SubjectComponent>();
    public DbSet<GpaConfiguration> GpaConfigurations => Set<GpaConfiguration>();

    // Result DbSets
    public DbSet<MarkAuditLog> MarkAuditLogs => Set<MarkAuditLog>();
    public DbSet<MarkEntryDraft> MarkEntryDrafts => Set<MarkEntryDraft>();
    public DbSet<ResultLock> ResultLocks => Set<ResultLock>();
    public DbSet<PromotionHistory> PromotionHistories => Set<PromotionHistory>();
    public DbSet<MeritResult> MeritResults => Set<MeritResult>();
    public DbSet<RollNumberAssignment> RollNumberAssignments => Set<RollNumberAssignment>();

    // Student Group DbSet
    public DbSet<StudentGroupAssignment> StudentGroupAssignments => Set<StudentGroupAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserRole>().HasKey(x => new { x.UserId, x.RoleId });
        modelBuilder.Entity<RolePermission>().HasKey(x => new { x.RoleId, x.PermissionId });
        modelBuilder.Entity<AdmissionListResultDto>().HasNoKey();
        modelBuilder.Entity<StudentListItemDto>().HasNoKey();

        modelBuilder.Entity<ApplicationUser>().HasIndex(x => x.UserName).IsUnique();
        modelBuilder.Entity<ApplicationUser>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<UserSession>().HasIndex(x => x.SessionId).IsUnique();
        modelBuilder.Entity<AdmissionApplication>().HasIndex(x => x.ApplicationNo).IsUnique();
        modelBuilder.Entity<Student>().HasIndex(x => x.StudentNo).IsUnique();
        modelBuilder.Entity<Student>().HasIndex(x => new { x.ClassId, x.SectionId, x.RollNumber }).IsUnique();
        modelBuilder.Entity<MarkEntry>().HasIndex(x => new { x.ExamId, x.StudentId, x.SubjectId }).IsUnique();
        modelBuilder.Entity<StudentSubjectResult>().HasIndex(x => new { x.ExamId, x.StudentId, x.SubjectId }).IsUnique();
        modelBuilder.Entity<StudentExamResult>().HasIndex(x => new { x.ExamId, x.StudentId }).IsUnique();
        modelBuilder.Entity<FinalResult>().HasIndex(x => new { x.AcademicYearId, x.StudentId }).IsUnique();
        modelBuilder.Entity<ReEvaluationRequest>().HasIndex(x => new { x.ExamId, x.StudentId, x.SubjectId }).IsUnique();
        modelBuilder.Entity<RollNumberAssignment>().HasIndex(x => new { x.AcademicYearId, x.StudentId, x.ToClassId }).IsUnique();
        modelBuilder.Entity<AttendanceRecord>().HasIndex(x => new { x.StudentId, x.AttendanceDate }).IsUnique().HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<AttendanceNotificationLog>().HasIndex(x => new { x.StudentId, x.AttendanceDate, x.NotificationType, x.NotificationChannel }).IsUnique().HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<Subject>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<Teacher>().HasIndex(x => x.TeacherCode).IsUnique();
        modelBuilder.Entity<FeeInvoice>().HasIndex(x => x.InvoiceNo).IsUnique();
        modelBuilder.Entity<Book>().HasIndex(x => x.AccessionNo).IsUnique();

        // Employee Indexes
        modelBuilder.Entity<Employee>().HasIndex(x => x.EmployeeCode).IsUnique();
        modelBuilder.Entity<Employee>().HasIndex(x => x.Phone).IsUnique();
        modelBuilder.Entity<Employee>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<Employee>().HasIndex(x => x.NIDNumber).IsUnique();
        modelBuilder.Entity<DesignationRoleMapping>().HasIndex(x => new { x.DesignationId, x.RoleId }).IsUnique();
        modelBuilder.Entity<EmployeeAttendance>().HasIndex(x => new { x.EmployeeId, x.AttendanceDate }).IsUnique();

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties().Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetPrecision(18);
                property.SetScale(2);
            }
        }

        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        modelBuilder.Entity<Section>()
            .HasOne(s => s.ParentSection)
            .WithMany(s => s.SubSections)
            .HasForeignKey(s => s.ParentSectionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Section>()
            .HasOne(s => s.StudentGroup)
            .WithMany()
            .HasForeignKey(s => s.StudentGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Section>()
            .HasIndex(x => new { x.SchoolClassId, x.StudentGroupId, x.Name })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<TeacherClassAssignment>()
            .HasIndex(x => new { x.TeacherId, x.ClassId, x.SectionId, x.AcademicYearId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
        
        // Enforce only one active Class Teacher per Class, Section, and Academic Year
        modelBuilder.Entity<TeacherClassAssignment>()
            .HasIndex(x => new { x.ClassId, x.SectionId, x.AcademicYearId })
            .IsUnique()
            .HasFilter("[IsActive] = 1 AND [IsDeleted] = 0");

        modelBuilder.Entity<TeacherSubjectAssignment>()
            .HasIndex(x => new { x.TeacherId, x.SubjectId, x.ClassId, x.SectionId, x.AcademicYearId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<TeacherTimetable>().HasIndex(x => new { x.TeacherId, x.DayOfWeek, x.StartTime }).IsUnique();

        // Exam Configuration Indexes
        modelBuilder.Entity<ExamType>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<ExamConfiguration>().HasIndex(x => new { x.ExamTypeId, x.ClassId }).IsUnique();
        modelBuilder.Entity<SubjectComponent>().HasIndex(x => new { x.ClassSubjectId, x.ComponentName }).IsUnique();
        modelBuilder.Entity<GpaConfiguration>().HasIndex(x => x.Grade).IsUnique();
        modelBuilder.Entity<GpaConfiguration>().HasIndex(x => new { x.MinMarks, x.MaxMarks }).IsUnique();

        // Result Indexes
        modelBuilder.Entity<MarkAuditLog>().HasIndex(x => x.MarkEntryId);
        modelBuilder.Entity<MarkEntryDraft>().HasIndex(x => new { x.ExamId, x.StudentId, x.SubjectId });
        modelBuilder.Entity<ResultLock>().HasIndex(x => x.ExamId);
        modelBuilder.Entity<PromotionHistory>().HasIndex(x => new { x.StudentId, x.AcademicYearId }).IsUnique();
        modelBuilder.Entity<MeritResult>().HasIndex(x => new { x.ExamId, x.StudentId }).IsUnique();
        modelBuilder.Entity<MeritResult>().HasIndex(x => new { x.ExamId, x.SectionId, x.Position });

        // Student Group Indexes
        modelBuilder.Entity<StudentGroupAssignment>().HasIndex(x => new { x.StudentId, x.SchoolClassId, x.AcademicYearId }).IsUnique();
        modelBuilder.Entity<StudentGroup>().HasIndex(x => x.Code).IsUnique();

        // Configure MarkEntry relationship with MarkAuditLog
        modelBuilder.Entity<MarkEntry>()
            .HasMany(m => m.AuditLogs)
            .WithOne(a => a.MarkEntry)
            .HasForeignKey(a => a.MarkEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Student relationship with religion subject
        modelBuilder.Entity<Student>()
            .HasOne(s => s.AssignedReligionSubject)
            .WithMany()
            .HasForeignKey(s => s.AssignedReligionSubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Student relationship with StudentGroup
        modelBuilder.Entity<Student>()
            .HasOne(s => s.StudentGroup)
            .WithMany()
            .HasForeignKey(s => s.StudentGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure StudentGroupAssignment
        modelBuilder.Entity<StudentGroupAssignment>()
            .HasOne(a => a.Student)
            .WithMany(s => s.GroupAssignments)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudentGroupAssignment>()
            .HasOne(a => a.StudentGroup)
            .WithMany(g => g.StudentAssignments)
            .HasForeignKey(a => a.StudentGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure PromotionHistory
        modelBuilder.Entity<PromotionHistory>()
            .HasOne(p => p.FromClass)
            .WithMany()
            .HasForeignKey(p => p.FromClassId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PromotionHistory>()
            .HasOne(p => p.ToClass)
            .WithMany()
            .HasForeignKey(p => p.ToClassId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure ClassSubject relationship with SubjectComponent
        modelBuilder.Entity<SubjectComponent>()
            .HasOne(s => s.ClassSubject)
            .WithMany(c => c.SubjectComponents)
            .HasForeignKey(s => s.ClassSubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        DbInitializer.Seed(modelBuilder);
    }
}
 
