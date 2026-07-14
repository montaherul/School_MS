using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.DTOs.Identity;
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
using SchoolManagementSystem.Models.Entities.Guardian;
using SchoolManagementSystem.Models.Entities.System;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Models.Entities.Transport;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.Entities.Accounting;


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
    public DbSet<WorkflowDefinition> WorkflowDefinitions => Set<WorkflowDefinition>();
    public DbSet<WorkflowTransition> WorkflowTransitions => Set<WorkflowTransition>();
    public DbSet<WorkflowInstance> WorkflowInstances => Set<WorkflowInstance>();
    public DbSet<WorkflowHistoryEntry> WorkflowHistoryEntries => Set<WorkflowHistoryEntry>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Guardian> Guardians => Set<Guardian>();
    public DbSet<StudentGuardian> StudentGuardians => Set<StudentGuardian>();
    public DbSet<GuardianNotification> GuardianNotifications => Set<GuardianNotification>();
    public DbSet<GuardianNotificationLog> GuardianNotificationLogs => Set<GuardianNotificationLog>();
    public DbSet<StudentDocument> StudentDocuments => Set<StudentDocument>();
    public DbSet<StudentPromotion> StudentPromotions => Set<StudentPromotion>();
    public DbSet<TransferCertificate> TransferCertificates => Set<TransferCertificate>();
    public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();
    public DbSet<SchoolClass> Classes => Set<SchoolClass>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<StudentListItemDto> StudentListItemResults => Set<StudentListItemDto>();
    public DbSet<StudentIdCardListDto> StudentIdCardListResults => Set<StudentIdCardListDto>();
    public DbSet<EmployeeIdCardListDto> EmployeeIdCardListResults => Set<EmployeeIdCardListDto>();
    public DbSet<StudentIdCardBulkDto> StudentIdCardBulkResults => Set<StudentIdCardBulkDto>();
    public DbSet<EmployeeIdCardBulkDto> EmployeeIdCardBulkResults => Set<EmployeeIdCardBulkDto>();
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
    public DbSet<StudentLeaveApplication> StudentLeaveApplications => Set<StudentLeaveApplication>();
    public DbSet<StudentAttendance> StudentAttendances => Set<StudentAttendance>();
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<AttendanceSetting> AttendanceSettings => Set<AttendanceSetting>();
    public DbSet<AttendanceLog> AttendanceLogs => Set<AttendanceLog>();
    public DbSet<AttendanceSession> AttendanceSessions => Set<AttendanceSession>();
    public DbSet<AttendanceNotificationLog> AttendanceNotificationLogs => Set<AttendanceNotificationLog>();
    public DbSet<AttendanceRevision> AttendanceRevisions => Set<AttendanceRevision>();
    public DbSet<AutoAbsentExecutionLog> AutoAbsentExecutionLogs => Set<AutoAbsentExecutionLog>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<ExamClass> ExamClasses => Set<ExamClass>();
    public DbSet<ExamSection> ExamSections => Set<ExamSection>();
    public DbSet<ExamSubject> ExamSubjects => Set<ExamSubject>();
    public DbSet<ExamSubjectComponent> ExamSubjectComponents => Set<ExamSubjectComponent>();
    public DbSet<ExamSchedule> ExamSchedules => Set<ExamSchedule>();
    public DbSet<AdmitCard> AdmitCards => Set<AdmitCard>();
    public DbSet<MarkEntry> Marks => Set<MarkEntry>();
    public DbSet<GradingRule> GradingRules => Set<GradingRule>();
    public DbSet<ResultPublication> ResultPublications => Set<ResultPublication>();
    public DbSet<StudentSubjectResult> StudentSubjectResults => Set<StudentSubjectResult>();
    public DbSet<StudentExamResult> StudentExamResults => Set<StudentExamResult>();
    public DbSet<StudentComponentMark> StudentComponentMarks => Set<StudentComponentMark>();
    public DbSet<FinalResult> FinalResults => Set<FinalResult>();
    public DbSet<ResultAuditLog> ResultAuditLogs => Set<ResultAuditLog>();
    public DbSet<ReEvaluationRequest> ReEvaluationRequests => Set<ReEvaluationRequest>();
    public DbSet<AssignmentTask> Assignments => Set<AssignmentTask>();
    public DbSet<AssignmentSubmission> AssignmentSubmissions => Set<AssignmentSubmission>();
    public DbSet<FeeCategory> FeeCategories => Set<FeeCategory>();
    public DbSet<FeeStructure> FeeStructures => Set<FeeStructure>();
    public DbSet<StudentFeeAssignment> StudentFeeAssignments => Set<StudentFeeAssignment>();
    public DbSet<FeeInvoice> FeeInvoices => Set<FeeInvoice>();
    public DbSet<FeeInvoiceItem> FeeInvoiceItems => Set<FeeInvoiceItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<FeeDiscount> FeeDiscounts => Set<FeeDiscount>();
    public DbSet<FeeWaiver> FeeWaivers => Set<FeeWaiver>();
    public DbSet<FeeRefund> FeeRefunds => Set<FeeRefund>();
    public DbSet<FeeLedger> FeeLedgers => Set<FeeLedger>();
    public DbSet<FeeCollectionSummary> FeeCollectionSummaries => Set<FeeCollectionSummary>();
    public DbSet<OnlinePaymentRequest> OnlinePaymentRequests => Set<OnlinePaymentRequest>();
    public DbSet<PaymentGatewayTransaction> PaymentGatewayTransactions => Set<PaymentGatewayTransaction>();
    public DbSet<AdmissionReceipt> AdmissionReceipts => Set<AdmissionReceipt>();
    public DbSet<LateFeeRule> LateFeeRules => Set<LateFeeRule>();
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
    public DbSet<StoredProcedureDeploymentHistory> StoredProcedureDeploymentHistories => Set<StoredProcedureDeploymentHistory>();
    public DbSet<StudentGroup> StudentGroups => Set<StudentGroup>();
    public DbSet<CurriculumVersion> CurriculumVersions => Set<CurriculumVersion>();
    public DbSet<CurriculumSubject> CurriculumSubjects => Set<CurriculumSubject>();

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
    public DbSet<EmployeeInvitation> EmployeeInvitations => Set<EmployeeInvitation>();
    public DbSet<EmployeeBankAccount> EmployeeBankAccounts => Set<EmployeeBankAccount>();
    public DbSet<EmployeePromotion> EmployeePromotions => Set<EmployeePromotion>();
    public DbSet<EmployeeTransfer> EmployeeTransfers => Set<EmployeeTransfer>();
    public DbSet<EmployeeTraining> EmployeeTrainings => Set<EmployeeTraining>();
    public DbSet<EmployeeAward> EmployeeAwards => Set<EmployeeAward>();
    public DbSet<EmployeeDisciplinaryAction> EmployeeDisciplinaryActions => Set<EmployeeDisciplinaryAction>();

    // Public Website DbSets
    public DbSet<SchoolSetting> SchoolSettings => Set<SchoolSetting>();
    public DbSet<WebsitePage> WebsitePages => Set<WebsitePage>();
    public DbSet<Slider> Sliders => Set<Slider>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventCategory> EventCategories => Set<EventCategory>();
    public DbSet<Gallery> Galleries => Set<Gallery>();
    public DbSet<GalleryImage> GalleryImages => Set<GalleryImage>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();
    public DbSet<AdmissionFeeStructure> AdmissionFeeStructures => Set<AdmissionFeeStructure>();
    public DbSet<EventNotification> EventNotifications => Set<EventNotification>();
    public DbSet<EventNotificationRecipient> EventNotificationRecipients => Set<EventNotificationRecipient>();
    public DbSet<EventNotificationLog> EventNotificationLogs => Set<EventNotificationLog>();
    public DbSet<EventNotificationQueue> EventNotificationQueues => Set<EventNotificationQueue>();
    public DbSet<GuardainNotificationPreference> GuardainNotificationPreferences => Set<GuardainNotificationPreference>();
    public DbSet<EventNotificationAttachment> EventNotificationAttachments => Set<EventNotificationAttachment>();
    public DbSet<ScheduledNotification> ScheduledNotifications => Set<ScheduledNotification>();
    public DbSet<ReminderConfig> ReminderConfigs => Set<ReminderConfig>();

    public DbSet<ClassSubject> ClassSubjects => Set<ClassSubject>();
    public DbSet<ClassSubjectGroup> ClassSubjectGroups => Set<ClassSubjectGroup>();

    // Exam Configuration DbSets
    public DbSet<ExamType> ExamTypes => Set<ExamType>();
    public DbSet<ExamConfiguration> ExamConfigurations => Set<ExamConfiguration>();
    public DbSet<ExamComponent> ExamComponents => Set<ExamComponent>();
    public DbSet<SubjectMarkStructure> SubjectMarkStructures => Set<SubjectMarkStructure>();
    public DbSet<ExamTemplate> ExamTemplates => Set<ExamTemplate>();

    // Result DbSets
    public DbSet<ResultSetting> ResultSettings => Set<ResultSetting>();
    public DbSet<ClassPromotionRule> ClassPromotionRules => Set<ClassPromotionRule>();
    public DbSet<ResultLock> ResultLocks => Set<ResultLock>();
    public DbSet<PromotionHistory> PromotionHistories => Set<PromotionHistory>();

    // FIN-02: Enterprise Accounting DbSets
    public DbSet<ChartOfAccount> ChartOfAccounts => Set<ChartOfAccount>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalEntryLine> JournalEntryLines => Set<JournalEntryLine>();
    public DbSet<GeneralLedgerEntry> GeneralLedgerEntries => Set<GeneralLedgerEntry>();
    public DbSet<BankTransaction> BankTransactions => Set<BankTransaction>();
    public DbSet<FinancialPeriod> FinancialPeriods => Set<FinancialPeriod>();

    // Phase 5: Dynamic Result Policy & Promotion Engine DbSets
    public DbSet<ResultPolicy> ResultPolicies => Set<ResultPolicy>();
    public DbSet<ResultPolicyExamWeight> ResultPolicyExamWeights => Set<ResultPolicyExamWeight>();
    public DbSet<RankingRule> RankingRules => Set<RankingRule>();
    public DbSet<PromotionPolicy> PromotionPolicies => Set<PromotionPolicy>();
    public DbSet<PromotionPolicyRule> PromotionPolicyRules => Set<PromotionPolicyRule>();
    public DbSet<PromotionExecution> PromotionExecutions => Set<PromotionExecution>();
    public DbSet<RollGenerationConfig> RollGenerationConfigs => Set<RollGenerationConfig>();
    public DbSet<GroupPromotionConfig> GroupPromotionConfigs => Set<GroupPromotionConfig>();
    public DbSet<PromotioSession> PromotioSessions => Set<PromotioSession>();
    public DbSet<ClassProgressionRule> ClassProgressionRules => Set<ClassProgressionRule>();
    public DbSet<ReportCardPrintQueueItem> ReportCardPrintQueueItems => Set<ReportCardPrintQueueItem>();
    public DbSet<AcademicCalendar> AcademicCalendars { get; set; }
    public DbSet<AcademicCalendarEvent> AcademicCalendarEvents { get; set; }
    public DbSet<HolidayMaster> HolidayMasters { get; set; }

    // Student Group DbSet
    public DbSet<StudentGroupAssignment> StudentGroupAssignments => Set<StudentGroupAssignment>();

    // Academic Foundation DbSets
    public DbSet<SchoolSession> SchoolSessions => Set<SchoolSession>();
    public DbSet<SchoolShift> SchoolShifts => Set<SchoolShift>();
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<SubjectCategory> SubjectCategories => Set<SubjectCategory>();

    // Routine Module DbSets
    public DbSet<Models.Entities.Routine.RoutinePeriod> RoutinePeriods => Set<Models.Entities.Routine.RoutinePeriod>();
    public DbSet<Models.Entities.Routine.Room> Rooms => Set<Models.Entities.Routine.Room>();
    public DbSet<Models.Entities.Routine.SubjectRequirement> SubjectRequirements => Set<Models.Entities.Routine.SubjectRequirement>();
    public DbSet<Models.Entities.Routine.RoutineEntry> RoutineEntries => Set<Models.Entities.Routine.RoutineEntry>();
    public DbSet<Models.Entities.Routine.WorkingDay> WorkingDays => Set<Models.Entities.Routine.WorkingDay>();
    public DbSet<Models.Entities.Routine.TeacherAvailability> TeacherAvailabilities => Set<Models.Entities.Routine.TeacherAvailability>();
    public DbSet<Models.Entities.Routine.RoutineGeneration> RoutineGenerations => Set<Models.Entities.Routine.RoutineGeneration>();
    public DbSet<Models.Entities.Routine.RoutineConflict> RoutineConflicts => Set<Models.Entities.Routine.RoutineConflict>();
    public DbSet<Models.Entities.Routine.RoutineVersion> RoutineVersions => Set<Models.Entities.Routine.RoutineVersion>();
    public DbSet<Models.Entities.Routine.SubstituteAssignment> SubstituteAssignments => Set<Models.Entities.Routine.SubstituteAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserRole>().HasKey(x => new { x.UserId, x.RoleId });
        modelBuilder.Entity<RolePermission>().HasKey(x => new { x.RoleId, x.PermissionId });
        modelBuilder.Entity<AdmissionListResultDto>().HasNoKey();
        modelBuilder.Entity<StudentListItemDto>().HasNoKey();
        modelBuilder.Entity<StudentIdCardListDto>().HasNoKey();
        modelBuilder.Entity<EmployeeIdCardListDto>().HasNoKey();

        modelBuilder.Entity<ApplicationUser>().HasIndex(x => x.UserName).IsUnique();
        modelBuilder.Entity<ApplicationUser>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<UserSession>().HasIndex(x => x.SessionId).IsUnique();
        modelBuilder.Entity<AdmissionApplication>().HasIndex(x => x.ApplicationNo).IsUnique();
        modelBuilder.Entity<AdmissionApplication>().HasIndex(x => x.LinkedGuardianId);
        modelBuilder.Entity<AttendanceNotificationLog>().HasIndex(x => x.GuardianId);
        modelBuilder.Entity<Student>().HasIndex(x => x.StudentNo).IsUnique();
        modelBuilder.Entity<Guardian>().HasIndex(x => x.GuardianCode).IsUnique();
        modelBuilder.Entity<Guardian>().HasIndex(x => x.MobileNumber).IsUnique();
        modelBuilder.Entity<StudentGuardian>().HasIndex(x => new { x.StudentId, x.GuardianId }).IsUnique();
        modelBuilder.Entity<Student>().HasIndex(x => new { x.ClassId, x.SectionId, x.RollNumber }).IsUnique();
        modelBuilder.Entity<MarkEntry>().HasIndex(x => new { x.ExamId, x.StudentId, x.SubjectId }).IsUnique();
        modelBuilder.Entity<StudentSubjectResult>().HasIndex(x => new { x.ExamId, x.StudentId, x.SubjectId }).IsUnique();
        modelBuilder.Entity<StudentExamResult>().HasIndex(x => new { x.ExamId, x.StudentId }).IsUnique();
        modelBuilder.Entity<ExamSubject>().HasIndex(x => new { x.ExamId, x.SubjectId }).IsUnique().HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<ExamSchedule>().HasIndex(x => new { x.ExamId, x.SubjectId, x.ClassId, x.StudentGroupId, x.SectionId }).IsUnique().HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<AdmitCard>().HasIndex(x => new { x.ExamId, x.StudentId }).IsUnique().HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<FinalResult>().HasIndex(x => new { x.AcademicYearId, x.StudentId }).IsUnique();
        modelBuilder.Entity<ReEvaluationRequest>().HasIndex(x => new { x.ExamId, x.StudentId, x.SubjectId }).IsUnique();
        modelBuilder.Entity<AttendanceRecord>().HasIndex(x => new { x.StudentId, x.AttendanceDate }).IsUnique().HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<AttendanceSession>().HasIndex(x => new { x.SchoolClassId, x.SectionId, x.StudentGroupId, x.AttendanceDate }).IsUnique().HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<AttendanceNotificationLog>()
            .HasIndex(x => new { x.StudentId, x.AttendanceDate, x.NotificationType, x.NotificationChannel })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0 AND [EmployeeId] IS NULL");
        modelBuilder.Entity<AttendanceNotificationLog>()
            .HasIndex(x => new { x.EmployeeId, x.AttendanceDate, x.NotificationType, x.NotificationChannel })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0 AND [EmployeeId] IS NOT NULL");
        modelBuilder.Entity<Subject>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<Teacher>().HasIndex(x => x.TeacherCode).IsUnique();
        modelBuilder.Entity<FeeCategory>().HasIndex(x => x.Name).IsUnique().HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<FeeStructure>().HasIndex(x => new { x.SchoolClassId, x.FeeCategoryId, x.FeeName }).IsUnique().HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<FeeInvoice>().HasIndex(x => x.InvoiceNo).IsUnique();
        modelBuilder.Entity<StudentFeeAssignment>().HasIndex(x => new { x.StudentId, x.FeeStructureId, x.AcademicYearId }).IsUnique().HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<FeeDiscount>().HasIndex(x => x.Name).IsUnique().HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<FeeLedger>().HasIndex(x => new { x.StudentId, x.TransactionDate });
        modelBuilder.Entity<FeeCollectionSummary>().HasIndex(x => new { x.CollectionDate, x.PaymentMethod }).IsUnique().HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<LateFeeRule>().HasIndex(x => x.Name).IsUnique().HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<Book>().HasIndex(x => x.AccessionNo).IsUnique();
        modelBuilder.Entity<AdmissionFeeStructure>().HasIndex(x => x.SchoolClassId).IsUnique();

        // Employee Indexes
        modelBuilder.Entity<Employee>().HasIndex(x => x.EmployeeCode).IsUnique();
        modelBuilder.Entity<Employee>().HasIndex(x => x.Phone).IsUnique();
        modelBuilder.Entity<Employee>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<Employee>().HasIndex(x => x.NIDNumber).IsUnique().HasFilter("[NIDNumber] IS NOT NULL");
        modelBuilder.Entity<Employee>().HasIndex(x => x.EmployeeCardNumber).IsUnique().HasFilter("[EmployeeCardNumber] IS NOT NULL");
        modelBuilder.Entity<Employee>().HasIndex(x => x.QRVerificationCode).IsUnique().HasFilter("[QRVerificationCode] IS NOT NULL");
        modelBuilder.Entity<Employee>().HasIndex(x => x.BirthCertificateNo).IsUnique().HasFilter("[BirthCertificateNo] IS NOT NULL");
        modelBuilder.Entity<Employee>().HasIndex(x => x.PassportNo).IsUnique().HasFilter("[PassportNo] IS NOT NULL");
        modelBuilder.Entity<Employee>().HasIndex(x => x.TIN).IsUnique().HasFilter("[TIN] IS NOT NULL");
        modelBuilder.Entity<Employee>().HasIndex(x => x.DrivingLicenseNo).IsUnique().HasFilter("[DrivingLicenseNo] IS NOT NULL");
        modelBuilder.Entity<Employee>().HasIndex(x => x.Status).HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<Employee>().HasIndex(x => x.DepartmentId).HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<Employee>().HasIndex(x => x.DesignationId).HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<Employee>().HasIndex(x => x.UserId).HasFilter("[UserId] IS NOT NULL AND [IsDeleted] = 0");
        modelBuilder.Entity<Employee>().HasIndex(x => x.IsTeachingStaff).HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<Employee>().HasIndex(x => x.JoiningDate).HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<DesignationRoleMapping>().HasIndex(x => new { x.DesignationId, x.RoleId }).IsUnique();
        modelBuilder.Entity<EmployeeAttendance>().HasIndex(x => new { x.EmployeeId, x.AttendanceDate }).IsUnique();

        modelBuilder.Entity<EmployeeInvitation>().HasIndex(x => x.InvitationCode).IsUnique();
        modelBuilder.Entity<EmployeeInvitation>().HasIndex(x => x.InvitationToken).IsUnique();
        modelBuilder.Entity<EmployeeInvitation>().HasIndex(x => x.Email).HasFilter("[IsDeleted] = 0");

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
            .HasIndex(x => new { x.SchoolClassId, x.Name })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<TeacherClassAssignment>()
            .HasIndex(x => new { x.TeacherId, x.ClassId, x.SectionId, x.GroupId, x.AcademicYearId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
        
        // Enforce only one active class teacher per class, section, group, and academic year.
        modelBuilder.Entity<TeacherClassAssignment>()
            .HasIndex(x => new { x.ClassId, x.SectionId, x.GroupId, x.AcademicYearId })
            .IsUnique()
            .HasFilter("[IsActive] = 1 AND [IsDeleted] = 0");

        modelBuilder.Entity<TeacherSubjectAssignment>()
            .HasIndex(x => new { x.TeacherId, x.SubjectId, x.ClassId, x.SectionId, x.GroupId, x.AcademicYearId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<TeacherTimetable>().HasIndex(x => new { x.TeacherId, x.DayOfWeek, x.StartTime }).IsUnique();

        // Exam Configuration Indexes
        modelBuilder.Entity<ExamType>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<ExamConfiguration>().HasIndex(x => new { x.ExamTypeId, x.ClassId }).IsUnique();
        modelBuilder.Entity<ExamComponent>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<SubjectMarkStructure>().HasIndex(x => new { x.ComponentId, x.SubjectId, x.StudentGroupId }).IsUnique();

        // ExamClass
        modelBuilder.Entity<ExamClass>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Exam).WithMany(g => g.Classes).HasForeignKey(e => e.ExamId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Class).WithMany().HasForeignKey(e => e.ClassId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.ExamId, e.ClassId }).IsUnique().HasFilter("[IsDeleted] = 0");
        });

        // ExamSection
        modelBuilder.Entity<ExamSection>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.ExamClass).WithMany(c => c.Sections).HasForeignKey(e => e.ExamClassId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Section).WithMany().HasForeignKey(e => e.SectionId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.ExamClassId, e.SectionId }).IsUnique().HasFilter("[IsDeleted] = 0");
        });

        // ExamSubject
        modelBuilder.Entity<ExamSubject>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Exam).WithMany(c => c.ExamSubjects).HasForeignKey(e => e.ExamId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Subject).WithMany().HasForeignKey(e => e.SubjectId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Teacher).WithMany().HasForeignKey(e => e.TeacherId).OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => new { e.ExamId, e.ClassId, e.SubjectId }).IsUnique().HasFilter("[IsDeleted] = 0");
        });

// ExamSubjectComponent
        modelBuilder.Entity<ExamSubjectComponent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.ExamSubject).WithMany(s => s.Components).HasForeignKey(e => e.ExamSubjectId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Component).WithMany().HasForeignKey(e => e.ComponentId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.ExamSubjectId, e.ComponentId }).IsUnique().HasFilter("[IsDeleted] = 0");
        });

// ClassSubject unique constraint (no more GroupName — junction table handles multi-group)
        modelBuilder.Entity<ClassSubject>()
            .HasIndex(x => new { x.SchoolClassId, x.SubjectId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        // ClassSubjectGroup junction: each (ClassSubject, StudentGroup) once
        modelBuilder.Entity<ClassSubjectGroup>()
            .HasIndex(x => new { x.ClassSubjectId, x.StudentGroupId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<ClassSubjectGroup>()
            .HasOne(x => x.ClassSubject)
            .WithMany(cs => cs.ClassSubjectGroups)
            .HasForeignKey(x => x.ClassSubjectId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ClassSubjectGroup>()
            .HasOne(x => x.StudentGroup)
            .WithMany(g => g.ClassSubjectGroups)
            .HasForeignKey(x => x.StudentGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        // Result Indexes
        modelBuilder.Entity<ResultLock>().HasIndex(x => x.ExamId);
        modelBuilder.Entity<PromotionHistory>().HasIndex(x => new { x.StudentId, x.AcademicYearId }).IsUnique();

        // Phase 5: Result Policy & Promotion Engine Indexes
        modelBuilder.Entity<ResultPolicy>().HasIndex(x => new { x.AcademicYearId, x.SchoolClassId }).IsUnique().HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<ResultPolicyExamWeight>().HasIndex(x => new { x.ResultPolicyId, x.ExamTypeId }).IsUnique().HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<RankingRule>().HasIndex(x => new { x.AcademicYearId, x.SchoolClassId }).IsUnique().HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<PromotionPolicy>().HasIndex(x => new { x.AcademicYearId, x.SchoolClassId }).IsUnique().HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<PromotionExecution>().HasIndex(x => new { x.AcademicYearId, x.SchoolClassId }).IsUnique().HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<RollGenerationConfig>().HasIndex(x => new { x.AcademicYearId, x.SchoolClassId }).IsUnique().HasFilter("[IsDeleted] = 0");
        modelBuilder.Entity<GroupPromotionConfig>().HasIndex(x => new { x.AcademicYearId, x.FromClassId }).IsUnique().HasFilter("[IsDeleted] = 0");

        // ResultPolicy FK
        modelBuilder.Entity<ResultPolicy>()
            .HasOne(p => p.AcademicYear).WithMany()
            .HasForeignKey(p => p.AcademicYearId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ResultPolicy>()
            .HasOne(p => p.SchoolClass).WithMany()
            .HasForeignKey(p => p.SchoolClassId).OnDelete(DeleteBehavior.Restrict);

        // ResultPolicyExamWeight FK
        modelBuilder.Entity<ResultPolicyExamWeight>()
            .HasOne(w => w.ResultPolicy).WithMany(p => p.ExamWeights)
            .HasForeignKey(w => w.ResultPolicyId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ResultPolicyExamWeight>()
            .HasOne(w => w.ExamType).WithMany()
            .HasForeignKey(w => w.ExamTypeId).OnDelete(DeleteBehavior.Restrict);

        // RankingRule FK
        modelBuilder.Entity<RankingRule>()
            .HasOne(r => r.AcademicYear).WithMany()
            .HasForeignKey(r => r.AcademicYearId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<RankingRule>()
            .HasOne(r => r.SchoolClass).WithMany()
            .HasForeignKey(r => r.SchoolClassId).OnDelete(DeleteBehavior.Restrict);

        // PromotionPolicy FK
        modelBuilder.Entity<PromotionPolicy>()
            .HasOne(p => p.AcademicYear).WithMany()
            .HasForeignKey(p => p.AcademicYearId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PromotionPolicy>()
            .HasOne(p => p.SchoolClass).WithMany()
            .HasForeignKey(p => p.SchoolClassId).OnDelete(DeleteBehavior.Restrict);

        // PromotionPolicyRule FK
        modelBuilder.Entity<PromotionPolicyRule>()
            .HasOne(r => r.PromotionPolicy).WithMany(p => p.Rules)
            .HasForeignKey(r => r.PromotionPolicyId).OnDelete(DeleteBehavior.Cascade);

        // PromotionExecution FK
        modelBuilder.Entity<PromotionExecution>()
            .HasOne(e => e.AcademicYear).WithMany()
            .HasForeignKey(e => e.AcademicYearId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PromotionExecution>()
            .HasOne(e => e.SchoolClass).WithMany()
            .HasForeignKey(e => e.SchoolClassId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PromotionExecution>()
            .HasOne(e => e.PromotionPolicy).WithMany()
            .HasForeignKey(e => e.PromotionPolicyId).OnDelete(DeleteBehavior.Restrict);

        // RollGenerationConfig FK
        modelBuilder.Entity<RollGenerationConfig>()
            .HasOne(c => c.AcademicYear).WithMany()
            .HasForeignKey(c => c.AcademicYearId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<RollGenerationConfig>()
            .HasOne(c => c.SchoolClass).WithMany()
            .HasForeignKey(c => c.SchoolClassId).OnDelete(DeleteBehavior.Restrict);

        // GroupPromotionConfig FK
        modelBuilder.Entity<GroupPromotionConfig>()
            .HasOne(c => c.AcademicYear).WithMany()
            .HasForeignKey(c => c.AcademicYearId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<GroupPromotionConfig>()
            .HasOne(c => c.FromClass).WithMany()
            .HasForeignKey(c => c.FromClassId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<GroupPromotionConfig>()
            .HasOne(c => c.ToClass).WithMany()
            .HasForeignKey(c => c.ToClassId).OnDelete(DeleteBehavior.Restrict);

        // PromotioSession configuration
        modelBuilder.Entity<PromotioSession>(entity =>
        {
            entity.HasIndex(e => new { e.AcademicYearId, e.Status });
            entity.HasOne(e => e.AcademicYear).WithMany().HasForeignKey(e => e.AcademicYearId).OnDelete(DeleteBehavior.Restrict);
        });

        // ClassProgressionRule configuration
        modelBuilder.Entity<ClassProgressionRule>(entity =>
        {
            entity.HasIndex(e => new { e.FromClassId, e.ToClassId }).IsUnique().HasFilter("IsDeleted = 0");
            entity.HasOne(e => e.FromClass).WithMany().HasForeignKey(e => e.FromClassId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.ToClass).WithMany().HasForeignKey(e => e.ToClassId).OnDelete(DeleteBehavior.Restrict);
        });

        // Student Group Indexes
        modelBuilder.Entity<StudentGroupAssignment>().HasIndex(x => new { x.StudentId, x.SchoolClassId, x.AcademicYearId }).IsUnique();
        modelBuilder.Entity<StudentGroup>().HasIndex(x => x.Code).IsUnique();

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

        modelBuilder.Entity<AcademicCalendar>()
            .HasOne(x => x.AcademicYear)
            .WithMany()
            .HasForeignKey(x => x.AcademicYearId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AcademicCalendar>()
            .HasIndex(x => x.Date)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<AcademicCalendar>()
            .HasIndex(x => new { x.AcademicYearId, x.Date })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<AcademicCalendar>()
            .HasIndex(x => new { x.Date, x.IsHoliday })
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<AcademicCalendar>()
            .HasIndex(x => new { x.Date, x.IsExamDay })
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<AcademicCalendar>()
            .HasIndex(x => new { x.Date, x.IsEventDay })
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<AttendanceSetting>()
            .HasIndex(x => x.IsActive)
            .IsUnique()
            .HasFilter("[IsActive] = 1");

        modelBuilder.Entity<AcademicCalendarEvent>()
                    .HasOne(x => x.AcademicCalendar)
                    .WithMany()
                    .HasForeignKey(x => x.AcademicCalendarId)
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<HolidayMaster>()
            .HasIndex(x => new { x.Name, x.HolidayDate })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<HolidayMaster>()
            .HasIndex(x => x.HolidayDate);

        modelBuilder.Entity<Building>()
            .HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<SchoolSession>()
            .HasIndex(x => new { x.AcademicYearId, x.Name })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<SchoolSession>()
            .HasOne(x => x.AcademicYear)
            .WithMany()
            .HasForeignKey(x => x.AcademicYearId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SchoolShift>()
            .HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<SubjectCategory>()
            .HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<SubjectCategory>()
            .HasIndex(x => new { x.Name, x.Code })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<StudentLeaveApplication>(entity =>
{
    entity.HasOne(s => s.Student)
          .WithMany()
          .HasForeignKey(s => s.StudentId)
          .OnDelete(DeleteBehavior.Restrict);
    entity.HasOne(s => s.Guardian)
          .WithMany()
          .HasForeignKey(s => s.GuardianId)
          .OnDelete(DeleteBehavior.Restrict);
});

// Configure SubjectMarkStructure relationships
modelBuilder.Entity<SubjectMarkStructure>()
    .HasOne(s => s.Component)
    .WithMany()
    .HasForeignKey(s => s.ComponentId)
    .OnDelete(DeleteBehavior.Restrict);

modelBuilder.Entity<SubjectMarkStructure>()
    .HasOne(s => s.Class)
    .WithMany()
    .HasForeignKey(s => s.ClassId)
    .OnDelete(DeleteBehavior.Restrict);

modelBuilder.Entity<SubjectMarkStructure>()
    .HasOne(s => s.Subject)
    .WithMany()
    .HasForeignKey(s => s.SubjectId)
    .OnDelete(DeleteBehavior.Restrict);

modelBuilder.Entity<SubjectMarkStructure>()
    .HasOne(s => s.StudentGroup)
    .WithMany()
    .HasForeignKey(s => s.StudentGroupId)
    .OnDelete(DeleteBehavior.Restrict);

// AdmitCard relationships
modelBuilder.Entity<AdmitCard>()
    .HasOne(a => a.Exam)
    .WithMany()
    .HasForeignKey(a => a.ExamId)
    .OnDelete(DeleteBehavior.Restrict);

modelBuilder.Entity<AdmitCard>()
    .HasOne(a => a.Student)
    .WithMany()
    .HasForeignKey(a => a.StudentId)
    .OnDelete(DeleteBehavior.Restrict);

modelBuilder.Entity<AdmitCard>()
    .HasIndex(a => a.AdmitCardNumber)
    .IsUnique()
    .HasFilter("[IsDeleted] = 0 AND [AdmitCardNumber] IS NOT NULL");

// Routine Module Indexes
        modelBuilder.Entity<Models.Entities.Routine.RoutineEntry>()
            .HasIndex(x => new { x.AcademicYearId, x.DayNumber, x.RoutinePeriodId, x.RoomId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<Models.Entities.Routine.RoutineEntry>()
            .HasIndex(x => new { x.AcademicYearId, x.DayNumber, x.RoutinePeriodId, x.TeacherId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<Models.Entities.Routine.RoutineEntry>()
            .HasIndex(x => new { x.AcademicYearId, x.DayNumber, x.RoutinePeriodId, x.ClassId, x.SectionId, x.GroupId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<Models.Entities.Routine.RoutinePeriod>()
            .HasIndex(x => new { x.PeriodNumber, x.IsBreak });

        modelBuilder.Entity<Models.Entities.Routine.Room>()
            .HasIndex(x => x.RoomNo)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<Models.Entities.Routine.SubjectRequirement>()
            .HasIndex(x => new { x.AcademicYearId, x.ClassId, x.SectionId, x.GroupId, x.SubjectId, x.TeacherId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<Models.Entities.Routine.TeacherAvailability>()
            .HasIndex(x => new { x.TeacherId, x.DayNumber, x.RoutinePeriodId })
            .IsUnique();

        modelBuilder.Entity<Models.Entities.Routine.WorkingDay>()
            .HasIndex(x => new { x.AcademicYearId, x.DayNumber })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        // RoutineEntry relationships
        modelBuilder.Entity<Models.Entities.Routine.RoutineEntry>()
            .HasOne(e => e.AcademicYear).WithMany()
            .HasForeignKey(e => e.AcademicYearId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Models.Entities.Routine.RoutineEntry>()
            .HasOne(e => e.Class).WithMany()
            .HasForeignKey(e => e.ClassId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Models.Entities.Routine.RoutineEntry>()
            .HasOne(e => e.Section).WithMany()
            .HasForeignKey(e => e.SectionId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Models.Entities.Routine.RoutineEntry>()
            .HasOne(e => e.Group).WithMany()
            .HasForeignKey(e => e.GroupId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Models.Entities.Routine.RoutineEntry>()
            .HasOne(e => e.Subject).WithMany()
            .HasForeignKey(e => e.SubjectId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Models.Entities.Routine.RoutineEntry>()
            .HasOne(e => e.Teacher).WithMany()
            .HasForeignKey(e => e.TeacherId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Models.Entities.Routine.RoutineEntry>()
            .HasOne(e => e.Room).WithMany()
            .HasForeignKey(e => e.RoomId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Models.Entities.Routine.RoutineEntry>()
            .HasOne(e => e.RoutinePeriod).WithMany()
            .HasForeignKey(e => e.RoutinePeriodId).OnDelete(DeleteBehavior.Restrict);

        // TeacherAvailability relationships
        modelBuilder.Entity<Models.Entities.Routine.TeacherAvailability>()
            .HasOne(a => a.Teacher).WithMany()
            .HasForeignKey(a => a.TeacherId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Models.Entities.Routine.TeacherAvailability>()
            .HasOne(a => a.RoutinePeriod).WithMany()
            .HasForeignKey(a => a.RoutinePeriodId).OnDelete(DeleteBehavior.Restrict);

        // SubstituteAssignment relationships
        modelBuilder.Entity<Models.Entities.Routine.SubstituteAssignment>()
            .HasOne(a => a.RoutineEntry).WithMany()
            .HasForeignKey(a => a.RoutineEntryId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Models.Entities.Routine.SubstituteAssignment>()
            .HasOne(a => a.OriginalTeacher).WithMany()
            .HasForeignKey(a => a.OriginalTeacherId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Models.Entities.Routine.SubstituteAssignment>()
            .HasOne(a => a.SubstituteTeacher).WithMany()
            .HasForeignKey(a => a.SubstituteTeacherId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Models.Entities.Routine.SubstituteAssignment>()
            .HasOne(a => a.AssignedBy).WithMany()
            .HasForeignKey(a => a.AssignedById).OnDelete(DeleteBehavior.Restrict);

// Workflow Entity Configurations
        modelBuilder.Entity<WorkflowDefinition>()
            .HasIndex(x => x.Name)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<WorkflowTransition>()
            .HasIndex(x => new { x.WorkflowDefinitionId, x.FromState, x.ToState })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<WorkflowTransition>()
            .HasOne(x => x.WorkflowDefinition)
            .WithMany(x => x.Transitions)
            .HasForeignKey(x => x.WorkflowDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WorkflowInstance>()
            .HasIndex(x => x.AdmissionApplicationId)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<WorkflowInstance>()
            .HasOne(x => x.AdmissionApplication)
            .WithMany()
            .HasForeignKey(x => x.AdmissionApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkflowInstance>()
            .HasOne(x => x.WorkflowDefinition)
            .WithMany()
            .HasForeignKey(x => x.WorkflowDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkflowHistoryEntry>()
            .HasIndex(x => new { x.WorkflowInstanceId, x.ActionedAt });

        modelBuilder.Entity<WorkflowHistoryEntry>()
            .HasOne(x => x.WorkflowInstance)
            .WithMany(x => x.History)
            .HasForeignKey(x => x.WorkflowInstanceId)
            .OnDelete(DeleteBehavior.Cascade);

        // FIN-02: Accounting Entity Configurations
        modelBuilder.Entity<ChartOfAccount>(entity =>
        {
            entity.Property(e => e.OpeningBalance).HasPrecision(18, 2);
            entity.HasIndex(e => e.AccountCode).IsUnique().HasFilter("[IsDeleted] = 0");
            entity.HasOne<ChartOfAccount>().WithMany().HasForeignKey(e => e.ParentAccountId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<JournalEntry>(entity =>
        {
            entity.HasIndex(e => e.JournalNo).IsUnique().HasFilter("[IsDeleted] = 0");
            entity.HasOne<FinancialPeriod>().WithMany().HasForeignKey(e => e.FinancialPeriodId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<JournalEntryLine>(entity =>
        {
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.HasOne<JournalEntry>().WithMany().HasForeignKey(e => e.JournalEntryId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<ChartOfAccount>().WithMany().HasForeignKey(e => e.AccountId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.JournalEntryId, e.AccountId }).HasFilter("[IsDeleted] = 0");
        });

        modelBuilder.Entity<GeneralLedgerEntry>(entity =>
        {
            entity.Property(e => e.DebitAmount).HasPrecision(18, 2);
            entity.Property(e => e.CreditAmount).HasPrecision(18, 2);
            entity.Property(e => e.RunningBalance).HasPrecision(18, 2);
            entity.HasOne<ChartOfAccount>().WithMany().HasForeignKey(e => e.AccountId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<JournalEntry>().WithMany().HasForeignKey(e => e.JournalEntryId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne<FinancialPeriod>().WithMany().HasForeignKey(e => e.FinancialPeriodId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.AccountId, e.EntryDate });
        });

        modelBuilder.Entity<BankTransaction>(entity =>
        {
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.HasOne<ChartOfAccount>().WithMany().HasForeignKey(e => e.AccountId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<FinancialPeriod>().WithMany().HasForeignKey(e => e.FinancialPeriodId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.AccountId, e.TransactionDate });
            entity.HasIndex(e => e.ReferenceNo).HasFilter("[ReferenceNo] IS NOT NULL");
        });

        modelBuilder.Entity<FinancialPeriod>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique().HasFilter("[IsDeleted] = 0");
        });

        modelBuilder.Entity<OnlinePaymentRequest>(entity =>
        {
            entity.HasIndex(e => e.AdmissionApplicationId).HasFilter("[AdmissionApplicationId] IS NOT NULL");
        });

        modelBuilder.Entity<AdmissionReceipt>(entity =>
        {
            entity.HasIndex(e => e.ReceiptNo).IsUnique().HasFilter("[IsDeleted] = 0");
            entity.HasIndex(e => e.AdmissionApplicationId);
        });

DbInitializer.Seed(modelBuilder);
    }
}
 
