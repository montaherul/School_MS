using System.Reflection;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Result;

namespace SchoolManagementSystem.Services.Implementations.Result;

/// <summary>
/// Maps ExamComponent codes to MarkEntry entity property names.
/// Supports the 12 standard component fields plus dynamic components via JSON.
/// This is the bridge between the dynamic SubjectMarkStructure system and
/// the existing hardcoded MarkEntry columns — no duplicate storage needed.
/// </summary>
public static class ComponentFieldMapper
{
    /// <summary>
    /// Maps ExamComponent.Code → MarkEntry property name for the 12 standard fields.
    /// Components with codes not in this map are stored as dynamic extras.
    /// </summary>
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

    /// <summary>Gets the MarkEntry property name for a component code, or null if not mapped.</summary>
    public static string? GetPropertyName(string componentCode)
    {
        return CodeToPropertyMap.TryGetValue(componentCode, out var propName) ? propName : null;
    }

    /// <summary>
    /// Gets the value of a component from a MarkEntry by component code.
    /// Returns null if the component code is not in the standard map.
    /// </summary>
    public static decimal? GetValue(MarkEntry entry, string componentCode)
    {
        if (PropertyCache.TryGetValue(componentCode, out var prop))
            return (decimal?)prop.GetValue(entry);

        if (!string.IsNullOrEmpty(entry.ComponentValues))
        {
            var parsed = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, decimal?>>(entry.ComponentValues);
            if (parsed != null && parsed.TryGetValue(componentCode, out var dynamicVal))
                return dynamicVal;
        }

        return null;
    }

    /// <summary>
    /// Sets the value of a component on a MarkEntry by component code.
    /// Only works for mapped (standard) components. Unmapped components are ignored.
    /// </summary>
    public static void SetValue(MarkEntry entry, string componentCode, decimal? value)
    {
        if (PropertyCache.TryGetValue(componentCode, out var prop))
            prop.SetValue(entry, value);
    }

    /// <summary>
    /// Computes the total marks from all non-null component values in a MarkEntry.
    /// Uses the SubjectMarkStructure-defined components to determine which fields to sum.
    /// </summary>
    public static decimal ComputeTotal(MarkEntry entry, IEnumerable<(string Code, string Name)>? componentCodes = null)
    {
        decimal total = 0;

        if (componentCodes == null)
        {
            // Sum all standard mapped fields (backward compatibility)
            foreach (var prop in PropertyCache.Values)
            {
                var val = (decimal?)prop.GetValue(entry);
                if (val.HasValue) total += val.Value;
            }
        }
        else
        {
            // Sum only the configured components
            foreach (var (code, _) in componentCodes)
            {
                var val = GetValue(entry, code);
                if (val.HasValue) total += val.Value;
            }
        }

        return total;
    }

    /// <summary>
    /// Checks if a component code maps to a standard field.
    /// </summary>
    public static bool IsStandardField(string componentCode)
    {
        return CodeToPropertyMap.ContainsKey(componentCode);
    }

    /// <summary>
    /// Gets a component value from a MarkEntryDto by component code.
    /// Eliminates duplicate switch statements across services.
    /// </summary>
    public static decimal? GetDtoValue(MarkEntryDto dto, string componentCode)
    {
        var propName = GetPropertyName(componentCode);
        if (propName == null)
        {
            dto.ComponentValues.TryGetValue(componentCode, out var dynamicVal);
            return dynamicVal;
        }

        return propName switch
        {
            nameof(MarkEntry.WrittenMarks) => dto.WrittenMarks,
            nameof(MarkEntry.MCQMarks) => dto.MCQMarks,
            nameof(MarkEntry.CQMarks) => dto.CQMarks,
            nameof(MarkEntry.PracticalMarks) => dto.PracticalMarks,
            nameof(MarkEntry.VivaMarks) => dto.VivaMarks,
            nameof(MarkEntry.LabMarks) => dto.LabMarks,
            nameof(MarkEntry.OralMarks) => dto.OralMarks,
            nameof(MarkEntry.AssignmentMarks) => dto.AssignmentMarks,
            nameof(MarkEntry.ContinuousAssessmentMarks) => dto.ContinuousAssessmentMarks,
            nameof(MarkEntry.CompetencyMarks) => dto.CompetencyMarks,
            nameof(MarkEntry.BehaviourMarks) => dto.BehaviourMarks,
            nameof(MarkEntry.ParticipationMarks) => dto.ParticipationMarks,
            _ => null
        };
    }

    /// <summary>
    /// Gets a component value from a MarkEntry entity by component code,
    /// with optional dynamic values fallback.
    /// Eliminates duplicate switch statements across services.
    /// </summary>
    public static decimal? GetEntityValue(MarkEntry entry, string componentCode, Dictionary<string, decimal?>? dynamicValues = null)
    {
        var propName = GetPropertyName(componentCode);
        if (propName == null)
            return dynamicValues?.GetValueOrDefault(componentCode);

        return propName switch
        {
            nameof(MarkEntry.WrittenMarks) => entry.WrittenMarks,
            nameof(MarkEntry.MCQMarks) => entry.MCQMarks,
            nameof(MarkEntry.CQMarks) => entry.CQMarks,
            nameof(MarkEntry.PracticalMarks) => entry.PracticalMarks,
            nameof(MarkEntry.VivaMarks) => entry.VivaMarks,
            nameof(MarkEntry.LabMarks) => entry.LabMarks,
            nameof(MarkEntry.OralMarks) => entry.OralMarks,
            nameof(MarkEntry.AssignmentMarks) => entry.AssignmentMarks,
            nameof(MarkEntry.ContinuousAssessmentMarks) => entry.ContinuousAssessmentMarks,
            nameof(MarkEntry.CompetencyMarks) => entry.CompetencyMarks,
            nameof(MarkEntry.BehaviourMarks) => entry.BehaviourMarks,
            nameof(MarkEntry.ParticipationMarks) => entry.ParticipationMarks,
            _ => null
        };
    }

    /// <summary>
    /// Applies all standard field values from a MarkEntryDto to a MarkEntry entity.
    /// </summary>
    public static void ApplyStandardFieldValues(MarkEntry entry, MarkEntryDto dto)
    {
        entry.WrittenMarks = dto.WrittenMarks;
        entry.MCQMarks = dto.MCQMarks;
        entry.CQMarks = dto.CQMarks;
        entry.PracticalMarks = dto.PracticalMarks;
        entry.VivaMarks = dto.VivaMarks;
        entry.LabMarks = dto.LabMarks;
        entry.OralMarks = dto.OralMarks;
        entry.AssignmentMarks = dto.AssignmentMarks;
        entry.ContinuousAssessmentMarks = dto.ContinuousAssessmentMarks;
        entry.CompetencyMarks = dto.CompetencyMarks;
        entry.BehaviourMarks = dto.BehaviourMarks;
        entry.ParticipationMarks = dto.ParticipationMarks;
    }
}
