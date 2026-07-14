using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.RateLimiting;
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
using SchoolManagementSystem.Services.Implementations;
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
using SchoolManagementSystem.Helpers;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

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

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // Anti-forgery enforced globally — individual actions can opt out with [IgnoreAntiforgeryToken]
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new DateOnlyConverter());
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
})
.AddRazorOptions(options =>
{
    options.ViewLocationExpanders.Add(new SchoolManagementSystem.Extensions.FeeViewLocationExpander());
});
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath))
    .SetApplicationName("SchoolManagementSystem");
builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SchoolDb"),
        sql => sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));


builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("AdmissionApply", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
    options.AddFixedWindowLimiter("Login", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddHostedService<StoredProcedureInstaller>();
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
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    options.Secure = CookieSecurePolicy.Always;
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

builder.Services.Configure<SchoolManagementSystem.Models.DTOs.Fees.SslCommerzConfig>(builder.Configuration.GetSection("SslCommerz"));
builder.Services.AddHttpClient("SslCommerz", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Increase max request header total size from default 32KB to 64KB
    // to accommodate large auth cookies containing all permission claims.
    // Prevents HTTP 431 (Request Header Fields Too Large).
    serverOptions.Limits.MaxRequestHeadersTotalSize = 65536;

    var port = Environment.GetEnvironmentVariable("PORT");
    if (!string.IsNullOrEmpty(port))
    {
        serverOptions.ListenAnyIP(int.Parse(port));
    }
});
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.MimeTypes = new[] { "text/plain", "text/html", "text/css", "application/javascript", "application/json", "image/svg+xml" };
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseResponseCompression();

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

app.UseHttpsRedirection();

app.UseGlobalExceptionMiddleware();

app.UseRateLimiter();

app.UseStaticFiles();

app.UseSecurityHeaders();

app.UseRouting();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax,
    Secure = CookieSecurePolicy.SameAsRequest
});

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();


app.UseMiddleware<AuditLoggingMiddleware>();
app.UseMiddleware<SchoolManagementSystem.Middleware.HealthCheckMiddleware>();
app.UseMiddleware<SchoolManagementSystem.Middleware.MetricsMiddleware>();

// Metrics endpoint
app.MapGet("/metrics", (HttpContext ctx) =>
{
    var snapshot = SchoolManagementSystem.Middleware.MetricsMiddleware.Snapshot();
    return ctx.Response.WriteAsJsonAsync(snapshot);
});

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

    var seederSubjectMark = scope.ServiceProvider.GetRequiredService<SchoolManagementSystem.Services.Implementations.Result.SubjectMarkStructureSeeder>();
    await seederSubjectMark.SeedAsync();

    await FinanceRbacSeeder.SeedAsync(db);

    // RBAC: Accounting permissions (Chart of Accounts, Journal, Ledger, Trial Balance, Financial Statements, Bank Book, Financial Periods)
    await AccountingRbacSeeder.SeedAsync(db);

    // RBAC: ensure Exam Controller role exists and has the required permissions
    await ExamControllerRbacSeeder.SeedAsync(db);

    // RBAC: ensure Website admin permissions exist and are granted to admin roles
    await WebsiteRbacSeeder.SeedAsync(db);

    // RBAC safety net: ensure Guardian role is permanently restricted to the
    // 9 portal permissions (run after all seeders so it can correct any
    // drift introduced by historical or future migrations).
    var rbacLogger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("GuardianRbacEnforcer");
    var (wasCompliant, removed, added) = await GuardianRbacEnforcer.EnforceAsync(db, rbacLogger);
    if (!wasCompliant)
    {
        Console.WriteLine($"[RBAC] Guardian role repaired: removed {removed}, added {added}.");
    }
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
