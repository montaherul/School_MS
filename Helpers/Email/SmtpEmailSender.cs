using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace SchoolManagementSystem.Helpers.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly EmailOptions _options;

    public SmtpEmailSender(IOptions<EmailOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        using var client = new SmtpClient();
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("School Management System", _options.From));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            // For common local development/server issues with SSL certificates
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            client.CheckCertificateRevocation = false;
            client.Timeout = 10000; // 10 seconds timeout

            // Determine security based on port
            var security = SecureSocketOptions.Auto;
            if (_options.Port == 465) security = SecureSocketOptions.SslOnConnect;
            else if (_options.Port == 587) security = SecureSocketOptions.StartTls;

            await client.ConnectAsync(_options.Host, _options.Port, security, cancellationToken);

            if (!string.IsNullOrWhiteSpace(_options.UserName))
            {
                await client.AuthenticateAsync(_options.UserName, _options.Password, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            // Log with more detail
            System.Diagnostics.Debug.WriteLine($"[Email Error] To: {to}, Error: {ex.Message}");
            Console.WriteLine($"[Email Error] To: {to}, Error: {ex}");
        }
    }
}
