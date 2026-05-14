using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.ViewModels.Shared;
using SchoolManagementSystem.Services.Interfaces.Base;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Common;

[Authorize]
public abstract class GenericCrudController<TEntity> : Controller where TEntity : BaseEntity, new()
{
    protected readonly IBaseService<TEntity> _service;
    protected readonly string _moduleName;

    protected GenericCrudController(IBaseService<TEntity> service, string moduleName)
    {
        _service = service;
        _moduleName = moduleName;
    }
    
    protected string ControllerName => GetType().Name.Replace("Controller", string.Empty);

    protected virtual IQueryable<TEntity> ApplySecurityFilters(IQueryable<TEntity> query)
    {
        return query;
    }

    public virtual async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        
        var result = await _service.GetPagedAsync(page, pageSize, search, User, cancellationToken);

        var model = new CrudListViewModel
        {
            ModuleName = _moduleName,
            ControllerName = ControllerName,
            Columns = EditableProperties().Take(8).Select(p => p.Name).ToList(),
            Rows = result.Items.Select(ToRow).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalItems = result.TotalItems,
            Search = search
        };
        return View(model);
    }

    public virtual IActionResult Create()
    {
        return RedirectToAction(nameof(CreateEdit));
    }

    public virtual async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
    {
        return await CreateEdit(id, cancellationToken);
    }

    public virtual async Task<IActionResult> CreateEdit(int? id = null, CancellationToken cancellationToken = default)
    {
        var entity = id is > 0
            ? await _service.GetByIdAsync(id.Value, cancellationToken)
            : new TEntity();
            
        if (entity is null && id > 0)
        {
            return NotFound();
        }

        return View("CreateEdit", ToForm(entity ?? new TEntity()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Save(IFormCollection form, CancellationToken cancellationToken = default)
    {
        var id = int.TryParse(form["Id"], out var parsedId) ? parsedId : 0;
        var entity = id > 0
            ? await _service.GetByIdAsync(id, cancellationToken)
            : new TEntity();
            
        if (entity is null && id > 0)
        {
            return NotFound();
        }

        foreach (var property in EditableProperties())
        {
            if (!form.TryGetValue(property.Name, out var raw))
            {
                continue;
            }

            property.SetValue(entity, ConvertValue(raw.ToString(), property.PropertyType));
        }

        var user = User.Identity?.Name ?? "system";
        if (id == 0)
        {
            await _service.CreateAsync(entity!, user, cancellationToken);
        }
        else
        {
            await _service.UpdateAsync(entity!, user, cancellationToken);
        }

        TempData["SuccessMessage"] = $"{_moduleName} saved successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual Task<IActionResult> CreateEdit(IFormCollection form, CancellationToken cancellationToken = default)
    {
        return Save(form, cancellationToken);
    }

    public virtual async Task<IActionResult> Details(int id, CancellationToken cancellationToken = default)
    {
        var query = _service.Query().Where(x => x.Id == id);
        query = ApplySecurityFilters(query);
        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        return entity is null ? NotFound() : View(ToDetails(entity));
    }

    public virtual async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _service.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(ToDetails(entity));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken = default)
    {
        var exists = await _service.ExistsAsync(id, cancellationToken);
        if (!exists)
        {
            return NotFound();
        }

        var user = User.Identity?.Name ?? "system";
        await _service.DeleteAsync(id, user, cancellationToken);
        
        TempData["SuccessMessage"] = $"{_moduleName} deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    private CrudFormViewModel ToForm(TEntity entity)
    {
        return new CrudFormViewModel
        {
            ModuleName = _moduleName,
            ControllerName = ControllerName,
            Id = entity.Id,
            Fields = EditableProperties().Select(p => ToField(p, entity)).ToList()
        };
    }

    private CrudDetailsViewModel ToDetails(TEntity entity)
    {
        return new CrudDetailsViewModel
        {
            ModuleName = _moduleName,
            ControllerName = ControllerName,
            Id = entity.Id,
            Fields = EditableProperties().Select(p => ToField(p, entity)).ToList()
        };
    }

    private Dictionary<string, string?> ToRow(TEntity entity)
    {
        var row = new Dictionary<string, string?> { ["Id"] = entity.Id.ToString(CultureInfo.InvariantCulture) };
        foreach (var property in EditableProperties().Take(8))
        {
            row[property.Name] = FormatValue(property.GetValue(entity));
        }

        return row;
    }

    private CrudFieldViewModel ToField(PropertyInfo property, TEntity entity)
    {
        var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        return new CrudFieldViewModel
        {
            Name = property.Name,
            Label = property.GetCustomAttribute<DisplayAttribute>()?.Name ?? SplitWords(property.Name),
            Value = FormatValue(property.GetValue(entity)),
            InputType = InputType(propertyType),
            IsRequired = property.GetCustomAttribute<RequiredAttribute>() is not null || (!IsNullable(property) && propertyType != typeof(bool)),
            Options = propertyType.IsEnum ? Enum.GetNames(propertyType) : []
        };
    }

    private static IReadOnlyList<PropertyInfo> EditableProperties()
    {
        var excluded = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            nameof(BaseEntity.Id), nameof(BaseEntity.CreatedAt), nameof(BaseEntity.CreatedBy),
            nameof(BaseEntity.UpdatedAt), nameof(BaseEntity.UpdatedBy), nameof(BaseEntity.IsDeleted)
        };

        return typeof(TEntity).GetProperties()
            .Where(p => p.CanRead && p.CanWrite && !excluded.Contains(p.Name))
            .Where(p => IsSupportedType(Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType))
            .ToList();
    }

    private static bool IsSupportedType(Type type)
    {
        return type == typeof(string) || type == typeof(int) || type == typeof(decimal) || type == typeof(bool) ||
               type == typeof(DateTime) || type == typeof(DateOnly) || type == typeof(TimeOnly) || type.IsEnum;
    }

    private static object? ConvertValue(string value, Type targetType)
    {
        var nullableType = Nullable.GetUnderlyingType(targetType);
        var type = nullableType ?? targetType;
        if (string.IsNullOrWhiteSpace(value) && nullableType is not null)
        {
            return null;
        }

        if (type == typeof(string)) return value?.Trim() ?? string.Empty;
        if (type == typeof(int)) return int.TryParse(value, out var intValue) ? intValue : 0;
        if (type == typeof(decimal)) return decimal.TryParse(value, out var decimalValue) ? decimalValue : 0;
        if (type == typeof(bool)) return value is "true" or "on" or "1";
        if (type == typeof(DateTime)) return DateTime.TryParse(value, out var dateTimeValue) ? dateTimeValue : DateTime.UtcNow;
        if (type == typeof(DateOnly)) return DateOnly.TryParse(value, out var dateOnlyValue) ? dateOnlyValue : DateOnly.FromDateTime(DateTime.UtcNow);
        if (type == typeof(TimeOnly)) return TimeOnly.TryParse(value, out var timeOnlyValue) ? timeOnlyValue : TimeOnly.MinValue;
        if (type.IsEnum) return Enum.TryParse(type, value, true, out var enumValue) ? enumValue : Activator.CreateInstance(type);
        return null;
    }

    private static string? FormatValue(object? value)
    {
        return value switch
        {
            null => null,
            DateTime dateTime => dateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            DateOnly dateOnly => dateOnly.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            TimeOnly timeOnly => timeOnly.ToString("HH:mm", CultureInfo.InvariantCulture),
            bool boolean => boolean ? "true" : "false",
            _ => Convert.ToString(value, CultureInfo.InvariantCulture)
        };
    }

    private static string InputType(Type type)
    {
        if (type == typeof(DateTime) || type == typeof(DateOnly)) return "date";
        if (type == typeof(TimeOnly)) return "time";
        if (type == typeof(int) || type == typeof(decimal)) return "number";
        if (type == typeof(bool)) return "checkbox";
        return "text";
    }

    private static bool IsNullable(PropertyInfo property)
    {
        return Nullable.GetUnderlyingType(property.PropertyType) is not null || !property.PropertyType.IsValueType;
    }

    private static string SplitWords(string value)
    {
        return string.Concat(value.Select((ch, i) => i > 0 && char.IsUpper(ch) ? " " + ch : ch.ToString()));
    }
}
