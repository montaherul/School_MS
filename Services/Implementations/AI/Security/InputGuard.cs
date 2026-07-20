using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace SchoolManagementSystem.Services.Implementations.AI.Security;

public partial class InputGuard
{
    private readonly ILogger<InputGuard> _logger;

    public InputGuard(ILogger<InputGuard> logger) { _logger = logger; }

    public string Sanitize(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        var sanitized = input.Trim();
        sanitized = StripControlChars(sanitized);
        sanitized = StripNullBytes(sanitized);

        return sanitized;
    }

    public bool ContainsPromptInjection(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;

        var lower = input.ToLowerInvariant();

        var injectionPatterns = new[]
        {
            @"ignore all previous instructions",
            @"ignore all instructions",
            @"ignore the above",
            @"you are now",
            @"act as if",
            @"pretend to be",
            @"system prompt:",
            @"forget everything",
            @"override",
            @"new instructions:",
            @"you must obey",
            @"you are not",
            @"do not follow",
            @"disregard",
            @"roleplay",
            @"simulate",
            @"jailbreak",
            @"dan",
            @"do anything now"
        };

        foreach (var pattern in injectionPatterns)
        {
            if (lower.Contains(pattern))
            {
                _logger.LogWarning("Prompt injection detected: pattern '{Pattern}'", pattern);
                return true;
            }
        }

        return false;
    }

    public string MaskPii(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;

        var result = input;

        // Mask email addresses
        result = EmailRegex().Replace(result, "***@***.***");

        // Mask Bangladeshi mobile numbers (01XXXXXXXXX)
        result = BdMobileRegex().Replace(result, "01*********");

        // Mask NID numbers (10 or 17 digit)
        result = NidRegex().Replace(result, "**********");

        // Mask birth certificate numbers (11-17 digit)
        result = BirthCertRegex().Replace(result, "************");

        // Mask IP addresses
        result = IpRegex().Replace(result, "***.***.***.***");

        return result;
    }

    private static string StripControlChars(string input)
        => ControlCharRegex().Replace(input, "");

    private static string StripNullBytes(string input)
        => NullByteRegex().Replace(input, "");

    [GeneratedRegex(@"[\x00-\x08\x0B\x0C\x0E-\x1F]")]
    private static partial Regex ControlCharRegex();

    [GeneratedRegex(@"\x00")]
    private static partial Regex NullByteRegex();

    [GeneratedRegex(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}")]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"01[3-9]\d{8}")]
    private static partial Regex BdMobileRegex();

    [GeneratedRegex(@"\b\d{10}(?:\d{7})?\b")]
    private static partial Regex NidRegex();

    [GeneratedRegex(@"\b\d{11,17}\b")]
    private static partial Regex BirthCertRegex();

    [GeneratedRegex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b")]
    private static partial Regex IpRegex();
}
