using Microsoft.Extensions.Options;
using SchoolManagementSystem.Helpers.Email;
using SchoolManagementSystem.Services.Interfaces.Email;

namespace SchoolManagementSystem.Services.Implementations.Email;

public class EmailService : IEmailService
{
    private readonly IEmailSender _emailSender;
    private readonly EmailOptions _options;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EmailService(
     IEmailSender emailSender,
     IOptions<EmailOptions> options,
     IHttpContextAccessor httpContextAccessor   )
    {
        _emailSender = emailSender;
        _options = options.Value;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task SendStudentActivationAsync(
      string toEmail,
      string applicantName,
      string userName,
      string token,
      CancellationToken cancellationToken = default)
    {
        var baseUrl = _options.BaseUrl?.TrimEnd('/');

        var activationUrl = $"{baseUrl}/Auth/Activate?token={Uri.EscapeDataString(token)}";

        var htmlBody = $@"
<p>Dear <strong>{applicantName}</strong>,</p>

<p>Your admission has been approved.</p>

<p><strong>Username:</strong> {userName}</p>

<p>
    <a href=""{activationUrl}"" style=""color:#1a56db;font-weight:bold;"">
        Set your password
    </a>
</p>

<p>This activation link expires in 24 hours.</p>";

        await _emailSender.SendAsync(
            to: toEmail,
            subject: "Admission Approved - Set Your Password",
            htmlBody: htmlBody,
            cancellationToken: cancellationToken);
    }

    public async Task SendAdmissionReceivedAsync(string toEmail, string applicantName, string applicationNo, CancellationToken cancellationToken = default)
    {
        var htmlBody = $@"
<p>Dear {applicantName},</p>
<p>Thank you for applying to our school. We have received your admission application.</p>
<p><strong>Application Number:</strong> {applicationNo}</p>
<p>Our admission team will review your application and get back to you shortly.</p>
<p>Regards,<br/>Admission Team</p>";

        await _emailSender.SendAsync(
            to: toEmail,
            subject: "Admission Application Received",
            htmlBody: htmlBody,
            cancellationToken: cancellationToken);
    }

    public async Task SendTeacherAccountAsync(
    string toEmail,
    string teacherName,
    string userName,
    string password,
    CancellationToken cancellationToken = default)
{
    var baseUrl = _options.BaseUrl?.TrimEnd('/');

    var loginUrl = $"{baseUrl}/Auth/Login";

    var htmlBody = $@"
<p>Dear <strong>{teacherName}</strong>,</p>

<p>Your teacher account has been created successfully.</p>

<p>
    <strong>Username:</strong> {userName}<br/>
    <strong>Password:</strong> {password}
</p>

<p>
    <a href=""{loginUrl}"" style=""color:#1a56db;font-weight:bold;"">
        Login Here
    </a>
</p>

<p>Please change your password after first login.</p>

<p>Regards,<br/>School Administration</p>";

    await _emailSender.SendAsync(
        to: toEmail,
        subject: "Teacher Account Created",
        htmlBody: htmlBody,
        cancellationToken: cancellationToken);
}
}


