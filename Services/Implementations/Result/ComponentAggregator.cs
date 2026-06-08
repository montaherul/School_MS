using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Services.Interfaces.Result;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class ComponentAggregator : IComponentAggregator
{
    public decimal Aggregate(MarkEntry entry, List<ComponentColumnDto> components)
    {
        Dictionary<string, decimal?>? dynamicValues = null;
        if (!string.IsNullOrEmpty(entry.ComponentValues))
        {
            try
            {
                dynamicValues = System.Text.Json.JsonSerializer
                    .Deserialize<Dictionary<string, decimal?>>(entry.ComponentValues);
            }
            catch { }
        }

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
        Dictionary<string, decimal?>? dynamicValues = null;
        if (!string.IsNullOrEmpty(entry.ComponentValues))
        {
            try
            {
                dynamicValues = System.Text.Json.JsonSerializer
                    .Deserialize<Dictionary<string, decimal?>>(entry.ComponentValues);
            }
            catch { }
        }

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
