using SchoolManagementSystem.Helpers.Email;
using SchoolManagementSystem.Helpers.Files;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Helpers.Security;
using SchoolManagementSystem.Repositories.Guardian;
using SchoolManagementSystem.Repositories.Implementations;
using SchoolManagementSystem.Repositories.Implementations.Academic;
using SchoolManagementSystem.Repositories.Implementations.Admission;
using SchoolManagementSystem.Repositories.Implementations.Attendance;
using SchoolManagementSystem.Repositories.Implementations.Auth;
using SchoolManagementSystem.Repositories.Implementations.Dashboard;
using SchoolManagementSystem.Repositories.Implementations.Employee;
using SchoolManagementSystem.Repositories.Implementations.Fees;
using SchoolManagementSystem.Repositories.Implementations.Guardian;
using SchoolManagementSystem.Repositories.Implementations.Result;
using SchoolManagementSystem.Repositories.Implementations.Students;
using SchoolManagementSystem.Repositories.Implementations.Teachers;
using SchoolManagementSystem.Repositories.Implementations.Website;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Admission;
using SchoolManagementSystem.Repositories.Interfaces.Attendance;
using SchoolManagementSystem.Repositories.Interfaces.Auth;
using SchoolManagementSystem.Repositories.Interfaces.Dashboard;
using SchoolManagementSystem.Repositories.Interfaces.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Fees;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Repositories.Interfaces.Students;
using SchoolManagementSystem.Repositories.Interfaces.Teachers;
using SchoolManagementSystem.Repositories.Interfaces.Website;
using SchoolManagementSystem.Service.Implementations.Dashboard;
using SchoolManagementSystem.Service.Interfaces.Dashboard;
using SchoolManagementSystem.Services.Guardian;
using SchoolManagementSystem.Services.Implementations.Academic;
using SchoolManagementSystem.Services.Implementations.Admin;
using SchoolManagementSystem.Services.Implementations.Admissions;
using SchoolManagementSystem.Services.Implementations.Email;
using SchoolManagementSystem.Services.Implementations.Employee;
using SchoolManagementSystem.Services.Implementations.Fees;
using SchoolManagementSystem.Services.Implementations.Guardian;
using SchoolManagementSystem.Services.Implementations.Result;
using SchoolManagementSystem.Services.Implementations.Students;
using SchoolManagementSystem.Services.Implementations.Teachers;
using SchoolManagementSystem.Services.Implementations.Website;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.Services.Interfaces.Admissions;
using SchoolManagementSystem.Services.Interfaces.Email;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.Services.Interfaces.Website;
using SchoolManagementSystem.UnitOfWork.Implementations;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Extensions;

public static class ServiceRegistration
{
    public static IServiceCollection AddSchoolApplicationServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IUnitOfWork, SchoolManagementSystem.UnitOfWork.Implementations.UnitOfWork>();

        // Register Module Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IAdmissionRepository, AdmissionRepository>();
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();
        services.AddScoped<IFeeStructureRepository, FeeStructureRepository>();
        services.AddScoped<IFeeInvoiceRepository, FeeInvoiceRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<ITeacherRepository, TeacherRepository>();
        services.AddScoped<ITeacherClassAssignmentRepository, TeacherClassAssignmentRepository>();
        services.AddScoped<ITeacherSubjectAssignmentRepository, TeacherSubjectAssignmentRepository>();
        
        // Register Guardian Repositories
        services.AddScoped<IGuardianRepository, GuardianRepository>();
        
        // Register Employee Repositories
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IDesignationRepository, DesignationRepository>();
        services.AddScoped<IEmployeeQualificationRepository, EmployeeQualificationRepository>();
        services.AddScoped<IEmployeeDocumentRepository, EmployeeDocumentRepository>();
        services.AddScoped<IEmployeeExperienceRepository, EmployeeExperienceRepository>();
        services.AddScoped<IEmployeeInvitationRepository, EmployeeInvitationRepository>();

        services.AddScoped<IAcademicYearRepository, AcademicYearRepository>();
        services.AddScoped<ISchoolClassRepository, SchoolClassRepository>();
        services.AddScoped<ISectionRepository, SectionRepository>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();
        services.AddScoped<IExamRepository, ExamRepository>();
        services.AddScoped<IMarkEntryRepository, MarkEntryRepository>();
        services.AddScoped<IGradingRuleRepository, GradingRuleRepository>();
        services.AddScoped<IResultPublicationRepository, ResultPublicationRepository>();
        services.AddScoped<IStudentSubjectResultRepository, StudentSubjectResultRepository>();
        services.AddScoped<IStudentExamResultRepository, StudentExamResultRepository>();
        services.AddScoped<IReEvaluationRequestRepository, ReEvaluationRequestRepository>();
        services.AddScoped<IResultAuditLogRepository, ResultAuditLogRepository>();
        services.AddScoped<IMeritResultRepository, MeritResultRepository>();
        services.AddScoped<IFinalResultRepository, FinalResultRepository>();
        services.AddScoped<IPromotionHistoryRepository, PromotionHistoryRepository>();
        services.AddScoped<ITeacherResultRepository, TeacherResultRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<IDashboardQueryRepository, DashboardQueryRepository>();
        services.AddScoped<IAcademicCalendarRepository, AcademicCalendarRepository>();
        services.AddScoped<IAcademicCalendarEventRepository, AcademicCalendarEventRepository>();
        // Register Public Website Repositories
        services.AddScoped<ISchoolSettingRepository, SchoolSettingRepository>();
        services.AddScoped<IWebsitePageRepository, WebsitePageRepository>();
        services.AddScoped<ISliderRepository, SliderRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IGalleryRepository, GalleryRepository>();
        services.AddScoped<IGalleryImageRepository, GalleryImageRepository>();
        services.AddScoped<INoticeRepository, NoticeRepository>();
        services.AddScoped<IContactMessageRepository, ContactMessageRepository>();
        services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();

        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IAdmissionService, AdmissionService>();
        services.AddScoped<IGuardianService, GuardianService>();
        services.AddScoped<IFeeStructureService, FeeStructureService>();
        services.AddScoped<IFeeInvoiceService, FeeInvoiceService>();
        services.AddScoped<ITeacherService, TeacherService>();
        services.AddScoped<ITeacherSynchronizationService, TeacherSynchronizationService>();
        services.AddScoped<ITeacherScopeService, TeacherScopeService>();
        services.AddScoped<ITeacherAssignmentService, TeacherAssignmentService>();
        services.AddScoped<SchoolManagementSystem.Services.Interfaces.Attendance.IAttendanceAuthorizationService, SchoolManagementSystem.Services.Implementations.Attendance.AttendanceAuthorizationService>();
        services.AddScoped<SchoolManagementSystem.Services.Interfaces.Result.IResultAuthorizationService, SchoolManagementSystem.Services.Implementations.Result.ResultAuthorizationService>();
        services.AddScoped<IAcademicCalendarService,AcademicCalendarService>();
        // Register Employee Services
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IEmployeeInvitationService, EmployeeInvitationService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IDesignationService, DesignationService>();
        services.AddScoped<IUserProvisionService, UserProvisionService>();
        services.AddScoped<EmployeeModuleSeeder>();

        services.AddScoped<IResultCalculationService, ResultCalculationService>();
        services.AddScoped<IGradeCalculator, GradeCalculator>();
        services.AddScoped<IComponentAggregator, ComponentAggregator>();
        services.AddScoped<IPassFailPolicy, PassFailPolicy>();
        services.AddScoped<IAuditLogger, AuditLogger>();
        services.AddScoped<IGPACalculationService, GPACalculationService>();
        services.AddScoped<IMeritCalculationService, MeritCalculationService>();
        services.AddScoped<IMarkEntryService, MarkEntryService>();
        services.AddScoped<IPromotionService, PromotionService>();
        services.AddScoped<IResultPublicationService, ResultPublicationService>();
        services.AddScoped<IReEvaluationService, ReEvaluationService>();
        services.AddScoped<IReportCardService, ReportCardService>();
        services.AddScoped<IResultAnalyticsService, ResultAnalyticsService>();
        services.AddScoped<IExamComponentService, ExamComponentService>();
        services.AddScoped<ISubjectMarkStructureService, SubjectMarkStructureService>();
        services.AddScoped<IExamValidationService, ExamValidationService>();

        // Register Public Website Services
        services.AddScoped<ISchoolWebsiteService, SchoolWebsiteService>();
        services.AddScoped<ISliderService, SliderService>();
        services.AddScoped<INoticeService, NoticeService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IGalleryService, GalleryService>();
        services.AddScoped<IWebsitePageService, WebsitePageService>();
        services.AddScoped<IContactMessageService, ContactMessageService>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        services.AddScoped<IAdmissionFeeStructureService, AdmissionFeeStructureService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IPasswordHashService, Pbkdf2PasswordHashService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPdfGenerator, PlainPdfGenerator>();
        services.AddScoped<IViewRendererService, ViewRendererService>();

        // Attendance & Leave Management Modules
        services.AddScoped<SchoolManagementSystem.Repositories.Interfaces.Attendance.IStudentAttendanceRepository, SchoolManagementSystem.Repositories.Implementations.Attendance.StudentAttendanceRepository>();
        services.AddScoped<SchoolManagementSystem.Repositories.Interfaces.Attendance.IEmployeeAttendanceRepository, SchoolManagementSystem.Repositories.Implementations.Attendance.EmployeeAttendanceRepository>();
        services.AddScoped<SchoolManagementSystem.Repositories.Interfaces.Attendance.ILeaveTypeRepository, SchoolManagementSystem.Repositories.Implementations.Attendance.LeaveTypeRepository>();
        services.AddScoped<SchoolManagementSystem.Repositories.Interfaces.Attendance.ILeaveApplicationRepository, SchoolManagementSystem.Repositories.Implementations.Attendance.LeaveApplicationRepository>();
        services.AddScoped<SchoolManagementSystem.Repositories.Interfaces.Attendance.IAttendanceSettingRepository, SchoolManagementSystem.Repositories.Implementations.Attendance.AttendanceSettingRepository>();
        services.AddScoped<SchoolManagementSystem.Repositories.Interfaces.Attendance.IAttendanceLogRepository, SchoolManagementSystem.Repositories.Implementations.Attendance.AttendanceLogRepository>();
        services.AddScoped<SchoolManagementSystem.Repositories.Interfaces.Attendance.IAttendanceSessionRepository, SchoolManagementSystem.Repositories.Implementations.Attendance.AttendanceSessionRepository>();

        services.AddScoped<SchoolManagementSystem.Services.Interfaces.Attendance.IAttendanceNotificationService, SchoolManagementSystem.Services.Implementations.Attendance.AttendanceNotificationService>();
        services.AddScoped<SchoolManagementSystem.Services.Interfaces.Attendance.IStudentAttendanceService, SchoolManagementSystem.Services.Implementations.Attendance.StudentAttendanceService>();
        services.AddScoped<SchoolManagementSystem.Services.Interfaces.Attendance.IEmployeeAttendanceService, SchoolManagementSystem.Services.Implementations.Attendance.EmployeeAttendanceService>();
        services.AddScoped<SchoolManagementSystem.Services.Interfaces.Attendance.ILeaveService, SchoolManagementSystem.Services.Implementations.Attendance.LeaveService>();
        services.AddScoped<SchoolManagementSystem.Services.Interfaces.Attendance.IAttendanceReportService, SchoolManagementSystem.Services.Implementations.Attendance.AttendanceReportService>();
        services.AddScoped<SchoolManagementSystem.Services.Interfaces.Attendance.IAttendanceSettingService, SchoolManagementSystem.Services.Implementations.Attendance.AttendanceSettingService>();
        services.AddScoped<SchoolManagementSystem.Services.Interfaces.Attendance.IAttendanceValidationService, SchoolManagementSystem.Services.Implementations.Attendance.AttendanceValidationService>();
        services.AddScoped<SchoolManagementSystem.Services.Interfaces.Attendance.IAttendancePercentageService, SchoolManagementSystem.Services.Implementations.Attendance.AttendancePercentageService>();
        services.AddScoped<SchoolManagementSystem.Services.Interfaces.Attendance.IAutoAbsentService, SchoolManagementSystem.Services.Implementations.Attendance.AutoAbsentService>();
        services.AddHostedService<SchoolManagementSystem.Services.Implementations.Attendance.AttendanceNotificationWorker>();
        services.AddHostedService<SchoolManagementSystem.Services.Implementations.Attendance.AutoAbsentWorker>();

       
        return services;
    }
}
