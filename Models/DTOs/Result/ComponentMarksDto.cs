using System.Collections;
namespace SchoolManagementSystem.Models.DTOs.Result;

public class ComponentMarksDto : IEnumerable<KeyValuePair<string, decimal?>>
{
    private readonly Dictionary<string, decimal?> _marks = new(StringComparer.OrdinalIgnoreCase);

    public decimal? this[string code]
    {
        get => _marks.GetValueOrDefault(code);
        set => _marks[code] = value;
    }

    public int Count => _marks.Count;
    public bool ContainsKey(string code) => _marks.ContainsKey(code);
    public Dictionary<string, decimal?>.KeyCollection Keys => _marks.Keys;
    public Dictionary<string, decimal?>.ValueCollection Values => _marks.Values;

    public void Set(string code, decimal? value) => _marks[code] = value;
    public decimal? Get(string code) => _marks.GetValueOrDefault(code);
    public bool Remove(string code) => _marks.Remove(code);
    public void Clear() => _marks.Clear();

    public IEnumerator<KeyValuePair<string, decimal?>> GetEnumerator() => _marks.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _marks.GetEnumerator();

    public static ComponentMarksDto FromDictionary(Dictionary<string, decimal?>? dict)
    {
        var result = new ComponentMarksDto();
        if (dict != null)
            foreach (var kvp in dict)
                result[kvp.Key] = kvp.Value;
        return result;
    }

    public Dictionary<string, decimal?> ToDictionary() =>
        new(_marks, StringComparer.OrdinalIgnoreCase);
}
