using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Extensions;
using SchoolManagementSystem.Helpers.Email;
using SchoolManagementSystem.Helpers.Files;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Helpers.Security;
using SchoolManagementSystem.Middleware;
using SchoolManagementSystem.Repositories.Implementations;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Service.Implementations.Dashboard;
using SchoolManagementSystem.Service.Interfaces.Dashboard;
using SchoolManagementSystem.Services.Implementations.Academic;
using SchoolManagementSystem.Services.Implementations.Admin;
using SchoolManagementSystem.Services.Implementations.Admissions;
using SchoolManagementSystem.Services.Implementations.Assignment;
using SchoolManagementSystem.Services.Implementations.Attendance;
using SchoolManagementSystem.Services.Implementations.Auth;
using SchoolManagementSystem.Services.Implementations.Email;
using SchoolManagementSystem.Services.Implementations.Result;
using SchoolManagementSystem.Services.Implementations.Students;
using SchoolManagementSystem.Services.Implementations.Teachers;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.Services.Interfaces.Admissions;
using SchoolManagementSystem.Services.Interfaces.Assignment;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.Services.Interfaces.Auth;
using SchoolManagementSystem.Services.Interfaces.Email;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.UnitOfWork.Implementations;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.IO;
var builder = WebApplication.CreateBuilder(args);

ApplyEmailConfigurationOverride(builder.Configuration, "Email:Host", Environment.GetEnvironmentVariable("EMAIL_HOST"));
ApplyEmailConfigurationOverride(builder.Configuration, "Email:Port", Environment.GetEnvironmentVariable("EMAIL_PORT"));
ApplyEmailConfigurationOverride(builder.Configuration, "Email:EnableSsl", Environment.GetEnvironmentVariable("EMAIL_ENABLESSL"));
ApplyEmailConfigurationOverride(builder.Configuration, "Email:UserName", Environment.GetEnvironmentVariable("EMAIL_USERNAME"));
ApplyEmailConfigurationOverride(builder.Configuration, "Email:Password", Environment.GetEnvironmentVariable("EMAIL_PASSWORD"));
ApplyEmailConfigurationOverride(builder.Configuration, "Email:From", Environment.GetEnvironmentVariable("EMAIL_FROM"));
ApplyEmailConfigurationOverride(builder.Configuration, "Email:BaseUrl", Environment.GetEnvironmentVariable("EMAIL_BASEURL"));

// --- EMAIL CONFIGURATION VALIDATION ---
var emailOptions = builder.Configuration.GetSection("Email").Get<EmailOptions>();
if (emailOptions == null || string.IsNullOrEmpty(emailOptions.Host) || string.IsNullOrEmpty(emailOptions.UserName))
{
    Console.WriteLine("CRITICAL WARNING: Email configuration is missing or incomplete. Emails will likely fail.");
}
else
{
    Console.WriteLine($"Email Config Loaded: Host={emailOptions.Host}, Port={emailOptions.Port}, EnableSsl={emailOptions.EnableSsl}, From={emailOptions.From}, UserName={emailOptions.UserName}, PasswordConfigured={!string.IsNullOrWhiteSpace(emailOptions.Password)}");
}
// --------------------------------------

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
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/tmp/keys"))
    .SetApplicationName("SchoolManagementSystem");
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
      //  options.Cookie.SameSite = SameSiteMode.Lax;//by ip login
        options.Cookie.SameSite = SameSiteMode.None;
        /*  options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;*/
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
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
builder.Services.AddScoped<SchoolManagementSystem.Services.Interfaces.Academic.IClassSubjectMappingService, SchoolManagementSystem.Services.Implementations.Academic.ClassSubjectMappingService>();
builder.Services.AddScoped<SchoolManagementSystem.Services.Interfaces.Fees.IPaymentService, SchoolManagementSystem.Services.Implementations.Fees.PaymentService>();
builder.Services.AddScoped(typeof(SchoolManagementSystem.Services.Interfaces.Base.IBaseService<>), typeof(SchoolManagementSystem.Services.Implementations.Base.BaseService<>));
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ClassSubjectMappingSeeder>();
builder.Services.AddScoped<SchoolManagementSystem.Services.Implementations.Website.WebsiteSeeder>();
builder.Services.AddScoped<IAttendanceRecordService, AttendanceRecordService>();
builder.Services.AddScoped<IStudentAttendanceService, StudentAttendanceService>();

var port = Environment.GetEnvironmentVariable("PORT");

if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        serverOptions.ListenAnyIP(int.Parse(port));
    });
}
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Email diagnostic CLI flag removed from main branch. Use development tools or run the diagnostics locally when needed.

app.UseStatusCodePagesWithReExecute("/Error/Index", "?statusCode={0}");
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error/Index");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseCookiePolicy();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization(); 


app.UseMiddleware<AuditLoggingMiddleware>();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();

    // FIRST create/update database tables
    try
    {
        await db.Database.MigrateAsync();
        Console.WriteLine("Database migration successful");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration failed: {ex.Message}");
    }
    // THEN run seeders
    var seeder = scope.ServiceProvider.GetRequiredService<ClassSubjectMappingSeeder>();
    await seeder.SeedAsync();

    var seederEmployee = scope.ServiceProvider.GetRequiredService<SchoolManagementSystem.Services.Implementations.Employee.EmployeeModuleSeeder>();
    await seederEmployee.SeedAsync();

    var seederWebsite = scope.ServiceProvider.GetRequiredService<SchoolManagementSystem.Services.Implementations.Website.WebsiteSeeder>();
    await seederWebsite.SeedAsync();

    await FinanceRbacSeeder.SeedAsync(db);
}




app.Run();

static void ApplyEmailConfigurationOverride(IConfiguration configuration, string key, string? value)
{
    if (!string.IsNullOrWhiteSpace(value))
    {
        configuration[key] = value;
    }
}

static string? GetArgumentValue(string[] args, string name)
{
    for (var index = 0; index < args.Length - 1; index++)
    {
        if (string.Equals(args[index], name, StringComparison.OrdinalIgnoreCase))
        {
            return args[index + 1];
        }
    }

    return null;
}
