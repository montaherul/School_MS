using System.Reflection;
using System.Text.Json;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Result;

namespace SchoolManagementSystem.Services.Implementations.Result;

/// <summary>
/// Maps ExamComponent codes to MarkEntry entity property names.
/// Supports the 12 standard component fields plus dynamic components via JSON.
/// </summary>
public static class ComponentFieldMapper
{
    private static readonly Dictionary<string, string> CodeToPropertyMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["WRITTEN"] = nameof(MarkEntry.WrittenMarks),
        ["MCQ"] = nameof(MarkEntry.MCQMarks),
        ["CQ"] = nameof(MarkEntry.CQMarks),
        ["PRACTICAL"] = nameof(MarkEntry.PracticalMarks),
        ["VIVA"] = nameof(MarkEntry.VivaMarks),
        ["LAB"] = nameof(MarkEntry.LabMarks),
        ["ORAL"] = nameof(MarkEntry.OralMarks),
        ["ASSIGNMENT"] = nameof(MarkEntry.AssignmentMarks),
        ["CONTINUOUS_ASSESSMENT"] = nameof(MarkEntry.ContinuousAssessmentMarks),
        ["COMPETENCY"] = nameof(MarkEntry.CompetencyMarks),
        ["BEHAVIOUR"] = nameof(MarkEntry.BehaviourMarks),
        ["PARTICIPATION"] = nameof(MarkEntry.ParticipationMarks),
    };

    private static readonly Dictionary<string, PropertyInfo> PropertyCache = [];

    static ComponentFieldMapper()
    {
        var props = typeof(MarkEntry).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType == typeof(decimal?))
            .ToDictionary(p => p.Name, p => p, StringComparer.Ordinal);

        foreach (var (code, propName) in CodeToPropertyMap)
        {
            if (props.TryGetValue(propName, out var prop))
                PropertyCache[code] = prop;
        }
    }

    public static string? GetPropertyName(string componentCode)
        => CodeToPropertyMap.TryGetValue(componentCode, out var propName) ? propName : null;

    public static decimal? GetValue(MarkEntry entry, string componentCode)
    {
        if (PropertyCache.TryGetValue(componentCode, out var prop))
            return (decimal?)prop.GetValue(entry);

        if (!string.IsNullOrEmpty(entry.ComponentValues))
        {
            var parsed = JsonSerializer.Deserialize<Dictionary<string, decimal?>>(entry.ComponentValues);
            if (parsed != null && parsed.TryGetValue(componentCode, out var dynamicVal))
                return dynamicVal;
        }

        return null;
    }

    public static void SetValue(MarkEntry entry, string componentCode, decimal? value)
    {
        if (PropertyCache.TryGetValue(componentCode, out var prop))
            prop.SetValue(entry, value);
    }

    public static decimal ComputeTotal(MarkEntry entry, IEnumerable<(string Code, string Name)>? componentCodes = null)
    {
        decimal total = 0;

        if (componentCodes == null)
        {
            foreach (var prop in PropertyCache.Values)
            {
                var val = (decimal?)prop.GetValue(entry);
                if (val.HasValue) total += val.Value;
            }
        }
        else
        {
            foreach (var (code, _) in componentCodes)
            {
                var val = GetValue(entry, code);
                if (val.HasValue) total += val.Value;
            }
        }

        return total;
    }

    public static bool IsStandardField(string componentCode)
        => CodeToPropertyMap.ContainsKey(componentCode);

    /// <summary>
    /// Returns the code-to-column mapping for building ComponentMarksDto from DB readers.
    /// </summary>
    public static Dictionary<string, string> GetCodeToColumnMap()
        => new(CodeToPropertyMap, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets a component value from any ComponentMarksDto by component code.
    /// Replaces all switch statements across the codebase.
    /// </summary>
    public static decimal? GetDtoValue(ComponentMarksDto marks, string componentCode)
        => marks[componentCode];

    /// <summary>
    /// Gets a component value from a MarkEntryDto by component code.
    /// </summary>
    public static decimal? GetDtoValue(MarkEntryDto dto, string componentCode)
        => dto.ComponentMarks[componentCode];

    /// <summary>
    /// Gets a component value from a MarkEntry entity by component code.
    /// </summary>
    public static decimal? GetEntityValue(MarkEntry entry, string componentCode, Dictionary<string, decimal?>? dynamicValues = null)
    {
        if (PropertyCache.TryGetValue(componentCode, out var prop))
            return (decimal?)prop.GetValue(entry);

        return dynamicValues?.GetValueOrDefault(componentCode);
    }

    /// <summary>
    /// Creates ComponentMarksDto from a MarkEntry entity (reads 12 columns + ComponentValues JSON).
    /// </summary>
    public static ComponentMarksDto FromEntity(MarkEntry entry)
    {
        var marks = new ComponentMarksDto();
        foreach (var (code, propName) in CodeToPropertyMap)
        {
            if (PropertyCache.TryGetValue(code, out var prop))
            {
                var val = (decimal?)prop.GetValue(entry);
                if (val.HasValue)
                    marks[code] = val;
            }
        }

        if (!string.IsNullOrEmpty(entry.ComponentValues))
        {
            try
            {
                var parsed = JsonSerializer.Deserialize<Dictionary<string, decimal?>>(entry.ComponentValues);
                if (parsed != null)
                    foreach (var kvp in parsed)
                        if (!marks.ContainsKey(kvp.Key))
                            marks[kvp.Key] = kvp.Value;
            }
            catch { }
        }

        return marks;
    }

    /// <summary>
    /// Applies ComponentMarksDto to a MarkEntry entity (sets 12 standard fields, returns JSON for dynamic).
    /// </summary>
    public static string? ApplyToEntity(ComponentMarksDto marks, MarkEntry entry)
    {
        var dynamicValues = new Dictionary<string, decimal?>();
        foreach (var (code, value) in marks)
        {
            if (PropertyCache.TryGetValue(code, out var prop))
                prop.SetValue(entry, value);
            else if (value.HasValue)
                dynamicValues[code] = value;
        }

        return dynamicValues.Count > 0
            ? JsonSerializer.Serialize(dynamicValues)
            : null;
    }

    /// <summary>
    /// Extracts ComponentMarksDto from a MarkEntry entity, given configured components.
    /// </summary>
    public static ComponentMarksDto ExtractConfiguredComponents(MarkEntry entry, IEnumerable<(string Code, string Name)>? components)
    {
        var marks = new ComponentMarksDto();
        if (components == null) return marks;

        foreach (var (code, _) in components)
        {
            var val = GetValue(entry, code);
            if (val.HasValue)
                marks[code] = val.Value;
        }

        return marks;
    }

    /// <summary>
    /// Serializes non-standard components from ComponentMarksDto to JSON.
    /// </summary>
    public static string? SerializeDynamicComponents(ComponentMarksDto marks)
    {
        var dynamicValues = new Dictionary<string, decimal?>();
        foreach (var (code, value) in marks)
        {
            if (!CodeToPropertyMap.ContainsKey(code) && value.HasValue)
                dynamicValues[code] = value;
        }
        return dynamicValues.Count > 0
            ? JsonSerializer.Serialize(dynamicValues)
            : null;
    }

    /// <summary>
    /// Computes total from ComponentMarksDto for given component codes.
    /// </summary>
    public static decimal ComputeTotalFromDto(ComponentMarksDto marks, IEnumerable<(string Code, string Name)>? components)
    {
        if (components == null) return 0;

        decimal total = 0;
        foreach (var (code, _) in components)
        {
            var val = marks[code];
            if (val.HasValue) total += val.Value;
        }
        return total;
    }
}
