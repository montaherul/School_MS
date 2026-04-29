using SchoolManagementSystem.Helpers.Email;
using SchoolManagementSystem.Helpers.Files;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Helpers.Security;
using SchoolManagementSystem.Repositories.Implementations;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Service.Implementations.Dashboard;
using SchoolManagementSystem.Service.Interfaces.Dashboard;
using SchoolManagementSystem.Services.Implementations.Admissions;
using SchoolManagementSystem.Services.Implementations.Students;
using SchoolManagementSystem.Services.Interfaces.Admissions;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.UnitOfWork.Implementations;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Extensions;

public static class ServiceRegistration
{
    public static IServiceCollection AddSchoolApplicationServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, SchoolManagementSystem.UnitOfWork.Implementations.UnitOfWork>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IAdmissionService, AdmissionService>();
        services.AddScoped<IPasswordHashService, Pbkdf2PasswordHashService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<IPdfGenerator, PlainPdfGenerator>();
        return services;
    }
}
