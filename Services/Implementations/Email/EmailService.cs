using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using SchoolManagementSystem.Helpers.Email;
using SchoolManagementSystem.Services.Interfaces.Email;

namespace SchoolManagementSystem.Services.Implementations.Email;

public class EmailService : IEmailService
{
    private readonly IEmailSender _emailSender;
    private readonly EmailOptions _options;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IEmailSender emailSender,
        IOptions<EmailOptions> options,
        ILogger<EmailService> logger)
    {
        _emailSender = emailSender;
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendStudentActivationAsync(
      string toEmail,
      string applicantName,
      string userName,
      string token,
      CancellationToken cancellationToken = default)
    {
        var baseUrl = ResolveBaseUrl();
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

        await SendWorkflowEmailAsync("Student activation", toEmail, "Admission Approved - Set Your Password", htmlBody, cancellationToken);
    }

    public async Task SendAdmissionReceivedAsync(string toEmail, string applicantName, string applicationNo, CancellationToken cancellationToken = default)
    {
        var htmlBody = $@"
<p>Dear {applicantName},</p>
<p>Thank you for applying to our school. We have received your admission application.</p>
<p><strong>Application Number:</strong> {applicationNo}</p>
<p>Our admission team will review your application and get back to you shortly.</p>
<p>Regards,<br/>Admission Team</p>";

        await SendWorkflowEmailAsync("Admission received", toEmail, "Admission Application Received", htmlBody, cancellationToken);
    }

    public async Task SendTeacherAccountAsync(
    string toEmail,
    string teacherName,
    string userName,
    string password,
    CancellationToken cancellationToken = default)
{
    var baseUrl = ResolveBaseUrl();
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

    await SendWorkflowEmailAsync("Teacher account creation", toEmail, "Teacher Account Created", htmlBody, cancellationToken);
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

        await SendWorkflowEmailAsync("Employee account creation", toEmail, "Employee Account Created", htmlBody, cancellationToken);
    }

    public async Task SendEmployeeInvitationAsync(
        string toEmail,
        string employeeName,
        string invitationToken,
        DateTime expiresAt,
        CancellationToken cancellationToken = default)
    {
        var baseUrl = ResolveBaseUrl();
        var onboardingUrl = $"{baseUrl}/Onboarding/Start?token={Uri.EscapeDataString(invitationToken)}";

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

        await SendWorkflowEmailAsync("Employee invitation", toEmail, "Invitation to Join Our School Team", htmlBody, cancellationToken);
    }

    public async Task SendPasswordResetAsync(string toEmail, string userName, string otp, CancellationToken cancellationToken = default)
    {
        var safeUserName = HtmlEncoder.Default.Encode(userName);
        var safeOtp = HtmlEncoder.Default.Encode(otp);

        var htmlBody = $@"<div style='font-family: sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee; border-radius: 10px;'><h2>Password Reset</h2><p>Hello, {safeUserName}.</p><p>You requested a password reset for your account.</p><div style='background: #f0f7ff; padding: 15px; text-align: center; border-radius: 8px; margin: 20px 0;'><span style='font-size: 24px; font-weight: bold; letter-spacing: 5px; color: #1a56db;'>{safeOtp}</span></div><p>This code will expire in 10 minutes.</p></div>";

        await SendWorkflowEmailAsync("Password reset", toEmail, "Password Reset Code", htmlBody, cancellationToken);
    }

    public async Task SendAttendanceNotificationAsync(string toEmail, string studentName, string rollNumber, string className, string sectionName, DateOnly attendanceDate, string schoolName, CancellationToken cancellationToken = default)
    {
        var enc = HtmlEncoder.Default;
        var htmlBody = $@"
<p>Dear Guardian,</p>
<p>Your child was marked absent today.</p>
<table style=""border-collapse:collapse;width:100%;max-width:560px"">
  <tr><td style=""padding:6px 0;font-weight:bold"">Student Name</td><td>{enc.Encode(studentName)}</td></tr>
  <tr><td style=""padding:6px 0;font-weight:bold"">Roll Number</td><td>{enc.Encode(rollNumber)}</td></tr>
  <tr><td style=""padding:6px 0;font-weight:bold"">Class</td><td>{enc.Encode(className)}</td></tr>
  <tr><td style=""padding:6px 0;font-weight:bold"">Section</td><td>{enc.Encode(sectionName)}</td></tr>
  <tr><td style=""padding:6px 0;font-weight:bold"">Attendance Date</td><td>{attendanceDate:yyyy-MM-dd}</td></tr>
  <tr><td style=""padding:6px 0;font-weight:bold"">Absence Status</td><td>Absent</td></tr>
  <tr><td style=""padding:6px 0;font-weight:bold"">School</td><td>{enc.Encode(schoolName)}</td></tr>
</table>
<p>Please contact the school office if you believe this was recorded in error.</p>
<p>Regards,<br/>{enc.Encode(schoolName)}</p>";

        await SendWorkflowEmailAsync("Attendance notification", toEmail, "Student Absence Notification", htmlBody, cancellationToken);
    }

    private async Task SendWorkflowEmailAsync(string workflowName, string toEmail, string subject, string htmlBody, CancellationToken cancellationToken)
    {
        ValidateEmailConfiguration();
        ValidateRecipient(toEmail);
        ValidateSubjectAndBody(subject, htmlBody);

        _logger.LogInformation("Sending email workflow {WorkflowName} to {Recipient} with subject {Subject}", workflowName, toEmail, subject);

        try
        {
            await _emailSender.SendAsync(toEmail, subject, htmlBody, cancellationToken);
            _logger.LogInformation("Email workflow {WorkflowName} sent successfully to {Recipient} with subject {Subject}", workflowName, toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email workflow {WorkflowName} failed for {Recipient} with subject {Subject}", workflowName, toEmail, subject);
            throw;
        }
    }

    private string ResolveBaseUrl()
    {
        var baseUrl = string.IsNullOrWhiteSpace(_options.BaseUrl) ? _options.LocalUrl : _options.BaseUrl;

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException("Email configuration is missing BaseUrl/LocalUrl for link generation.");
        }

        return baseUrl.TrimEnd('/');
    }

    private void ValidateEmailConfiguration()
    {
        if (string.IsNullOrWhiteSpace(_options.Host))
        {
            throw new InvalidOperationException("Email configuration is missing EMAIL_HOST / Email:Host.");
        }

        if (_options.Port <= 0)
        {
            throw new InvalidOperationException("Email configuration is missing a valid EMAIL_PORT / Email:Port.");
        }

        if (string.IsNullOrWhiteSpace(_options.From))
        {
            throw new InvalidOperationException("Email configuration is missing EMAIL_FROM / Email:From.");
        }
    }

    private static void ValidateRecipient(string toEmail)
    {
        if (string.IsNullOrWhiteSpace(toEmail) || !MimeKit.MailboxAddress.TryParse(toEmail, out _))
        {
            throw new InvalidOperationException($"Invalid recipient email address: '{toEmail}'.");
        }
    }

    private static void ValidateSubjectAndBody(string subject, string htmlBody)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new InvalidOperationException("Email subject cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(htmlBody))
        {
            throw new InvalidOperationException("Email body cannot be empty.");
        }
    }
}


