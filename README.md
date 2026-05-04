# School Management System

ASP.NET Core MVC (.NET 8) and SQL Server school management system with layered architecture.

## Implemented

- ASP.NET Core MVC with Razor Views, Bootstrap 5, jQuery, AJAX-ready module grids.
- SQL Server + EF Core model covering admissions, students, academics, attendance, exams, marks/results, assignments, fees/payments, communication, library, transport, health, notifications, audit logs, roles, and permissions.
- Layered structure: Controllers, Services, Repositories, UnitOfWork, DTOs, ViewModels, Helpers, Middleware, Data.
- Cookie authentication with secure PBKDF2 password hashing, session timeout, anti-forgery tokens, and RBAC permission filter.
- Seeded roles: Super Admin, Principal, Assistant Head, Senior Lecturer, Lecturer, Office Staff, Student.
- Admission service with application ID generation, approve/reject workflow, and applicant-to-student conversion.
- Student service with create/edit/delete, unique student ID generation, guardian storage, roll uniqueness, search, and pagination.
- Dashboard with statistics and charts.
- Local file upload helper with extension and size validation.
- SMTP email abstraction using `System.Net.Mail`.
- PDF generation abstraction. Current fallback returns report bytes; swap implementation to iText7 when NuGet access is available.
- Audit logging middleware for authenticated POST actions.

## Required NuGet Packages for Full Mandatory Stack

The environment blocked NuGet downloads during implementation. When network access is available, add:

```powershell
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.0
dotnet add package MailKit --version 4.8.0
dotnet add package itext7 --version 8.0.5
```

Then replace the current cookie-auth store with `IdentityDbContext<ApplicationUser, Role, int>`, replace `SmtpEmailSender` with a MailKit implementation, and replace `PlainPdfGenerator` with an iText7 generator.

## Run

```powershell
cd "C:\Users\islam\OneDrive\Documents\New project\MvcSqlServerApp"
$env:DOTNET_CLI_HOME="C:\Users\islam\OneDrive\Documents\New project\.dotnet-home"
dotnet build
dotnet run --urls http://localhost:5073
```

Default seeded login:

- User: `admin`
- Password: `Admin@12345`

## Database

Connection string is in `appsettings.json`.

```json
"SchoolDb": "Server=MONTAHERUL\\SQLEXPRESS;Database=SchoolManagementSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;Encrypt=False"
```

Generate/apply migrations:

```powershell
.\.tools\dotnet-ef migrations add MigrationName
.\.tools\dotnet-ef database update
```

## IIS Deployment Notes

- Publish with `dotnet publish -c Release`.
- Configure IIS app pool to `No Managed Code`.
- Install the .NET 8 Hosting Bundle on the server.
- Put production SQL Server connection string in environment-specific configuration.
- Store SMTP credentials outside source control.
- Use HTTPS and secure cookies in production.
