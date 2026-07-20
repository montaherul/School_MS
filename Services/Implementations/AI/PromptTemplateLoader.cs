using Microsoft.Extensions.Hosting;

namespace SchoolManagementSystem.Services.Implementations.AI;

public class PromptTemplateLoader
{
    private readonly string _templatesPath;
    private readonly Dictionary<string, string> _cache = new();

    public PromptTemplateLoader(IHostEnvironment env)
    {
        _templatesPath = Path.Combine(env.ContentRootPath, "Prompts");
    }

    public string Load(string templateName)
    {
        if (_cache.TryGetValue(templateName, out var cached))
            return cached;

        var path = Path.Combine(_templatesPath, templateName);
        if (!File.Exists(path))
            throw new FileNotFoundException($"Prompt template not found: {path}");

        var content = File.ReadAllText(path);
        _cache[templateName] = content;
        return content;
    }

    public string Render(string templateName, Dictionary<string, string> variables)
    {
        var template = Load(templateName);
        foreach (var kvp in variables)
        {
            template = template.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
        }
        return template;
    }
}
