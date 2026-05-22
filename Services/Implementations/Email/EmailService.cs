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

    public async Task SendEmployeeAccountAsync(
        string toEmail,
        string employeeName,
        string userName,
        string password,
        CancellationToken cancellationToken = default)
    {
        var baseUrl = _options.BaseUrl?.TrimEnd('/');
        var loginUrl = $"{baseUrl}/Auth/Login";

        var htmlBody = $@"
<p>Dear <strong>{employeeName}</strong>,</p>

<p>Your employee account has been created successfully.</p>

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
            subject: "Employee Account Created",
            htmlBody: htmlBody,
            cancellationToken: cancellationToken);
    }

    public async Task SendEmployeeInvitationAsync(
        string toEmail,
        string employeeName,
        string invitationToken,
        DateTime expiresAt,
        CancellationToken cancellationToken = default)
    {
        var baseUrl = _options.BaseUrl?.TrimEnd('/');
        var onboardingUrl = $"{baseUrl}/Onboarding/Welcome?token={invitationToken}";

        var htmlBody = $@"
<div style=""font-family: Arial, sans-serif; line-height: 1.6; color: #333;"">
    <h2 style=""color: #1a56db;"">Welcome to Our School Team!</h2>
    <p>Dear <strong>{employeeName}</strong>,</p>
    <p>We are excited to invite you to join our team. To complete your onboarding process, please click the button below to fill out your employee profile and set up your account.</p>

    <div style=""margin: 30px 0;"">
        <a href=""{onboardingUrl}"" style=""background-color: #1a56db; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;"">
            Start Onboarding
        </a>
    </div>

    <p>Please note that this invitation link will expire on <strong>{expiresAt:dd MMM yyyy, hh:mm tt}</strong>.</p>

    <p>If the button doesn't work, you can copy and paste the following link into your browser:</p>
    <p style=""font-size: 0.9em; color: #666;"">{onboardingUrl}</p>

    <hr style=""border: 0; border-top: 1px solid #eee; margin: 20px 0;"" />
    <p>Regards,<br/>School Administration</p>
</div>";

        await _emailSender.SendAsync(
            to: toEmail,
            subject: "Invitation to Join Our School Team",
            htmlBody: htmlBody,
            cancellationToken: cancellationToken);
    }
}


