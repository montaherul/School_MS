using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using SchoolManagementSystem.Repositories.Interfaces.Website;

namespace SchoolManagementSystem.Helpers.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly EmailOptions _fileOptions;
    private readonly ISchoolSettingRepository _settingRepo;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<EmailOptions> options, ISchoolSettingRepository settingRepo, ILogger<SmtpEmailSender> logger)
    {
        _fileOptions = options.Value;
        _settingRepo = settingRepo;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        var settings = await _settingRepo.GetCurrentSettingsAsync(cancellationToken);
        var host = settings?.SmtpHost ?? _fileOptions.Host;
        var port = settings?.SmtpPort > 0 ? settings.SmtpPort : _fileOptions.Port;
        var enableSsl = settings?.SmtpEnableSsl ?? _fileOptions.EnableSsl;
        var userName = settings?.SmtpUserName ?? _fileOptions.UserName;
        var password = settings?.SmtpPassword ?? _fileOptions.Password;
        var from = settings?.SmtpFromEmail ?? _fileOptions.From;

        using var client = new SmtpClient();
        try
        {
            ValidateConfiguration(host, port, from);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("School Management System", from));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            var security = ResolveSecurity(enableSsl, port);

            _logger.LogInformation("Opening SMTP connection to {Host}:{Port} for recipient {Recipient}", host, port, to);

            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            client.CheckCertificateRevocation = false;
            client.Timeout = 10000;

            await client.ConnectAsync(host, port, security, cancellationToken);
            _logger.LogInformation("SMTP connection opened successfully to {Host}:{Port}", host, port);

            if (!string.IsNullOrWhiteSpace(userName))
            {
                await client.AuthenticateAsync(userName, password, cancellationToken);
                _logger.LogInformation("SMTP authentication succeeded for {UserName}", userName);
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

    private static void ValidateConfiguration(string host, int port, string from)
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            throw new InvalidOperationException("Email SMTP host is missing.");
        }

        if (port <= 0)
        {
            throw new InvalidOperationException("Email SMTP port is invalid.");
        }

        if (string.IsNullOrWhiteSpace(from))
        {
            throw new InvalidOperationException("Email sender address is missing.");
        }
    }

    private static SecureSocketOptions ResolveSecurity(bool enableSsl, int port)
    {
        if (!enableSsl)
        {
            return SecureSocketOptions.None;
        }

        return port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
    }
}
