using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Services.Interfaces.Result;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class ComponentAggregator : IComponentAggregator
{
    private readonly ILogger<ComponentAggregator> _logger;

    public ComponentAggregator(ILogger<ComponentAggregator> logger)
    {
        _logger = logger;
    }

    private Dictionary<string, decimal?>? DeserializeComponentValues(string? raw, int entryId)
    {
        if (string.IsNullOrEmpty(raw)) return null;
        try
        {
            return System.Text.Json.JsonSerializer
                .Deserialize<Dictionary<string, decimal?>>(raw);
        }
        catch (System.Text.Json.JsonException ex)
        {
            _logger.LogWarning(ex, "Malformed ComponentValues JSON on MarkEntry {EntryId}", entryId);
            return null;
        }
    }

    public decimal Aggregate(MarkEntry entry, List<ComponentColumnDto> components)
    {
        var dynamicValues = DeserializeComponentValues(entry.ComponentValues, entry.Id);

        decimal total = 0;
        foreach (var component in components)
        {
            var val = GetComponentValue(entry, component.ComponentCode, dynamicValues);
            if (val.HasValue) total += val.Value;
        }

        return total;
    }

    public decimal AggregateAll(MarkEntry entry)
    {
        var dynamicValues = DeserializeComponentValues(entry.ComponentValues, entry.Id);

        decimal total = ComponentFieldMapper.ComputeTotal(entry);

        if (dynamicValues != null)
        {
            foreach (var kvp in dynamicValues)
            {
                if (kvp.Value.HasValue && !ComponentFieldMapper.IsStandardField(kvp.Key))
                    total += kvp.Value.Value;
            }
        }

        return total;
    }

    private static decimal? GetComponentValue(
        MarkEntry entry, string componentCode,
        Dictionary<string, decimal?>? dynamicValues)
    {
        return ComponentFieldMapper.GetEntityValue(entry, componentCode, dynamicValues);
    }
}
