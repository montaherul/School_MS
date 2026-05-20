using SchoolManagementSystem.Helpers.Email;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Helpers.Security;
using SchoolManagementSystem.Repositories.Interfaces.Website;
using SchoolManagementSystem.Repositories.Implementations.Website;
using SchoolManagementSystem.Services.Interfaces.Website;
using SchoolManagementSystem.Services.Implementations.Website;
using SchoolManagementSystem.Repositories.Implementations;
using SchoolManagementSystem.Repositories.Implementations.Academic;
using SchoolManagementSystem.Repositories.Implementations.Admission;
using SchoolManagementSystem.Repositories.Implementations.Attendance;
using SchoolManagementSystem.Repositories.Implementations.Fees;
using SchoolManagementSystem.Repositories.Implementations.Students;
using SchoolManagementSystem.Repositories.Implementations.Teachers;
using SchoolManagementSystem.Repositories.Implementations.Result;
using SchoolManagementSystem.Repositories.Implementations.Dashboard;
using SchoolManagementSystem.Repositories.Implementations.Auth;
<<<<<<< HEAD
using SchoolManagementSystem.Repositories.Implementations.Employee;

=======
>>>>>>> d8b24e6 (attendece and website curtomize)
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Admission;
using SchoolManagementSystem.Repositories.Interfaces.Attendance;
using SchoolManagementSystem.Repositories.Interfaces.Fees;
using SchoolManagementSystem.Repositories.Interfaces.Students;
using SchoolManagementSystem.Repositories.Interfaces.Teachers;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Repositories.Interfaces.Auth;
using SchoolManagementSystem.Repositories.Interfaces.Dashboard;
<<<<<<< HEAD
using SchoolManagementSystem.Repositories.Interfaces.Employee;

=======
>>>>>>> d8b24e6 (attendece and website curtomize)
using SchoolManagementSystem.Service.Implementations.Dashboard;
using SchoolManagementSystem.Service.Interfaces.Dashboard;
using SchoolManagementSystem.Services.Implementations.Admissions;
using SchoolManagementSystem.Services.Implementations.Admin;
<<<<<<< HEAD
using SchoolManagementSystem.Services.Implementations.Academic;
using SchoolManagementSystem.Services.Implementations.Dashboard;
using SchoolManagementSystem.Services.Implementations.Email;
using SchoolManagementSystem.Services.Implementations.Result;
using SchoolManagementSystem.Services.Implementations.Students;
using SchoolManagementSystem.Services.Implementations.Teachers;
using SchoolManagementSystem.Services.Implementations.Employee;

using SchoolManagementSystem.Services.Interfaces.Admissions;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.Services.Interfaces.Dashboard;
using SchoolManagementSystem.Services.Interfaces.Email;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.Services.Interfaces.Employee;

using SchoolManagementSystem.UnitOfWork.Implementations;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Services.Interfaces.Infrastructure;
using SchoolManagementSystem.Services.Implementations.Infrastructure;
using SchoolManagementSystem.Data.Seeders;
using Microsoft.Extensions.Caching.Memory;
=======
using SchoolManagementSystem.Services.Implementations.Email;
using SchoolManagementSystem.Services.Implementations.Fees;
using SchoolManagementSystem.Services.Implementations.Result;
using SchoolManagementSystem.Services.Implementations.Students;
using SchoolManagementSystem.Services.Implementations.Teachers;
using SchoolManagementSystem.Services.Interfaces.Admissions;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.Services.Interfaces.Email;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.UnitOfWork.Implementations;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Employee;
using SchoolManagementSystem.Repositories.Implementations.Employee;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.Services.Implementations.Employee;
>>>>>>> d8b24e6 (attendece and website curtomize)

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
<<<<<<< HEAD
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
=======
>>>>>>> d8b24e6 (attendece and website curtomize)
        services.AddScoped<IAdmissionRepository, AdmissionRepository>();
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();
        services.AddScoped<IFeeStructureRepository, FeeStructureRepository>();
        services.AddScoped<IFeeInvoiceRepository, FeeInvoiceRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<ITeacherRepository, TeacherRepository>();
        services.AddScoped<ITeacherClassAssignmentRepository, TeacherClassAssignmentRepository>();
        services.AddScoped<ITeacherSubjectAssignmentRepository, TeacherSubjectAssignmentRepository>();
<<<<<<< HEAD
=======
        
        // Register Employee Repositories
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IDesignationRepository, DesignationRepository>();
        services.AddScoped<IEmployeeQualificationRepository, EmployeeQualificationRepository>();
        services.AddScoped<IEmployeeDocumentRepository, EmployeeDocumentRepository>();
        services.AddScoped<IEmployeeExperienceRepository, EmployeeExperienceRepository>();

>>>>>>> d8b24e6 (attendece and website curtomize)
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
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<IDashboardQueryRepository, DashboardQueryRepository>();
<<<<<<< HEAD
        
        // Employee Module
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IDesignationRepository, DesignationRepository>();
        services.AddScoped<IEmployeeAttendanceRepository, EmployeeAttendanceRepository>();
        services.AddScoped<IEmployeeLeaveRepository, EmployeeLeaveRepository>();
        services.AddScoped<ILeaveTypeRepository, LeaveTypeRepository>();
        services.AddScoped<IEmployeePayrollRepository, EmployeePayrollRepository>();
        services.AddScoped<ISalaryStructureRepository, SalaryStructureRepository>();
        services.AddScoped<IHolidayRepository, HolidayRepository>();
        services.AddScoped<IEmployeeDocumentRepository, EmployeeDocumentRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

=======

        // Register Public Website Repositories
        services.AddScoped<ISchoolSettingRepository, SchoolSettingRepository>();
        services.AddScoped<IWebsitePageRepository, WebsitePageRepository>();
        services.AddScoped<ISliderRepository, SliderRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IGalleryRepository, GalleryRepository>();
        services.AddScoped<IGalleryImageRepository, GalleryImageRepository>();
        services.AddScoped<INoticeRepository, NoticeRepository>();
>>>>>>> d8b24e6 (attendece and website curtomize)

        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IAdmissionService, AdmissionService>();
<<<<<<< HEAD
        services.AddScoped<ITeacherService, TeacherService>();
        services.AddScoped<ITeacherScopeService, TeacherScopeService>();
        services.AddScoped<ITeacherHRService, TeacherHRService>();
=======
        services.AddScoped<IFeeStructureService, FeeStructureService>();
        services.AddScoped<IFeeInvoiceService, FeeInvoiceService>();
        services.AddScoped<ITeacherService, TeacherService>();
        services.AddScoped<ITeacherSynchronizationService, TeacherSynchronizationService>();
        services.AddScoped<ITeacherScopeService, TeacherScopeService>();
        services.AddScoped<ITeacherAssignmentService, TeacherAssignmentService>();
        services.AddScoped<SchoolManagementSystem.Services.Interfaces.Attendance.IAttendanceAuthorizationService, SchoolManagementSystem.Services.Implementations.Attendance.AttendanceAuthorizationService>();
        services.AddScoped<SchoolManagementSystem.Services.Interfaces.Result.IResultAuthorizationService, SchoolManagementSystem.Services.Implementations.Result.ResultAuthorizationService>();

        // Register Employee Services
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IDesignationService, DesignationService>();
        services.AddScoped<IUserProvisionService, UserProvisionService>();
        services.AddScoped<EmployeeModuleSeeder>();

>>>>>>> d8b24e6 (attendece and website curtomize)
        services.AddScoped<IResultCalculationService, ResultCalculationService>();
        services.AddScoped<IGPACalculationService, GPACalculationService>();
        services.AddScoped<IMeritCalculationService, MeritCalculationService>();
        services.AddScoped<IMarkEntryService, MarkEntryService>();
        services.AddScoped<IPromotionService, PromotionService>();
        services.AddScoped<IResultPublicationService, ResultPublicationService>();
        services.AddScoped<IReEvaluationService, ReEvaluationService>();
        services.AddScoped<IReportCardService, ReportCardService>();
        services.AddScoped<IResultAnalyticsService, ResultAnalyticsService>();
<<<<<<< HEAD
=======

        // Register Public Website Services
        services.AddScoped<ISchoolWebsiteService, SchoolWebsiteService>();
        services.AddScoped<ISliderService, SliderService>();
        services.AddScoped<INoticeService, NoticeService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IGalleryService, GalleryService>();
        services.AddScoped<IWebsitePageService, WebsitePageService>();
>>>>>>> d8b24e6 (attendece and website curtomize)
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IPasswordHashService, Pbkdf2PasswordHashService>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IPdfGenerator, PlainPdfGenerator>();

<<<<<<< HEAD
        // Employee Module
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IEmployeeAttendanceService, EmployeeAttendanceService>();
        services.AddScoped<IEmployeeLeaveService, EmployeeLeaveService>();
        services.AddScoped<ILeaveTypeService, LeaveTypeService>();
        services.AddScoped<IEmployeePayrollService, EmployeePayrollService>();
        services.AddScoped<ISalaryStructureService, SalaryStructureService>();
        services.AddScoped<IDashboardResolverService, SchoolManagementSystem.Services.Implementations.Dashboard.DashboardResolverService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IDesignationService, DesignationService>();
        services.AddScoped<IClassRoutineService, ClassRoutineService>();
        services.AddScoped<ITeacherAcademicService, TeacherAcademicService>();
        services.AddScoped<SchoolManagementSystem.Services.Interfaces.Auth.INotificationService, SchoolManagementSystem.Services.Implementations.Auth.NotificationService>();

        // Infrastructure & Hardening
        services.AddMemoryCache();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        
        // Data Seeders
        services.AddScoped<IDataSeederRunner, DataSeederRunner>();
        services.AddScoped<IDataSeeder, RolePermissionSeeder>();
        services.AddScoped<IDataSeeder, HrReferenceDataSeeder>();
=======
        // Attendance & Leave Management Modules
        services.AddScoped<SchoolManagementSystem.Repositories.Interfaces.Attendance.IStudentAttendanceRepository, SchoolManagementSystem.Repositories.Implementations.Attendance.StudentAttendanceRepository>();
        services.AddScoped<SchoolManagementSystem.Repositories.Interfaces.Attendance.IEmployeeAttendanceRepository, SchoolManagementSystem.Repositories.Implementations.Attendance.EmployeeAttendanceRepository>();
        services.AddScoped<SchoolManagementSystem.Repositories.Interfaces.Attendance.ILeaveTypeRepository, SchoolManagementSystem.Repositories.Implementations.Attendance.LeaveTypeRepository>();
        services.AddScoped<SchoolManagementSystem.Repositories.Interfaces.Attendance.ILeaveApplicationRepository, SchoolManagementSystem.Repositories.Implementations.Attendance.LeaveApplicationRepository>();
        services.AddScoped<SchoolManagementSystem.Repositories.Interfaces.Attendance.IAttendanceSettingRepository, SchoolManagementSystem.Repositories.Implementations.Attendance.AttendanceSettingRepository>();
        services.AddScoped<SchoolManagementSystem.Repositories.Interfaces.Attendance.IAttendanceLogRepository, SchoolManagementSystem.Repositories.Implementations.Attendance.AttendanceLogRepository>();

        services.AddScoped<SchoolManagementSystem.Services.Interfaces.Attendance.IAttendanceNotificationService, SchoolManagementSystem.Services.Implementations.Attendance.AttendanceNotificationService>();
        services.AddScoped<SchoolManagementSystem.Services.Interfaces.Attendance.IStudentAttendanceService, SchoolManagementSystem.Services.Implementations.Attendance.StudentAttendanceService>();
        services.AddScoped<SchoolManagementSystem.Services.Interfaces.Attendance.IEmployeeAttendanceService, SchoolManagementSystem.Services.Implementations.Attendance.EmployeeAttendanceService>();
        services.AddScoped<SchoolManagementSystem.Services.Interfaces.Attendance.ILeaveService, SchoolManagementSystem.Services.Implementations.Attendance.LeaveService>();
        services.AddScoped<SchoolManagementSystem.Services.Interfaces.Attendance.IAttendanceReportService, SchoolManagementSystem.Services.Implementations.Attendance.AttendanceReportService>();
>>>>>>> d8b24e6 (attendece and website curtomize)

        return services;
    }
}
