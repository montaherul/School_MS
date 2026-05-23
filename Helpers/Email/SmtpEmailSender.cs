using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace SchoolManagementSystem.Helpers.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly EmailOptions _options;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<EmailOptions> options, ILogger<SmtpEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        using var client = new SmtpClient();
        try
        {
            ValidateConfiguration();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("School Management System", _options.From));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            var security = ResolveSecurity();

            _logger.LogInformation("Opening SMTP connection to {Host}:{Port} using {Security} for recipient {Recipient}", _options.Host, _options.Port, security, to);

            // For common local development/server issues with SSL certificates
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            client.CheckCertificateRevocation = false;
            client.Timeout = 10000; // 10 seconds timeout

            await client.ConnectAsync(_options.Host, _options.Port, security, cancellationToken);
            _logger.LogInformation("SMTP connection opened successfully to {Host}:{Port}", _options.Host, _options.Port);

            if (!string.IsNullOrWhiteSpace(_options.UserName))
            {
                await client.AuthenticateAsync(_options.UserName, _options.Password, cancellationToken);
                _logger.LogInformation("SMTP authentication succeeded for {UserName}", _options.UserName);
            }

            await client.SendAsync(message, cancellationToken);
            _logger.LogInformation("SMTP send succeeded for recipient {Recipient} with subject {Subject}", to, subject);
            await client.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMTP send failed for recipient {Recipient} with subject {Subject}", to, subject);
            System.Diagnostics.Debug.WriteLine($"[Email Error] To: {to}, Error: {ex}");
            Console.WriteLine($"[Email Error] To: {to}, Error: {ex}");
            throw;
        }
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrWhiteSpace(_options.Host))
        {
            throw new InvalidOperationException("Email SMTP host is missing.");
        }

        if (_options.Port <= 0)
        {
            throw new InvalidOperationException("Email SMTP port is invalid.");
        }

        if (string.IsNullOrWhiteSpace(_options.From))
        {
            throw new InvalidOperationException("Email sender address is missing.");
        }
    }

    private SecureSocketOptions ResolveSecurity()
    {
        if (!_options.EnableSsl)
        {
            return SecureSocketOptions.None;
        }

        return _options.Port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
    }
}
