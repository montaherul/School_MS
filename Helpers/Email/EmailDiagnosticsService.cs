using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace SchoolManagementSystem.Helpers.Email;

public sealed class EmailDiagnosticsService
{
    private readonly EmailOptions _options;

    public EmailDiagnosticsService(IOptions<EmailOptions> options)
    {
        _options = options.Value;
    }

    public async Task<EmailDiagnosticResult> RunAsync(string recipientEmail, string subject, string body, CancellationToken cancellationToken = default)
    {
        var result = new EmailDiagnosticResult
        {
            Deployment = string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("PORT")) ? "localhost" : "render",
            SmtpHost = _options.Host,
            SmtpPort = _options.Port,
            EnableSsl = _options.EnableSsl,
            Username = _options.UserName,
            PasswordConfigured = !string.IsNullOrWhiteSpace(_options.Password),
            SenderEmail = _options.From,
            RecipientEmail = recipientEmail,
            Subject = subject,
            RenderedBody = body,
            TemplateRendered = !string.IsNullOrWhiteSpace(body),
            TemplateContainsExpectedText = body.Contains("School Management System deployed on Render", StringComparison.OrdinalIgnoreCase),
            RecipientValidated = IsValidMailbox(recipientEmail),
            SenderValidated = IsValidMailbox(_options.From)
        };

        try
        {
            using var client = new SmtpClient();
            client.ServerCertificateValidationCallback = (_, _, _, _) => true;
            client.CheckCertificateRevocation = false;
            client.Timeout = 10000;

            result.SocketSecurity = ResolveSocketSecurity(_options.Port, _options.EnableSsl).ToString();

            await client.ConnectAsync(_options.Host, _options.Port, ResolveSocketSecurity(_options.Port, _options.EnableSsl), cancellationToken);
            result.ConnectionOpened = client.IsConnected;
            result.TlsHandshakeSucceeded = client.IsSecure;

            if (!string.IsNullOrWhiteSpace(_options.UserName))
            {
                await client.AuthenticateAsync(_options.UserName, _options.Password, cancellationToken);
            }

            result.AuthenticationSucceeded = client.IsAuthenticated;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("School Management System", _options.From));
            message.To.Add(MailboxAddress.Parse(recipientEmail));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = body }.ToMessageBody();

            result.MessageSubjectMatches = string.Equals(message.Subject, subject, StringComparison.Ordinal);
            result.MessageRecipientMatches = message.To.Mailboxes.Any(mailbox => string.Equals(mailbox.Address, recipientEmail, StringComparison.OrdinalIgnoreCase));
            result.MessageSenderMatches = message.From.Mailboxes.Any(mailbox => string.Equals(mailbox.Address, _options.From, StringComparison.OrdinalIgnoreCase));

            await client.SendAsync(message, cancellationToken);
            result.EmailSent = true;
            result.Success = true;

            await client.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.EmailSent = false;
            result.ExceptionMessage = ex.Message;
            result.ExceptionType = ex.GetType().FullName;
            result.StackTrace = ex.ToString();
            result.InnerException = ex.InnerException?.ToString();
        }

        return result;
    }

    private static SecureSocketOptions ResolveSocketSecurity(int port, bool enableSsl)
    {
        if (!enableSsl)
        {
            return SecureSocketOptions.None;
        }

        return port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
    }

    private static bool IsValidMailbox(string? value)
    {
        return !string.IsNullOrWhiteSpace(value) && MailboxAddress.TryParse(value, out _);
    }
}

public sealed class EmailDiagnosticResult
{
    public bool Success { get; set; }
    public bool EmailSent { get; set; }
    public string Deployment { get; set; } = string.Empty;
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public bool EnableSsl { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool PasswordConfigured { get; set; }
    public string SenderEmail { get; set; } = string.Empty;
    public string RecipientEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string RenderedBody { get; set; } = string.Empty;
    public bool TemplateRendered { get; set; }
    public bool TemplateContainsExpectedText { get; set; }
    public bool RecipientValidated { get; set; }
    public bool SenderValidated { get; set; }
    public string SocketSecurity { get; set; } = string.Empty;
    public bool ConnectionOpened { get; set; }
    public bool TlsHandshakeSucceeded { get; set; }
    public bool AuthenticationSucceeded { get; set; }
    public bool MessageSubjectMatches { get; set; }
    public bool MessageRecipientMatches { get; set; }
    public bool MessageSenderMatches { get; set; }
    public string? ExceptionType { get; set; }
    public string? ExceptionMessage { get; set; }
    public string? StackTrace { get; set; }
    public string? InnerException { get; set; }
}