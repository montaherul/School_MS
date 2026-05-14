using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Data.Seeders;
using SchoolManagementSystem.Extensions;
using SchoolManagementSystem.Helpers.Email;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Helpers.Security;
using SchoolManagementSystem.Middleware;
using SchoolManagementSystem.Repositories.Implementations;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Service.Implementations.Dashboard;
using SchoolManagementSystem.Service.Interfaces.Dashboard;
using SchoolManagementSystem.Services.Implementations.Academic;
using SchoolManagementSystem.Services.Implementations.Admissions;
using SchoolManagementSystem.Services.Implementations.Email;
using SchoolManagementSystem.Services.Implementations.Result;
using SchoolManagementSystem.Services.Implementations.Students;
using SchoolManagementSystem.Services.Implementations.Teachers;
using SchoolManagementSystem.Services.Interfaces.Admissions;
using SchoolManagementSystem.Services.Interfaces.Email;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.Services.Implementations.Admin;
using SchoolManagementSystem.Services.Implementations.Assignment;
using SchoolManagementSystem.Services.Implementations.Auth;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.Services.Interfaces.Assignment;
using SchoolManagementSystem.Services.Interfaces.Auth;
using SchoolManagementSystem.UnitOfWork.Implementations;
using SchoolManagementSystem.UnitOfWork.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Ensure app_data directory for data protection keys
var dataProtectionKeysPath = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "DataProtectionKeys");
if (!Directory.Exists(dataProtectionKeysPath))
{
    Directory.CreateDirectory(dataProtectionKeysPath);
}

// Clear providers and add better logging for debugging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
});
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath));
builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SchoolDb"),
        sql => sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Home/Privacy";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.Cookie.HttpOnly = true;
        //options.Cookie.SameSite = SameSiteMode.Strict;// by localhostlogin
        options.Cookie.SameSite = SameSiteMode.Lax;//by ip login
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });
builder.Services.AddAuthorization();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));

// All repository and service registrations are now in ServiceRegistration.AddSchoolApplicationServices()
builder.Services.AddSchoolApplicationServices();

// Additional services not in ServiceRegistration
builder.Services.AddScoped<SchoolManagementSystem.Services.Interfaces.Academic.IAcademicYearService, SchoolManagementSystem.Services.Implementations.Academic.AcademicYearService>();
builder.Services.AddScoped<SchoolManagementSystem.Services.Interfaces.Academic.ISchoolClassService, SchoolManagementSystem.Services.Implementations.Academic.SchoolClassService>();
builder.Services.AddScoped<SchoolManagementSystem.Services.Interfaces.Academic.ISectionService, SchoolManagementSystem.Services.Implementations.Academic.SectionService>();
builder.Services.AddScoped<SchoolManagementSystem.Services.Interfaces.Academic.ISubjectService, SchoolManagementSystem.Services.Implementations.Academic.SubjectService>();
builder.Services.AddScoped<SchoolManagementSystem.Services.Interfaces.Fees.IPaymentService, SchoolManagementSystem.Services.Implementations.Fees.PaymentService>();
builder.Services.AddScoped(typeof(SchoolManagementSystem.Services.Interfaces.Base.IBaseService<>), typeof(SchoolManagementSystem.Services.Implementations.Base.BaseService<>));
builder.Services.AddScoped<ITeacherAssignmentService, TeacherAssignmentService>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ClassSubjectMappingSeeder>();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5000); // 🔥 important
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddSchoolHealthChecks(builder.Configuration);
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseHsts();
}

//app.UseHttpsRedirection();commnnt out for ip run
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuditLoggingMiddleware>();
app.MapSchoolHealthChecks();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await using (var scope = app.Services.CreateAsyncScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IDataSeederRunner>();
    await runner.RunAllAsync();
}

app.Run();
