using SchoolManagementSystem.Helpers.Email;
using SchoolManagementSystem.Services.Interfaces.Email;

namespace SchoolManagementSystem.Services.Implementations.Email;

public class EmailService : IEmailService
{
    private readonly IEmailSender _emailSender;

    public EmailService(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task SendStudentActivationAsync(string toEmail, string token, CancellationToken cancellationToken = default)
    {
        var activationUrl = $"/Auth/Activate?token={Uri.EscapeDataString(token)}";

        var htmlBody = $@"
<p>Your admission is approved. Click the link to set your password.</p>
<p><a href=""{activationUrl}"">Set your password</a></p>
<p>This activation link expires in 24 hours.</p>";

        await _emailSender.SendAsync(
            to: toEmail,
            subject: "Admission Approved - Set Your Password",
            htmlBody: htmlBody,
            cancellationToken: cancellationToken);
    }
}

