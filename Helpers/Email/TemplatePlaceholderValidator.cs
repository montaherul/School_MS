using System.Text.RegularExpressions;

namespace SchoolManagementSystem.Helpers.Email;

public static partial class TemplatePlaceholderValidator
{
    private static readonly Regex PlaceholderPattern = PlaceholderRegex();

    [GeneratedRegex(@"\{(\w+)\}", RegexOptions.Compiled)]
    private static partial Regex PlaceholderRegex();

    public static List<string> FindUnresolved(string text)
    {
        if (string.IsNullOrEmpty(text)) return [];

        var unresolved = new List<string>();
        var matches = PlaceholderPattern.Matches(text);

        foreach (Match match in matches)
        {
            var placeholder = match.Value;
            unresolved.Add(placeholder);
        }

        return unresolved;
    }

    public static bool HasUnresolved(string text)
    {
        return !string.IsNullOrEmpty(text) && PlaceholderPattern.IsMatch(text);
    }

    public static string SanitizeForLog(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        return text.Length > 200 ? text[..200] + "..." : text;
    }
}
