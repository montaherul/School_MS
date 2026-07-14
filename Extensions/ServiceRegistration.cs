using SchoolManagementSystem.Helpers.Email;
using SchoolManagementSystem.Helpers.Files;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Helpers.Security;
using SchoolManagementSystem.Repositories.Guardian;
using SchoolManagementSystem.Repositories.Implementations;
using SchoolManagementSystem.Repositories.Implementations.Academic;
using SchoolManagementSystem.Repositories.Implementations.Identity;
using SchoolManagementSystem.Repositories.Interfaces.Identity;
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
using SchoolManagementSystem.Repositories.Implementations.Routine;
using SchoolManagementSystem.Repositories.Interfaces.Routine;
using SchoolManagementSystem.Repositories.Interfaces.Accounting;
using SchoolManagementSystem.Repositories.Implementations.Accounting;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Services.Implementations.Exam;
using SchoolManagementSystem.Services.Interfaces.Routine;
using SchoolManagementSystem.Services.Implementations.Routine;
using SchoolManagementSystem.Services.Interfaces.Exam;
using SchoolManagementSystem.Services.Interfaces.Audit;
using SchoolManagementSystem.Services.Implementations.Audit;
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
using SchoolManagementSystem.Services.Interfaces.Guardian;
using SchoolManagementSystem.Services.Interfaces.Student;
using SchoolManagementSystem.Services.Interfaces.Base;
using SchoolManagementSystem.Services.Implementations.Base;
using SchoolManagementSystem.Services.Interfaces.Assignment;
using SchoolManagementSystem.Services.Implementations.Assignment;
using SchoolManagementSystem.Services.Interfaces.Auth;
using SchoolManagementSystem.Services.Implementations.Auth;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.Services.Implementations.Attendance;
using SchoolManagementSystem.Services.Implementations.Academic;
using SchoolManagementSystem.Services.Implementations.Identity;
using SchoolManagementSystem.Services.Interfaces.Identity;
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
using SchoolManagementSystem.Services.Implementations.Academic;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.Services.Interfaces.Admissions;
using SchoolManagementSystem.Services.Interfaces.Email;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.Services.Interfaces.Website;
using SchoolManagementSystem.Services.Implementations.Website;
using SchoolManagementSystem.Services.Interfaces.Accounting;
using SchoolManagementSystem.Services.Implementations.Accounting;
using SchoolManagementSystem.Services.Implementations.Student;
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
        services.AddScoped<IAdmissionDashboardRepository, AdmissionDashboardRepository>();
        services.AddScoped<IWorkflowRepository, WorkflowRepository>();
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();
        services.AddScoped<IFeeCategoryRepository, FeeCategoryRepository>();
        services.AddScoped<IFeeStructureRepository, FeeStructureRepository>();
        services.AddScoped<IFeeInvoiceRepository, FeeInvoiceRepository>();
        services.AddScoped<IStudentFeeAssignmentRepository, StudentFeeAssignmentRepository>();
        services.AddScoped<IFeeInvoiceItemRepository, FeeInvoiceItemRepository>();
        services.AddScoped<IFeePaymentRepository, FeePaymentRepository>();
        services.AddScoped<IFeeDiscountRepository, FeeDiscountRepository>();
        services.AddScoped<IFeeWaiverRepository, FeeWaiverRepository>();
        services.AddScoped<IFeeRefundRepository, FeeRefundRepository>();
        services.AddScoped<IFeeLedgerRepository, FeeLedgerRepository>();
        services.AddScoped<IFeeCollectionSummaryRepository, FeeCollectionSummaryRepository>();
        services.AddScoped<ILateFeeRuleRepository, LateFeeRuleRepository>();
        services.AddScoped<IFineRuleRepository, FineRuleRepository>();
        services.AddScoped<IFeeDashboardRepository, FeeDashboardRepository>();
        services.AddScoped<IFeeReportRepository, FeeReportRepository>();
        services.AddScoped<IStudentFinanceRepository, StudentFinanceRepository>();
        services.AddScoped<IAutoBillingRepository, AutoBillingRepository>();
        services.AddScoped<IAutoFeeAssignmentRepository, AutoFeeAssignmentRepository>();

        // Register Accounting Repositories
        services.AddScoped<IChartOfAccountRepository, ChartOfAccountRepository>();
        services.AddScoped<IJournalEntryRepository, JournalEntryRepository>();
        services.AddScoped<ILedgerRepository, LedgerRepository>();
        services.AddScoped<IBankTransactionRepository, BankTransactionRepository>();
        services.AddScoped<IFinancialPeriodRepository, FinancialPeriodRepository>();

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
        services.AddScoped<IEmployeeBankAccountRepository, EmployeeBankAccountRepository>();
        services.AddScoped<IEmployeePromotionRepository, EmployeePromotionRepository>();
        services.AddScoped<IEmployeeTransferRepository, EmployeeTransferRepository>();
        services.AddScoped<IEmployeeTrainingRepository, EmployeeTrainingRepository>();
        services.AddScoped<IEmployeeAwardRepository, EmployeeAwardRepository>();
        services.AddScoped<IEmployeeDisciplinaryActionRepository, EmployeeDisciplinaryActionRepository>();
        services.AddScoped<IIdCardRepository, IdCardRepository>();

        services.AddScoped<IAcademicYearRepository, AcademicYearRepository>();
        services.AddScoped<ISchoolClassRepository, SchoolClassRepository>();
        services.AddScoped<ISectionRepository, SectionRepository>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();
        services.AddScoped<IClassSubjectRepository, ClassSubjectRepository>();
        services.AddScoped<ISchoolSessionRepository, SchoolSessionRepository>();
        services.AddScoped<ISchoolShiftRepository, SchoolShiftRepository>();
        services.AddScoped<IBuildingRepository, BuildingRepository>();
        services.AddScoped<ISubjectCategoryRepository, SubjectCategoryRepository>();
        services.AddScoped<IExamRepository, ExamRepository>();
        services.AddScoped<IMarkEntryRepository, MarkEntryRepository>();
        services.AddScoped<IGradingRuleRepository, GradingRuleRepository>();
        services.AddScoped<IResultPublicationRepository, ResultPublicationRepository>();
        services.AddScoped<IStudentSubjectResultRepository, StudentSubjectResultRepository>();
        services.AddScoped<IStudentExamResultRepository, StudentExamResultRepository>();
        services.AddScoped<IReEvaluationRequestRepository, ReEvaluationRequestRepository>();
        services.AddScoped<IResultAuditLogRepository, ResultAuditLogRepository>();
        services.AddScoped<IFinalResultRepository, FinalResultRepository>();
        services.AddScoped<IPromotionHistoryRepository, PromotionHistoryRepository>();
        services.AddScoped<IPromotioSessionRepository, PromotioSessionRepository>();
        services.AddScoped<IClassProgressionRuleRepository, ClassProgressionRuleRepository>();
services.AddScoped<ITeacherResultRepository, TeacherResultRepository>();
services.AddScoped<IExamWizardRepository, ExamWizardRepository>();
services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<IDashboardQueryRepository, DashboardQueryRepository>();
        services.AddScoped<IAcademicCalendarRepository, AcademicCalendarRepository>();
        services.AddScoped<IAcademicCalendarEventRepository, AcademicCalendarEventRepository>();
        services.AddScoped<ISyllabusRepository, SyllabusRepository>();
        services.AddScoped<ILessonPlanRepository, LessonPlanRepository>();
        services.AddScoped<IStudyMaterialRepository, StudyMaterialRepository>();
        services.AddScoped<INctbComplianceRepository, NctbComplianceRepository>();
        // Register Public Website Repositories
        services.AddScoped<ISchoolSettingRepository, SchoolSettingRepository>();
        services.AddScoped<IWebsitePageRepository, WebsitePageRepository>();
        services.AddScoped<ISliderRepository, SliderRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IEventCategoryRepository, EventCategoryRepository>();
        services.AddScoped<IGalleryRepository, GalleryRepository>();
        services.AddScoped<IGalleryImageRepository, GalleryImageRepository>();
        services.AddScoped<INoticeRepository, NoticeRepository>();
        services.AddScoped<IContactMessageRepository, ContactMessageRepository>();
        services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
        services.AddScoped<IEventNotificationRepository, EventNotificationRepository>();
        services.AddScoped<IEventNotificationRecipientRepository, EventNotificationRecipientRepository>();
        services.AddScoped<IEventNotificationLogRepository, EventNotificationLogRepository>();
        services.AddScoped<IEventNotificationQueueRepository, EventNotificationQueueRepository>();
        services.AddScoped<IGuardainNotificationPreferenceRepository, GuardainNotificationPreferenceRepository>();
        services.AddScoped<IEventNotificationAttachmentRepository, EventNotificationAttachmentRepository>();
        services.AddScoped<IScheduledNotificationRepository, ScheduledNotificationRepository>();
        services.AddScoped<IReminderConfigRepository, ReminderConfigRepository>();

        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IAdmissionService, AdmissionService>();
        services.AddScoped<IAdmissionFinanceService, AdmissionFinanceService>();
        services.AddScoped<IStudentRollGenerationService, StudentRollGenerationService>();
        services.AddScoped<ISectionAllocationService, SectionAllocationService>();
        services.AddScoped<IConversionPipelineService, ConversionPipelineService>();
        services.AddScoped<IAdmissionDashboardService, AdmissionDashboardService>();
        services.AddScoped<IAdmissionReportService, AdmissionReportService>();
        services.AddScoped<IAdmissionPaymentReportRepository, AdmissionPaymentReportRepository>();
        services.AddScoped<IAdmissionPaymentReportService, AdmissionPaymentReportService>();
        services.AddScoped<IDocumentVerificationService, DocumentVerificationService>();
        services.AddScoped<IWorkflowService, WorkflowService>();
        services.AddScoped<IGuardianService, GuardianService>();
        services.AddScoped<IStudentPortalService, StudentPortalService>();
        services.AddScoped<IFeeCategoryService, FeeCategoryService>();
        services.AddScoped<IFeeStructureService, FeeStructureService>();
        services.AddScoped<IFeeInvoiceService, FeeInvoiceService>();
        services.AddScoped<IStudentFeeAssignmentService, StudentFeeAssignmentService>();
        services.AddScoped<IFeeInvoiceItemService, FeeInvoiceItemService>();
        services.AddScoped<IFeePaymentService, FeePaymentService>();
        services.AddScoped<IFeeDiscountService, FeeDiscountService>();
        services.AddScoped<IFeeWaiverService, FeeWaiverService>();
        services.AddScoped<IFeeRefundService, FeeRefundService>();
        services.AddScoped<IFeeLedgerService, FeeLedgerService>();
        services.AddScoped<IFeeCollectionSummaryService, FeeCollectionSummaryService>();
        services.AddScoped<ILateFeeRuleService, LateFeeRuleService>();
        services.AddScoped<IFineRuleService, FineRuleService>();
        services.AddScoped<IFeeDashboardService, FeeDashboardService>();
        services.AddScoped<IEnhancedFeeDashboardService, EnhancedFeeDashboardService>();
        services.AddScoped<IFeeReportService, FeeReportService>();
        services.AddScoped<ILateFeeEngineService, LateFeeEngineService>();
        services.AddScoped<IAutoBillingService, AutoBillingService>();
        services.AddScoped<IFeeReceiptService, FeeReceiptService>();
        services.AddScoped<IFeeSecurityService, FeeSecurityService>();
        services.AddScoped<ICashierCollectionService, CashierCollectionService>();
        services.AddScoped<IFeeStructureWizardService, FeeStructureWizardService>();
        services.AddScoped<IStudentFinanceService, StudentFinanceService>();
        services.AddScoped<IStudentFeeProfileService, StudentFeeProfileService>();
        services.AddScoped<IOnlinePaymentService, OnlinePaymentService>();
        services.AddScoped<IPaymentGatewayService, SslCommerzGatewayService>();
        services.AddScoped<IAuditService, AuditService>();

        // Register Accounting Services
        services.AddScoped<IChartOfAccountService, ChartOfAccountService>();
        services.AddScoped<IJournalEntryService, JournalEntryService>();
        services.AddScoped<ILedgerService, LedgerService>();
        services.AddScoped<IBankService, BankService>();
        services.AddScoped<IFinancialPeriodService, FinancialPeriodService>();
        services.AddScoped<IFinancePostingService, FinancePostingService>();

        services.AddScoped<ITeacherService, TeacherService>();
        services.AddScoped<ITeacherSynchronizationService, TeacherSynchronizationService>();
        services.AddScoped<ITeacherScopeService, TeacherScopeService>();
        services.AddScoped<ITeacherAssignmentService, TeacherAssignmentService>();
        services.AddScoped<IAutoTeacherAssignmentService, AutoTeacherAssignmentService>();
        services.AddScoped<SchoolManagementSystem.Services.Interfaces.Attendance.IAttendanceAuthorizationService, SchoolManagementSystem.Services.Implementations.Attendance.AttendanceAuthorizationService>();
        services.AddScoped<SchoolManagementSystem.Services.Interfaces.Result.IResultAuthorizationService, SchoolManagementSystem.Services.Implementations.Result.ResultAuthorizationService>();
        services.AddScoped<IAcademicYearService, AcademicYearService>();
        services.AddScoped<ISchoolClassService, SchoolClassService>();
        services.AddScoped<ISectionService, SectionService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<IClassSubjectMappingService, ClassSubjectMappingService>();
        services.AddScoped<IAcademicCalendarService,AcademicCalendarService>();
        services.AddScoped<IAcademicCalendarEventService, AcademicCalendarEventService>();
        services.AddScoped<IHolidayMasterService, HolidayMasterService>();
        services.AddScoped<ICalendarGenerationService, CalendarGenerationService>();
        services.AddScoped<ICalendarDashboardService, CalendarDashboardService>();
        services.AddScoped<IAcademicDashboardService, AcademicDashboardService>();
        services.AddScoped<ICalendarAuditService, CalendarAuditService>();
        services.AddScoped<ISyllabusService, SyllabusService>();
        services.AddScoped<ILessonPlanService, LessonPlanService>();
        services.AddScoped<IStudyMaterialService, StudyMaterialService>();
        services.AddScoped<INctbComplianceService, NctbComplianceService>();
        services.AddScoped<IStudentGroupService, StudentGroupService>();
        services.AddScoped<ITransferService, TransferService>();
        services.AddScoped<IAcademicReportService, AcademicReportService>();
        services.AddScoped<ISchoolSessionService, SchoolSessionService>();
        services.AddScoped<ISchoolShiftService, SchoolShiftService>();
        services.AddScoped<IBuildingService, BuildingService>();
        services.AddScoped<ISubjectCategoryService, SubjectCategoryService>();
        // Register Employee Services
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IEmployeeInvitationService, EmployeeInvitationService>();
        services.AddScoped<IEmployeePayrollService, EmployeePayrollService>();
        services.AddScoped<IEmployeeHrService, EmployeeHrService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IDesignationService, DesignationService>();
        services.AddScoped<IUserProvisionService, UserProvisionService>();
        services.AddScoped<IIdCardService, IdCardService>();
        services.AddScoped<EmployeeModuleSeeder>();

        services.AddScoped<IResultCalculationService, ResultCalculationService>();
        services.AddScoped<IGradeCalculator, GradeCalculator>();
        services.AddScoped<IComponentAggregator, ComponentAggregator>();
        services.AddScoped<IPassFailPolicy, PassFailPolicy>();
        services.AddScoped<IAuditLogger, AuditLogger>();
        services.AddScoped<IMeritCalculationService, MeritCalculationService>();
        services.AddScoped<IMarkEntryService, MarkEntryService>();
        services.AddScoped<IPromotionService, PromotionService>();
        services.AddScoped<IResultPublicationService, ResultPublicationService>();
        services.AddScoped<IReEvaluationService, ReEvaluationService>();
        services.AddScoped<IReportCardService, ReportCardService>();
        services.AddScoped<IResultAnalyticsService, ResultAnalyticsService>();
        services.AddScoped<ITeacherResultService, TeacherResultService>();
services.AddScoped<IExamComponentService, ExamComponentService>();
services.AddScoped<ISubjectMarkStructureService, SubjectMarkStructureService>();
services.AddScoped<IExamSubjectComponentTeacherService, ExamSubjectComponentTeacherService>();
services.AddScoped<IExamValidationService, ExamValidationService>();
        services.AddScoped<IResultValidationService, ResultValidationService>();
        services.AddScoped<IAdmitCardService, AdmitCardService>();
        services.AddScoped<IExamSubjectService, ExamSubjectService>();
        services.AddScoped<IStudentComponentMarkService, StudentComponentMarkService>();
        services.AddScoped<IExamRoutineService, ExamRoutineService>();
        services.AddScoped<IAutoScheduleService, AutoScheduleService>();
        services.AddScoped<IStudentSubjectFilterService, StudentSubjectFilterService>();
        services.AddScoped<ITranscriptService, TranscriptService>();

        // Phase 5: Dynamic Result Policy & Promotion Engine Services
        services.AddScoped<IResultPolicyService, ResultPolicyService>();
        services.AddScoped<IPromotionPolicyService, PromotionPolicyService>();
        services.AddScoped<IRollGenerationService, RollGenerationService>();
        services.AddScoped<IPromotioSessionService, PromotioSessionService>();
        services.AddScoped<IPromotionWizardService, PromotionWizardService>();

        // Register Public Website Services
        services.AddScoped<ISchoolWebsiteService, SchoolWebsiteService>();
        services.AddScoped<ISliderService, SliderService>();
        services.AddScoped<INoticeService, NoticeService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IEventCategoryService, EventCategoryService>();
        services.AddScoped<IGalleryService, GalleryService>();
        services.AddScoped<IWebsitePageService, WebsitePageService>();
        services.AddScoped<IContactMessageService, ContactMessageService>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        services.AddScoped<IEventNotificationService, EventNotificationService>();
        services.AddScoped<IAdmissionFeeStructureService, AdmissionFeeStructureService>();
        services.AddScoped<IAnnouncementService, AnnouncementService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IPasswordHashService, Pbkdf2PasswordHashService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<EmailUrlResolver>();
        services.AddScoped<SchoolManagementSystem.Services.Interfaces.Fees.IPaymentService, SchoolManagementSystem.Services.Implementations.Fees.PaymentService>();
        services.AddScoped(typeof(SchoolManagementSystem.Services.Interfaces.Base.IBaseService<>), typeof(SchoolManagementSystem.Services.Implementations.Base.BaseService<>));
        services.AddScoped<IExamService, ExamService>();
        services.AddScoped<IExamWizardService, ExamWizardService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IAssignmentService, AssignmentService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ClassSubjectMappingSeeder>();
        services.AddScoped<SchoolManagementSystem.Services.Implementations.Website.WebsiteSeeder>();
        // AccountingRbacSeeder is static — called directly from Program.cs
        services.AddScoped<SchoolManagementSystem.Services.Implementations.Result.SubjectMarkStructureSeeder>();
        services.AddScoped<IAttendanceRecordService, AttendanceRecordService>();
        services.AddMemoryCache();
        services.AddSingleton<PlaywrightPdfEngine>();
        services.AddSingleton<IPermissionCacheService, PermissionCacheService>();
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
        services.AddHostedService<SchoolManagementSystem.Services.Implementations.Website.EventNotificationWorker>();
        services.AddHostedService<SchoolManagementSystem.Services.Implementations.Website.EventReminderWorker>();

        // Finance — automatic monthly invoice generation
        services.AddHostedService<SchoolManagementSystem.Services.Implementations.Fees.AutoBillingScheduler>();

        // Finance — expire stale gateway-pending payments after 24h
        services.AddHostedService<SchoolManagementSystem.Services.Implementations.Fees.PaymentExpiryWorker>();

        // Routine Module
        services.AddScoped<IRoutinePeriodRepository, RoutinePeriodRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<ISubjectRequirementRepository, SubjectRequirementRepository>();
        services.AddScoped<IRoutineEntryRepository, RoutineEntryRepository>();
        services.AddScoped<IWorkingDayRepository, WorkingDayRepository>();
        services.AddScoped<ITeacherAvailabilityRepository, TeacherAvailabilityRepository>();
        services.AddScoped<IRoutineGenerationRepository, RoutineGenerationRepository>();
        services.AddScoped<IRoutineConflictRepository, RoutineConflictRepository>();
        services.AddScoped<IRoutineVersionRepository, RoutineVersionRepository>();
        services.AddScoped<ISubstituteAssignmentRepository, SubstituteAssignmentRepository>();

        services.AddScoped<IRoutineDashboardRepository, RoutineDashboardRepository>();
        services.AddScoped<IRoutineAnalyticsRepository, RoutineAnalyticsRepository>();
        services.AddScoped<ITeacherLoadRepository, TeacherLoadRepository>();
        services.AddScoped<IRoomUtilizationRepository, RoomUtilizationRepository>();

        services.AddScoped<IRoutinePeriodService, RoutinePeriodService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<ISubjectRequirementService, SubjectRequirementService>();
        services.AddScoped<IRoutineEntryService, RoutineEntryService>();
        services.AddScoped<IWorkingDayService, WorkingDayService>();
        services.AddScoped<ITeacherAvailabilityService, TeacherAvailabilityService>();
        services.AddScoped<IRoutineGenerationService, RoutineGenerationService>();
        services.AddScoped<IRoutineVersionService, RoutineVersionService>();
        services.AddScoped<IRoutineEngineService, RoutineEngineService>();
        services.AddScoped<ISubstituteService, SubstituteService>();

        services.AddSingleton<RoutineGenerationQueue>();
        services.AddHostedService<RoutineGenerationWorker>();

        services.AddSingleton<AdmissionBackgroundQueue>();
        services.AddHostedService<AdmissionBackgroundWorker>();

        return services;
    }
}
